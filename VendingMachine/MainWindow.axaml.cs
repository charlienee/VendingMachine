using System;
using Avalonia.Controls;


namespace VendingMachine;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent.Content = new AuthendeficationPage(this);
    }
    public void Navigate(UserControl page)
    {
        MainContent.Content = page;
    }
}