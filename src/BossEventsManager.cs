using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats
{
    public class BossEventsManager : GlobalNPC
    {
        public enum PacketType : byte
        {
            DamageDealt,
            DamageTaken 
        }

        #region // Send BossEventsManager packets
        private void SendDamageDealt(int damageDone)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)PacketManager.PacketType.BossEventsManager);
            packet.Write((byte)PacketType.DamageDealt);
            packet.Write(damageDone);
            packet.Send();
        }

        private void SendBossFightActive(bool bossFightActive)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)PacketManager.PacketType.BossFightManager);
            packet.Write((byte)BossFightManager.PacketType.BossFightActive);
            packet.Write(bossFightActive);
            packet.Send();
        }
        #endregion

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!npc.boss)
                return;

            if (Main.netMode == NetmodeID.Server)
            {
                if (!BossFightManager.activeBossFight)
                {
                    BossFightManager.ResetBossFight();
                    BossFightManager.activeBossFight = true;
                    SendBossFightActive(true);
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            if (!npc.boss)
                return;
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!BossFightManager.activeBossFight)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendDamageDealt(damageDone);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                BossFightManager.AddDamage(Main.myPlayer, damageDone);
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!BossFightManager.activeBossFight)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendDamageDealt(damageDone);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                BossFightManager.AddDamage(Main.myPlayer, damageDone);
            }
        }
    }
}