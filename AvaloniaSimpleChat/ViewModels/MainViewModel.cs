using System;
using AvaloniaSimpleChat.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaSimpleChat.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessenger _messageBus;
    [ObservableProperty] private ViewModelBase _currentPage;
    
    public MainViewModel(IServiceProvider serviceProvider, IMessenger messageBus)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        CurrentPage = serviceProvider.GetRequiredService<LoginViewModel>();

        _messageBus.Register<UserLoggedInMessage>(this, (s, e) => 
            CurrentPage = serviceProvider.GetRequiredService<ChatViewModel>());
    }
}