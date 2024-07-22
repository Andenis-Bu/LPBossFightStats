using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats.src
{
    public class PacketManager : Mod
    {
        // Enum for defining the primary packet types
        public enum PacketTypeL1 : byte
        {
            PacketManager,
            BossEventsManager,
            BossFightManager
        }

        #region // Handle PacketManager sent packets

        // Enum for defining the secondary packet types
        public enum PacketTypeL2 : byte
        {
            RecieveMessage
        }

        // Sends a message packet
        public void SendMessage(string message)
        {
            try
            {
                ModPacket packet = GetPacket();
                packet.Write((byte)PacketTypeL1.PacketManager);
                packet.Write((byte)PacketTypeL2.RecieveMessage);
                packet.Write(message);
                packet.Send();
            }
            catch (Exception ex)
            {
                Logger.Error("Error sending message packet: " + ex.Message);
            }
        }

        #endregion

        // Main method for handling incoming packets
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            base.HandlePacket(reader, whoAmI);

            try
            {
                // Read the primary packet type
                PacketTypeL1 packetTypeL1 = (PacketTypeL1)reader.ReadByte();

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
            catch (Exception ex)
            {
                Logger.Error("Error handling packet: " + ex.Message);
            }
        }

        #region // Handles packets on the server side

        // Handles server-side packets based on the primary packet type
        private void HandleServerPacket(PacketTypeL1 packetTypeL1, BinaryReader reader, int whoAmI)
        {
            switch (packetTypeL1)
            {
                case PacketTypeL1.PacketManager:
                    HandleServerPacketManagerPacket(reader, whoAmI);
                    break;
                case PacketTypeL1.BossEventsManager:
                    HandleServerBossEventsManagerPacket(reader, whoAmI);
                    break;
                case PacketTypeL1.BossFightManager:
                    // Add handling logic if needed
                    break;
            }
        }

        // Handles PacketManager packets on the server side
        private void HandleServerPacketManagerPacket(BinaryReader reader, int whoAmI)
        {
            try
            {
                PacketTypeL2 packetTypeL2 = (PacketTypeL2)reader.ReadByte();
                switch (packetTypeL2)
                {
                    case PacketTypeL2.RecieveMessage:
                        Console.WriteLine($"{Main.player[whoAmI].name}: {reader.ReadString()}");
                        break;
                }
            }
            catch (Exception ex)
            {
               Logger.Error("Error handling server PacketManager packet: " + ex.Message);
            }
        }

        // Handles BossEventsManager packets on the server side
        private void HandleServerBossEventsManagerPacket(BinaryReader reader, int whoAmI)
        {
            try
            {
                BossEventsManager.PacketTypeL2 packetTypeL2 = (BossEventsManager.PacketTypeL2)reader.ReadByte();
                switch (packetTypeL2)
                {
                    case BossEventsManager.PacketTypeL2.DamageDealt:
                        int DamageDealt = reader.ReadInt32();
                        BossFightManager.AddDamage(whoAmI, DamageDealt);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error handling server BossEventsManager packet: " + ex.Message);
            }
        }

        #endregion

        #region // Handles packets on the client side

        // Handles client-side packets based on the primary packet type
        private void HandleClientPacket(PacketTypeL1 packetTypeL1, BinaryReader reader)
        {
            switch (packetTypeL1)
            {
                case PacketTypeL1.PacketManager:
                    HandleClientPacketManagerPacket(reader);
                    break;
                case PacketTypeL1.BossEventsManager:
                    // Add handling logic if needed
                    break;
                case PacketTypeL1.BossFightManager:
                    HandleClientBossFightManagerPacket(reader);
                    break;
            }
        }

        // Handles PacketManager packets on the client side
        private void HandleClientPacketManagerPacket(BinaryReader reader)
        {
            try
            {
                PacketTypeL2 packetTypeL2 = (PacketTypeL2)reader.ReadByte();
                switch (packetTypeL2)
                {
                    case PacketTypeL2.RecieveMessage:
                        Main.NewText($"Server: {reader.ReadString()}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error handling client PacketManager packet: " + ex.Message);
            }
        }

        // Handles BossFightManager packets on the client side
        private void HandleClientBossFightManagerPacket(BinaryReader reader)
        {
            try
            {
                BossFightManager.PacketTypeL2 packetTypeL2 = (BossFightManager.PacketTypeL2)reader.ReadByte();
                switch (packetTypeL2)
                {
                    case BossFightManager.PacketTypeL2.BossFightActive:
                        BossFightManager.IsBossFightActive = reader.ReadBoolean();
                        break;
                    case BossFightManager.PacketTypeL2.BossFightStats:
                        int recordCount = reader.ReadInt32();
                        for (int i = 0; i < recordCount; ++i)
                        {
                            int playerID = reader.ReadInt32();
                            string playerName = playerID == 255 ? "Environment" : Main.player[playerID].name;
                            int damageDone = reader.ReadInt32();
                            double damagePercent = reader.ReadDouble();

                            // Display damage stats in the chat
                            Main.NewText($"{playerName}:");
                            Main.NewText($"Damage Dealt: {damageDone}");
                            Main.NewText($"Damage Percent: {damagePercent:0.##}%");
                            Main.NewText("");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error handling client BossFightManager packet: " + ex.Message);
            }
        }

        #endregion
    }
}
