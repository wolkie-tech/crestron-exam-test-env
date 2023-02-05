using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CCP_Test_Environment
{
    public class IntersystemBridge
    {
        public UdpClient OutboundConnection { get; set; }
        public UdpClient InboundConnection { get; set; }
        public bool MaintainConnection { get; set; }

        public IntersystemBridge() 
        {
            MaintainConnection = true;
            InboundConnection = new UdpClient(5003);
            OutboundConnection = new UdpClient();
            OutboundConnection.Connect("192.168.1.255", 5003);
        }

        public async Task MonitorBridgeAsync()
        {
            //Console.WriteLine("Monitoring UDP bridge");
            while(MaintainConnection)
            {
                var inboundPacket = await InboundConnection.ReceiveAsync();
                var textResponse = Encoding.ASCII.GetString(inboundPacket.Buffer);
                Console.WriteLine($"UDB packet received: {textResponse}");
            }
        }

        public void SendPoll(char studio)
        {
            Console.WriteLine($"Sending Poll for studio {studio}");
            var packet = $"\x02{studio},*,|POLL|0|\x03";
            var buffer = Encoding.ASCII.GetBytes(packet);
            OutboundConnection.Send(buffer, buffer.Length);
        }
    }
}
