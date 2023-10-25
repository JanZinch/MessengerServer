using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MessengerServer;

public static class Program
{
    private const int MaxConnections = 100;
    
    public static async Task Main(string[] args)
    {
        var tcpListener = new TcpListener(IPAddress.Any, 8888);
 
        try
        {
            tcpListener.Start();    // запускаем сервер
            Console.WriteLine("Сервер запущен. Ожидание подключений... ");
 
            while (true)
            {
                // получаем подключение в виде TcpClient
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
 
                // создаем новую задачу для обслуживания нового клиента
                Task.Run(async ()=>await ProcessClientAsync(tcpClient));
 
                // вместо задач можно использовать стандартный Thread
                // new Thread(async ()=>await ProcessClientAsync(tcpClient)).Start();
            }
        }
        finally
        {
            tcpListener.Stop();
        }
// обрабатываем клиент
        async Task ProcessClientAsync(TcpClient tcpClient)
        {
            // условный словарь
            var words = new Dictionary<string, string>()
            {
                {"red", "красный" },
                {"blue", "синий" },
                {"green", "зеленый" },
            };
            var stream = tcpClient.GetStream();
            // буфер для входящих данных
            var response = new List<byte>();
            int bytesRead = 10;
            while (true)
            {
                // считываем данные до конечного символа
                while ((bytesRead = stream.ReadByte()) != '\n')
                {
                    // добавляем в буфер
                    response.Add((byte)bytesRead);
                }
                var word = Encoding.UTF8.GetString(response.ToArray());
 
                // если прислан маркер окончания взаимодействия,
                // выходим из цикла и завершаем взаимодействие с клиентом
                if (word == "END") break;
 
                Console.WriteLine($"Клиент {tcpClient.Client.RemoteEndPoint} запросил перевод слова {word}");
                // находим слово в словаре и отправляем обратно клиенту
                if (!words.TryGetValue(word, out var translation)) translation = "не найдено в словаре";
                // добавляем символ окончания сообщения 
                translation += '\n';
                // отправляем перевод слова из словаря
                await stream.WriteAsync(Encoding.UTF8.GetBytes(translation));
                response.Clear();
            }
            tcpClient.Close();
        }
    } 
    
    
}