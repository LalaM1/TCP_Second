using System.Net;
using System.Net.Sockets;

namespace Server2
{
    public class Program
    {
        static TcpListener listener = null;
        static BinaryReader br = null;
        static BinaryWriter bw = null;
        static List<TcpClient> Clients { get; set; }

        static object obj = new object();
        static void Main(string[] args)
        {
            Console.WriteLine("Server is starting...");
            var ip = IPAddress.Parse("10.2.13.1");
            var port = 27001;

            var ep = new IPEndPoint(ip, port);
            listener = new TcpListener(ep);
            listener.Start();

            Clients = new List<TcpClient>();

            Console.WriteLine($"Listening on {listener.LocalEndpoint}");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Clients.Add(client);

                Console.WriteLine($"{client.Client.RemoteEndPoint} connected . . . ");


                #region For Many

                Task.Run(() =>
                {

                    var findOnlines = Task.Run(() =>
                    {
                        //online olanlari ve offline olanlari tapmaq ucun bir loop yaradiriq ve her 2 saniyede bir yoxlayiriq
                        while (true)
                        {
                            foreach (var item in Clients)
                            {
                                try
                                {
                                    var stream = item.GetStream();
                                    bw = new BinaryWriter(stream);
                                    bw.Write("1_1_1_2"); // bu mesaj serverden clienta gonderilir ve client bunu alirsa demeli online-dir
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"{item.Client.RemoteEndPoint} IS ONLINE");
                                    Console.ResetColor();
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"{item.Client.RemoteEndPoint} IS OFFLINE");
                                    Console.ResetColor();
                                }
                            }
                            Thread.Sleep(2000);
                            Console.Clear();
                            Console.WriteLine();
                        }
                    });

                    var writer = Task.Run(() =>
                    {
                        while (true)
                        {
                            var msg = Console.ReadLine();
                            foreach (var item in Clients)
                            {
                                var stream = item.GetStream();

                                bw = new BinaryWriter(stream);
                                bw.Write(msg ?? "no message");
                            }
                        }
                    });

                    while (true)
                    {

                        foreach (var item in Clients)
                        {
                            var stream = item.GetStream();
                            if (stream.DataAvailable)
                            {

                                br = new BinaryReader(stream);
                                var msg = br.ReadString();
                                Console.WriteLine($"{item.Client.RemoteEndPoint} : MSG : {msg}");
                            }
                            else
                            {
                                continue;
                            }

                        }

                    }
                });

                #endregion

            }
        }
    }
}




