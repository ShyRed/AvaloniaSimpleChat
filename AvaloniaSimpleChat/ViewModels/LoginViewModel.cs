using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaSimpleChat.ApiClient.Models;
using AvaloniaSimpleChat.Messages;
using AvaloniaSimpleChat.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AvaloniaSimpleChat.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [CustomValidation(typeof(LoginViewModel), nameof(ValidateUrl))] [NotifyDataErrorInfo] [ObservableProperty]
    private string _serverUrl = "http://localhost:5000";

    [CustomValidation(typeof(LoginViewModel), nameof(ValidateUsername))]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [ObservableProperty]
    private string _username = string.Empty;

    [CustomValidation(typeof(LoginViewModel), nameof(ValidatePassword))]
    [NotifyDataErrorInfo]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty] private string _errorText = string.Empty;

    private readonly ApiClient.ApiClient _apiClient;
    private readonly IRequestAdapter _requestAdapter;
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IMessenger _messageBus;
    private readonly IChatService _chatService;

    public LoginViewModel(ApiClient.ApiClient apiClient,
        IRequestAdapter requestAdapter,
        IAuthenticationProvider authenticationProvider,
        IMessenger messageBus, IChatService chatService)
    {
        _apiClient = apiClient;
        _requestAdapter = requestAdapter;
        _authenticationProvider = authenticationProvider;
        _messageBus = messageBus;
        _chatService = chatService;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    public async Task Login()
    {
        try
        {
            _requestAdapter.BaseUrl = ServerUrl;
            string? token = await _apiClient.Api.Login.PostAsync(new LoginRequest()
            {
                Username = Username,
                Password = Password
            });
            ((TokenAuthenticationProvider)_authenticationProvider).Token = token ?? string.Empty;

            if (string.IsNullOrEmpty(token))
                return;

            await _chatService.ConnectAsync(ServerUrl, token);

            if (!string.IsNullOrEmpty(token))
                _messageBus.Send(new UserLoggedInMessage(Username, token));
        }
        catch (Exception e)
        {
            ErrorText = e.Message;
        }
    }

    public static ValidationResult? ValidateUrl(string? url, ValidationContext context)
    {
        if (string.IsNullOrEmpty(url))
            return new ValidationResult("Server URL cannot be empty");

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            return new ValidationResult("Server URL must be a valid URL");

        return ValidationResult.Success;
    }

    public static ValidationResult? ValidateUsername(string? username, ValidationContext context)
    {
        if (string.IsNullOrEmpty(username))
            return new ValidationResult("Username cannot be empty");

        if (username.Any(char.IsWhiteSpace))
            return new ValidationResult("Username cannot contain spaces");

        return ValidationResult.Success;
    }

    public static ValidationResult? ValidatePassword(string? password, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(password))
            return new ValidationResult("Password cannot be empty");
        
        if (password.Length < 8)
            return new ValidationResult("Password must be at least 8 characters long");

        return ValidationResult.Success;
    }

    private bool CanLogin() => !HasErrors && 
                               !string.IsNullOrWhiteSpace(Username) && 
                               !string.IsNullOrWhiteSpace(Password);

}