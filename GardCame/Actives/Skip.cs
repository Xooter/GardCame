using GardCame.GameLoop;
using GardCame.Interfaces;
using GardCame.Models;
using System.Diagnostics;

namespace GardCame.Actives
{
    public class Skip : IActive
    {
        public void Active(Card card)
        {
            if (Program.Network.IsServer)
                NextTurnServerRpc("2");
            else
                Program.Network.Send($"{nameof(Main.NextTurnServerRpc)}|2;");
        }

        public void NextTurnServerRpc(string turnCount = "2")
        {
            if (Program.Network.IsServer)
            {
                Program.Network.Sync($"NextTurnClientRpc|{turnCount};");
            }
            else
            {
                Program.Network.Send($"{nameof(NextTurnServerRpc)}|{turnCount};");
            }
        }
    }
}
