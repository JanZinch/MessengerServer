using MessengerServer.Core.Models;

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
        
        /*_server = new Server();
        await _server.Run();*/

        /*using (_databaseContext = new DatabaseContext())
        {
            await _databaseContext.ConnectToDatabase();
            
            Console.WriteLine("User exists: " + await _databaseContext.IsUserExists(new User()
            {
                Nickname = "Jan Zinch",
                Password = "111111"
            }));
        }*/

        /*Console.WriteLine("Программа завершила работу.");
        Console.Read();*/
    }
    
}