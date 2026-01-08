using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VendingMachine.Models;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Encodings.Web;
using Avalonia.Controls.Primitives;
using Avalonia.Collections;
using Avalonia.Data;

namespace VendingMachine;

public partial class VmPage : UserControl
{
    private List<VM> _allVMs;
    private List<VM> _filteredVMs;
    private readonly Paginator<VM> _paginator = new(5);
    private readonly TextFilter<VM> _filter = new (vm => new [] {vm.Name});
    private MainWindow _mainWindow;
    public enum ViewMode {Table, Tile}
    private ViewMode _currentViewMode = ViewMode.Table;
    private ToggleButton? _activeToggle;
    public VmPage()
    {
        InitializeComponent();
        LoadAllCompanies();
        DataContext = this;
        _activeToggle = tableBtn;
    }
    public VmPage(MainWindow mainWindow) : this()
    {
        _mainWindow = mainWindow;
        DataContext = this;
        ApplyPagination();
    }
    private void ViewToggle_Checked(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton toggle)
            return;
        if (_activeToggle == toggle)
            return;
        if (_activeToggle != null)
            _activeToggle.IsChecked = false;
        _activeToggle = toggle;
        if (toggle == tableBtn)
            SetViewMode(ViewMode.Table);
        else if (toggle == tileBtn)
            SetViewMode(ViewMode.Tile);
    }
    private void ViewModeToggle_Unchecked (object? sender, RoutedEventArgs e)
    {
        if (sender == _activeToggle)
            ((ToggleButton)sender!).IsChecked = true;
    }
    private void SetViewMode(ViewMode mode)
    {
        if (_currentViewMode == mode)
            return;
        _currentViewMode = mode;
        switch (mode)
        {
            case ViewMode.Table:
                VMs_dp.IsVisible = false;
                VMs_dg.IsVisible = true;
                tileBtn.IsChecked = false;
                break;
            case ViewMode.Tile:
                VMs_dp.IsVisible = true;
                VMs_dg.IsVisible = false;
                tableBtn.IsChecked = false;
                break;
        }
    }
    private void LoadAllCompanies()
    {
        using var dataWorker = new DataWorker();
        _allVMs = dataWorker.GetAllVM();
        _filteredVMs = _allVMs;
    }
    private void ApplyPagination()
    {
        VMs_dg.ItemsSource = new ObservableCollection<VM>(_paginator.Apply(_filteredVMs));
        vMs_ug.Bind(ItemsControl.ItemsSourceProperty, new Binding {Source = _paginator.Apply(_filteredVMs)});
        PageTb.Text = _paginator.CurrentPage.ToString();
        CountTb.Text = $"Записи с {Math.Min(_paginator.PageSize * (_paginator.CurrentPage - 1) + 1, _filteredVMs.Count)} по {Math.Min(_paginator.PageSize * _paginator.CurrentPage, _filteredVMs.Count)} из {_filteredVMs.Count}";
    }
    private void ApplyVMSearch(string text)
    {
        _filteredVMs = _filter.Apply(_allVMs, text).ToList();
        _paginator.Reset();
        ApplyPagination();
    }
    private void ResetSearchFilter()
    {
        SearchTB.Text = string.Empty;
    }
    private void SearchTB_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!IsInitialized || sender is not TextBox textBox)
            return;
        ApplyVMSearch(textBox.Text);
    }
    private void NextPageBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (!_paginator.CanMoveNext(_filteredVMs.Count))
            return;
        _paginator.MoveNext();
        ApplyPagination();
    }
    private void BackPageBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (_paginator.CurrentPage <= 0)
            return;
        _paginator.MovePrevious();
        ApplyPagination();
    }
    private void PageSizeCB_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized || sender is not ComboBox comboBox)
            return;
        _paginator.PageSize = comboBox.SelectedIndex
        switch
        {
            0 => 5,
            1 => 10,
            2 => 15,
            _ => _filteredVMs.Count
        };
        _paginator.Reset();
        ApplyPagination();
    }
    public async Task<string> SelectFolder(Window parentWindow)
    {
        string folderPath = AppContext.BaseDirectory;
        var topLevel = TopLevel.GetTopLevel(parentWindow);
        if (topLevel == null)
        {
            return folderPath;
        }
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Выберите папку",
                AllowMultiple = false
            }
        );
        if (folders.Count > 0)
        {
            folderPath = folders[0].Path.LocalPath;
        }
        return folderPath;
    }
    public void ImportJSON(string folderPath, IEnumerable<VM> vMs)
    {
        var fullPath = Path.Combine(folderPath, $"VMReport.json");
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var json = JsonSerializer.Serialize(vMs.Select(m => 
        new
        {
            m.ID_VM,
            m.Name,
            m.Brand,
            m.Model,
            m.CompanyCode,
            m.CompanyName,
            m.Modem,
            m.Address,
            m.PlaceDescription,
            m.InstallationDate
        }), options);
        File.WriteAllText(fullPath, json);
    }
    private async void ReportCSVBtn_Click(object? sender, RoutedEventArgs e)
    {
        var folderPath = await SelectFolder(_mainWindow);
        if (string.IsNullOrEmpty(folderPath))
            return;
        ImportJSON(folderPath, _allVMs);
    }
    private void DeleteBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button {DataContext: VM vm})
            return;
        using var dataWorker = new DataWorker();
        dataWorker.DeleteVM(vm.ID_VM);
        ResetSearchFilter();
        LoadAllCompanies();
        ApplyPagination();
    }
    private void EditBtn_Click (object? sender, RoutedEventArgs e)
    {
        
    }
    private void ModemBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button {DataContext: VM vm})
            return;
        using var dataWorker = new DataWorker();
        dataWorker.UpdateModem(vm.ID_VM);
        LoadAllCompanies();
        ApplyPagination();
    }
    private void CreateVMBtn_Click(object? sender, RoutedEventArgs e)
    {
        var window = new CreateVMWindow();
        window.ShowDialog(_mainWindow);
    }
}