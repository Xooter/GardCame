using GardCame.Models;
using System.Collections.Generic;

namespace GardCame.Environment
{
    public static class Turn
    {
        private static int TurnIndex = 0;

        private static int TurnDirection = 1;


        public static int NextTurn(int skip = 1)
        {
            var PlayerList = GetPlayers();
            int PlayersCount = PlayerList.Count();

            TurnIndex += TurnDirection * skip;

            if (TurnIndex >= PlayersCount)
                TurnIndex -= PlayersCount;
            if (TurnIndex < 0)
                TurnIndex = PlayersCount - 1;

            Character playerTurn = PlayerList[TurnIndex];

            return playerTurn.Id;
        }

        public static void ReverseTurnList()
        {
            if (TurnDirection == 1)
                TurnDirection = -1;
            else
                TurnDirection = 1;
        }

        public static int CurrentTurn()
        {
            if (GetPlayers().Count() > 1)
            { 
                Character playerTurn = GetPlayers()[TurnIndex];
                return playerTurn.Id;
            }
            return -1;
        }

        public static bool IsLocalTurn()
        {
            Character playerTurn = GetPlayers()[TurnIndex];

            return Board.LocalPlayer.Id == playerTurn.Id;
        }

        private static List<Character> GetPlayers()
        {

            var List = new List<Character>(Program.Network.ConnectedSockets);
            if (Program.Network.IsServer)
            {
                List.Reverse();
                List.Add(Board.LocalPlayer);
                List.Reverse();
            }
                return List;
        }
    }
}
