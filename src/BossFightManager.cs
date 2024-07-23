using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                            bossFightStats.StartBossFightTimer();
                        }
                        else
                        {
                            bossFightStats.StopBossFightTimer();
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
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketTypeL1.BossFightManager);
                packet.Write((byte)PacketTypeL2.BossFightStats);

                packet.Write(bossFightStats.TotalFightDuration);
                packet.Write(bossFightStats.TotalDamageTaken);
                packet.Write(bossFightStats.TotalDamageDealt);

                List<PlayerStats> playerStats = GetPlayerStats();
                packet.Write(playerStats.Count);

                foreach (PlayerStats player in playerStats)
                {
                    packet.Write(player.PlayerID);
                    packet.Write(player.DamageTaken);
                    packet.Write(player.HitsTaken);
                    packet.Write(player.DamageDealt);
                    packet.Write(player.HitsDealt);
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

        // Adds damage taken to the player's statistics
        public static void AddDamageTaken(int playerID, int damageTaken)
        {
            lock (bossFightStats)
            {
                bossFightStats.TotalDamageTaken += damageTaken;

                var playerStats = bossFightStats.EngagedPlayers.FirstOrDefault(ps => ps.PlayerID == playerID);
                if (playerStats == null)
                {
                    playerStats = new PlayerStats(playerID);
                    bossFightStats.EngagedPlayers.Add(playerStats);
                }

                playerStats.DamageTaken += damageTaken;
                playerStats.HitsTaken += 1;
            }
        }


        // Adds damage dealt to the player's statistics
        public static void AddDamageDealt(int playerID, int damageDealt)
        {
            lock (bossFightStats)
            {
                bossFightStats.TotalDamageDealt += damageDealt;

                var playerStats = bossFightStats.EngagedPlayers.FirstOrDefault(ps => ps.PlayerID == playerID);
                if (playerStats == null)
                {
                    playerStats = new PlayerStats(playerID);
                    bossFightStats.EngagedPlayers.Add(playerStats);
                }

                playerStats.DamageDealt += damageDealt;
                playerStats.HitsDealt += 1;
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
                    player.DamagePercent = ((double)player.DamageDealt / bossFightStats.TotalDamageDealt) * 100;
                }

                return bossFightStats.EngagedPlayers;
            }
        }

        private class BossFightStats
        {
            // Private field to hold the stopwatch instance
            private Stopwatch totalFightDuration;

            // Public property to get the formatted fight duration
            public string TotalFightDuration => string.Format("{0:D2}:{1:D2}:{2:D2}",
                                                              totalFightDuration.Elapsed.Hours,
                                                              totalFightDuration.Elapsed.Minutes,
                                                              totalFightDuration.Elapsed.Seconds);

            // Total damage taken by all players
            public int TotalDamageTaken { get; set; }

            // Total damage dealt by all players
            public int TotalDamageDealt { get; set; }

            // List of engaged players' statistics
            public List<PlayerStats> EngagedPlayers { get; }

            public BossFightStats()
            {
                totalFightDuration = new Stopwatch();
                TotalDamageDealt = 0;

                EngagedPlayers = new List<PlayerStats>();
                EngagedPlayers.Add(new PlayerStats(255));
            }

            // Method to start the stopwatch
            public void StartBossFightTimer()
            {
                totalFightDuration.Restart();
            }

            // Method to stop the stopwatch
            public void StopBossFightTimer()
            {
                totalFightDuration.Stop();
            }
        }

        #endregion
    }

    public class PlayerStats
    {
        // Player ID
        public int PlayerID { get; }

        // Damage taken by the player
        public int DamageTaken { get; set; }

        // Hits taken by the player
        public int HitsTaken { get; set; }

        // Damage dealt by the player
        public int DamageDealt { get; set; }

        // Hits dealt by the player
        public int HitsDealt { get; set; }

        // Damage percentage contribution of the player
        public double DamagePercent { get; set; }

        public PlayerStats(int playerID)
        {
            PlayerID = playerID;
            DamageTaken = 0;
            HitsTaken = 0;
            DamageDealt = 0;
            HitsDealt = 0;
            DamagePercent = 0;
        }
    }
}
