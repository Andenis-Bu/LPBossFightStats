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
            DamageTaken
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
        #endregion

        // Triggered when a Player is hurt
        public override void OnHurt(Player.HurtInfo info)
        {
            if (Main.myPlayer != Player.whoAmI)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendDamageTaken(info.Damage);
            }
        }
    }
}
