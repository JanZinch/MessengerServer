using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using MessengerServer.Core.Infrastructure;
using MessengerServer.Core.Models;

namespace MessengerServer.Server;

public class AppServer : IAsyncDisposable
{
    private TcpListener _tcpListener;
    private DatabaseContext _databaseContext;

    private readonly object _stateLocker = new object();
    private bool _isRunning;
    
    private ConcurrentQueue<Message> _messages;
    
    private bool IsRunning
    {
        get
        {
            lock (_stateLocker)
            {
                return _isRunning;
            }
        }
        
        set
        {
            lock (_stateLocker)
            {
                _isRunning = value;
            }
        }
    }
    
    public async void StartAsync()
    {
        try
        {
            _databaseContext = new DatabaseContext();
            await _databaseContext.ConnectToDatabaseAsync();
            _messages = new ConcurrentQueue<Message>(await _databaseContext.GetAllMessagesAsync());
            
            _tcpListener = new TcpListener(IPAddress.Any, 8888);
            _tcpListener.Start();

            IsRunning = true;
            Console.WriteLine("Server is started.");

            while (IsRunning)
            {
                TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
                Task.Run(() => HandleClientAsync(tcpClient));
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
    
    private async void HandleClientAsync(TcpClient tcpClient)
    {
        try
        {
            NetworkAdaptor networkAdaptor = new NetworkAdaptor(tcpClient.GetStream());
            bool quitCommandReceived = false;
            
            while (!quitCommandReceived)
            {
                Query query = await networkAdaptor.ReceiveQueryAsync();
                Response response;
                
                switch (query.Header)
                {
                    case QueryHeader.SignIn:
                        response = await SignIn(query.JsonDataString);
                        await networkAdaptor.SendResponseAsync(response);
                        break;
                    
                    case QueryHeader.SignUp:
                        response = await SignUp(query.JsonDataString);
                        await networkAdaptor.SendResponseAsync(response);
                        break;

                    case QueryHeader.UpdateChat:
                        string jsonMessageBuffer = JsonSerializer.Serialize(_messages.ToArray());
                        response = new Response(jsonMessageBuffer);
                        await networkAdaptor.SendResponseAsync(response);
                        break;
                    
                    case QueryHeader.PostMessage:
                        response = await PostMessage(query.JsonDataString);
                        await networkAdaptor.SendResponseAsync(response);
                        break;
                        
                    case QueryHeader.Quit:
                        quitCommandReceived = true;
                        break;

                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }

            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        finally
        {
            tcpClient.Close();
        }
        
    }

    private async Task<Response> SignIn(string jsonDataString)
    {
        User user = JsonSerializer.Deserialize<User>(jsonDataString);
        bool success = await _databaseContext.IsUserExistsAsync(user);

        string jsonMessageBuffer = JsonSerializer.Serialize(success);
        return new Response(jsonMessageBuffer);
    }
    
    private async Task<Response> SignUp(string jsonDataString)
    {
        User user = JsonSerializer.Deserialize<User>(jsonDataString);
        bool success = await _databaseContext.CreateUserAsync(user);

        string jsonMessageBuffer = JsonSerializer.Serialize(success);
        return new Response(jsonMessageBuffer);
    }
    
    private async Task<Response> PostMessage(string jsonDataString)
    {
        Message message = JsonSerializer.Deserialize<Message>(jsonDataString);
        bool success = await _databaseContext.PostMessageAsync(message);
                        
        if (success)
        {
            _messages.Clear();
            _messages = new ConcurrentQueue<Message>(await _databaseContext.GetAllMessagesAsync());
        }

        string jsonMessageBuffer = JsonSerializer.Serialize(success);
        return new Response(jsonMessageBuffer);
    }
    

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