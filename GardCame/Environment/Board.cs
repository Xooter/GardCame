using GardCame.Actives;
using GardCame.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace GardCame.Environment
{
    public static class Board
    {
        private static List<char> strings = new List<char>() { '♠', '♦', '♥', '♣', };

        public static int MaxCardsOnBoard = 3;

        public static Character LocalPlayer { get; private set; }

        public static Deck CompleteDeck { get; set; } = new Deck();

        private static int ammount = 0;
        public static int Ammount
        {
            get
            {
                return ammount;
            }
            set
            {
                ammount = value;
                RenderAmmount();
            }
        }

        public static void RenderAmmount()
        {
            var AsciiAmmount = new FigletText(ammount.ToString());
            Color color = Color.White;

            if (Ammount > 75)
                color = Color.Orange1;
            if (Ammount > 85)
                color = Color.OrangeRed1;
            if (Ammount > 95)
                color = Color.Red;

            AsciiAmmount.Color(color);
            MainLayout.mainLayout["Ammount"].Update(
                Align.Center(AsciiAmmount, VerticalAlignment.Middle)
                 );
        }
        public static void RenderLastCard(Card _card)
        {
            IRenderable card = Align.Center(_card.RenderCard().NoBorder());

            MainLayout.mainLayout["LastCard"].Update(
                Align.Center(card, VerticalAlignment.Middle)
                 );
        }

        public static void LoadPlayer()
        {
            LocalPlayer = new Character();
            LocalPlayer.Name = Program.LocalName;
        }

        public static void LoadFullDeck()
        {
            foreach (char type in strings)
            {
                for (int i = 1; i < 14; i++)
                {
                    Card newCard = new Card()
                    {
                        Value = i,
                        Type = type
                    };

                    newCard = LoadActives(newCard);

                    CompleteDeck.AddCard(newCard);
                }
            }
            CompleteDeck.Shuffle();
        }

        public static Card LoadActives(Card card)
        {

            if (card.Value == 1)
            {
                card.Active = new As();
                card.Description = $"Remove 1 from the pile";
            }
            else if (card.Value == 4)
            {
                card.Active = new Reverse();
                card.Description = "Reverse direction of play\n No value";
            }
            else if (card.Value == 9)
            {
                card.Active = new Skip();
                card.Description = "Skip one turn\n No value";
            }
            else if (card.Value == 10)
            {
                card.Active = new Minus();
                card.Description = "Remove 10 from the pile";
            }
            else if (card.Value == 11 || card.Value == 12)
            {
                card.Active = new JackQueen();
                card.Description = "Add 10 to the pile";
            }
            else if (card.Value == 13)
            {
                card.Active = new King();
                card.Description = "Takes the count to 99";
            }
            else
            {
                card.Active = new Add();
                card.Description = $"Add {card.Value} to the pile";
            }

            return card;
        }

        public static string CollectCards(int cards)
        {
            string command = string.Empty;

            for (int i = 0; i < cards; i++)
            {
                Card removedCard = CompleteDeck.RemoveCard();
                if(removedCard != null)
                    command += $"{CompleteDeck.RemoveCard().ToString()}";
            }

            return command;
        }

        public static List<IRenderable> RenderPlayers()
        {
            List<IRenderable> renders = new List<IRenderable>();

            if (Program.Network.IsServer)
                renders.Add(LocalPlayer.RenderPlayer());

            foreach (Character item in Program.Network.ConnectedSockets)
            {
                Panel card = item.RenderPlayer();
                renders.Add(card);
            }

            return renders;
        }

        public static void RefreshLife()
        {
            MainLayout.mainLayout["Life"].Update(
                Align.Center
                (
                    new Rows
                    (
                        new Text($"Current health: {LocalPlayer.Health}"),
                        new Rule()
                    ),VerticalAlignment.Bottom
                )
            );
        }
    }
}
