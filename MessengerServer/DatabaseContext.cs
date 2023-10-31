using System.Data;
using MessengerServer.Core.Models;
using Microsoft.Data.SqlClient;

namespace MessengerServer;

public class DatabaseContext : IAsyncDisposable
{
    private const string ConnectionString =
        "Server=localhost;Database=ChatAppDB;Trusted_Connection=True;TrustServerCertificate=True;";
    
    private const string FindUserTemplate = "SELECT [Nickname] FROM [User] WHERE [Nickname] = '{0}' AND [Password] = '{1}'";
    
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
    
    /*public void Dispose()
    {
        if (_connection.State == ConnectionState.Open)
        {
            _connection.CloseAsync().ContinueWith(task =>
            {
                Console.WriteLine("Disconnected from database");
            });
        }
        
    }*/

    public async ValueTask DisposeAsync()
    {
        if (_connection.State == ConnectionState.Open)
        {
            await _connection.CloseAsync();
            Console.WriteLine("Disconnected from database");
        }
    }
}