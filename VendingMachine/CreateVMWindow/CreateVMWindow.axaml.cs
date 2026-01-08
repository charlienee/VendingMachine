using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using VendingMachine.Models;

namespace VendingMachine;

public partial class CreateVMWindow : Window
{
    private List<User> _clients = new();
    private List<User> _managers = new();
    private List<User> _engineers = new();
    private List<User> _operators = new();
    private List<Model> _models = new();
    private List<Critical_Threshold_Rule> _rules = new();
    private List<Product_Matrix> _matrices = new();
    private List<Notification_Template> _templates = new();
    public CreateVMWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadClient();
        LoadManager();
        LoadOperator();
        LoadEngineer();
        LoadTimezone();
        LoadModel();
        LoadRules();
        LoadMatrices();
        LoadTemplates();
    }
    private void LoadClient()
    {
        using var dataWorker = new DataWorker();
        _clients = dataWorker.GetAllUser().Where(u => u.Role == "Клиент-франчайзи").ToList();
        client_cb.ItemsSource = _clients;
    }
    private void LoadManager()
    {
        using var dataWorker = new DataWorker();
        _managers = dataWorker.GetAllUser().Where(u => u.Role == "Менеджер").ToList();
        manager_cb.ItemsSource = _managers;
    }
    private void LoadOperator()
    {
        using var dataWorker = new DataWorker();
        _operators = dataWorker.GetAllUser().Where(u => u.Role == "Оператор-техник").ToList();
        operator_cb.ItemsSource = _operators;
    }
    private void LoadEngineer()
    {
        using var dataWorker = new DataWorker();
        _engineers = dataWorker.GetAllUser().Where(u => u.Role == "Инженер").ToList();
        engineer_tb.ItemsSource = _engineers;
    }
    private void LoadTimezone()
    {
        var timezones = Enumerable.Range(-12, 27).Select(tz => tz == 0 ? "UTC" : $"UTC {(tz > 0 ? "+" : "")}{tz}");
        timezone_tb.ItemsSource = timezones;
    }
    private void LoadModel()
    {
        using var dataWorker = new DataWorker();
        _models = dataWorker.GetAllModel();
        model_cb.ItemsSource = _models;
        Brand_cb.ItemsSource = _models.GroupBy(b => b.Brand).Select(b => b.First()).ToList();
    }
    private void LoadRules()
    {
        using var dataWorker = new DataWorker();
        _rules = dataWorker.GetAllRule();
        criticalthereshold_cb.ItemsSource = _rules;
    }
    private void LoadMatrices()
    {
        using var dataWorker = new DataWorker();
        _matrices = dataWorker.GetAllProductMatrix();
        productmatrix_cb.ItemsSource = _matrices;
    }
    private void LoadTemplates()
    {
        using var dataWorker = new DataWorker();
        _templates = dataWorker.GetAllNotificationTemplate();
        notificationTM_tb.ItemsSource = _templates;
    }
}