using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderCreationForm.ViewModels
{
    public class OrderDetailsViewModel
    {

        string CartId { get; set; }
        decimal GrandTotal { get; set; }
        int RequisitionNo { get; set; }
        string EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Select File")]
        public HttpPostedFileBase QuotationFile { get; set; }

    }
}