using MessengerServer.Core.Models;

namespace MessengerServer;

public static class Program
{
    private static DatabaseService _databaseService;
    private static Server _server;
    
    public static async Task Main(string[] args)
    {
        /*_server = new Server();
        await _server.Run();*/

        using (_databaseService = new DatabaseService())
        {
            await _databaseService.ConnectToDatabase();
            
            Console.WriteLine("User exists: " + await _databaseService.IsUserExists(new User()
            {
                Nickname = "Jan Zinch",
                Password = "111111"
            }));
        }
        
        Console.WriteLine("Программа завершила работу.");
        Console.Read();
    } 
    
    
}