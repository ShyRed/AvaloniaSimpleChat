using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AvaloniaSimpleChat.Services;
using AvaloniaSimpleChat.ViewModels;
using AvaloniaSimpleChat.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace AvaloniaSimpleChat;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ServiceCollection services = new();
        
        // Views & ViewModels
        services.AddTransient<ChatView>();
        services.AddTransient<ChatViewModel>();
        services.AddTransient<LoginView>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainView>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
        services.AddSingleton<ViewLocator>();
        
        // ApiClient
        services.AddSingleton<ApiClient.ApiClient>();
        services.AddSingleton<IAuthenticationProvider, TokenAuthenticationProvider>();
        services.AddSingleton<IRequestAdapter, HttpClientRequestAdapter>();

        services.AddSingleton<IChatService, ChatService>();
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        DataTemplates.Add(serviceProvider.GetRequiredService<ViewLocator>());
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            desktop.MainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = serviceProvider.GetRequiredService<MainView>();
            singleViewPlatform.MainView.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}