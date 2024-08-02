using System.Windows;
using System.Windows.Input;
using EasyChat.ViewModels;

namespace EasyChat.Views.SubControls;

public partial class UcLogin
{
    public UcLogin()
    {
        InitializeComponent();
    }

    private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        
        ((LoginViewModel)DataContext).LoginCommand.Execute(Window.GetWindow(this));
    }
}