using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VendingMachine.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Input;

namespace VendingMachine;

public partial class CreateCompanyWindow : Window
{
    public event Action? CompanyCreated;
    private int codeCompany;
    public CreateCompanyWindow()
    {
        InitializeComponent();
        GenerateCode();
    }
    private void GenerateCode()
    {
        using (var dataWorker = new DataWorker())
        {
            codeCompany = dataWorker.GeneratedUniqueCompanyCode();
            Code_tb.Text = Convert.ToString(codeCompany);
        }
    }
    private void infoMessage(string text)
    {
        info_tb.IsVisible = true;
        info_tb.Text = text;
    }
    private void RegeneratedCodeBtn_Click(object? sender, RoutedEventArgs e)
    {
        GenerateCode();
    }
    private void ResponsiblePersonPhoneTb_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;
        var text = textBox.Text ?? string.Empty;
        textBox.Text = FormatPhone(text);
        textBox.CaretIndex = FormatPhone(text).Length;
    }
    private string FormatPhone(string input)
    {
        
        input = new string(input.Where(char.IsDigit).ToArray());
        if (input.Length == 0)
        {
            return string.Empty;
        }
        if (input.StartsWith("8"))
        {
            input = "7" + input[1..];
        }
        if (!input.StartsWith("7"))
        {
            input = "7" + input;
        }
        return input.Length switch
        {
            <= 1 => "+7",
            <= 4 => $"+7 ({input.Substring(1)}",
            <= 7 => $"+7 ({input.Substring(1, 3)}) {input.Substring(4)}",
            <= 9 => $"+7 ({input.Substring(1, 3)}) {input.Substring(4, 3)}-{input.Substring(7)}",
            <= 10 => $"+7 ({input.Substring(1, 3)}) {input.Substring(4, 3)}-{input.Substring(7, 2)}-{input.Substring(9)}",
            _ => $"+7 ({input.Substring(1, 3)}) {input.Substring(4, 3)}-{input.Substring(7, 2)}-{input.Substring(9, 2)}"
        };
    }
    private void CreateCompanyBtn_Click(object? sender, RoutedEventArgs e)
    {
        using (var dataWorker = new DataWorker())
        {
            dataWorker.CreateCompany(codeCompany, CompanyName_tb.Text, Address_tb.Text, Website_tb.Text, Status_tb.Text, 
            ResponsiblePersonName_tb.Text, ResponsiblePersonSurname_tb.Text, ResponsiblePersonMiddlename_tb.Text, ResponsiblePersonEmail_tb.Text, ResponsiblePersonPhone_tb.Text);
        }
        CompanyCreated?.Invoke();
        Close();
    }
}