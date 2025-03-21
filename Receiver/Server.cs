using Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


internal static class Server
{
    internal static async Task Run()
    {
        TcpListener server = null;
        Int32 port = 13000;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        server = new TcpListener(localAddr, port);
        server.Start();
        while (true)
        {
            Console.Write("Waiting for a connection... ");
            TcpClient client = server.AcceptTcpClient();
            _ = Task.Run(() => ProcessClient(client));
        }
    }

    internal static async Task ProcessClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();

        StreamReader streamReader = new(stream);
        string? metadataString = await streamReader.ReadLineAsync();
        ArgumentException.ThrowIfNullOrWhiteSpace(metadataString);
        var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(metadataString);
        ArgumentNullException.ThrowIfNull(metadata);
        int filesize = int.Parse(metadata["filesize"]);
        Console.WriteLine(metadata["filename"]);

        FileStream inputFile = new($"{Constants.ServerOutputFolder}/{metadata["filename"]}", FileMode.Create);

        byte[] bytes = new byte[filesize];

        int readBytes = stream.Read(bytes, 0, filesize);
        await inputFile.WriteAsync(bytes, 0, filesize);
        client.Close();
    }
}
