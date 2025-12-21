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
    private UserControl _homePage;
    private bool _isMenuOpen;
    public MainContentPage(MainWindow mainWindow, User currentUser)
    {
        InitializeComponent();
        _homePage = new HomePage();
        _isMenuOpen = true;
        PageContent.Content = _homePage;
        ShowUserPicture(currentUser);
        ShowUserInfo(currentUser);
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