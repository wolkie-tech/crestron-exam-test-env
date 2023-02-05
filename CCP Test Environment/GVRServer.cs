using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CCP_Test_Environment
{
    public class GVRServer
    {
        public TcpListener Server { get; set; }
        public bool AutoListen { get; set; }
        public bool Connected { get; set; }
        public string LastError { get; set; }

        public GVRServer()
        {
            AutoListen = true;
            Connected = false;
            Server = new TcpListener(IPAddress.Parse("127.0.0.1"), 53001);
        }

        public async Task StartListenAndReceiveAsync()
        {
            var client = new TcpClient();
            NetworkStream stream;
            byte[] serverBuffer = new byte[64];
            Server.Start();
            while (AutoListen)
            {
                try
                {
                    client = await Server.AcceptTcpClientAsync();
                    Console.WriteLine("GVR Server has a client connected");
                    Connected = true;
                    stream = client.GetStream();
                    while(Connected)
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
                    if(AutoListen)
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
            byte understood = 0;
            if (buf[5] == 1) understood = 1;
            var response = new byte[11];
            response[0] = 2;
            response[1] = 9;
            response[2] = understood;
            for (var i = 6; i < 9; i++)
            {
                response[i - 3] = buf[i];
                response[i] = 0;
            }
            response[9] = 3;
            response[10] = 13;
            return response;
        }

    }
}
