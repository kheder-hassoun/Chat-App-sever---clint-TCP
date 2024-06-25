using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    // 😁 list of stupied clints 

    static List<TcpClient> clients = new List<TcpClient>();
    static TcpListener server; // step one 

    static void Main()
    {
        StartServer();
    }

    static void StartServer()
    {
        server = new TcpListener(IPAddress.Any, 8000);
        server.Start();
        Console.WriteLine("\n\t\t\t hi kheder \n");
        Console.WriteLine("  am the sever now am listening for clients connections on port 8000  ******* : ");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient(); 
            clients.Add(client);

            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);

            // we got this from the enternet (thanks for AI ) 😁😁😁

            Console.WriteLine($"Client connected from {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}");
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream clientStream = tcpClient.GetStream();

        while (true)
        {
            byte[] messageBytes = new byte[4096];
            int bytesRead;

            try
            {
                bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0)
                break;

            string message = Encoding.UTF8.GetString(messageBytes, 0, bytesRead);
            BroadcastMessage(message, tcpClient);
            Console.WriteLine($" a message frome  <{((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}:{((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}>: {message}");
        }
        // we should remove the stupied clints from the list  after they leave
        clients.Remove(tcpClient);
        tcpClient.Close();
    }

    static void BroadcastMessage(string message, TcpClient excludeClient = null)
    {
        foreach (TcpClient client in clients)
        {
            if (client != excludeClient)
            {
                NetworkStream clientStream = client.GetStream();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                clientStream.Write(messageBytes, 0, messageBytes.Length);
                clientStream.Flush();
            }
        }
    }
}
