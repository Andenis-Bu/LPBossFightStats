using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats
{
    public class BossFightManager : ModSystem
    {
        private static BossFightStats bossFightStats = new BossFightStats();

        public static bool activeBossFight = false;

        public enum PacketType
        {
            BossFightStats,
            BossFightActive
        }

        #region // Send BossFightManager packets
        private void SendBossFightStats()
        {
            List<PlayerStats> playerStats = GetPlayerStats();

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)PacketManager.PacketType.BossFightManager);
            packet.Write((byte)PacketType.BossFightStats);
            packet.Write(playerStats.Count);
            foreach (PlayerStats player in playerStats)
            {
                packet.Write(player.PlayerID);
                packet.Write(player.TotalDamage);
                packet.Write(player.TotalDamagePercentage);
            }
            packet.Send();
        }

        private void SendBossFightActive(bool bossFightActive)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)PacketManager.PacketType.BossFightManager);
            packet.Write((byte)PacketType.BossFightActive);
            packet.Write(bossFightActive);
            packet.Send();
        }
        #endregion

        private bool IsBossFightActive()
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

        public override void PostUpdateEverything()
        {
            if (Main.netMode != NetmodeID.Server)
                return; 

            if (!activeBossFight || IsBossFightActive())
                return;

            activeBossFight = false;

            SendBossFightActive(false);

            SendBossFightStats();
        }

        public static void AddDamage(int playerID, int damageDone)
        {
            lock (bossFightStats)
            {
                bossFightStats.TotalDamage += damageDone;

                var playerStats = bossFightStats.EngagedPlayers.FirstOrDefault(ps => ps.PlayerID == playerID);
                if (playerStats == null)
                {
                    playerStats = new PlayerStats(playerID);
                    bossFightStats.EngagedPlayers.Add(playerStats);
                }

                playerStats.TotalDamage += damageDone;
            }
        }

        public static void ResetBossFight()
        {
            lock (bossFightStats)
            {
                bossFightStats = new BossFightStats();
            }
        }

        public static List<PlayerStats> GetPlayerStats()
        {
            lock (bossFightStats)
            {
                foreach (PlayerStats player in bossFightStats.EngagedPlayers)
                {
                    player.TotalDamagePercentage = (player.TotalDamage / bossFightStats.TotalDamage) * 100;
                }

                return bossFightStats.EngagedPlayers;
            }
        }
    }

    public class BossFightStats
    {
        public double TotalDamage { get; set; }
        public List<PlayerStats> EngagedPlayers { get; }

        public BossFightStats()
        {
            TotalDamage = 0;
            EngagedPlayers = new List<PlayerStats>();
        }
    }

    public class PlayerStats
    {
        public int PlayerID { get; }
        public int TotalDamage { get; set; }
        public double TotalDamagePercentage { get; set; }

        public PlayerStats(int playerID)
        {
            PlayerID = playerID;
            TotalDamage = 0;
            TotalDamagePercentage = 0;
        }
    }
}