using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI;

namespace StoneDuster
{
    public static class Comms
    {
        public static void MessageHandler(byte[] data)
        {

            try
            {
                var receivedData = MyAPIGateway.Utilities.SerializeFromBinary<SyncData>(data);
                ActionControls.SyncDrillData(receivedData);
            }
            catch (Exception exc)
            {
            }
        }

        public static void SyncToOthers(SyncData data)
        {
            try
            {
                var sendData = MyAPIGateway.Utilities.SerializeToBinary(data);
                List<IMyPlayer> playerList = new List<IMyPlayer>();
                MyAPIGateway.Players.GetPlayers(playerList);
                foreach (var player in playerList)
                {
                    if (player == null || player.IsBot) continue;
                    if (player == MyAPIGateway.Session.LocalHumanPlayer) continue;
                    
                    MyAPIGateway.Multiplayer.SendMessageTo(4700, sendData, player.SteamUserId);
                }

                MyAPIGateway.Multiplayer.SendMessageToServer(4700, sendData);
            }
            catch (Exception exc)
            {

            }
        }
    }
}
