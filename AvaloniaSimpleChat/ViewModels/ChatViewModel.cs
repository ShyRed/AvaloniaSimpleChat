using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaSimpleChat.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaSimpleChat.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    public record ChatMessage(string Username, string Message);
    
    [ObservableProperty] private ObservableCollection<ChatMessage> _messages = new();
    [ObservableProperty] private string _message = string.Empty;
    private readonly IChatService _chatService;
    private readonly IMessenger _messageBus;

    public ChatViewModel(IChatService chatService, IMessenger messageBus)
    {
        _chatService = chatService;
        _messageBus = messageBus;

        _messageBus.Register<MessageReceivedMessage>(this, (s, e) =>
            Dispatcher.UIThread.InvokeAsync(() => Messages.Add(new ChatMessage(e.Username, e.Message))));
    }

    [RelayCommand]
    private async Task SendMessage()
    {
        await _chatService.SendMessageAsync(Message);
        Message = string.Empty;
    }
    
}