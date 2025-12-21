using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using VendingMachine.Models;
using Avalonia.Threading;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Linq;

namespace VendingMachine;

public partial class HomePage : UserControl
{
    private DispatcherTimer _timer;
    public HomePage()
    {
        InitializeComponent();

        SetValueEff();
        SetValuesSaleTotalSum();
        SetValuesSaleCount();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _timer.Tick += (_, _) =>
        {
            
            SetValuesSaleTotalSum();
            SetValueEff();
        };
        _timer.Start();
    }
    private void SetValueEff()
    {
        int workingVM = 0;
        int nWorkingVM = 0;
        int VMInService = 0;
        using (var dataWorker = new DataWorker())
        {
            List<VM> vMs = dataWorker.GetAllVM();
            foreach (var v in vMs)
            {
                if (v.Status.Trim() == "Рабочий")
                {
                    ++workingVM;
                    
                }
                else if (Regex.Replace(v.Status, @"\s", "") == "Нерабочий")
                {
                    ++nWorkingVM;
                    
                }
                else if (v.Status.Trim() == "На обслуживании")
                {
                    ++VMInService;
                }
            }
            workingVM_Gauge.GaugeValue = (double)workingVM / (double)vMs.Count * 100;
            nWorkingVM_Gauge.GaugeValue = (double)nWorkingVM / (double)vMs.Count * 100;
            serviceVM_Gauge.GaugeValue = (double)VMInService / (double)vMs.Count * 100;
            eff_needle.Value = (double)workingVM / (double)vMs.Count * 100;
            eff_tb.Text = $"Работающих автоматов - {(int)((double)workingVM / (double)vMs.Count * 100)}%";
        }
    }
        
    public void SetValuesSaleTotalSum()
    {
        using (var dataWorker = new DataWorker())
        {
            List<Sale> sales = dataWorker.GetAllSales();
            Console.WriteLine(sales.Count);
            List<Sale> lastSale = new List<Sale>();
            DateTimePoint[] Values = Enumerable.Range(0, 10)
            .Select(i => DateTime.Today.AddDays(-9 + i))
            .Select(d => new DateTimePoint(d, sales.Where(s => s.Sale_DateTime.Date == d)
            .Sum(s => s.TotalSum)
            )).ToArray();
            SalesTotalSum_XCL.Values = Values;
            SalesTotalSum_XCL.MaxBarWidth = double.MaxValue;
            Func<DateTime, string> Formatter = date => date.ToString("dd.MM");
            SalesTotalSum_XDTA.DateFormatter = Formatter;
        }
    }
    public void SetValuesSaleCount()
    {
        using (var dataWorker = new DataWorker())
        {
            var sales = dataWorker.GetAllSales();
            DateTimePoint[] Values = Enumerable.Range(0, 10)
            .Select(i => DateTime.Today.AddDays(-9 + i))
            .Select(d => new DateTimePoint(
                d, sales.Where(s => s.Sale_DateTime == d)
                .Sum(s => s.SaledProductCount)
            )).ToArray();
            SalesCount_XCL.Values = Values;
            SalesCount_XCL.MaxBarWidth = double.MaxValue;
            Func<DateTime, string> Formatter = date => date.ToString("dd.MM");
            SalesCount_XDTA.DateFormatter = Formatter;
        }
    }
}