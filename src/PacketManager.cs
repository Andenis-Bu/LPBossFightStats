using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats
{
    public class PacketManager : Mod
    {
        // Enum for defining the primary packet types
        public enum PacketType : byte
        {
            // Layer one packets L1
            PacketManager,
            BossEventsManager,
            BossFightManager,

            // Layer two packets L2
            RecieveMessage
        }

        public void SendMessage(string message)
        {
            ModPacket packet = GetPacket();
            packet.Write((byte)PacketType.PacketManager);
            packet.Write((byte)PacketType.RecieveMessage);
            packet.Write(message);
            packet.Send();
        }

        // Main method for handling incoming packets
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            base.HandlePacket(reader, whoAmI);

            // Read the primary packet type
            PacketType packetTypeL1 = (PacketType)reader.ReadByte();

            // Handle packet based on the network mode (Server/Client)
            if (Main.netMode == NetmodeID.Server)
            {
                HandleServerPacket(packetTypeL1, reader, whoAmI);
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                HandleClientPacket(packetTypeL1, reader);
            }
        }

        #region // Handles packets on the server side
        private void HandleServerPacket(PacketType packetTypeL1, BinaryReader reader, int whoAmI)
        {
            switch (packetTypeL1)
            {
                case PacketType.PacketManager:
                    HandleServerPacketManagerPacket(reader, whoAmI);
                    break;
                case PacketType.BossEventsManager:
                    HandleServerBossEventsManagerPacket(reader, whoAmI);
                    break;
                case PacketType.BossFightManager:
                    //
                    break;
            }
        }

        private void HandleServerPacketManagerPacket(BinaryReader reader, int whoAmI)
        {
            PacketType packetTypeL2 = (PacketType)reader.ReadByte();
            switch (packetTypeL2)
            {
                case PacketType.RecieveMessage:
                    Console.WriteLine($"{Main.player[whoAmI].name}: {reader.ReadString()}");
                    break;
            }
        }

        // Handles BossEventsManager packets on the server side
        private void HandleServerBossEventsManagerPacket(BinaryReader reader, int whoAmI)
        {
            BossEventsManager.PacketType packetTypeL2 = (BossEventsManager.PacketType)reader.ReadByte();
            switch (packetTypeL2)
            {
                case BossEventsManager.PacketType.DamageDealt:
                    int damageDone = reader.ReadInt32();
                    BossFightManager.AddDamage(whoAmI, damageDone);
                    //SendMessage($"{Main.player[whoAmI].name} has doene {damageDone}dmg");
                    break;
            }
        }
        #endregion

        #region// Handles packets on the client side
        private void HandleClientPacket(PacketType packetTypeL1, BinaryReader reader)
        {
            switch (packetTypeL1)
            {
                case PacketType.PacketManager:
                    HandleClientPacketManagerPacket(reader);
                    break;
                case PacketType.BossEventsManager:
                    // 
                    break;
                case PacketType.BossFightManager:
                    HandleClientBossFightManagerPacket(reader);
                    break;  
            }
        }

        private void HandleClientPacketManagerPacket(BinaryReader reader)
        {
            PacketType packetTypeL2 = (PacketType)reader.ReadByte();
            switch (packetTypeL2)
            {
                case PacketType.RecieveMessage:
                    Main.NewText($"Server: {reader.ReadString()}");
                    break;
            }
        }

        // Handles BossFightManager packets on the client side
        private void HandleClientBossFightManagerPacket(BinaryReader reader)
        {
            BossFightManager.PacketType packetTypeL2 = (BossFightManager.PacketType)reader.ReadByte();
            switch (packetTypeL2)
            {
                case BossFightManager.PacketType.BossFightActive:
                    BossFightManager.activeBossFight = reader.ReadBoolean();
                    break;
                case BossFightManager.PacketType.BossFightStats:
                    int recordCount = reader.ReadInt32();
                    for (int i = 0; i < recordCount; ++i)
                    {
                        int playerID = reader.ReadInt32();
                        string playerName = playerID == 255 ? "Environment" : Main.player[playerID].name;
                        int damageDone = reader.ReadInt32();
                        double percentage = reader.ReadDouble();

                        // Display damage stats in the chat
                        Main.NewText($"{playerName}:");
                        Main.NewText($"Total damage: {damageDone}");
                        Main.NewText($"Percentage: {percentage:0.##}%");
                        Main.NewText("");
                    }
                    break;
            }
        }
        #endregion
    }
}