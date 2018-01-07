using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCreationForm.ViewModels
{
    public class CartViewModel
    {
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public string EmployeeId { get; set; }
        public string CartType { get; set; }
        public int GroupNum { get; set; }
    }
}