using System.Collections.Concurrent;
using System.Data;
using MessengerServer.Core.Models;
using Microsoft.Data.SqlClient;

namespace MessengerServer;

public class DatabaseContext : IAsyncDisposable
{
    private const string ConnectionString =
        "Server=localhost;Database=ChatAppDB;Trusted_Connection=True;TrustServerCertificate=True;";
    
    private const string FindUserTemplate = "SELECT [Nickname] FROM [User] WHERE [Nickname] = '{0}' AND [Password] = '{1}'";
    private const string GetAllMessagesExpression = "SELECT * FROM [Message]";
    
    private SqlConnection _connection;

    public DatabaseContext()
    {
        _connection = new SqlConnection(ConnectionString);
    }

    public async Task ConnectToDatabase()
    {
        try
        {
            await _connection.OpenAsync();
            Console.WriteLine("Connected with database");
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        
    }

    public async Task<bool> IsUserExists(User user)
    {
        try
        {
            string findUserExpression = string.Format(FindUserTemplate, user.Nickname, user.Password);
            
            SqlCommand command = new SqlCommand(findUserExpression, _connection);
            SqlDataReader reader = await command.ExecuteReaderAsync();

            bool result = reader.HasRows;
            await reader.CloseAsync();
            
            return result;
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }
    
    public async Task<LinkedList<Message>> GetAllMessages()
    {
        try
        {
            SqlCommand command = new SqlCommand(GetAllMessagesExpression, _connection);
            SqlDataReader reader = await command.ExecuteReaderAsync();
            
            LinkedList<Message> _messages = new LinkedList<Message>();
            
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Message message = new Message()
                    {
                        SenderNickname = reader.GetString(0),
                        ReceiverNickname = reader.GetString(1),
                        Text = reader.GetString(2),
                        PostDateTime = reader.GetDateTime(3)
                    };

                    _messages.AddLast(message);
                }
            }

            await reader.CloseAsync();
            
            return _messages;
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection.State == ConnectionState.Open)
        {
            await _connection.CloseAsync();
            Console.WriteLine("Disconnected from database");
        }
    }
}