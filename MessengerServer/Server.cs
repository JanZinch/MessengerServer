using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessengerServer.Core;
using MessengerServer.Core.Infrastructure;

namespace MessengerServer;

public class Server
{
    private const int MaxConnections = 100;
    
    private readonly ConcurrentQueue<Message> _chatMessages = new ConcurrentQueue<Message>();

    public async Task Run()
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
        
        try
        {
            tcpListener.Start();
            
            Console.WriteLine("Server is started.");
 
            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                
                Task.Run(async ()=> await ProcessClientAsync(tcpClient));
                
                //Task.Run(()=> ProcessClientAsync(tcpClient));
            }
        }
        finally
        {
            tcpListener.Stop();
        }
    }
    
    private async Task ProcessClientAsync(TcpClient tcpClient)
    {
        try
        {
            NetworkStream networkStream = tcpClient.GetStream();
            
            StreamReader reader = new StreamReader(networkStream);
            string rawLine = await reader.ReadLineAsync();
            
            Console.WriteLine("Raw line: " + rawLine);

            Query query = Query.FromRawSource(rawLine);
            
            Console.WriteLine("Json: " + query.JsonDataString);
            
            Message postedMessage = JsonSerializer.Deserialize<Message>(query.JsonDataString);
        
            Console.WriteLine("Msg: " + postedMessage);
        
            /*_chatMessages.Enqueue(postedMessage);
        
            StreamWriter writer = new StreamWriter(networkStream);
            jsonMessageBuffer = JsonSerializer.Serialize(_chatMessages.ToArray());
            await writer.WriteAsync(jsonMessageBuffer + "\n");
            await writer.FlushAsync();
        
            Console.WriteLine("Write");*/
        
            tcpClient.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        
    }
    
}