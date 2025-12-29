using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VendingMachine.Models;

namespace VendingMachine;

public partial class CompanyPage : UserControl
{
    public ObservableCollection<Company> Companies {get; set;}
    private User user;
    private MainWindow _mainWindow;
    public CompanyPage()
    {
        InitializeComponent();
    }
    public CompanyPage(User currentUser, MainWindow mainWindow) : this()
    {
        user = currentUser;
        DataContext = this;
        _mainWindow = mainWindow;
        UpdateCompanies();
        Company_dg.Columns.Add(new DataGridTemplateColumn
        {
            Header = "Действия"
        });
        UpdateActionCompanies();
        CreateCompany_btn.IsVisible = currentUser.Role == "Менеджер";
    }
    public void UpdateActionCompanies()
    {
        var actionColumn = Company_dg.Columns.OfType<DataGridTemplateColumn>().FirstOrDefault(c => c.Header?.ToString() == "Действия");
        if (actionColumn == null)
            return;
        actionColumn.CellTemplate = new FuncDataTemplate<Company>((company, _)=>
        {
            var panel = new StackPanel{Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing=4};
            var activateBTN = new Button
            {
                Content="Активировать",
                IsVisible = user.Role == "Менеджер" && company.Status == "Привязка пользователя"
            };
            var blockBTN = new Button
            {
                Content="Заблокировать",
                IsVisible = user.Role == "Менеджер" && company.Status == "Активно"
            };
            activateBTN.Click += ActivateCompanyBtn_Click;
            blockBTN.Click += BlockBtn_Click;
            panel.Children.Add(activateBTN);
            panel.Children.Add(blockBTN);
            return panel;
        });
    }
    public void BlockBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Company company)
        {
            using (var dataWorker = new DataWorker())
            {
                dataWorker.UpdateCompanyStatus(company.CompanyCode, "Заблокировано");
                dataWorker.UpdateIsBlocked(company.UserId, true);
            }
        }
        UpdateCompanies();
        UpdateActionCompanies();
    }
    public void ActivateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Company company)
        {
            using (var dataWorker = new DataWorker())
            {
                dataWorker.UpdateCompanyStatus(company.CompanyCode, "Активно");
            }
        }
        UpdateCompanies();
        UpdateActionCompanies();
    }
    public void UpdateCompanies()
    {
        using (var dataWorker = new DataWorker())
        {
            Companies = new ObservableCollection<Company>(dataWorker.GetAllCompanies());
            Company_dg.ItemsSource = Companies;
        }
    }
    private void CreateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        var createCompanyWindow = new CreateCompanyWindow();
        createCompanyWindow.CompanyCreated += UpdateCompanies;
        createCompanyWindow.ShowDialog(_mainWindow);
        
    }
}