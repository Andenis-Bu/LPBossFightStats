using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LPBossFightStats.src.UIElements;

namespace LPBossFightStats.src
{
    public class BossFightManager : ModSystem
    {
        // Boss fight statistics instance
        public static BossFightStats BossFightStats { get; set; } = new BossFightStats();

        private static Stopwatch stopwatch = new Stopwatch();
        private static string Stopwatch => string.Format("{0:D2}:{1:D2}:{2:D2}",
                                                  stopwatch.Elapsed.Hours,
                                                  stopwatch.Elapsed.Minutes,
                                                  stopwatch.Elapsed.Seconds);

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

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        return; // Exit if client

                    if (value)
                    {
                        stopwatch.Restart();
                        ResetBossFight();
                    }
                    else
                    {
                        stopwatch.Stop();
                        BossFightStats.TotalFightDuration = Stopwatch;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            ModContent.GetInstance<BossFightManager>().SendBossFightActive();
                            ModContent.GetInstance<BossFightManager>().SendBossFightStats();
                        }
                        else if(Main.netMode == NetmodeID.SinglePlayer)
                        {
                            GetPlayerStats();
                            DisplayBossFightStats();
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

                packet.Write(BossFightStats.TotalFightDuration);
                packet.Write(BossFightStats.TotalDeathCount);
                packet.Write(BossFightStats.TotalDamageTaken);
                packet.Write(BossFightStats.TotalKillCount);
                packet.Write(BossFightStats.TotalDamageDealt);

                List<PlayerStats> playerStats = GetPlayerStats();
                packet.Write(playerStats.Count);

                foreach (PlayerStats player in playerStats)
                {
                    packet.Write(player.PlayerID);
                    packet.Write(player.DeathCount);
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
        private void SendBossFightActive()
        {
            try
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketTypeL1.BossFightManager);
                packet.Write((byte)PacketTypeL2.BossFightActive);
                packet.Write(IsBossFightActive);
                packet.Send();
            }
            catch (Exception ex)
            {
                Mod.Logger.Error("Error updating boss fight active state: " + ex.Message);
            }
        }

        #endregion

        // Updates the boss fight state after every game update
        public override void PostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return; // Exit if client

            if (IsBossFightActive)
            {
                // Checks if any active NPC is a boss
                foreach (NPC npc in Main.npc)
                {
                    // Check if the NPC is a boss and is active
                    if (npc.active && npc.boss)
                    {
                        return;
                    }
                }

                IsBossFightActive = false;
            }
        }

        #region // Handle boss fight stats data
        // Display boss fight stats
        public static void DisplayBossFightStats()
        {
            StatsVizualiser.DisplayStats(BossFightStats);
        }

        // Adds death report to the player's statistics
        public static void AddDeathReport(int playerID)
        {
            lock (BossFightStats)
            {
                BossFightStats.TotalDeathCount += 1;

                var playerStats = BossFightStats.EngagedPlayers.FirstOrDefault(ps => ps.PlayerID == playerID);
                if (playerStats == null)
                {
                    playerStats = new PlayerStats(playerID);
                    BossFightStats.EngagedPlayers.Add(playerStats);
                }

                playerStats.DeathCount += 1;
            }
        }

        // Adds damage taken to the player's statistics
        public static void AddDamageTaken(int playerID, int damageTaken)
        {
            lock (BossFightStats)
            {
                BossFightStats.TotalDamageTaken += damageTaken;

                var playerStats = BossFightStats.EngagedPlayers.FirstOrDefault(ps => ps.PlayerID == playerID);
                if (playerStats == null)
                {
                    playerStats = new PlayerStats(playerID);
                    BossFightStats.EngagedPlayers.Add(playerStats);
                }

                playerStats.DamageTaken += damageTaken;
                playerStats.HitsTaken += 1;
            }
        }

        // Adds death report to the player's statistics
        public static void AddKillReport()
        {
            lock (BossFightStats)
            {
                BossFightStats.TotalKillCount += 1;
            }
        }

        // Adds damage dealt to the player's statistics
        public static void AddDamageDealt(int playerID, int damageDealt)
        {
            lock (BossFightStats)
            {
                BossFightStats.TotalDamageDealt += damageDealt;

                var playerStats = BossFightStats.EngagedPlayers.FirstOrDefault(ps => ps.PlayerID == playerID);
                if (playerStats == null)
                {
                    playerStats = new PlayerStats(playerID);
                    BossFightStats.EngagedPlayers.Add(playerStats);
                }

                playerStats.DamageDealt += damageDealt;
                playerStats.HitsDealt += 1;
            }
        }

        // Resets the boss fight statistics
        public static void ResetBossFight()
        {
            lock (BossFightStats)
            {
                BossFightStats = new BossFightStats();
            }
        }

        // Retrieves player statistics and calculates damage percentage
        public static List<PlayerStats> GetPlayerStats()
        {
            lock (BossFightStats)
            {
                foreach (PlayerStats player in BossFightStats.EngagedPlayers)
                {
                    player.DamagePercent = ((double)player.DamageDealt / BossFightStats.TotalDamageDealt) * 100;
                }

                return BossFightStats.EngagedPlayers;
            }
        }

        #endregion
    }

    public class BossFightStats
    {
        // Private field to hold the stopwatch instance
        public string TotalFightDuration { get; set; }

        // Total death count by all players
        public int TotalDeathCount { get; set; }

        // Total damage taken by all players
        public int TotalDamageTaken { get; set; }

        // Total kill count by all players
        public int TotalKillCount { get; set; }

        // Total damage dealt by all players
        public int TotalDamageDealt { get; set; }

        // List of engaged players' statistics
        public List<PlayerStats> EngagedPlayers { get; }

        public BossFightStats()
        {
            TotalFightDuration = "00:00:00";
            TotalDeathCount = 0;
            TotalDamageTaken = 0;
            TotalKillCount = 0;
            TotalDamageDealt = 0;

            EngagedPlayers = new List<PlayerStats>();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                EngagedPlayers.Add(new PlayerStats(255));
            }     
        }
    }

    public class PlayerStats
    {
        // Player ID
        public int PlayerID { get; }

        // Death count by the player
        public int DeathCount { get; set; }

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
            DeathCount = 0;
            DamageTaken = 0;
            HitsTaken = 0;
            DamageDealt = 0;
            HitsDealt = 0;
            DamagePercent = 0;
        }
    }
}