using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using VendingMachine.Models;

namespace VendingMachine;

public partial class MainContentPage : UserControl
{
    private MainWindow mainWindow;
    private UserControl _homePage;
    private UserControl _companyPage;
    private bool _isMenuOpen;
    public MainContentPage(MainWindow mainWindow, User currentUser)
    {
        this.mainWindow = mainWindow;
        InitializeComponent();
        _homePage = new HomePage();
        _companyPage = new CompanyPage(currentUser, mainWindow);
        _isMenuOpen = true;
        PageContent.Content = _homePage;
        ShowUserPicture(currentUser);
        ShowUserInfo(currentUser);
    }
    private void ExitMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        mainWindow.Navigate(new AuthendeficationPage(mainWindow));
    }
    private void MenuTree_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sidebar_tv.SelectedItem is not TreeViewItem item)
        {
            return;
        }
        switch (item.Tag?.ToString())
        {
            case "Home":
                PageContent.Content = new HomePage();
                break;
            case "VM":
                PageContent.Content = new VmPage(mainWindow);
                break;
            case "Company":
                PageContent.Content = _companyPage;
                break;
            case "MonitorVM":
                PageContent.Content = new MonitorVMPage();
                break;
        }
        sidebar_tv.SelectedItem = null;
    }
    public void ShowUserPicture(User currentUser)
    {
        if (currentUser.Picture == null || currentUser.Picture.Length == 0)
        {
            return;
        }
        using var ms = new MemoryStream(currentUser.Picture);
        userPicture_img.Source = new Bitmap(ms);
    }
    public void ShowUserInfo(User currentUser)
    {
        userName_tb.Text = $"{currentUser.Surname} {currentUser.Name[0]}. {currentUser.Middlename[0]}.";
        userRole_tb.Text = $"{currentUser.Role}";
    }
    public void menuButton_click(object? sender, RoutedEventArgs e)
    {
        _isMenuOpen = !_isMenuOpen;
        sidebar_tv.IsVisible = _isMenuOpen;
        if (!sidebar_tv.IsVisible)
        {
            general_grid.ColumnDefinitions[0].Width = new GridLength(0);
        }
        else
        {
            general_grid.ColumnDefinitions[0].Width = GridLength.Star;
        }
    }
}