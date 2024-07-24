using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats.src
{
    internal class PlayerEventsManager : ModPlayer
    {
        // Enum for defining the secondary packet types
        public enum PacketTypeL2 : byte
        {
            DamageTaken,
            DamageDealt
        }

        #region // Send BossEventsManager packets

        // Sends damage taken information to clients
        private void SendDamageTaken(int damageTaken)
        {
            try
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketTypeL1.PlayerEventsManager);
                packet.Write((byte)PacketTypeL2.DamageTaken);
                packet.Write(damageTaken);
                packet.Send();
            }
            catch (Exception ex)
            {
                Mod.Logger.Error("Error sending damage taken packet: " + ex.Message);
            }
        }

        // Sends damage dealt information to clients
        private void SendDamageDealt(int damageDealt)
        {
            try
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketTypeL1.PlayerEventsManager);
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

        // Triggered when a Player takes damage
        public override void OnHurt(Player.HurtInfo info)
        {
            if(!BossFightManager.IsBossFightActive)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendDamageTaken(info.Damage);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                BossFightManager.AddDamageTaken(Main.myPlayer, info.Damage);
            }
        }

        // Triggered when a Player deals damage
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!BossFightManager.IsBossFightActive)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendDamageDealt(damageDone);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                BossFightManager.AddDamageDealt(Main.myPlayer, damageDone);
            }
        }
    }
}