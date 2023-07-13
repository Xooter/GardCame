using GardCame.Environment;
using GardCame.Interfaces;
using GardCame.Models;
using System.Diagnostics;

namespace GardCame.Actives
{
    public class JackQueen : IActive
    {
        public void Active(Card card)
        {
            Board.Ammount += 10;
        }
    }
}
