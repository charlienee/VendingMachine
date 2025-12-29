using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VendingMachine.Models
{
    public class Company
    {
        public int CompanyCode {get;set;}
        public string CompanyName {get;set;}
        public string Address {get; set;}
        public string Website {get; set;}
        public int UserId {get;set;}
        public string Status {get;set;}
        public Company(int companyCode, string name, string address, string website, int userId, string status)
        {
            CompanyCode = companyCode;
            CompanyName = name;
            Address = address;
            Website = website;
            UserId = userId;
            Status = status;
        }
    }
}