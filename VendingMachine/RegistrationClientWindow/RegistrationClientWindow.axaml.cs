using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VendingMachine.Models;

namespace VendingMachine;

public partial class RegistrationClientWindow : Window
{
    private MainWindow mainWindow;
    public RegistrationClientWindow(MainWindow mainWindow)
    {
        InitializeComponent();
        this.mainWindow = mainWindow;

    }
    private void infoMessage(string message)
    {
        info_tb.IsVisible = true;
        info_tb.Text = message;
    }
    private void registrationBtn_Click(object? sender, RoutedEventArgs e)
    {
        using (var dataWorker = new DataWorker())
        {
            List<Company> companies = dataWorker.GetAllCompanies();
            List<User> users = dataWorker.GetAllUser();
            int companyCode;
            User? currentUser;
            Company? company;
            if (CompanyCode_tb.Text == null || CompanyCode_tb.Text.Trim() == null || Password_tb.Text == null || Password_tb.Text.Trim() == null)
            {
                infoMessage("Заполните поля логин и пароль");
            }
            if (int.TryParse(CompanyCode_tb.Text.Trim(), out companyCode))
            {
                if (companies.Exists(c => c.CompanyCode == companyCode))
                {
                    company = companies.FirstOrDefault(c => c.CompanyCode == companyCode);
                    currentUser = users.FirstOrDefault(u => u.ID_User == company.UserId);
                    dataWorker.UpdateUserPassword(currentUser.ID_User, Password_tb.Text.Trim());
                    dataWorker.UpdateCompanyStatus(company.CompanyCode, "Привязка пользователя");
                    mainWindow.Navigate(new MainContentPage(this.mainWindow, currentUser));
                    this.Close();
                }
            }
            else
            {
                infoMessage("Неверный формат кода компании. Введите число.");
            }
        }
    }
}