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
            PlayerEventsManager,
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
                case PacketTypeL1.PlayerEventsManager:
                    HandleServerPlayerEventsManagerPacket(reader, whoAmI);
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

        // Handles PlayerEventsManager packets on the server side
        private void HandleServerPlayerEventsManagerPacket(BinaryReader reader, int whoAmI)
        {
            try
            {
                PlayerEventsManager.PacketTypeL2 packetTypeL2 = (PlayerEventsManager.PacketTypeL2)reader.ReadByte();
                switch (packetTypeL2)
                {
                    case PlayerEventsManager.PacketTypeL2.DamageTaken:
                        int DamageTaken = reader.ReadInt32();
                        BossFightManager.AddDamageTaken(whoAmI, DamageTaken);
                        break;
                    case PlayerEventsManager.PacketTypeL2.DamageDealt:
                        int DamageDealt = reader.ReadInt32();
                        BossFightManager.AddDamageDealt(whoAmI, DamageDealt);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error handling server PlayerEventsManager packet: " + ex.Message);
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
                case PacketTypeL1.PlayerEventsManager:
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
                        string totalFightDuration = reader.ReadString();
                        int totalDamageTaken = reader.ReadInt32();
                        int totalDamageDealt = reader.ReadInt32();

                        BossFightStats bossFightStats = new BossFightStats();

                        bossFightStats.TotalFightDuration = totalFightDuration;
                        bossFightStats.TotalDamageTaken = totalDamageTaken;
                        bossFightStats.TotalDamageDealt = totalDamageDealt;

                        int recordCount = reader.ReadInt32();
                        for (int i = 0; i < recordCount; ++i)
                        {
                            int playerID = reader.ReadInt32();
                            int damageTaken = reader.ReadInt32();
                            int hitsTaken = reader.ReadInt32();
                            int damageDealt = reader.ReadInt32();
                            int hitsDealt = reader.ReadInt32();
                            double damagePercent = reader.ReadDouble();

                            PlayerStats player = new PlayerStats(playerID);

                            player.DamageTaken = damageTaken;
                            player.HitsTaken = hitsTaken;
                            player.DamageDealt = damageDealt;
                            player.HitsDealt = hitsDealt;
                            player.DamagePercent = damagePercent;
                            

                            bossFightStats.EngagedPlayers.Add(player);
                        }

                        BossFightManager.BossFightStats = bossFightStats;
                        BossFightManager.DisplayBossFightStats();
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
