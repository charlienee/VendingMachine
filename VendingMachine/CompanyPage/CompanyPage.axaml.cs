using System.Collections.ObjectModel;
using System.Reflection.Metadata;
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
        using (var dataWorker = new DataWorker())
        {
            Company_dg.ItemsSource = new ObservableCollection<Company>(dataWorker.GetAllCompanies());
        }
    }
    private void CreateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        var createCompanyWindow = new CreateCompanyWindow();
        createCompanyWindow.ShowDialog(_mainWindow);
    }
}