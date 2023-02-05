using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CCP_Test_Environment
{
    public class PosServer
    {
        public TcpListener Server { get; set; }
        public bool AutoReconnect { get; set; }
        public bool Connected { get; set; }
        public string LastError { get; set; }

        public PosServer()
        {
            AutoReconnect = true;
            Connected = false;
            Server = new TcpListener(IPAddress.Parse("127.0.0.1"), 53002);
        }

        public async Task StartListenAndReceiveAsync()
        {
            var client = new TcpClient();
            NetworkStream stream;
            byte[] serverBuffer = new byte[64];
            Server.Start();
            while (AutoReconnect)
            {
                try
                {
                    client = await Server.AcceptTcpClientAsync();
                    Console.WriteLine("POS Server has a client connected");
                    Connected = true;
                    stream = client.GetStream();
                    while (Connected)
                    {
                        await stream.ReadAsync(serverBuffer);
                        try
                        {
                            var tx = ProcessResponse(serverBuffer);
                            await stream.WriteAsync(tx);
                        }
                        catch (Exception e)
                        {
                            LastError = e.Message;
                            Connected = false;
                        }
                    }
                    client.Close();
                    client.Dispose();
                }
                catch (Exception e)
                {
                    LastError = e.Message;
                    if (AutoReconnect)
                    {
                        Console.WriteLine("Reset connections");
                        Connected = false;
                        client.Dispose();
                    }
                }
            }
        }

        private static byte[] ProcessResponse(byte[] buf)
        {
            var request = Encoding.ASCII.GetString(buf).Replace("\x00", string.Empty);
            string response = "\r\n";
            if (request.Contains("query balance member"))
            {   
                var rnd = new Random();
                response = $"show balance member {request.Split(" ")[3]} {rnd.Next(0, 501)}\r\n";
            }
            else if(request.Contains("order item member"))
            {
                response = $"sent item member {request.Split(" ")[3]} code {request.Split(" ")[5]}\r\n";
            }
            else if(request.Contains("rental start member"))
            {
                response = $"charge start member {request.Split(" ")[3]}\r\n";
            }
            else if (request.Contains("rental end member"))
            {
                response = $"charge end member {request.Split(" ")[3]}\r\n";
            }
            else if(request.Contains("display +") || request.Contains("display -"))
            {
                response = "display ok\r\n";
            }
            return Encoding.ASCII.GetBytes(response);
        }
    }
}
