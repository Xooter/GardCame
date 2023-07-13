using GardCame.GameLoop;
using GardCame.Interfaces;
using GardCame.Models;
using System.Diagnostics;

namespace GardCame.Actives
{
    public class Reverse : IActive
    {
        public void Active(Card card)
        {
            if (Program.Network.IsServer)
                ReverseTurnServerRpc();
            else
                Program.Network.Send($"{nameof(Main.ReverseTurnServerRpc)};");
        }
        public void ReverseTurnServerRpc()
        {
            if (Program.Network.IsServer)
            {
                Program.Network.Sync($"{nameof(Main.ReverseTurnClientRpc)};");
            }
            else
            {
                Program.Network.Send($"{nameof(Main.ReverseTurnServerRpc)};");
            }
        }

    }
}
