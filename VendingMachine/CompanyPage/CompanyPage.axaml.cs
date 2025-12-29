using System.Runtime.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VendingMachine.Models;

namespace VendingMachine;

public partial class CompanyPage : UserControl
{
    private MainWindow _mainWindow;
    public CompanyPage(User currentUser, MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        if (currentUser.Role != "Менеджер")
        {
            CreateCompany_btn.IsVisible = false;
        }
    }
    private void CreateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        var createCompanyWindow = new CreateCompanyWindow();
        createCompanyWindow.ShowDialog(_mainWindow);
    }
}