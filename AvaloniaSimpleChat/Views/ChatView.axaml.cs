using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaSimpleChat.Views;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        MessageTextBox.Focus();
        if (DataContext is ViewModels.ChatViewModel vm)
            vm.Messages.CollectionChanged += MessagesOnCollectionChanged;
    }

    private async void MessagesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // Ensure we scroll after the new item has been rendered
            await Task.Delay(250);
            ChatScrollViewer.ScrollToEnd();
        }

    }
}