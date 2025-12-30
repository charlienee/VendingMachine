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
    private bool _isEmailMode;
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
    private void loginTB_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;
        var text = textBox.Text ?? string.Empty;
        bool isEmail = text.Any(char.IsLetter) || text.Contains("@");
        if (isEmail)
        {
            var formatted = FormatEmail(text);
            if (formatted != text)
            {
                textBox.TextChanged -= loginTB_OnTextChanged;
                textBox.Text = formatted;
                textBox.CaretIndex = formatted.Length;
                textBox.TextChanged += loginTB_OnTextChanged;
            }
            return;
        }
        _isEmailMode = false;
        var digits = new string(text.Where(char.IsDigit).ToArray());
        textBox.Text = FormatPhone(digits);
        textBox.CaretIndex = textBox.Text.Length;
    }
    private string FormatEmail(string input)
    {
        input = input.Replace(" ", "");
        int atIndex = input.IndexOf('@');
        if (atIndex < 0)
        {
            return input;
        }
        var beforeAt = input[..atIndex];
        var afterAt = input[(atIndex + 1)..].Replace("@", "");
        int dotIndex = afterAt.IndexOf('.');
        if (dotIndex < 0)
        {
            return $"{beforeAt}@{FilterLetters(afterAt)}";
        }
        var domain = FilterLetters(afterAt[..dotIndex]);
        var ending = FilterLetters(afterAt[(dotIndex + 1)..]);
        return $"{beforeAt}@{domain}.{ending}";
    }
    private string FilterLetters(string value)
    {
        return new string(value.Where(char.IsLetter).ToArray());
    }
    private string FormatPhone (string digits)
    {
        if (digits.Length == 0)
        {
            return string.Empty;
        }
        if (digits.StartsWith("8"))
        {
            digits = "7" + digits[1..];
        }
        if (!digits.StartsWith("7"))
        {
            digits = "7" + digits;
        }
        return digits.Length switch
        {
            <= 1 => "+7",
            <= 4 => $"+7 ({digits.Substring(1)}",
            <= 7 => $"+7 ({digits.Substring(1, 3)}) {digits.Substring(4)}",
            <= 9 => $"+7 ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7)}",
            <= 10 => $"+7 ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7, 2)}-{digits.Substring(9)}",
            _ => $"+7 ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7, 2)}-{digits.Substring(9, 2)}"
        };
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
                        if (!currentUser.IsBlocked)
                        {
                            mainWindow.Navigate(new MainContentPage(this.mainWindow, currentUser));
                        }
                        else
                        {
                            infoMessage("Пользователь заблокирован");
                        }
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
                        if (!currentUser.IsBlocked)
                        {
                            mainWindow.Navigate(new MainContentPage(this.mainWindow, currentUser));
                        }
                        else
                        {
                            infoMessage("Пользователь заблокирован");
                        }
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