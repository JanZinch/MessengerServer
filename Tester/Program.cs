using System.Net.Sockets;
using System.Text;

namespace Tester;

public static class Program
{
    private static Client _client;
    
    public static async Task Main(string[] args)
    {
        /*string s = "redroom\ntext\n0\n";

        s = s.Replace("\n", string.Empty);
        s = s.Remove(0, 3);*/
        //s.Substring(0, 3);
        
        
        //Console.WriteLine(s);

        /*for (int i = 0; i < 256; i++)
        {
            Console.WriteLine(Convert.ToString(i, 2));
        }
        */
        
        //Console.WriteLine(Convert.ToByte("000001"));
        

        _client = new Client();
        await _client.Run();
    }

    
    
    
}