using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using VendingMachine.Models;

namespace VendingMachine;

public partial class EditCompanyWindow : Window
{
    public event Action? CompanyEdited;
    private MainWindow _mainWidow;
    private Company _company;
    public EditCompanyWindow(MainWindow mainWindow, Company company)
    {
        InitializeComponent();
        _mainWidow = mainWindow;
        _company = company;
        CompanyName_tb.Text = company.CompanyName;
        Address_tb.Text = company.Address;
        Website_tb.Text = company.Website;
        Code_tb.Text = company.CompanyCode.ToString();
        ResponsiblePersonName_tb.Text = company.UserName;
        ResponsiblePersonSurname_tb.Text = company.UserSurname;
        ResponsiblePersonMiddlename_tb.Text = company.UserMiddlename;
        ResponsiblePersonPhone_tb.Text = company.Phone;
        ResponsiblePersonEmail_tb.Text = company.Email;
    }
    private void infoMessage(string text)
    {
        info_tb.Text = text;
        info_tb.IsVisible = true;
    }
    private void SaveChange(object? sender, RoutedEventArgs e)
    {
        using var dataWorker = new DataWorker();
        var name = CompanyName_tb.Text;
        var address = Address_tb.Text;
        var website = Website_tb.Text;
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(website))
        {
            infoMessage("Заполните все поля");
            return;
        }
        dataWorker.UpdateCompanyInfo(Convert.ToInt32(Code_tb.Text), name, address, website);
        CompanyEdited?.Invoke();
        Close(this);
    }
}