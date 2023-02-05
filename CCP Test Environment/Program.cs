// See https://aka.ms/new-console-template for more information
using CCP_Test_Environment;

Console.WriteLine("CCP Test Environment Running...");
var GVR = new GVRServer();
var POS = new PosServer();
var httpServer = new ManifestWebServer();
var udpBridge = new IntersystemBridge();

Task.Run(GVR.StartListenAndReceiveAsync);
Task.Run(POS.StartListenAndReceiveAsync);
Task.Run(httpServer.StartHttpServerListeningAsync);
Task.Run(udpBridge.MonitorBridgeAsync);

while(true)
{
    Console.WriteLine("");
    Console.Write(">_ ");
    var response = Console.ReadLine();

    if (response.ToUpper() == "EXIT") break;
    else if(response.ToUpper() == "GET GVR STATE")
    {
        var connectState = GVR.Connected ? "Connected" : "Not Connected";
        Console.WriteLine($"GVR client is currently {connectState}");
    }
    else if (response.ToUpper() == "GET GVR ERROR")
    {
        if (GVR.LastError is not null) Console.WriteLine($"GVR error is: {GVR.LastError}");
        else Console.WriteLine("GVR has not yet raised an error");
    }
    else if(response.ToUpper().Contains("SEND UDP POLL"))
    {
        var studio = response.ToUpper().Split(" ").Last().ToCharArray()[0];
        udpBridge.SendPoll(studio);
    }
    else Console.WriteLine("Unknown Command");
}