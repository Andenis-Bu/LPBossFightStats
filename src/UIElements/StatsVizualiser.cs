using System;
using Terraria;
using Microsoft.Xna.Framework;

namespace LPBossFightStats.src.UIElements
{
    public class StatsVizualiser
    {
        public static void DisplayStats(BossFightStats bossFightStats)
        {
            var config = ModConfigs.Instance;

            Main.NewText("[c/E1B500:======Bossfight stats======]");

            if (config.ShowFightDuration)
                Main.NewText($"[c/{AdjustColor(config.FightDurationTextColor).Hex3()}:Total Fight Duration:] [c/{config.FightDurationTextColor.Hex3()}:{bossFightStats.TotalFightDuration}]");
            if (config.ShowDeathCount)
                Main.NewText($"[c/{AdjustColor(config.DeathCountTextColor).Hex3()}:Total Death Count:] [c/{config.DeathCountTextColor.Hex3()}:{bossFightStats.TotalDeathCount}]");
            if (config.ShowDamageTaken)
                Main.NewText($"[c/{AdjustColor(config.DamageTakenTextColor).Hex3()}:Total Damage Taken:] [c/{config.DamageTakenTextColor.Hex3()}:{bossFightStats.TotalDamageTaken}]");
            if (config.ShowKillCount)
                Main.NewText($"[c/{AdjustColor(config.KillCountTextColor).Hex3()}:Total Kill Count:] [c/{config.KillCountTextColor.Hex3()}:{bossFightStats.TotalKillCount}]");
            if (config.ShowDamageDealt)
                Main.NewText($"[c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:Total Damage Dealt:] [c/{config.DamageDealtTextColor.Hex3()}:{bossFightStats.TotalDamageDealt}]");

            foreach (PlayerStats player in bossFightStats.EngagedPlayers)
            {
                if (player.PlayerID == 255)
                {
                    Main.NewText($"[c/E1B500:____Environment]");

                    if (config.ShowDamageDealt)
                        Main.NewText($"[c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:Damage Dealt:] [c/{config.DamageDealtTextColor.Hex3()}:{player.DamageDealt}] [c/{config.DamageDealtTextColor.Hex3()}:in] [c/{config.DamageDealtTextColor.Hex3()}:{player.HitsDealt}] [c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:hits]");
                    if (config.ShowDamagePercent)
                        Main.NewText($"[c/{AdjustColor(config.DamagePercentTextColor).Hex3()}:Damage Percent]: [c/{config.DamagePercentTextColor.Hex3()}:{player.DamagePercent:0.##}%]");
                }
                else
                {
                    Main.NewText($"[c/E1B500:____{Main.player[player.PlayerID].name}]");

                    if (config.ShowDeathCount)
                        Main.NewText($"[c/{AdjustColor(config.DeathCountTextColor).Hex3()}:Death Count:] [c/{config.DeathCountTextColor.Hex3()}:{player.DeathCount}]");
                    if (config.ShowDamageTaken)
                        Main.NewText($"[c/{AdjustColor(config.DamageTakenTextColor).Hex3()}:Damage Taken:] [c/{config.DamageTakenTextColor.Hex3()}:{player.DamageTaken}] [c/{config.DamageTakenTextColor.Hex3()}:in] [c/{config.DamageTakenTextColor.Hex3()}:{player.HitsTaken}] [c/{AdjustColor(config.DamageTakenTextColor).Hex3()}:hits]");
                    if (config.ShowDamageDealt)
                        Main.NewText($"[c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:Damage Dealt:] [c/{config.DamageDealtTextColor.Hex3()}:{player.DamageDealt}] [c/{config.DamageDealtTextColor.Hex3()}:in] [c/{config.DamageDealtTextColor.Hex3()}:{player.HitsDealt}] [c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:hits]");
                    if (config.ShowDamagePercent)
                        Main.NewText($"[c/{AdjustColor(config.DamagePercentTextColor).Hex3()}:Damage Percent:] [c/{config.DamagePercentTextColor.Hex3()}:{player.DamagePercent:0.##}%]");
                }
            }
            Main.NewText("[c/E1B500:======================]");
        }

        private static Color AdjustColor(Color baseColor)
        {
            int r = Math.Min(baseColor.R + 50, 255);
            int g = Math.Min(baseColor.G + 50, 255);
            int b = Math.Min(baseColor.B + 50, 255);
            return new Color(r, g, b);
        }
    }
}
