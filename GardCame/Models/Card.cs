using GardCame.Interfaces;
using Spectre.Console;

namespace GardCame.Models
{
    public class Card
    {
        public int Value { get; set; } = -1;
        public char Type { get; set; } = '░';

        public IActive Active { get; set; }
        public string Description { get; set; } = string.Empty;

        private string ValueString
        {
            get
            {
                if (Value == 11)
                    return "J";
                else if (Value == 12)
                    return "Q";
                else if (Value == 13)
                    return "K";
                else if (Value == 1)
                    return "A";

                return Value == 10 ? "1" : Value.ToString();
            }
        }
        private string UpBlankSpaceCard { get { return Value == 10 ? "│░10░░░░░░│" : $"│░{ValueString}░░░░░░░│"; } }
        private string DownBlankSpaceCard { get { return Value == 10 ? "│░░░░░░10░│" : $"│░░░░░░░{ValueString}░│"; } }

      
        //https://www.alt-codes.net/suit-cards.php
        //♠ // ♦ //♥ //♣
        public virtual Panel RenderCard()
        {
            Panel card = new Panel(
               @$"    ╭─────────╮
    {UpBlankSpaceCard}
    │░░░░░░░░░│
    │░░░░{Type}░░░░│
    │░░░░░░░░░│
    {DownBlankSpaceCard}
    ╰─────────╯") 
                .Expand()
                .Border(BoxBorder.None);
            card.Width = 10;
            return card;
        }

        public override string ToString()
        {
            return $"{Value}${(int)Type}${Description}#";
        }
    }
}
