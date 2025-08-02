using System;
using System.Net.Http;
using System.Threading.Tasks;
using AvaloniaSimpleChat.ApiClient.Models;
using AvaloniaSimpleChat.Messages;
using AvaloniaSimpleChat.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using ReactiveUI;

namespace AvaloniaSimpleChat.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty] private string _serverUrl = "http://localhost:5000";
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;

    private readonly ApiClient.ApiClient _apiClient;
    private readonly IRequestAdapter _requestAdapter;
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IMessageBus _messageBus;

    public LoginViewModel(ApiClient.ApiClient apiClient,
        IRequestAdapter requestAdapter,
        IAuthenticationProvider authenticationProvider,
        IMessageBus messageBus)
    {
        _apiClient = apiClient;
        _requestAdapter = requestAdapter;
        _authenticationProvider = authenticationProvider;
        _messageBus = messageBus;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    public async Task Login()
    {
        _requestAdapter.BaseUrl = ServerUrl;
        string? token = await _apiClient.Api.Login.PostAsync(new LoginRequest()
        {
            Username = Username,
            Password = Password
        });
        ((TokenAuthenticationProvider)_authenticationProvider).Token = token ?? string.Empty;

        if (!string.IsNullOrEmpty(token))
            _messageBus.SendMessage(new UserLoggedInMessage(Username, token));
    }

    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Username) &&
        !string.IsNullOrWhiteSpace(Password);

    partial void OnUsernameChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
}