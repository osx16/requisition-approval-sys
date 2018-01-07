using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCreationForm.Models
{
    public class CartBindingModel
    {
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        //public string CartType { get; set; }
    }
}