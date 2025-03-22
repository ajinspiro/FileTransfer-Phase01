using Common;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Sender;

internal static class Client
{
    internal static async Task Run()
    {
        // v4: v3 code modified to send image and its metadata. working
        using TcpClient client = new TcpClient("127.0.0.1", 13000);
        using NetworkStream channel = client.GetStream();
        using BinaryWriter channelWriter = new(channel);

        using FileStream fileStream = new(Constants.FilePath, FileMode.Open);
        using BinaryReader fileReader = new(fileStream);
        string filename = Path.GetFileName(Constants.FilePath);
        var metadataObj = new { filename, filesize = fileStream.Length };
        string metadata = JsonSerializer.Serialize(metadataObj);
        channelWriter.Write(metadata);
        for (int i = 0; i < fileStream.Length; i++)
        {
            byte readByte = fileReader.ReadByte();
            channelWriter.Write(readByte);
        }
        await Task.Delay(1000);
    }
    internal static async Task RunK()
    {
        // v3 : use binary writer to write image to stream. working correctly. (no metadata)
        using TcpClient client = new TcpClient("127.0.0.1", 13000);
        using NetworkStream channel = client.GetStream();
        using BinaryWriter channelWriter = new(channel);

        using FileStream fileStream = new(Constants.FilePath, FileMode.Open);
        using BinaryReader fileReader = new(fileStream);
        channelWriter.Write(fileStream.Length);
        for (int i = 0; i < fileStream.Length; i++)
        {
            byte readByte = fileReader.ReadByte();
            channelWriter.Write(readByte);
        }
        await Task.Delay(1000);
    }
    internal static async Task RunX()
    {
        // v2.1. working correctly
        using TcpClient client = new TcpClient("127.0.0.1", 13000);
        var channel = client.GetStream();

        using FileStream fileStream = new(Constants.FilePath, FileMode.Open);

        await fileStream.CopyToAsync(channel);
    }
    internal static async Task Run0()
    {
        // v1. not working
        // Preparing metadata
        using FileStream fileStream = new(Constants.FilePath, FileMode.Open);
        string filename = Path.GetFileName(fileStream.Name);
        long filesize = fileStream.Length;
        Dictionary<string, string> metadata = new() { { "filename", filename }, { "filesize", filesize.ToString() } };
        string metadataPayloadString = JsonSerializer.Serialize(metadata);
        byte[] metadataPayload = Encoding.UTF8.GetBytes(metadataPayloadString);

        // Sending metadata
        using TcpClient client = new TcpClient("127.0.0.1", 13000);
        var channel = client.GetStream();
        StreamWriter streamWriter = new(channel, Encoding.UTF8)
        {
            AutoFlush = true
        };
        await streamWriter.WriteLineAsync(metadataPayloadString);

        fileStream.CopyTo(channel);
    }
}
