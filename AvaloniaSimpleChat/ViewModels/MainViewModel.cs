using System;
using AvaloniaSimpleChat.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace AvaloniaSimpleChat.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    [ObservableProperty] private ViewModelBase _currentPage;
    
    public MainViewModel(IServiceProvider serviceProvider, IMessageBus messageBus)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        CurrentPage = serviceProvider.GetRequiredService<LoginViewModel>();

        _messageBus.Listen<UserLoggedInMessage>()
            .Subscribe(msg => CurrentPage = serviceProvider.GetRequiredService<ChatViewModel>());
    }
}