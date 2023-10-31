using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessengerServer.Core;
using MessengerServer.Core.Infrastructure;
using MessengerServer.Core.Models;

namespace MessengerServer;

public class AppServer : IAsyncDisposable
{
    private const int MaxConnections = 100;

    private TcpListener _tcpListener;
    private DatabaseContext _databaseContext;

    private bool _isRunning;
    
    private readonly ConcurrentQueue<Message> _chatMessages = new ConcurrentQueue<Message>();

    private bool IsRunning
    {
        get => Volatile.Read(ref _isRunning);
        set => Volatile.Write(ref _isRunning, value);
    }
    
    public async void StartAsync()
    {
        try
        {
            _databaseContext = new DatabaseContext();
            await _databaseContext.ConnectToDatabase();

            _tcpListener = new TcpListener(IPAddress.Any, 8888);
            _tcpListener.Start();

            IsRunning = true;
            Console.WriteLine("Server is started.");

            while (IsRunning)
            {
                TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();

                Task.Run(async () => await HandleClientAsync(tcpClient));

                //Task.Run(()=> ProcessClientAsync(tcpClient));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        finally
        {
            await DisposeAsync();
        }
    }
    
    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        try
        {
            NetworkStream networkStream = tcpClient.GetStream();
            
            StreamReader reader = new StreamReader(networkStream);
            string rawLine = await reader.ReadLineAsync();
            //Console.WriteLine("Raw line: " + rawLine);

            Query query = Query.FromRawLine(rawLine);
            //Console.WriteLine("Json: " + query.JsonDataString);  

            switch (query.Header)
            {
                case QueryHeader.Login:
                    break;
            }
            
            Message postedMessage = JsonSerializer.Deserialize<Message>(query.JsonDataString);
            //Console.WriteLine("Msg: " + postedMessage);   
        
            _chatMessages.Enqueue(postedMessage);
        
            StreamWriter writer = new StreamWriter(networkStream);
            
            string jsonMessageBuffer = JsonSerializer.Serialize(_chatMessages.ToArray());
            Response response = new Response(jsonMessageBuffer);
            
            await writer.WriteAsync(response.ToString());
            await writer.FlushAsync();
        
            //Console.WriteLine("Write");
        
            tcpClient.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        
    }

    /*public async void StopAsync()
    {
        await DisposeAsync();
    }*/
    
    public async ValueTask DisposeAsync()
    {
        IsRunning = false;
        _tcpListener.Stop();
        
        if (_databaseContext != null)
        {
            await _databaseContext.DisposeAsync();
        }
    }
}