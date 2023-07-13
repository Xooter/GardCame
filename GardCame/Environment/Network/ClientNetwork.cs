using GardCame.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GardCame.Environment.Network
{
    public class ClientNetwork : Network
    {

        public override void Start()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(Program.IP), 8080);
            Socket.Connect(iPEndPoint);

            Thread tr = new Thread(() => { SyncReceived(Socket); });

            tr.Start();
        }

        public override string Received(Socket s)
        {
            byte[] Buffer = new byte[1024];
            int recivedBytes = s.Receive(Buffer);

            return ByteToString(Buffer, recivedBytes);
        }

        public override void Send(string command)
        {
            byte[] commandBuffer = Encoding.ASCII.GetBytes(command);
            Socket.Send(commandBuffer);
        }

        public override void AddPlayer(string playersData,Socket socket = null)
        {
            Program.Network.ConnectedSockets.Clear();
            var playerDataList = playersData.Split("-").SkipLast(1);
            foreach (string name in playerDataList)
            {
                var IdName = name.Split(":");
                int Id = -1;
                string fixedName = name;
                if (IdName.Length > 1)
                {
                    Id = int.Parse(IdName[0]);
                    fixedName = IdName[1];
                }

                Program.Network.ConnectedSockets.Add(new Character() { Name = fixedName, Id = Id });
            }
        }
    }
}
