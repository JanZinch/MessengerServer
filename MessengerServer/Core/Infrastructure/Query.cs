namespace MessengerServer.Core.Infrastructure;

public class Query
{
    private const int HeaderLength = 3;
    private const string NewLine = "\n";
    
    public QueryHeader Header { get; private set; }
    public string JsonDataString { get; private set; }

    public Query(QueryHeader header, string jsonDataString)
    {
        Header = header;
        JsonDataString = jsonDataString;
    }
    
    public static Query FromRawSource(string source)
    {
        QueryHeader header = (QueryHeader) Convert.ToByte(source.Substring(0, HeaderLength));
        
        string jsonDataString = source.Remove(0, HeaderLength).Replace(NewLine, string.Empty);

        return new Query(header, jsonDataString);
    }

    private static string ToByteNotation(QueryHeader header)
    {
        const int byteStringLength = 3;
        string result = ((byte)header).ToString();
        int emptyCharsCount = byteStringLength - result.Length;

        if (emptyCharsCount > 0)
        {
            result = result.Insert(0, new string('0', emptyCharsCount));
        }

        return result;
    }

    public override string ToString()
    {
        return ToByteNotation(Header) + JsonDataString + NewLine;
    }
}