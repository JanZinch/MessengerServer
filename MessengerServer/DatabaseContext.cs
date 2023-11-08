using System.Collections.Concurrent;
using System.Data;
using MessengerServer.Core.Models;
using Microsoft.Data.SqlClient;

namespace MessengerServer;

public class DatabaseContext : IAsyncDisposable
{
    private const string ConnectionString =
        "Server=localhost;Database=ChatAppDB;Trusted_Connection=True;TrustServerCertificate=True;";
    
    private const string FindUserExpression = "SELECT [Nickname] FROM [User] WHERE [Nickname] = @Nickname AND [Password] = @Password";
    private const string CreateUserExpression = "INSERT INTO [User] VALUES (@Nickname, @Password)";
    
    private const string GetAllMessagesExpression = "SELECT * FROM [Message]";
    
    private const string PostMessageExpression = "INSERT INTO [Message] VALUES (@SenderNickname, @ReceiverNickname, @Text, @PostDateTime)";
    
    private SqlConnection _connection;

    public DatabaseContext() 
    {
        _connection = new SqlConnection(ConnectionString);
    }

    public async Task ConnectToDatabaseAsync()
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

    public async Task<bool> IsUserExistsAsync(User user)
    {
        try
        {
            SqlCommand command = new SqlCommand(FindUserExpression, _connection);
            command.Parameters.Add(new SqlParameter("@Nickname", user.Nickname));
            command.Parameters.Add(new SqlParameter("@Password", user.Password));
            
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
    
    public async Task<bool> CreateUserAsync(User user)
    {
        try
        {
            SqlCommand command = new SqlCommand(CreateUserExpression, _connection);
            command.Parameters.Add(new SqlParameter("@Nickname", user.Nickname));
            command.Parameters.Add(new SqlParameter("@Password", user.Password));
            
            int affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }
    
    public async Task<LinkedList<Message>> GetAllMessagesAsync()
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
                        ReceiverNickname = reader.GetStringSafe(1),
                        Text = reader.GetString(2),
                        PostDateTime = reader.GetDateTime(3)
                    };

                    _messages.AddLast(message);
                }
            }

            await reader.CloseAsync();

            _messages = new LinkedList<Message>(_messages.OrderBy(message => message.PostDateTime));
            
            return _messages;
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<bool> PostMessageAsync(Message message)
    {
        try
        {
            SqlCommand command = new SqlCommand(PostMessageExpression, _connection);
            command.Parameters.Add(new SqlParameter("@SenderNickname", message.SenderNickname));
            command.Parameters.Add(new SqlParameter("@ReceiverNickname", ToNullableDbObject(message.ReceiverNickname)));
            command.Parameters.Add(new SqlParameter("@Text", message.Text));
            command.Parameters.Add(new SqlParameter("@PostDateTime", message.PostDateTime));

            int affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
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

    private static object ToNullableDbObject<T>(T source) where T : class
    {
        return source != null ? source : DBNull.Value;
    }
}