using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpClient client;
    static NetworkStream stream;

    static void Main()
    {
        ConnectToServer();
        StartListeningForMessages();

        while (true)
        {
            string message = Console.ReadLine();
            SendMessage(message);
        }
    }

    static void ConnectToServer()
    {
        client = new TcpClient("127.0.0.1", 8000); // Connect to the server's IP and port
        stream = client.GetStream();
        Console.WriteLine("");
        Console.WriteLine($"am try to connected to server at {((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address}:{((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port}");
    }

    static void StartListeningForMessages()
    {
        Thread receiveThread = new Thread(() =>
        {
            while (true)
            {
                byte[] messageBytes = new byte[4096];
                int bytesRead;

                try
                {
                    bytesRead = stream.Read(messageBytes, 0, messageBytes.Length);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                    break;

                string message = Encoding.UTF8.GetString(messageBytes, 0, bytesRead);
                Console.WriteLine(message);
            }
        });

        receiveThread.Start();
    }

    static void SendMessage(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        stream.Write(messageBytes, 0, messageBytes.Length);
        stream.Flush();
    }
}
