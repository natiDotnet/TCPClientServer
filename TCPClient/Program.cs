// Client/Program.cs
using System.Net.Sockets;
using System.Text;

Console.WriteLine("TCP Client");
const string server = "127.0.0.1";
const int port = 5000;

try
{
    using var client = new TcpClient();
    await client.ConnectAsync(server, port);
    Console.WriteLine($"Connected to server {server}:{port}");

    var command = args.Length > 0 ? args[0] : "GET_STATUS";
    Console.WriteLine($"Sending command: {command}");

    using var stream = client.GetStream();
    using var writer = new StreamWriter(stream, Encoding.UTF8);
    using var reader = new StreamReader(stream, Encoding.UTF8);

    await writer.WriteLineAsync(command);
    await writer.FlushAsync();
    
    var response = await reader.ReadLineAsync();
    Console.WriteLine($"Server response: {response}");
}
catch (SocketException ex)
{
    Console.WriteLine($"Connection error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}