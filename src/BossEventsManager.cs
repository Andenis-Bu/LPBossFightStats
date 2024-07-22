using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats.src
{
    public class BossEventsManager : GlobalNPC
    {
        #region // Handle BossEventsManager sent packets

        // Enum for defining the secondary packet types
        public enum PacketTypeL2 : byte
        {
            DamageDealt
        }

        // Sends damage dealt information to clients
        private void SendDamageDealt(int damageDealt)
        {
            try
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketTypeL1.BossEventsManager);
                packet.Write((byte)PacketTypeL2.DamageDealt);
                packet.Write(damageDealt);
                packet.Send();
            }
            catch (Exception ex)
            {
                Mod.Logger.Error("Error sending damage dealt packet: " + ex.Message);
            }
        }

        #endregion

        // Triggered when an NPC spawns
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!npc.boss)
                return;

            // Activate boss fight if a boss NPC spawns on the server
            if (Main.netMode == NetmodeID.Server)
            {
                BossFightManager.IsBossFightActive = true;
            }
        }

        // Triggered when an NPC is hit by an item
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!IsValidBossHit(npc))
                return;

            HandleDamage(damageDone);
        }

        // Triggered when an NPC is hit by a projectile
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!IsValidBossHit(npc))
                return;

            HandleDamage(damageDone);
        }

        // Checks if the hit is valid for boss fight statistics
        private bool IsValidBossHit(NPC npc)
        {
            return BossFightManager.IsBossFightActive && !npc.friendly;
        }

        // Handles damage dealt in multiplayer scenarios
        private void HandleDamage(int damageDone)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendDamageDealt(damageDone);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                BossFightManager.AddDamage(Main.myPlayer, damageDone); // Ensure correct player ID
            }
        }
    }
}
