using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats.src
{
    public class PlayerEventsManager : ModPlayer
    {
        // Enum for defining the secondary packet types
        public enum PacketTypeL2 : byte
        {
            DeathReport,
            DamageTaken,
            DamageDealt
        }

        #region // Send BossEventsManager packets

        // Sends death report information to server
        private void SendDamageTaken()
        {
            try
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)PacketManager.PacketTypeL1.PlayerEventsManager);
                packet.Write((byte)PacketTypeL2.DeathReport);
                packet.Send();
            }
            catch (Exception ex)
            {
                Mod.Logger.Error("Error sending death report packet: " + ex.Message);
            }
        }

        // Sends damage taken information to server
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

        // Sends damage dealt information to server
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

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (!BossFightManager.IsBossFightActive || Player.whoAmI != Main.myPlayer)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendDamageTaken();
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                BossFightManager.AddDeathReport(Main.myPlayer);
            }
        }

        // Triggered when a Player takes damage
        public override void OnHurt(Player.HurtInfo info)
        {
            if(!BossFightManager.IsBossFightActive || Player.whoAmI != Main.myPlayer)
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
            if (!BossFightManager.IsBossFightActive || Player.whoAmI != Main.myPlayer)
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