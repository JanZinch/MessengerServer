using Microsoft.Data.SqlClient;

namespace MessengerServer;

public static class DatabaseExtensions
{
    public static string GetStringSafe(this SqlDataReader reader, int colIndex)
    {
        return reader.IsDBNull(colIndex) ? string.Empty : reader.GetString(colIndex);
    }
}