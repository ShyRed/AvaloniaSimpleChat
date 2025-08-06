using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.SignalR.Client;

namespace AvaloniaSimpleChat.Services;

public record MessageReceivedMessage(string Username, string Message); 
public record UserConnectedMessage(string Username);
public record UserDisconnectedMessage(string Username);

public interface IChatService
{
    Task ConnectAsync(string serverUrl, string token);
    Task DisconnectAsync();
    Task SendMessageAsync(string message);
    bool IsConnected { get; }
}

public class ChatService : IChatService
{
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    private HubConnection? _hubConnection;
    private readonly IMessenger _messenger;

    public ChatService(IMessenger messenger)
    {
        _messenger = messenger;
    }

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
            _messenger.Send(new MessageReceivedMessage(username, message));
        });

        _hubConnection.On<string>("UserConnected", (username) =>
        {
            _messenger.Send(new UserConnectedMessage(username));
        });

        _hubConnection.On<string>("UserDisconnected", (username) =>
        {
            _messenger.Send(new UserDisconnectedMessage(username));
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