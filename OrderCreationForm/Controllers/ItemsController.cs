using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Diagnostics;
using PagedList.Mvc;
using PagedList;
using System.Net;
using System.Data;
using System.Data.Entity;
using OrderCreationForm.Models;
using OrderCreationForm.ViewModels;
using OrderCreationForm.Repository;
using System.Threading.Tasks;

namespace OrderCreationForm.Controllers
{
    public class ItemsController : Controller
    {
        private IProductRepo _repository;
        private RASEntities db = new RASEntities();

        public ItemsController() : this(new ProductRepo())
        {
        }

        public ItemsController(IProductRepo repository)
        {
            _repository = repository;
        }

        // GET: Items
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewOrder()
        {
            ItemViewModel model = new ItemViewModel();
            //IEnumerable<ItemViewModel> model = new List<ItemViewModel>();

            //model.AvailableOrderStatuses.Add(new SelectListItem
            //{ Text = "-Please select-", Value = "Selects items" });
            //var orderStatuses = _repository.GetAllOrderStatuses();
            //foreach (var orderStatus in orderStatuses)
            //{
            //    model.AvailableOrderStatuses.Add(new SelectListItem()
            //    {
            //        Text = orderStatus.order_status_desc,
            //        Value = orderStatus.order_status_id.ToString()
            //    });
            //}

            model.AvailableSupplierTypes.Add(new SelectListItem
            { Text = "-Please select-", Value = "Selects items" });
            var supplierTypes = _repository.GetAllSupplierTypes();
            foreach (var supplierType in supplierTypes)
            {
                model.AvailableSupplierTypes.Add(new SelectListItem()
                {
                    Text = supplierType.SupplierTypeDesc,
                    Value = supplierType.SupplierTypeId.ToString()
                });
            }

            //model.AvailableProductCategories.Add(new SelectListItem
            //{ Text = "-Please select-", Value = "Selects items" });
            //var productCategories = _repository.GetAllProductCategories();
            //foreach (var productCategory in productCategories)
            //{
            //    model.AvailableProductCategories.Add(new SelectListItem()
            //    {
            //        Text = productCategory.ProductCatDesc,
            //        Value = productCategory.ProductCatId.ToString()
            //    });
            //}

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        // GET: Items
        public ActionResult SearchProducts(string supplierId, string searchString, int? page)
        {
            ViewBag.SupplierId = supplierId;
            int pageSize = 4;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            if (String.IsNullOrEmpty(supplierId))
            {
                throw new ArgumentNullException("supplierId");
            }
            int id = 0;
            bool isValid = Int32.TryParse(supplierId, out id);

            IPagedList<Product> products = (_repository.GetAllProductBySupplierId(id)).ToPagedList(pageIndex, pageSize);

            if (!String.IsNullOrEmpty(searchString))
            {
                IPagedList<Product> filteredList = (_repository.GetAllProductsBySupplierIdAndSearchString(id, searchString)).ToPagedList(pageIndex, pageSize);
                return PartialView("_ProductDetails", filteredList);
            }

            return PartialView("_ProductDetails", products);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetProductsBySupplierId(string supplierId, int? page)
        {
            ViewBag.SupplierId = supplierId;
            int pageSize = 4;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            if (String.IsNullOrEmpty(supplierId))
            {
                throw new ArgumentNullException("supplierId");
            }
            int id = 0;
            bool isValid = Int32.TryParse(supplierId, out id);


            IPagedList<Product> products = (_repository.GetAllProductBySupplierId(id)).ToPagedList(pageIndex, pageSize);

            //var products = _repository.GetAllProductBySupplierId(id);
            return PartialView("_ProductDetails", products);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSuppliersBySupplierTypeId(string supplier_type_id)
        {
            if (String.IsNullOrEmpty(supplier_type_id))
            {
                throw new ArgumentNullException("supplier_type_id");
            }
            int id = 0;
            bool isValid = Int32.TryParse(supplier_type_id, out id);
            var suppliers = _repository.GetAllSuppliersBySupplierTypeId(id);
            var result = (from s in suppliers
                          select new
                          {
                              id = s.SupplierId,
                              name = s.SupplierName
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> GetSupplierInfoByProductId(string ProductId)
        {
            if (String.IsNullOrEmpty(ProductId))
            {
                throw new ArgumentNullException("ProductId");
            }
            int id = 0;
            bool isValid = Int32.TryParse(ProductId, out id);

            int supplierId = Convert.ToInt32(db.Products.Where(p => p.ProductId.Equals(id)).First().SupplierId);
            var supplier = db.Suppliers.Where(s => s.SupplierId.Equals(supplierId)).Include(s => s.SupplierType);

            return PartialView("_Supplier",await supplier.ToListAsync());
        }
    }
}