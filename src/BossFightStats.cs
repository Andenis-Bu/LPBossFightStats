using System.Collections.Generic;

namespace LPBossFightStats
{

    public class BossFightStats
    {
        private static List<BossFightStats> acdiveBossFights = new List<BossFightStats>();

        private int npcID;
        private int totalDamage;
        private List<PlayerStats> engagedPlayers = new List<PlayerStats>();



        private BossFightStats(int npcID, int totalDamage = 0)
        {
            this.npcID = npcID;
            this.totalDamage = totalDamage;
        }

        public static void AddBossFight(int npcID)
        {
            BossFightStats bossFight = new BossFightStats(npcID);
            acdiveBossFights.Add(bossFight);
        }

        public static void AddDamage(int npcID, int playerID, int damageDone)
        {
            foreach (BossFightStats bossFight in acdiveBossFights)
            {
                if (bossFight.npcID != npcID)
                    continue;

                foreach (PlayerStats playerStats in bossFight.engagedPlayers)
                {
                    if (playerStats.PlayerID != playerID)
                        continue;

                    playerStats.TotalDamage += damageDone;

                    return;
                }

                bossFight.engagedPlayers.Add(new PlayerStats(playerID, damageDone));
            }
        }

        public static List<PlayerStats> GetPlayerStats(int npcID)
        {
            foreach (BossFightStats bossFight in acdiveBossFights)
            {
                if (bossFight.npcID != npcID)
                    continue;

                return bossFight.engagedPlayers;
            }

            return null;
        }

        public static void RemoveBossFight(int npcID)
        {
            foreach (BossFightStats bossFight in acdiveBossFights)
            {
                if (bossFight.npcID != npcID)
                    continue;

                acdiveBossFights.Remove(bossFight);
            }
        }
    }

    public class PlayerStats
    {
        public int PlayerID { get; private set; }
        public int TotalDamage { get; set; }

        public PlayerStats(int playerID, int totalDamage = 0)
        {
            PlayerID = playerID;
            TotalDamage = totalDamage;
        }
    }
}
