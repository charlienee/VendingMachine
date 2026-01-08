using System;
namespace VendingMachine.Models
{
    public class VM
    {
        public int ID_VM { get; set; }
        public string Address { get; set; }
        public string PlaceDescription { get; set; }
        public string FullAddress {get;set;}
        public string Model { get; set; }
        public string Brand { get; set; }
        public string FullModel {get;set;}
        public string Status { get; set; }
        public string Name { get; set; }
        public int CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Company { get; set; }
        public int Modem { get; set; }
        
        public DateOnly InstallationDate{get;set;}
        public VM(int id, string address, string placeDescription, string model, string brand, string status, string name, int companyCode,string companyName, int modem, DateOnly installationDate)
        {
            ID_VM = id;
            Address = address;
            PlaceDescription = placeDescription;
            Model = model;
            Brand = brand;
            Status = status;
            Name = name;
            CompanyCode = companyCode;
            CompanyName = companyName;
            Modem = modem;
            InstallationDate = installationDate;

            FullAddress = $"{Address} {PlaceDescription}";
            Company = $"{CompanyCode} - {CompanyName}";
            FullModel = $"{Brand} {Model}";
        }
    }
    public class Model
    {
        public int Id {get; set;}
        public string Brand {get; set;}
        public string Model_name {get; set;}
        public Model(int id, string brand, string model)
        {
            Id = id; 
            Brand = brand;
            Model_name = model;
        }
    }
    public class Critical_Threshold_Rule
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public Critical_Threshold_Rule(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class Product_Matrix
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public Product_Matrix(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public class Notification_Template
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public Notification_Template(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}