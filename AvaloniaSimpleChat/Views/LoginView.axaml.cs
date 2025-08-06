using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSimpleChat.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();

        this.Loaded += (sender, args) => UsernameTextBox.Focus();
    }
}