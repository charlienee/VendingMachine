using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachine.Models;

namespace VendingMachine;

public partial class CompanyPage : UserControl
{
    public CompanyPage(User currentUser)
    {
        InitializeComponent();
        if (currentUser.Role != "Менеджер")
        {
            CreateCompany_btn.IsVisible = false;
        }
    }
}