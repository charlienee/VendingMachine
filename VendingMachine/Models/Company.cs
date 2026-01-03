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
        public string UserName {get;set;}
        public string UserSurname {get;set;}
        public string UserMiddlename {get;set;}
        public string Phone {get;set;}
        public string Email {get;set;}
        public string Status {get;set;}
        public Company(int companyCode, string name, string address, string website, int userId, string userName, string userSurname, string userMiddlename, string phone, string email, string status)
        {
            CompanyCode = companyCode;
            CompanyName = name;
            Address = address;
            Website = website;
            UserId = userId;
            UserName = userName;
            UserSurname = userSurname;
            UserMiddlename = userMiddlename;
            Phone = phone;
            Email = email;
            Status = status;
        }
    }
}