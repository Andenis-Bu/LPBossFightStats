using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats
{
    public class BossEventsManager : GlobalNPC
    {
        private static BossFightManager bossFightManager = BossFightManager.Instance;
        public enum PacketType : byte
        {
            DamageReport,
            BossFightReport
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
            if (!npc.boss)
                return;
                
            if (Main.netMode == NetmodeID.Server)
            {
                bossFightManager.AddBossFight(npc.whoAmI);
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            if (!npc.boss)
                return;

            if (Main.netMode == NetmodeID.Server)
            {
                List<PlayerStats> playerStats = bossFightManager.GetPlayerStats(npc.whoAmI);

                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketType.BossEventsManager);
                packet.Write((byte)PacketType.BossFightReport);
                packet.Write(playerStats.Count);
                foreach (PlayerStats player in playerStats)
                {
                    packet.Write(player.PlayerID);
                    packet.Write(player.TotalDamage);
                }
                packet.Send();

                bossFightManager.RemoveBossFight(npc.whoAmI);
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByItem(npc, player, item, hit, damageDone);
            if (!npc.boss)
                return;

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)PacketManager.PacketType.BossEventsManager);
            packet.Write((byte)PacketType.DamageReport);
            packet.Write(npc.whoAmI);
            packet.Write(damageDone);
            packet.Send();
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
            if (!npc.boss)
                return;
                
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketType.BossEventsManager);
                packet.Write((byte)PacketType.DamageReport);
                packet.Write(npc.whoAmI);
                packet.Write(damageDone);
                packet.Send();
            }
        }
    }
}