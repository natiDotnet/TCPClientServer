using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPServer;

public class TcpServerService : BackgroundService
{
    private readonly ILogger<TcpServerService> _logger;
    private TcpListener _listener = null!;

    public TcpServerService(ILogger<TcpServerService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int port = 5000;
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        _logger.LogInformation($"Server started on port {port}");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync(stoppingToken);
                _ = HandleClientAsync(client, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Server is stopping
        }
        finally
        {
            _listener.Stop();
            _logger.LogInformation("Server stopped");
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
    {
        using (client)
        {
            try
            {
                _logger.LogInformation($"Client connected: {client.Client.RemoteEndPoint}");
                
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8);

                var command = await reader.ReadLineAsync();
                if (command == null) return;

                _logger.LogInformation($"Received command: {command}");
                var response = ProcessCommand(command);
                
                await writer.WriteLineAsync(response);
                await writer.FlushAsync();
                _logger.LogInformation($"Sent response: {response}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling client");
            }
            finally
            {
                _logger.LogInformation($"Client disconnected: {client.Client.RemoteEndPoint}");
            }
        }
    }

    private static string ProcessCommand(string command)
    {
        return command.Trim().ToUpper() switch
        {
            "GET_TEMP" => "25.3°C",
            "GET_STATUS" => "Active",
            _ => "ERROR: Unknown command"
        };
    }
}
