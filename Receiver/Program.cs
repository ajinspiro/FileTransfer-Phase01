// Server

using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using Common;

await Run();

static async Task Run()
{
    IPAddress localAddr = IPAddress.Parse("127.0.0.1");
    int port = 13000;
    TcpListener server = new(localAddr, port);
    server.Start();
    while (true)
    {
        Console.WriteLine("Listener: Waiting for a connection... ");
        TcpClient client = await server.AcceptTcpClientAsync();
        _ = Task.Run(() => ProcessClient_4(client));
    }
}
static async Task ProcessClient_4(TcpClient client)
{
    // v4: v3 code modified to send image and its metadata. woking correctly
    Console.WriteLine("Connected accepted.");
    using NetworkStream channel = client.GetStream();

    using BinaryReader channelReader = new(channel);
    string metadata = channelReader.ReadString();
    Metadata metadataObj = JsonSerializer.Deserialize<Metadata>(metadata) ?? throw new Exception();
    using FileStream imageFile = new($"{Constants.ServerOutputFolder}/{metadataObj.filename}", FileMode.Create);
    using BinaryWriter fileWriter = new(imageFile);
    Console.WriteLine(metadata);
    for (int i = 0; i < metadataObj.filesize; i++)
    {
        byte byteRead = channelReader.ReadByte();
        fileWriter.Write(byteRead);
    }
    client.Close();
    await Task.Delay(100);
}
static async Task ProcessClient_3(TcpClient client)
{
    // v3 : use binary writer to write image to stream. working correctly.
    using NetworkStream channel = client.GetStream();

    using FileStream imageFile = new($"{Constants.ServerOutputFolder}/IMG-20250318-WA0001.jpg", FileMode.Create);
    using BinaryWriter fileWriter = new(imageFile);
    using BinaryReader channelReader = new(channel);
    long length = channelReader.ReadInt64();
    for (int i = 0; i < length; i++)
    {
        byte byteRead = channelReader.ReadByte();
        fileWriter.Write(byteRead);
    }
    client.Close();
    await Task.Delay(100);
}
static async Task ProcessClient2_2(TcpClient client)
{
    // v2.2 hard coded. working correctly
    NetworkStream stream = client.GetStream();

    FileStream inputFile = new($"{Constants.ServerOutputFolder}/IMG-20250318-WA0001.jpg", FileMode.Create);

    await stream.CopyToAsync(inputFile);
    client.Close();
}
static async Task ProcessClient2_1(TcpClient client)
{
    // v2.1 hard coded. working correctly.
    NetworkStream stream = client.GetStream();

    FileStream inputFile = new($"{Constants.ServerOutputFolder}/IMG-20250318-WA0001.jpg", FileMode.Create);

    int filesize = 90674;
    byte[] bytes = new byte[filesize];

    int readBytes = stream.Read(bytes, 0, filesize);
    await inputFile.WriteAsync(bytes, 0, filesize);
    client.Close();
}
static async Task ProcessClient_1(TcpClient client)
{
    // v1. not working
    NetworkStream stream = client.GetStream();

    StreamReader streamReader = new(stream, Encoding.UTF8);
    string? metadataString = await streamReader.ReadLineAsync();
    ArgumentException.ThrowIfNullOrWhiteSpace(metadataString);
    var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(metadataString);
    ArgumentNullException.ThrowIfNull(metadata);
    int filesize = int.Parse(metadata["filesize"]);
    Console.WriteLine(metadata["filename"]);
    Console.WriteLine(filesize);

    FileStream inputFile = new($"{Constants.ServerOutputFolder}/{metadata["filename"]}", FileMode.Create);

    byte[] bytes = new byte[filesize];

    int readBytes = stream.Read(bytes, 0, filesize);
    await inputFile.WriteAsync(bytes, 0, filesize);
    client.Close();
}

record Metadata(long filesize, string filename);
