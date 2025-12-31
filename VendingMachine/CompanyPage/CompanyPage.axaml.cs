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
    private bool _isManager;
    private List<Company> _allCompanies;
    private List<Company> _filteredCompanies;
    private User _currentUser;
    private readonly Paginator<Company> _paginator = new(5);
    private readonly TextFilter<Company> _companyFilter = new(c => c.CompanyName);
    private MainWindow _mainWindow;
    public CompanyPage()
    {
        InitializeComponent();
        LoadAllCompanies();
    }
    public CompanyPage(User currentUser, MainWindow mainWindow) : this()
    {
        _currentUser = currentUser;
        _isManager = _currentUser.Role == "Менеджер";
        _mainWindow = mainWindow;

        DataContext = this;
        
        InitializeActionColumn();
        ApplyPagination();

        CreateCompany_btn.IsVisible = _isManager;
    }
    private void LoadAllCompanies()
    {
        using var db = new DataWorker();
        _allCompanies = db.GetAllCompanies();
        _filteredCompanies = _allCompanies;
    }
    private void ReloadCompanies()
    {
        LoadAllCompanies();
        _paginator.Reset();
        ApplyPagination();
    }
    private void ApplySearch(string searchText)
    {
        
        _filteredCompanies = _companyFilter
            .Apply(_allCompanies, searchText)
            .ToList();
        _paginator.Reset();
        ApplyPagination();
    }
    private void ResetSearchFilter()
    {
        SearchTB.Text = string.Empty;
    }
    private void ApplyPagination()
    {
        Company_dg.ItemsSource = new ObservableCollection<Company>(_paginator.Apply(_filteredCompanies));
        currentPageTb.Text=_paginator.CurrentPage.ToString();
    }
    private void InitializeActionColumn()
    {
        var actionColumn = Company_dg.Columns.OfType<DataGridTemplateColumn>().FirstOrDefault(c => c.Header?.ToString() == "Настройка статуса");
        if (actionColumn == null)
            return;
        actionColumn.CellTemplate = CreateActionTemplate();
    }
    private IDataTemplate CreateActionTemplate()
    {
        return new FuncDataTemplate<Company>((company, _) =>
        {
            var panel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal, 
                Spacing = 4
            };
            var activateBTN = new Button
            {
                Content="Активировать",
                IsVisible = _isManager && company.Status == "Привязка пользователя"
            };
            var blockBTN = new Button
            {
                Content="Заблокировать",
                IsVisible = _isManager && company.Status == "Активно"
            };
            activateBTN.Click += ActivateCompanyBtn_Click;
            blockBTN.Click += BlockBtn_Click;
            panel.Children.Add(activateBTN);
            panel.Children.Add(blockBTN);
            return panel;
        });
    }
    //Create company
    private void CreateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        var window = new CreateCompanyWindow();
        window.CompanyCreated += ReloadCompanies;
        window.ShowDialog(_mainWindow);
        SearchTB.Text = string.Empty;
    }
    //Change size page
    private void PageSizeCB_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized || sender is not ComboBox comboBox)
            return;
        _paginator.PageSize = comboBox.SelectedIndex
        switch
        {
            0 => 5,
            1 => 10,
            _ => _filteredCompanies.Count
        };
        _paginator.Reset();
        ApplyPagination();
    }
    //Search by company name
    private void SearchTB_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!IsInitialized || sender is not TextBox textBox)
            return;

        ApplySearch(textBox.Text);
    }
    
    //Companies buttons click
    public void DeleteCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button {DataContext: Company company})
            return;
        using var dataWorker = new DataWorker();
        dataWorker.DeleteCompany(company.CompanyCode);
        ResetSearchFilter();
        LoadAllCompanies();
        ApplyPagination();
    }
    public void BlockBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button {DataContext: Company company})
            return;
        using var dataWorker = new DataWorker();
        dataWorker.UpdateCompanyStatus(company.CompanyCode, "Заблокировано");
        LoadAllCompanies();
        ApplyPagination();
    }
    public void ActivateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button {DataContext: Company company})
            return;
        using var dataWorker = new DataWorker();
        dataWorker.UpdateCompanyStatus(company.CompanyCode, "Активно");
        LoadAllCompanies();
        ApplyPagination();
    }

    //Next and back page
    private void NextPageBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (!_paginator.CanMoveNext(_filteredCompanies.Count))
            return;
        _paginator.MoveNext();
        ApplyPagination();
    }
    private void BackPageBtn_Click(object? sender, RoutedEventArgs e)
    {
        _paginator.MovePrevious();
        ApplyPagination();
    }

}