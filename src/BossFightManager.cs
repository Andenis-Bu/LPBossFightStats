using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats.src
{
    public class BossFightManager : ModSystem
    {
        // Boss fight statistics instance
        private static BossFightStats bossFightStats = new BossFightStats();

        // Boss fight active state
        private static bool isBossFightActive = false;

        // Property to get/set boss fight active state
        public static bool IsBossFightActive
        {
            get
            {
                return isBossFightActive;
            }
            set
            {
                if (isBossFightActive != value)
                {
                    isBossFightActive = value;

                    // Server-specific logic
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModContent.GetInstance<BossFightManager>().SendBossFightActive(isBossFightActive);
                        if (value)
                        {
                            ResetBossFight();
                        }
                        else
                        {
                            ModContent.GetInstance<BossFightManager>().SendBossFightStats();
                        }
                    }
                }
            }
        }

        #region // Handle BossFightManager sent packets

        // Enum for defining the secondary packet types
        public enum PacketTypeL2 : byte
        {
            BossFightStats,
            BossFightActive
        }

        // Sends boss fight statistics to clients
        private void SendBossFightStats()
        {
            try
            {
                List<PlayerStats> playerStats = GetPlayerStats();

                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketTypeL1.BossFightManager);
                packet.Write((byte)PacketTypeL2.BossFightStats);
                packet.Write(playerStats.Count);
                foreach (PlayerStats player in playerStats)
                {
                    packet.Write(player.PlayerID);
                    packet.Write(player.DamageDealt);
                    packet.Write(player.DamagePercent);
                }
                packet.Send();
            }
            catch (Exception ex)
            {
                Mod.Logger.Error("Error sending boss fight stats: " + ex.Message);
            }
        }

        // Updates the boss fight active state on clients
        private void SendBossFightActive(bool isBossFightActive)
        {
            try
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketTypeL1.BossFightManager);
                packet.Write((byte)PacketTypeL2.BossFightActive);
                packet.Write(isBossFightActive);
                packet.Send();
            }
            catch (Exception ex)
            {
                Mod.Logger.Error("Error updating boss fight active state: " + ex.Message);
            }
        }

        #endregion

        // Checks if any active NPC is a boss
        private bool CheckBossFightActive()
        {
            // Loop through all NPCs
            foreach (NPC npc in Main.npc)
            {
                // Check if the NPC is a boss and is active
                if (npc.active && npc.boss)
                {
                    return true;
                }
            }
            return false;
        }

        // Updates the boss fight state after every game update
        public override void PostUpdateEverything()
        {
            if (Main.netMode != NetmodeID.Server)
                return; // Only execute on the server

            if (!IsBossFightActive || CheckBossFightActive())
                return; // No action needed if a boss fight is active or no boss is active

            IsBossFightActive = false; // Deactivate boss fight if no active bosses found
        }

        #region // Handle boss fight stats data

        // Adds damage to the player's statistics
        public static void AddDamage(int playerID, int damageDone)
        {
            lock (bossFightStats)
            {
                bossFightStats.TotalDamageDealt += damageDone;

                var playerStats = bossFightStats.EngagedPlayers.FirstOrDefault(ps => ps.PlayerID == playerID);
                if (playerStats == null)
                {
                    playerStats = new PlayerStats(playerID);
                    bossFightStats.EngagedPlayers.Add(playerStats);
                }

                playerStats.DamageDealt += damageDone;
            }
        }

        // Resets the boss fight statistics
        public static void ResetBossFight()
        {
            lock (bossFightStats)
            {
                bossFightStats = new BossFightStats();
            }
        }

        // Retrieves player statistics and calculates damage percentage
        public static List<PlayerStats> GetPlayerStats()
        {
            lock (bossFightStats)
            {
                foreach (PlayerStats player in bossFightStats.EngagedPlayers)
                {
                    player.DamagePercent = (player.DamageDealt / bossFightStats.TotalDamageDealt) * 100;
                }

                return bossFightStats.EngagedPlayers;
            }
        }

        #endregion

        private class BossFightStats
        {
            // Total damage dealt by all players
            public double TotalDamageDealt { get; set; }

            // List of engaged players' statistics
            public List<PlayerStats> EngagedPlayers { get; }

            public BossFightStats()
            {
                TotalDamageDealt = 0;
                EngagedPlayers = new List<PlayerStats>();
                EngagedPlayers.Add(new PlayerStats(255));
            }
        }
    }

    public class PlayerStats
    {
        // Player ID
        public int PlayerID { get; }

        // Damage dealt by the player
        public int DamageDealt { get; set; }

        // Damage percentage contribution of the player
        public double DamagePercent { get; set; }

        public PlayerStats(int playerID)
        {
            PlayerID = playerID;
            DamageDealt = 0;
            DamagePercent = 0;
        }
    }
}
