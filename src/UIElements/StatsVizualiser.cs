using System;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LPBossFightStats.src.UIElements
{
    public class StatsVizualiser
    {
        public static void DisplayStats(BossFightStats bossFightStats)
        {
            var config = ModConfigs.Instance;

            if (!config.CompactMode)
            {
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
            else
            {
                string globalStatsText = string.Empty;
                string globalStatsSeparator = " [c/E1B500:|] ";

                if (config.ShowFightDuration)
                    globalStatsText += $"[c/{AdjustColor(config.FightDurationTextColor).Hex3()}:[][c/{config.FightDurationTextColor.Hex3()}:{bossFightStats.TotalFightDuration}][c/{AdjustColor(config.FightDurationTextColor).Hex3()}:]]";
                if (config.ShowDeathCount)
                {
                    if (globalStatsText.Length > 0) globalStatsText += globalStatsSeparator;
                    globalStatsText += $"[c/{config.DeathCountTextColor.Hex3()}:{bossFightStats.TotalDeathCount}][c/{AdjustColor(config.DeathCountTextColor).Hex3()}: deaths]";
                }
                if (config.ShowDamageTaken)
                {
                    if (globalStatsText.Length > 0) globalStatsText += globalStatsSeparator;
                    globalStatsText += $"[c/{config.DamageTakenTextColor.Hex3()}:{bossFightStats.TotalDamageTaken}][c/{AdjustColor(config.DamageTakenTextColor).Hex3()}: damage taken]";
                }
                if (config.ShowKillCount)
                {
                    if (globalStatsText.Length > 0) globalStatsText += globalStatsSeparator;
                    globalStatsText += $"[c/{config.KillCountTextColor.Hex3()}:{bossFightStats.TotalKillCount}][c/{AdjustColor(config.KillCountTextColor).Hex3()}: kills]";
                }
                if (config.ShowDamageDealt)
                {
                    if (globalStatsText.Length > 0) globalStatsText += globalStatsSeparator;
                    globalStatsText += $"[c/{config.DamageDealtTextColor.Hex3()}:{bossFightStats.TotalDamageDealt}][c/{AdjustColor(config.DamageDealtTextColor).Hex3()}: damage dealt]";
                }

                if (globalStatsText.Length > 0) Main.NewText($"[c/E1B500:===]{globalStatsText}[c/E1B500:===]");

                // Sort players by damage dealt so the highest damage dealer is first
                List<PlayerStats> players = bossFightStats.EngagedPlayers;
                for (int i = 0; i < players.Count; i++)
                {
                    for (int j = i + 1; j < players.Count; j++)
                    {
                        if (players[i].DamageDealt < players[j].DamageDealt)
                        {
                            PlayerStats temp = players[i];
                            players[i] = players[j];
                            players[j] = temp;
                        }
                    }
                }

                string playerStatsSeparator = "[c/E1B500:,] ";

                for (int i = 0; i < players.Count; i++)
                {
                    PlayerStats player = players[i];
                    string playerStatsText = string.Empty;

                    if (player.PlayerID == 255)
                    {
                        string playerName = $"[c/E1B500:{i+1}. Environment]";
                        string playerData = string.Empty;

                        // Add ": " if is going to show information about the player
                        if (config.ShowDamageDealt || config.ShowDamagePercent)
                            playerName += "[c/E1B500::] ";

                        if (config.ShowDamageDealt)
                            playerData += $"[c/{config.DamageDealtTextColor.Hex3()}:{player.DamageDealt}] [c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:damage dealt]";
                        if (config.ShowDamagePercent)
                        {
                            if (playerData.Length > 0) playerData += $" [c/{AdjustColor(config.DamagePercentTextColor).Hex3()}:(][c/{config.DamagePercentTextColor.Hex3()}:{player.DamagePercent:0.##}%][c/{AdjustColor(config.DamagePercentTextColor).Hex3()}:)] ";
                            else playerData += $"[c/{config.DamagePercentTextColor.Hex3()}:{player.DamagePercent:0.##}%]";
                        }
                        if (config.ShowDamageDealt)
                            playerData += $" [c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:in] [c/{config.DamageDealtTextColor.Hex3()}:{player.HitsDealt}] [c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:hits]";

                        playerStatsText = $"{playerName}{playerData}";
                    }
                    else
                    {
                        string playerName = $"[c/E1B500:{i+1}. {Main.player[player.PlayerID].name}]";
                        string playerData = string.Empty;

                        // Add ": " if is going to show information about the player
                        if (config.ShowDeathCount || config.ShowDamageTaken || config.ShowDamageDealt || config.ShowDamagePercent)
                            playerName += "[c/E1B500::] ";

                        if (config.ShowDamageDealt)
                            playerData += $"[c/{config.DamageDealtTextColor.Hex3()}:{player.DamageDealt}] [c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:damage dealt]";
                        if (config.ShowDamagePercent)
                        {
                            if (playerData.Length > 0) playerData += $" [c/{AdjustColor(config.DamagePercentTextColor).Hex3()}:(][c/{config.DamagePercentTextColor.Hex3()}:{player.DamagePercent:0.##}%][c/{AdjustColor(config.DamagePercentTextColor).Hex3()}:)] ";
                            else playerData += $"[c/{config.DamagePercentTextColor.Hex3()}:{player.DamagePercent:0.##}%]";
                        }
                        if (config.ShowDamageDealt)
                            playerData += $" [c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:in] [c/{config.DamageDealtTextColor.Hex3()}:{player.HitsDealt}] [c/{AdjustColor(config.DamageDealtTextColor).Hex3()}:hits]";
                        if (config.ShowDamageTaken)
                        {
                            if (playerData.Length > 0) playerData += playerStatsSeparator;
                            playerData += $"[c/{config.DamageTakenTextColor.Hex3()}:{player.DamageTaken}] [c/{AdjustColor(config.DamageTakenTextColor).Hex3()}:damage taken in] [c/{config.DamageTakenTextColor.Hex3()}:{player.HitsTaken}] [c/{AdjustColor(config.DamageTakenTextColor).Hex3()}:hits]";
                        }
                        if (config.ShowDeathCount)
                        {
                            if (playerData.Length > 0) playerData += playerStatsSeparator;
                            playerData += $"[c/{config.DeathCountTextColor.Hex3()}:{player.DeathCount}] [c/{AdjustColor(config.DeathCountTextColor).Hex3()}:deaths]";
                        }

                        playerStatsText = $"{playerName}{playerData}";
                    }

                    Main.NewText($"{playerStatsText}");
                }
            }
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
