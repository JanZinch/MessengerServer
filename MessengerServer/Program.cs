using MessengerServer.Server;

namespace MessengerServer;

public static class Program
{
    private const string ShutdownCommand = "shutdown";
    private static AppServer _appServer;
    
    public static async Task Main(string[] args)
    {
        _appServer = new AppServer();
        _appServer.StartAsync();

        while (true)
        {
            string command = Console.ReadLine();

            if (command == ShutdownCommand)
            {
                break;
            }
        }
        
        await _appServer.DisposeAsync();
    }
    
}