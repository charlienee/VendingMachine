using System;
using System.Data.Common;

namespace VendingMachine.Models
{
    public class User
    {
        public int ID_User{get;set;}
        public string Name {get;set;}
        public string Surname {get;set;}
        public string Middlename {get;set;}
        public string Email {get;set;}
        public string Phone {get;set;}
        public string Role {get;set;}
        public string Password {get;set;}
        public byte[]? Picture {get;set;}

        public User (int id, string name, string surname, string middlename, string email, string phone, string role, string password, byte[] picture)
        {
            ID_User = id;
            Name = name;
            Surname = surname;
            Middlename = middlename;
            Email = email;
            Phone = phone;
            Role = role;
            Password = password;
            Picture = picture;
        }
    }
}