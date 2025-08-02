using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace AvaloniaSimpleChat.Services;

public interface IChatService
{
    event EventHandler<(string Username, string Message)> MessageReceived;
    event EventHandler<string> UserConnected;
    event EventHandler<string> UserDisconnected;
    
    Task ConnectAsync(string serverUrl, string token);
    Task DisconnectAsync();
    Task SendMessageAsync(string message);
    bool IsConnected { get; }
}

public class ChatService : IChatService
{
    private HubConnection? _hubConnection;
    
    public event EventHandler<(string Username, string Message)>? MessageReceived;
    public event EventHandler<string>? UserConnected;
    public event EventHandler<string>? UserDisconnected;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public async Task ConnectAsync(string serverUrl, string token)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/api/chatHub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect()
            .Build();

        // Set up message handlers
        _hubConnection.On<string, string>("ReceiveMessage", (username, message) =>
        {
            MessageReceived?.Invoke(this, (username, message));
        });

        _hubConnection.On<string>("UserConnected", (username) =>
        {
            UserConnected?.Invoke(this, username);
        });

        _hubConnection.On<string>("UserDisconnected", (username) =>
        {
            UserDisconnected?.Invoke(this, username);
        });

        await _hubConnection.StartAsync();
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("SendMessage", message);
        }
        else
        {
            throw new InvalidOperationException("Not connected to chat hub");
        }
    }
}