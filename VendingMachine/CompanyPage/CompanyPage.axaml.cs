using System;
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
    public ObservableCollection<Company> Companies {get; set;}
    private MainWindow _mainWindow;
    public CompanyPage(User currentUser, MainWindow mainWindow)
    {
        InitializeComponent();
        DataContext = this;
        _mainWindow = mainWindow;
        if (currentUser.Role != "Менеджер")
        {
            CreateCompany_btn.IsVisible = false;
        }
        using (var dataWorker = new DataWorker())
        {
            Companies = new ObservableCollection<Company>(dataWorker.GetAllCompanies());
            Console.WriteLine(Companies.Count);
            Company_dg.ItemsSource = Companies;
        }
    }
    private void CreateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        var createCompanyWindow = new CreateCompanyWindow();
        createCompanyWindow.ShowDialog(_mainWindow);
    }
}