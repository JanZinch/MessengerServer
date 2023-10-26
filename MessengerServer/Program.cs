namespace MessengerServer;

public static class Program
{
    
    private static Server _server;
    
    public static async Task Main(string[] args)
    {
        _server = new Server();
        await _server.Run();
    } 
    
    
}