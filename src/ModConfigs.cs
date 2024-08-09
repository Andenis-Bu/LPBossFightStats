using Terraria.ModLoader.Config;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ModLoader;

namespace LPBossFightStats.src
{
    public class ModConfigs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
    
        // Fight Duration
        [DefaultValue(true)]
        public bool ShowFightDuration { get; set; }
    
        [DefaultValue(typeof(Color), "158, 143, 236, 255"), ColorNoAlpha]
        public Color FightDurationTextColor { get; set; }
    
        // Death Count
        [DefaultValue(true)]
        public bool ShowDeathCount { get; set; }
    
        [DefaultValue(typeof(Color), "237, 112, 190, 255"), ColorNoAlpha]
        public Color DeathCountTextColor { get; set; }
    
        // Damage Taken
        [DefaultValue(true)]
        public bool ShowDamageTaken { get; set; }
    
        [DefaultValue(typeof(Color), "251, 114, 96, 255"), ColorNoAlpha]
        public Color DamageTakenTextColor { get; set; }
    
        // Kill Count
        [DefaultValue(true)]
        public bool ShowKillCount { get; set; }
    
        [DefaultValue(typeof(Color), "82, 162, 251, 255"), ColorNoAlpha]
        public Color KillCountTextColor { get; set; }
        
        // Damage Dealt
        [DefaultValue(true)]
        public bool ShowDamageDealt { get; set; }
    
        [DefaultValue(typeof(Color), "96, 196, 211, 255"), ColorNoAlpha]
        public Color DamageDealtTextColor { get; set; }
    
        // Damage Percent
        [DefaultValue(true)]
        public bool ShowDamagePercent { get; set; }
        
        [DefaultValue(typeof(Color), "85, 209, 157, 255"), ColorNoAlpha]
        public Color DamagePercentTextColor { get; set; }
    
        public static ModConfigs Instance => ModContent.GetInstance<ModConfigs>();
    }
}
