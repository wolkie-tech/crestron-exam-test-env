using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CCP_Test_Environment
{
    public class ManifestWebServer
    {
        public HttpListener Server { get; set; }
        public bool AutoListen { get; set; }
        public bool Listening { get; set; }

        public ManifestWebServer() 
        {
            AutoListen = true;
            Server = new HttpListener();
            Server.Prefixes.Add("http://*:53006/");
            Server.Start();
        }

        public async Task StartHttpServerListeningAsync()
        {
            while(AutoListen)
            {
                var context = await Server.GetContextAsync();
                Console.WriteLine("Manifest Server has been accessed");
                var request = context.Request;
                var response = context.Response;
                string filePath;
                if (request.RawUrl.Contains("config.json")) filePath = "C:\\Users\\PBowden\\OneDrive - Internet Videocommunications\\CCP 2022\\config.json";
                else if (request.RawUrl.Contains("items.json")) filePath = "C:\\Users\\PBowden\\OneDrive - Internet Videocommunications\\CCP 2022\\items.json";
                else filePath = "C:\\Users\\PBowden\\OneDrive - Internet Videocommunications\\CCP 2022\\nodata.json";
                using StreamReader streamReader = new StreamReader(filePath);
                var json = await streamReader.ReadToEndAsync();
                var buffer = Encoding.UTF8.GetBytes(json);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
                
            }
        }
    }
}
