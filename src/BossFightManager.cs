using Steamworks;
using System.Collections.Generic;
using System.Linq;

namespace LPBossFightStats
{
    public class BossFightManager
    {
        private static readonly BossFightManager instance = new BossFightManager();
        private List<BossFightStats> activeBossFights = new List<BossFightStats>();

        private BossFightManager() { }

        public static BossFightManager Instance => instance;

        public void AddBossFight(int npcID)
        {
            lock (activeBossFights)
            {
                BossFightStats bossFight = new BossFightStats(npcID);
                activeBossFights.Add(bossFight);
            }
        }

        public void AddDamage(int npcID, int playerID, int damageDone)
        {
            lock (activeBossFights)
            {
                var bossFight = activeBossFights.FirstOrDefault(bf => bf.NpcID == npcID);
                if (bossFight == null) return;

                var playerStats = bossFight.EngagedPlayers.FirstOrDefault(ps => ps.PlayerID == playerID);
                if (playerStats != null)
                {
                    playerStats.TotalDamage += damageDone;
                }
                else
                {
                    bossFight.EngagedPlayers.Add(new PlayerStats(playerID, damageDone));
                }
            }
        }

        public void RemoveBossFight(int npcID)
        {
            lock (activeBossFights)
            {
                var bossFight = activeBossFights.FirstOrDefault(bf => bf.NpcID == npcID);
                activeBossFights.Remove(bossFight);
            }
        }

        public List<PlayerStats> GetPlayerStats(int npcID)
        {
            lock (activeBossFights)
            {
                var bossFight = activeBossFights.FirstOrDefault(bf => bf.NpcID == npcID);
                return bossFight?.EngagedPlayers.ToList() ?? new List<PlayerStats>();
            }
        }
    }

    public class BossFightStats
    {
        public int NpcID { get; }
        public int TotalDamage { get; set; }
        public List<PlayerStats> EngagedPlayers { get; }

        public BossFightStats(int npcID, int totalDamage = 0)
        {
            NpcID = npcID;
            TotalDamage = totalDamage;
            EngagedPlayers = new List<PlayerStats>();
        }
    }

    public class PlayerStats
    {
        public int PlayerID { get; }
        public int TotalDamage { get; set; }

        public PlayerStats(int playerID, int totalDamage = 0)
        {
            PlayerID = playerID;
            TotalDamage = totalDamage;
        }
    }
}
