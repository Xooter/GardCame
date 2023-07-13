using GardCame.GameLoop;
using GardCame.Models;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace GardCame.Environment.Network
{
    public abstract class Network
    {
        protected Socket Socket { get; set; }

        public List<Character> ConnectedSockets { get; set; } = new List<Character>();

        public Main Main { get; set; }

        public bool IsServer { get { return this is ServerNetwork; } }
        public bool IsClient { get { return !IsServer; } }

        public Network()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public abstract void Start();
        public abstract string Received(Socket socket);
        public abstract void Send(string command);
        public abstract void AddPlayer(string PlayerData, Socket socket = null);

        public void Sync(string command)
        {
            ReflectionCommand(command);
            Send(command);
        }

        public void SyncReceived(Socket socket)
        {
            while (true)
            {
                string received = Program.Network.Received(socket);

                if (received == string.Empty) return;

                var receivedFunctions = received.Split(";");
                foreach (string receivedFunction in receivedFunctions)
                {
                    if (receivedFunction == string.Empty) continue;
                    Type programType = typeof(Main);
                    var receivedParts = receivedFunction.Split("|");
                    string methodName = receivedParts[0];
                    MethodInfo? method = programType.GetMethod(methodName);

                    if (method != null)
                    {

                        if (receivedParts.Count() < 2)
                        {
                            method.Invoke(Main, null);
                        }
                        else
                        {
                            List<string> ParamsList = receivedParts.Where(x => x != "").Skip(1).ToList();

                            if (IsClient)
                            {
                                var LocalId = receivedParts.Last();
                                Board.LocalPlayer.Id = int.Parse(LocalId);
                                ParamsList = new List<string>(ParamsList.SkipLast(1));
                            }

                            List<object> list = ParamsList.ToList<object>();

                            if (method.GetParameters().Count() > 1 && methodName != "SetCardOnBoardClientRpc" && methodName != "SendCardServerRpc")
                                list.Add(socket);

                            object[] parametersArray = list.ToArray();

                            method.Invoke(Main, parametersArray);
                        }
                    }
                    //else
                    //    throw new Exception("Metodo no encontrado al syncronizar.");
                }

            }
        }

        private void ReflectionCommand(string command)
        {
            var receivedFunctions = command.Split(";");
            foreach (string receivedFunction in receivedFunctions)
            {
                if (receivedFunction == string.Empty) continue;

                Type programType = typeof(Main);
                var receivedParts = receivedFunction.Split("|");
                string methodName = receivedParts[0];
                MethodInfo? method = programType.GetMethod(methodName);

                if (method != null)
                {
                    int parametersCount = method.GetParameters().Length;
                    if (parametersCount > 0)
                    {
                        List<string> ParamsList = receivedParts.Where(x => x != "").Skip(1).ToList();

                        object[] parametersArray = ParamsList.ToArray();

                        method.Invoke(Main, parametersArray);
                    }
                    else
                    {
                        method.Invoke(Main, null);
                    }

                }
                else
                    throw new Exception("Metodo no encontrado al syncronizar.");
            }
        }

        public void Stop()
        {
            Socket.Close(0);
        }

        protected string ByteToString(byte[] Buffer, int Bytes)
        {
            string response = Encoding.ASCII.GetString(Buffer, 0, Bytes);
            return response;
        }
    }
}
