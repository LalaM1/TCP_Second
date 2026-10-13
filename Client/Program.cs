using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;

namespace ClientTCP
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client is starting...");
            var client = new TcpClient();
            var ip = IPAddress.Parse("10.2.13.1");
            var port = 27001;

            var ep = new IPEndPoint(ip, port);

            try
            {
                client.Connect(ep);
                if (client.Connected)
                {
                    var writer = Task.Run(() =>
                    {
                        while (true)
                        {
                            var text = Console.ReadLine();
                            var stream = client.GetStream();
                            var bw = new BinaryWriter(stream);
                            bw.Write(text ?? "no message");
                        }
                    });

                    var reader = Task.Run(() =>
                    {
                        while (true)
                        {
                            var stream = client.GetStream();
                            var br = new BinaryReader(stream);
                            var result = br.ReadString();
                            if (result != "1_1_1_2") //burda clientin online olub olmadigini yoxlayiriq, eger serverden gelen mesaj 1_1_1_2 deyilse demeli serverden gelen mesajdir ve onu ekrana yazdiririq
                                Console.WriteLine($"FROM SERVER : {result}");
                        }
                    });

                    Task.WaitAll(writer, reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}




