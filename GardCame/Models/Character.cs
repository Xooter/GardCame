using GardCame.Environment;
using Spectre.Console.Rendering;
using Spectre.Console;
using GardCame.GameLoop;
using System.Net.Sockets;

namespace GardCame.Models
{
    public class Character
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; }

        public Socket? Socket;
        public int Health { get; set; } 
        public int MaxHealth { get; set; } = 9;

        public bool isDead = false;

        public Hand Hand { get; set; } = new Hand();

        public Character(Socket socket = null)
        {
            Socket = socket;
            Health = MaxHealth;
        }

        public List<IRenderable> RenderHand()
        {
            List<IRenderable> renders = new List<IRenderable>();
            foreach (Card item in Board.LocalPlayer.Hand.GetCards())
            {
                IRenderable card = item.RenderCard();
                renders.Add(card);//Align.Center(,VerticalAlignment.Middle)

                int index = renders.IndexOf(card);
                Color color = Color.Grey11;
                BoxBorder border = BoxBorder.Heavy;
                if (Main.Cursor == index)
                {
                    color = Color.White;
                    if(item is not BlankCard)
                        border = BoxBorder.Double;
                }

                (renders[index] as Panel).BorderColor(color);
                (renders[index] as Panel).Border(border); 
            }
            return renders;
        }

        public Panel RenderPlayer()
        {
            BoxBorder border = Id == Board.LocalPlayer.Id ? BoxBorder.Double : BoxBorder.Rounded;
            Color borderColor = Id == Turn.CurrentTurn() ? Color.Red : Color.White;
            string isdead = isDead ? "--Dead--" : "";
            Panel panel = new Panel($"{Name} {isdead}")
                .Border(border)
                .BorderColor(borderColor)
                .Expand();
            return panel;
        }

        public void SetCardOnBoard(Card card)
        {
            Hand.RemoveCard(card);
            //Board.AddCardOnBoard(this, card);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
                Die();
        }

        public void Heal(int health)
        {
            if (Health + health > MaxHealth)
                Health = MaxHealth;
            else
                Health += health;
        }

        public void Die()
        {

        }

        public override string ToString()
        {
            return $"{Name}-";
        }
    }
}
