using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Diagnostics;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VendingMachine.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VendingMachine;

public partial class AuthendeficationPage : UserControl
{
    public MainWindow mainWindow;
    public AuthendeficationPage(MainWindow mainWindow)
    {
        InitializeComponent();
        this.mainWindow = mainWindow;
    }

    public void infoMessage(string infoText)
    {
        info_tb.IsVisible = true;
        info_tb.Text = infoText;
    }
    private void registrationBtn_Click(object? sender, RoutedEventArgs e)
    {
        var registrationWindow = new RegistrationClientWindow(mainWindow);
        registrationWindow.ShowDialog(mainWindow);
    }
    private void enterButton_Click (object? sender, RoutedEventArgs e)
    {
        using (DataWorker dataWorker = new DataWorker())
        {
            List<User> users = dataWorker.GetAllUser();
            User? currentUser;
            if (login_tb.Text == null || login_tb.Text.Trim() == "" || password_tb.Text == null || password_tb.Text.Trim() == "")
            {
                infoMessage("Заполните поля логин и пароль");
            }
            else if (login_tb.Text.Contains("@"))
            {
                if (users.Exists(u => u.Email == login_tb.Text.Trim()))
                {
                    currentUser = users.FirstOrDefault(u => u.Email == login_tb.Text.Trim());
                    if(currentUser?.Password == password_tb.Text.Trim())
                    {
                        mainWindow.Navigate(new MainContentPage(this.mainWindow, currentUser));
                    }
                    else
                    {
                        infoMessage("Неверный пароль");
                    }
                }
                else
                {
                    infoMessage("Пользователь не найден");
                }
            }
            else if (login_tb.Text[0] == '+')
            {
                if (users.Exists(u => u.Phone == Regex.Replace(login_tb.Text, @"\s", "")))
                {
                    currentUser = users.FirstOrDefault(u => u.Phone == Regex.Replace(login_tb.Text, @"\s", ""));
                    if(currentUser?.Password == password_tb.Text.Trim())
                    {
                        mainWindow.Navigate(new MainContentPage(this.mainWindow, currentUser));
                    }
                    else
                    {
                        infoMessage("Неверный пароль");
                    }
                }
                else
                {
                    infoMessage("Пользователь не найден");
                }
            }
            else
            {
                infoMessage("Некорректный логин");
            }
        }
    }
}