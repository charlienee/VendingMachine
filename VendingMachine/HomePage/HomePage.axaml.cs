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
        SetValueNetwork();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(10)
        };
        _timer.Tick += (_, _) =>
        {
            SetValuesSaleCount();
            SetValuesSaleTotalSum();
            SetValueEff();
            SetValueNetwork();
        };
        _timer.Start();
    }
    private void SetValueEff()
    {
        int workingVM = 0;
        int othersVM;
        using (var dataWorker = new DataWorker())
        {
            List<VM> vMs = dataWorker.GetAllVM();
            foreach (var v in vMs)
            {
                if (v.Status.Trim() == "Рабочий")
                    ++workingVM;

            }
            workingVM_Gauge.GaugeValue = (double)workingVM / (double)vMs.Count * 100;
            eff_needle.Value = (double)workingVM / (double)vMs.Count * 100;
            eff_tb.Text = $"Работающих автоматов - {(int)((double)workingVM / (double)vMs.Count * 100)}%";
        }
    }
    private void SetValueNetwork()
    {
        int workingVM = 0;
        int nworkingVM = 0;
        int serviceVM = 0;
        using (var dataWorker = new DataWorker())
        {
            List<VM> vMs = dataWorker.GetAllVM();
            foreach (var v in vMs)
            {
                if (v.Status.Trim() == "Рабочий")
                    ++workingVM;
                else if (v.Status.Trim() == "Не рабочий")
                    ++nworkingVM;
                else if (v.Status.Trim() == "На обслуживании")
                    ++serviceVM;
            }
            workingVMNetwork_Gauge.GaugeValue = (double)workingVM / (double)vMs.Count * 100;
            nworkingVMNetwork_Gauge.GaugeValue = (double)nworkingVM / (double)vMs.Count * 100;
            serviceVMNetwork_Gauge.GaugeValue = (double)serviceVM / (double)vMs.Count * 100;
        }
    }
    public void SetValuesSaleTotalSum()
    {
        using (var dataWorker = new DataWorker())
        {
            List<Sale> sales = dataWorker.GetAllSales();
            
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
                d, sales.Where(s => s.Sale_DateTime.Date == d)
                .Sum(s => s.SaledProductCount)
            )).ToArray();
            SalesCount_XCL.Values = Values;
            SalesCount_XCL.MaxBarWidth = double.MaxValue;
            Func<DateTime, string> Formatter = date => date.ToString("dd.MM");
            SalesCount_XDTA.DateFormatter = Formatter;
        }
    }
}