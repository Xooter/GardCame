using GardCame.Models;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace GardCame.Environment.Network
{
    public class ServerNetwork : Network
    {
        public override async void Start()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 8080);
            Socket.Bind(iPEndPoint);

            Socket.Listen(100);
            Console.WriteLine("Waiting for a connection...");

            Task s = Task.Run(() =>
            {
                while (!Program.GameStarted)
                {
                    Socket newSocket = Socket.Accept();
                    if (newSocket.Connected)
                    {
                        ConnectedSockets.Add(new Character(newSocket));
                        Thread tr = new Thread(() => { SyncReceived(newSocket); });

                        tr.Start();
                    }
                }
            });
        }

        public override string Received(Socket s)
        {
            if (ConnectedSockets == null || ConnectedSockets.Count() == 0) return string.Empty;

            try
            {
                byte[] buffer = new byte[1024];
                int recivedBytes = s.Receive(buffer);
                return ByteToString(buffer, recivedBytes);

            }
            catch (Exception)
            {
                ConnectedSockets.Remove(ConnectedSockets.First(x => x.Socket == s));
                Main.RefreshPlayer();
                return string.Empty;
            }
        }

        public override void AddPlayer(string name, Socket socket = null)
        {
            string fixedName = name;
            int i = 0;
            while (ConnectedSockets.Any(x => x.Name == fixedName))
            {
                i++;
                fixedName = name + i;
            };

            Character player = ConnectedSockets.First(x => x.Socket == socket);
            player.Name = fixedName;
            player.Id = ConnectedSockets.IndexOf(player);


            //mandar 5 cartas al nuevo cliente que se acaba de conectar
            GiveCard(5,player.Id);
        }

        public override void Send(string command)
        {
            //byte[] commandBuffer = Encoding.ASCII.GetBytes(command);
            foreach (Character player in ConnectedSockets)
            {
                if (player.Socket == null) continue;

                byte[] commandBuffer = Encoding.ASCII.GetBytes(command.Replace(";","") + "|" + ConnectedSockets.IndexOf(player).ToString()+";");

                player.Socket.Send(commandBuffer);
            }
        }

        public void DamagePlayer(int playerId)
        {
            if (playerId != -1)
            {
                Character player = ConnectedSockets.First(x => x.Id == playerId);
                if (player == null) return;
                byte[] commandBuffer = Encoding.ASCII.GetBytes("DamagePlayer");
                if (player.Socket != null)
                    player.Socket.Send(commandBuffer);
            }
            else
            {
                Main.DamagePlayer();
            }
        }

        public void GiveCard(int count , int playerId)
        {
            Character player = ConnectedSockets.First(x => x.Id == playerId);
            if (player == null) return;
            byte[] commandBuffer = Encoding.ASCII.GetBytes("ReadCards|" + Board.CollectCards(count) + "|" + ConnectedSockets.IndexOf(player).ToString()+";");
            if (player.Socket != null)
                player.Socket.Send(commandBuffer);
        }

    }
}




