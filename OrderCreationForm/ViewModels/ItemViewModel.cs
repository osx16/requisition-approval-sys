using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderCreationForm.Models;

namespace OrderCreationForm.ViewModels
{
    public class ItemViewModel
    {
        public ItemViewModel()
        {
            AvailableSupplierTypes = new List<SelectListItem>();
            AvailableSuppliers = new List<SelectListItem>();
            AvailableProductCategories = new List<SelectListItem>();
            AvailableProducts = new List<SelectListItem>();
            //AvailableOrderStatuses = new List<SelectListItem>();
            ProductsList = new List<Product>();
            Products = new List<Product>();
        }

        //Product Attributes
        [Display(Name = "Product ID ")]
        public int? ProductId { get; set; }
        [Display(Name = "Product Description: ")]
        public string productDescription { get; set; }
        [Display(Name = "Product Price: ")]
        public decimal? ProductPrice { get; set; }
        public IList<SelectListItem> AvailableProducts { get; set; }
        public List<Product> ProductsList;

        //Product Category Attributes
        [Display(Name = "Product Category Id: ")]
        public int? ProdCatId { get; set; }
        [Display(Name = "Product Category: ")]
        public string productCategory { get; set; }
        public IList<SelectListItem> AvailableProductCategories { get; set; }

        //Suppier Type Attributes
        [Display(Name = "Supplier Type: ")]
        public int? supplier_type_id { get; set; }
        public IList<SelectListItem> AvailableSupplierTypes { get; set; }

        //Supplier Attributes
        [Display(Name = "Supplier: ")]
        public int? supplier_id { get; set; }
        [Display(Name = "Supplier Name: ")]
        public string SupplierName { get; set; }
        public IList<SelectListItem> AvailableSuppliers { get; set; }

        //Approval Attributes
        [Display(Name = "Approval Status: ")]
        public int? ApprovalStatusId { get; set; }
        [Display(Name = "Approver: ")]
        public int ApproverId { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
        //-------------------------------------------------------

        //Cart Atttributes
        [Display(Name = "Cart No.: ")]
        public string CartId { get; set; }
        [Display(Name = "Product Info: ")]
        public string ProductInfo { get; set; }
        [Display(Name = "Category.: ")]
        public string Category { get; set; }
        [Display(Name = "Supplier: ")]
        public string Supplier { get; set; }
        [Display(Name = "Quantity: ")]
        public int? quantity { get; set; }
        [Display(Name = "Unit Price: ")]
        public decimal? UnitPrice { get; set; }
        [Display(Name = "Sub Total: ")]
        public decimal? SubTotal { get; set; }
        [Display(Name = "Quotation: ")]
        public HttpPostedFileBase File { get; set; }

        //Requisition Attributes
        [Display(Name = "Requisition No.: ")]
        public int? RequisitionNum { get; set; }
        [Display(Name = "Employee: ")]
        public int? EmployeeId { get; set; }
        [Display(Name = "Order Status: ")]
        public int? OrderStatusId { get; set; }
        [Display(Name = "Order Max Total: ")]
        public int? OrderMaxTotal { get; set; }
        //public IList<SelectListItem> AvailableOrderStatuses { get; set; }
        //Cart Class List
       public List<Cart> CartDetails { get; set; }

        public List<CartBindingModel> CartElements { get; set; }

        //Requisition Class List
        public List<Requisition> RequisitionDetails { get; set; }

        public int ProdId { get; set; }
        public string ProdDesc { get; set; }
        public string ProdCatDesc { get; set; }
        public decimal? ProdPrice { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ProductCategory ProductCategories { get; set; }
        public virtual Supplier Suppliers { get; set; }
        public virtual IEnumerable<Product> Products { get; set; }
    }
}