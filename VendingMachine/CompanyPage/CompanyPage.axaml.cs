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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VendingMachine;

public partial class CompanyPage : UserControl
{
    private int PageSize = 5;
    private int _currentPage = 1;
    public List<Company> AllCompanies {get; set;}
    public List<Company> Companies {get; set;}
    private User user;
    private MainWindow _mainWindow;
    public CompanyPage()
    {
        InitializeComponent();
        using (var dataWorker = new DataWorker())
        {
            AllCompanies = dataWorker.GetAllCompanies();
            Companies = AllCompanies;
        }
    }
    public CompanyPage(User currentUser, MainWindow mainWindow) : this()
    {
        user = currentUser;
        DataContext = this;
        _mainWindow = mainWindow;
        UpdateCompanies();
        
        UpdateActionCompanies();
        CreateCompany_btn.IsVisible = currentUser.Role == "Менеджер";
    }
    //Create company
    private void CreateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        var createCompanyWindow = new CreateCompanyWindow();
        createCompanyWindow.CompanyCreated += UpdateCompanies;
        createCompanyWindow.ShowDialog(_mainWindow);
    }
    //Change size page
    private void PageSizeCB_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized)
        {
            return;
        }
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item)
        {
            var value = item.Content?.ToString() ?? string.Empty;
            switch (value)
            {
                case "5":
                    PageSize = 5;
                    break;
                case "10":
                    PageSize = 10;
                    break;
                case "Все":
                    PageSize = AllCompanies.Count;
                    break;
            }
        }
        UpdateCompanies();
        UpdateActionCompanies();
    }
    //Search by company name
    private void SearchTB_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!IsInitialized)
        {
            return;
        }
        if (sender is not TextBox textBox)
            return;
        if (textBox.Text?.Trim() == string.Empty || textBox.Text == null)
        {
            Companies = AllCompanies;
        }
        else
            Companies = new List<Company>(AllCompanies.Where(c => c.CompanyName.Contains(textBox.Text)));
        UpdateCompanies();
        UpdateActionCompanies();
    }
    
    //Update data in company datagrid
    public void UpdateActionCompanies()
    {
        var actionColumn = Company_dg.Columns.OfType<DataGridTemplateColumn>().FirstOrDefault(c => c.Header?.ToString() == "Настройка статуса");
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
    public void UpdateCompanies()
    {
        using (var dataWorker = new DataWorker())
        {
            
            Companies = new List<Company>(Companies);
            Company_dg.ItemsSource = new ObservableCollection<Company>(Companies.Skip((_currentPage - 1) * PageSize).Take(PageSize));
            currentPageTb.Text=_currentPage.ToString();
        }
    }
    //Companies buttons click
    public void DeleteCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Company company)
        {
            using (var dataWorker = new DataWorker())
            {
                dataWorker.DeleteCompany(company.CompanyCode);
            }
        }
        UpdateCompanies();
        UpdateActionCompanies();
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

    //Next and back page
    private void NextPageBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (AllCompanies.Count <= _currentPage*PageSize)
        {
            return;
        }
        _currentPage += 1;
        UpdateCompanies();
    }
    private void BackPageBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (_currentPage <= 1)
        {
            return;
        }
        _currentPage -= 1;
        UpdateCompanies();
    }

}