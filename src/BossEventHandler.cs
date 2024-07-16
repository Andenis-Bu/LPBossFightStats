using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace LPBossFightStats
{
    public class BossEventHandler : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        PacketHandler packetHandler = ModContent.GetInstance<PacketHandler>();

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
            if (npc.boss)
            {
                BossFightStats.AddBossFight(npc.whoAmI);
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByItem(npc, player, item, hit, damageDone);
            if (npc.boss)
            {
                BossFightStats.AddDamage(npc.whoAmI, player.whoAmI, damageDone);
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
            if (npc.boss)
            {
                BossFightStats.AddDamage(npc.whoAmI, projectile.owner, damageDone);
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            if (npc.boss)
            {
                
                List<PlayerStats> engagedPlayers = BossFightStats.GetPlayerStats(npc.whoAmI);
                foreach (PlayerStats playerStats in engagedPlayers)
                {
                    Main.NewText(playerStats.TotalDamage);
                }
                BossFightStats.RemoveBossFight(npc.whoAmI);
            }
        }
    }
}
