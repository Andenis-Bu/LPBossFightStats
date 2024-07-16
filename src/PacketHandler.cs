using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace LPBossFightStats
{
    public class PacketHandler : Mod
    {
        public enum PacketType
        {
            SendMessage = 1,
            ReceiveMessage = 2
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            switch (packetType)
            {
                case PacketType.SendMessage:
                    string message = reader.ReadString();
                    BroadcastMessage(whoAmI, message);
                    break;
                case PacketType.ReceiveMessage:
                    int receiverPlayerId = reader.ReadInt32();
                    string receivedMessage = reader.ReadString();
                    DisplayMessage(receiverPlayerId, receivedMessage);
                    break;
            }
        }

        public void BroadcastMessage(int senderPlayerId, string message)
        {
            ModPacket packet = GetPacket();
            packet.Write((byte)PacketType.ReceiveMessage);
            packet.Write(senderPlayerId);
            packet.Write(message);
            packet.Send(); // Send to all clients
        }


        private void DisplayMessage(int senderPlayerId, string message)
        {
            Player senderPlayer = Main.player[senderPlayerId];
            Main.NewText($"{senderPlayerId}: {message}", Color.Yellow);
        }

        // Client-Side: Method to send message to server
        public void SendMessageToServer(string message)
        {
            ModPacket packet = GetPacket();
            packet.Write((byte)PacketType.SendMessage);
            packet.Write(message);
            packet.Send();
        }
    }
}