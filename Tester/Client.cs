using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessengerServer.Core;
using MessengerServer.Core.Infrastructure;

namespace Tester;

public class Client
{
    public async Task Run()
    {
        try
        {
            using TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 8888);
            NetworkStream networkStream = tcpClient.GetStream();
        
            Console.WriteLine("Connected");

            
            StreamWriter writer = new StreamWriter(networkStream);
            
            string jsonMessageBuffer = JsonSerializer.Serialize(CreateTestMessage());
            
            Console.WriteLine("String: " + jsonMessageBuffer);

            Query query = new Query(QueryHeader.PostMessage, jsonMessageBuffer);
            
            await writer.WriteAsync(query.ToString());
            await writer.FlushAsync();
            
            
            /*Console.WriteLine("Wrote");
        
            StreamReader reader = new StreamReader(networkStream);
            Console.WriteLine("GetStream");
            
            jsonMessageBuffer = await reader.ReadLineAsync();
            Console.WriteLine("Read");
            
            List<Message> chat = JsonSerializer.Deserialize<List<Message>>(jsonMessageBuffer);

            Console.WriteLine("Read");
        
            foreach (Message message in chat)
            {
                Console.WriteLine(message);
            }
            
            tcpClient.Close();*/


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        
        
    }


    private Message CreateTestMessage()
    {
        return new Message()
        {
            Sender = new User()
            {
                Nickname = "Nick",
                Password = "111111"
            },

            Receiver = new User()
            {
                Nickname = "Mike",
                Password = "000000"
            },
            
            Text = "Just a test message.",
            
            PostDateTime = DateTime.UtcNow,
        };
        
    }
}