using MessengerServer.Server;

namespace MessengerServer;

public static class Program
{
    private const string ShutdownCommand = "shutdown";
    private static AppServer _appServer;
    
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Your control loop started on thread " + Thread.CurrentThread.ManagedThreadId);
        
        _appServer = new AppServer();
        _appServer.StartAsync();

        while (true)
        {
            Console.WriteLine("Input \"shutdown\" to shutdown the server");
            
            string command = Console.ReadLine();

            if (command == ShutdownCommand)
            {
                break;
            }
        }
        
        await _appServer.DisposeAsync();
    }
    
}