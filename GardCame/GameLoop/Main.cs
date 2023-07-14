using GardCame.Environment;
using GardCame.Environment.Network;
using GardCame.Models;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;
using System.Net.Sockets;

namespace GardCame.GameLoop
{
    public class Main : MainBase
    {
        ConsoleKey Key;
        public bool Quit = false;

        private static int cursor;
        public static int Cursor
        {
            get
            {
                return cursor;
            }
            set
            {
                cursor = value;
                RenderDescription();
                RenderDeck();
            }
        }

        public static BlankCard blankCard = new BlankCard();

        public void MainLoop()
        {
            do
            {
                ConsoleKeyInfo Input = Console.ReadKey(true);
                Key = Input.Key;
            } while (!Quit);
            Quit = true;
        }

        public override void OnStart()
        {
            Board.LoadPlayer();
            Board.RenderLastCard(blankCard);
            Board.RefreshLife();

            if (Program.Network.IsClient)
            {
                Program.Network.Send($"{nameof(AddClientPlayer)}|{Board.LocalPlayer.Name};");
            }
            else
            {
                Board.LoadFullDeck();
                ReadCards(Board.CollectCards(5));
            }

            Board.RenderAmmount();
        }

        public override void OnUpdate()
        {
            Console.SetCursorPosition(0, 0);
            Thread.Sleep(20);

            CursorMovemment();

            ControlsMovemment();

            Key = new ConsoleKey();
        }

        private static void RenderDescription()
        {
            Card selectedCard = Board.LocalPlayer.Hand.GetCards()[Cursor];
            Panel DeckPanel = new Panel(
                   Align.Center(new Text(selectedCard.Description).Centered(), VerticalAlignment.Middle))
                        .Expand()
                        .Border(BoxBorder.Rounded)
                        .BorderColor(Color.Grey11);

            MainLayout.mainLayout["Description"].Update(
               DeckPanel
               );
        }

        private static void RenderDeck()
        {
            List<IRenderable> LocalPlayerHandsRendered = Board.LocalPlayer.RenderHand();

            Panel DeckPanel = new Panel(
                   new Columns(LocalPlayerHandsRendered.Take(5))).Header("").Expand().NoBorder();

            MainLayout.mainLayout["Deck"].Update(
               DeckPanel
               );
        }

        private void CursorMovemment()
        {
            if (Key == ConsoleKey.RightArrow && Cursor < 4)
            {
                Cursor++;
            }
            else if (Key == ConsoleKey.LeftArrow && Cursor > 0)
            {
                Cursor--;
            }
        }

        private void ControlsMovemment()
        {
            Card selectedCard = Board.LocalPlayer.Hand.GetCards()[Cursor];
            if (Key == ConsoleKey.Enter && Turn.IsLocalTurn() && selectedCard is not BlankCard)
            {
                Board.LocalPlayer.Hand.GetCards()[Cursor] = blankCard;
                RenderDeck();
                RenderDescription();
                if (Program.Network.IsServer)
                {
                    if(!Program.GameStarted)
                    {
                        Program.GameStarted = true;
                    }

                    SendCardServerRpc(selectedCard.ToString(), Board.LocalPlayer.Id.ToString());
                }
                else
                {
                    Program.Network.Send($"{nameof(SendCardServerRpc)}|{selectedCard.ToString()}|{Board.LocalPlayer.Id};");
                }
            }

        }

        public void ReadCards(string data)
        {
            var cards = data.Split("#");

            foreach (string item in cards.SkipLast(1))
            {

                var values = item.Split("$");
                Card card = new Card()
                {
                    Value = int.Parse(values[0]),
                    Type = (char)int.Parse(values[1]),
                    Description = values[2]
                };
                card = Board.LoadActives(card);

                if (Board.LocalPlayer.Hand.HasBlankCard())
                    Board.LocalPlayer.Hand.ReplaceBlankCard(card);
                else
                    Board.LocalPlayer.Hand.AddCard(card);
            }
            RenderDeck();
            RenderDescription();
        }

        public void SendCardServerRpc(string data, string playerid)
        {
            if (Program.Network.IsServer)
            {
                Program.Network.Sync($"{nameof(SetCardOnBoardClientRpc)}|{data}|{playerid};");
                Thread.Sleep(200);
                if (data.Split("$")[0] != "9")
                {
                    int id = int.Parse(playerid);
                    string turnSteps = "1";

                    if (id == -1 && Board.LocalPlayer.isDead || id != -1 && Program.Network.ConnectedSockets.First(x => x.Id == id).isDead)
                    {
                        turnSteps = "2";
                    }

                    if (!Board.LocalPlayer.isDead && Program.Network.ConnectedSockets.All(x => x.isDead) || Board.LocalPlayer.isDead && Program.Network.ConnectedSockets.Count(x => x.isDead) == Program.Network.ConnectedSockets.Count - 1)
                    {
                        Character Winner;
                        Winner = Program.Network.ConnectedSockets.First(x => !x.isDead);
                        if (Winner != null && !Board.LocalPlayer.isDead)
                            Winner = Board.LocalPlayer;
                        Program.Network.Sync($"{nameof(EndGame)}|{Winner.Name};");
                    }
                    else
                    {
                        Program.Network.Send($"{nameof(NextTurnServerRpc)}|{turnSteps};");
                    }
                }
            }
            else
            {
                Program.Network.Send($"{nameof(SetCardOnBoardClientRpc)}|{data}|{playerid};");
                Thread.Sleep(200);
                if (data.Split("$")[0] != "9")
                {
                    Program.Network.Send($"{nameof(NextTurnServerRpc)}|1;");
                }
            }
        }

        public void EndGame(string winnerName)
        {
            Program.WinnerName = winnerName;
            Quit = true;
        }

        public void SetCardOnBoardClientRpc(string data, string playerId)
        {

            int id = int.Parse(playerId);
            var cardData = data.Replace("#", "");
            var cardValues = cardData.Split("$");

            Card card = new Card()
            {
                Value = int.Parse(cardValues[0]),
                Type = (char)int.Parse(cardValues[1]),
                Description = cardValues[2]
            };

            card = Board.LoadActives(card);
            Board.RenderLastCard(card);
            card.Active.Active(card);


            if (Program.Network.IsServer && Board.Ammount > 99)
            {
                (Program.Network as ServerNetwork).DamagePlayer(id);
            }

            if (Board.Ammount > 99)
            {
                Board.Ammount = 0;
            }
            //Board.Ammount += card.Value;
        }

        public void DamagePlayer()
        {
            Board.LocalPlayer.TakeDamage(1);
            Board.RefreshLife();

            if (!Board.LocalPlayer.isDead && Board.LocalPlayer.Health < 1)
            {
                if (Program.Network.IsServer)
                {
                    Board.LocalPlayer.isDead = true;
                }
                else
                {
                    Program.Network.Send($"{nameof(KillPlayer)}|{Board.LocalPlayer.Id};");
                }
            }
            //Quit = true;
        }

        public void KillPlayer(string playerId)
        {
            int id = int.Parse(playerId);

            Program.Network.ConnectedSockets.First(x => x.Id == id).isDead = true;
        }

        public void NextTurnServerRpc(string turnCount = "1")
        {
            if (Program.Network.IsServer)
            {
                Program.Network.Sync($"{nameof(NextTurnClientRpc)}|{turnCount};");
            }
            else
            {
                Program.Network.Send($"{nameof(NextTurnServerRpc)}|{turnCount};");
            }
        }

        public void NextTurnClientRpc(string turnCount)
        {
            int count = int.Parse(turnCount);
            Turn.NextTurn(count);

            if (Turn.CurrentTurn() == Board.LocalPlayer.Id && Board.LocalPlayer.isDead)
            {
                if (Program.Network.IsServer)
                {
                    NextTurnServerRpc();
                }
                else
                {
                    Program.Network.Send($"{nameof(NextTurnServerRpc)}|1;");
                }
            }

            if (Turn.CurrentTurn() == Board.LocalPlayer.Id && Board.LocalPlayer.Hand.HasBlankCard())
            {
                if (Program.Network.IsServer)
                    ReadCards(Board.CollectCards(1));
                else
                    Program.Network.Send($"{nameof(GiveCardClientRpc)}|{Board.LocalPlayer.Id};");
            }

            List<IRenderable> LocalPlayerHandsRendered = Board.RenderPlayers();

            Panel PlayersPanel = new Panel(
                   new Columns(LocalPlayerHandsRendered)).Header("Players").Expand();

            MainLayout.mainLayout["Players"].Update(
               PlayersPanel
               );
        }

        public void GiveCardClientRpc(string playerId)
        {
            int playerID = int.Parse(playerId);
            if (Program.Network.IsServer)
            {
                (Program.Network as ServerNetwork).GiveCard(1, playerID);
            }
        }

        public void ReverseTurnServerRpc()
        {
            if (Program.Network.IsServer)
            {
                Program.Network.Sync($"{nameof(ReverseTurnClientRpc)};");
            }
            else
            {
                Program.Network.Send($"{nameof(ReverseTurnServerRpc)};");
            }
        }

        public void ReverseTurnClientRpc()
        {
            Turn.ReverseTurnList();
        }

        public void AddClientPlayer(string name, Socket PlayerSocket)
        {
            Program.Network.AddPlayer(name, PlayerSocket);
            RefreshPlayer();
        }

        public void RefreshPlayer()
        {
            string playersData = $"{Program.LocalName}-";
            for (int i = 0; i < Program.Network.ConnectedSockets.Count(); i++)
            {
                playersData += $"{i}:{Program.Network.ConnectedSockets[i].ToString()}";
            }

            Program.Network.Sync($"{nameof(RenderPlayers)}|{playersData}");
        }

        public void RenderPlayers(string playersData)
        {
            if (Program.Network.IsClient)
                Program.Network.AddPlayer(playersData);

            List<IRenderable> LocalPlayerHandsRendered = Board.RenderPlayers();

            Panel PlayersPanel = new Panel(
                   new Columns(LocalPlayerHandsRendered)).Header("Players").Expand();

            MainLayout.mainLayout["Players"].Update(
               PlayersPanel
               );
        }
    }
}
