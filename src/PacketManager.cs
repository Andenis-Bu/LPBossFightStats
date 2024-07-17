using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LPBossFightStats
{
    public class PacketManager : Mod
    {
        private BossFightManager bossFightManager = BossFightManager.Instance;
        public enum PacketType : byte
        {
            BossEventsManager
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            base.HandlePacket(reader, whoAmI);
            if (Main.netMode == NetmodeID.Server)
            {

                PacketType packetTypeL1 = (PacketType)reader.ReadByte();
                switch (packetTypeL1)
                {
                    case PacketType.BossEventsManager:
                        BossEventsManager.PacketType packetTypeL2 = (BossEventsManager.PacketType)reader.ReadByte();
                        switch (packetTypeL2)
                        {
                            case BossEventsManager.PacketType.DamageReport:
                                int npcID = reader.ReadInt32();
                                int damageDone = reader.ReadInt32();
                                bossFightManager.AddDamage(npcID, whoAmI, damageDone);
                                break;
                        }
                        break;
                }
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {

                PacketType packetTypeL1 = (PacketType)reader.ReadByte();
                switch (packetTypeL1)
                {
                    case PacketType.BossEventsManager:
                        BossEventsManager.PacketType packetTypeL2 = (BossEventsManager.PacketType)reader.ReadByte();
                        switch (packetTypeL2)
                        {
                            case BossEventsManager.PacketType.BossFightReport:
                                int recordCount = reader.ReadInt32();
                                for (int i = 0; i < recordCount; ++i)
                                {
                                    int playerID = reader.ReadInt32();
                                    int damageDone = reader.ReadInt32();

                                    Main.NewText(Main.player[playerID].name + ":");
                                    Main.NewText("Total damage: " + damageDone);
                                    Main.NewText("");
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}