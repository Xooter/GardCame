using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardCame.Models
{
    public class BlankCard : Card
    {
        public override Panel RenderCard()
        {
            Panel card = new Panel($@"    ╭─────────╮
    │▓▓▓▓▓▓▓▓▓│
    │▓▓▓▓▓▓▓▓▓│
    │▓▓▓▓▓▓▓▓▓│
    │▓▓▓▓▓▓▓▓▓│
    │▓▓▓▓▓▓▓▓▓│
    ╰─────────╯")
                    .Expand()
                    .Border(BoxBorder.Heavy);
            card.Width = 10;
            return card;
        }
    }
}
