using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VendingMachine.Models
{
    public class Sale
    {
        public int ID_Sale {get;set;}
        public int SaledProductCount { get; set; }
        public float TotalSum { get; set; }
        public DateTime Sale_DateTime {get;set;}

        public Sale(int id, int count, float totalSum, DateTime dateTime)
        {
            ID_Sale = id;
            SaledProductCount = count;
            TotalSum = totalSum;
            Sale_DateTime = dateTime;
        }
    }
}