using Common;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Sender;

internal static class Client
{
    internal static async Task Run()
    {
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
