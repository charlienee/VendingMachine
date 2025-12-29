using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VendingMachine.Models;

namespace VendingMachine;

public partial class CreateCompanyWindow : Window
{
    private int codeCompany;
    public CreateCompanyWindow()
    {
        InitializeComponent();
        using (var dataWorker = new DataWorker())
        {
            codeCompany = dataWorker.GeneratedUniqueCompanyCode();
            Code_tb.Text = Convert.ToString(codeCompany);
        }
    }
    private void RegeneratedCodeBtn_Click(object? sender, RoutedEventArgs e)
    {
        using (var dataWorker = new DataWorker())
        {
            codeCompany = dataWorker.GeneratedUniqueCompanyCode();
            Code_tb.Text = Convert.ToString(codeCompany);
        }
    }
    private void CreateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        using (var dataWorker = new DataWorker())
        {
            dataWorker.CreateCompany(codeCompany, CompanyName_tb.Text, Address_tb.Text, Website_tb.Text, Status_tb.Text, 
            ResponsiblePersonName_tb.Text, ResponsiblePersonSurname_tb.Text, ResponsiblePersonMiddlename_tb.Text, ResponsiblePersonEmail_tb.Text, ResponsiblePersonPhone_tb.Text);
        }
    }
}