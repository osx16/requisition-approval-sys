using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using OrderCreationForm.Models;
using OrderCreationForm.ViewModels;
using OrderCreationForm.Repository;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Newtonsoft.Json;
using PagedList;
using System.Data.SqlClient;
using System.IO;
using System.Data.SqlTypes;

namespace OrderCreationForm.Controllers
{
    public class CartsController : Controller
    {
        private ICartRepo  _repository;
        private RASEntities db = new RASEntities();

        public CartsController() : this(new CartRepo())
        {
        }

        public CartsController(ICartRepo repository)
        {
            _repository = repository;
        }

        public ActionResult CartList()
        {
            return View();
        }

        public ActionResult GetCartType()
        {
            string employeeId = "79a0e692-d36e-455a-bfd9-32ed77c066b4";
            string cartType = _repository.GetCartTypeByEmployeeId(employeeId);
            int type;
            if(cartType == "Internal")
            {
                type = 1;
            }
            else
            {
                type = 2;
            }
            return Json(type, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllPassiveCartsId()
        {
            string employeeId = "79a0e692-d36e-455a-bfd9-32ed77c066b4";
            List<CartProperty> CartProperties = _repository.GetAllPassiveCartIdsByEmployeeId(employeeId);
            List<string> CartIds = new List<string>();
            foreach (var i in CartProperties)
            {
                CartIds.Add(i.CartId);
            }
            return Json(CartIds, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllCartItems(string cartId, int? page)
        {
            ViewBag.CartId = cartId;
            int pageSize = 4;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<Cart> items = (_repository.GetCartItemsByCartId(cartId)).ToPagedList(pageIndex, pageSize);
            ViewBag.DateCreated = items.First().DateCreated;
            ViewBag.Supplier = items.First().Product.Supplier.SupplierName;
            ViewBag.GroupNo = _repository.GetCartGroupNoByCartId(cartId);
            ViewBag.CartTotal = _repository.GetCartTotalByCartId(cartId);
            ViewBag.OrderDetailsNo = _repository.GetOrderDetailsNoByCartId(cartId);
            return PartialView("_CartSummary", items);
        }

        public async Task<ActionResult> MyCart(int page = 1, int pageSize = 5)
        {        
            var records = new PagedLister<Cart>();
            string employeeId = "79a0e692-d36e-455a-bfd9-32ed77c066b4";
            string cartId = _repository.GetActiveCartIdByEmployeeId(employeeId);
            int cartGroupNo = _repository.GetCartGroupNoByCartId(cartId);
            records.Content = await db.Carts
                              .Where(c => c.CartId.Equals(cartId))
                                     .Include(c => c.Product)
                              .OrderBy(x => x.DateCreated)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize).ToListAsync();

            if (records.Content.Count() > 0)
            {
                ViewBag.CartId = JsonConvert.SerializeObject(new { Text = records.Content[0].CartId });
                ViewBag.GroupNo = cartGroupNo;
                ViewBag.DateCreated =  records.Content[0].DateCreated;
                ViewBag.Supplier = records.Content[0].Product.Supplier.SupplierName;
                ViewBag.CartTotal = _repository.GetCartTotalByCartId(records.Content[0].CartId);
            }
            // Count
            records.TotalRecords = (
                                    db.Carts
                                    .Where
                                        (
                                            x => x.CartId.Equals(cartId)
                                        )
                                    ).Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;

            return View(records);
        }

        // GET: Carts
        public ActionResult Cart()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetCartTotal(string cartId)
        {
            decimal total = _repository.GetCartTotalByCartId(cartId);

            return Json(total, JsonRequestBehavior.AllowGet);
        }

        // GET: Carts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = await db.Carts.FindAsync(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        [HttpPost]
        public async Task<ActionResult> AddToCart(ItemViewModel IVM)
        {
            int status = 0;
            ViewBag.IntLim = 1;
            ViewBag.ExtLim = 3;
            string employeeId = "79a0e692-d36e-455a-bfd9-32ed77c066b4";
            RASEntities db = new RASEntities();
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    var CartContent = IVM.CartElements[0];                
                    bool ExistingActiveCartEntries = _repository.CheckIfUserHasActiveCartItems(employeeId);
                    var SupplierType = _repository.GetSupplierTypeByProductId(CartContent.ProductId);
                    var NewCartId = Convert.ToString(Guid.NewGuid());
                    int Quantity = 1;
                    int CartGrpNo = 1;

                    if (ExistingActiveCartEntries.Equals(true))
                    {
                        var ActiveCartItemEntries = _repository.GetAllActiveCartItemsByEmployeeId(employeeId); //Only one Acitve cart, Either Internal or External
                        Cart LastItemInActiveCart = ActiveCartItemEntries.OrderByDescending(x => x.DateCreated).First();
                        int productId = LastItemInActiveCart.ProductId;
                        bool match = CheckIfSupplierTypeMatches(productId, CartContent.ProductId);
                        if (!match)
                        {
                            status = GetAlertStatus(productId, CartContent.ProductId);
                            //return new JsonResult { Data = new { status = status } };
                        }
                        else
                        {
                            int CartSupplierId = _repository.GetSupplierIdByProductId(productId);
                            int SelectedSupplierId = _repository.GetSupplierIdByProductId(CartContent.ProductId);
                            if (SelectedSupplierId != CartSupplierId)
                            {
                                return new JsonResult { Data = new { status = status } };
                            }
                            else
                            {
                                List<Cart> Collection = new List<Cart>();
                                foreach (var i in ActiveCartItemEntries)
                                {
                                    if (i.ProductId.Equals(CartContent.ProductId))
                                    {
                                        Collection.Add(i);
                                    }
                                }

                                if (Collection.Count() > 0)
                                {
                                    Cart ExistingCartItem = Collection[0];
                                    /**Update Cart Item**/
                                    var data = from i in db.Carts
                                               where i.CartId == ExistingCartItem.CartId &&
                                               i.ProductId == ExistingCartItem.ProductId
                                               select i;
                                    var cartItemTobeUpdated = await data.FirstAsync();
                                    cartItemTobeUpdated.Quantity = (ExistingCartItem.Quantity + 1);
                                    cartItemTobeUpdated.SubTotal = (CartContent.UnitPrice * cartItemTobeUpdated.Quantity);
                                    var result = await db.SaveChangesAsync();
                                    if (result > 0)
                                    {
                                        ViewBag.Message = "Successfully added to cart!";
                                        status = 1;
                                    }
                                }
                                else
                                {
                                    int GrpNo = _repository.GetCartGroupNoByCartId(LastItemInActiveCart.CartId);
                                    string CartType = LastItemInActiveCart.Product.Supplier.SupplierType.SupplierTypeDesc;
                                    status = UpdateCart(LastItemInActiveCart.CartId, CartContent.ProductId,
                                                Quantity, CartContent.UnitPrice, employeeId, CartType, GrpNo);
                                }
                            }
                        }
                    }
                    else
                    {
                        bool ExistingExternalActiveCartEntries = _repository.CheckIfUserHasExternalActiveCartItems(employeeId);
                        bool ExistingInternalActiveCartEntries = _repository.CheckIfUserHasInternalActiveCartItems(employeeId);

                        if (ExistingExternalActiveCartEntries.Equals(true))
                        {
                            int prodId = _repository.GetProductIdForLastItemInExternalCart(employeeId);
                            bool match = CheckIfSupplierTypeMatches(prodId, CartContent.ProductId);
                            if (!match)
                            {
                                status = GetAlertStatus(prodId, CartContent.ProductId);
                                return new JsonResult { Data = new { status = status } };
                            }
                            else
                            {
                                bool limitReached = false;
                                var id = "";
                                bool response = _repository.CheckIfSupplierAlreadySelected(CartContent.ProductId);
                                if (response == true)
                                {
                                    status = 3;
                                }
                                else
                                {
                                    id = _repository.GetCartIdOfLastEntryOfExtActiveCartItem(employeeId);

                                    int GroupNum = _repository.GetCartGroupNoByCartId(id);
                                    if (GroupNum < 3)
                                    {
                                        //Increment Cart Group
                                        CartGrpNo = (GroupNum + 1);
                                    }
                                    else
                                    {
                                        CartGrpNo = GroupNum;
                                        limitReached = true;
                                    }

                                    if (limitReached.Equals(true))
                                    {
                                        bool reply = _repository.CheckIfCartIsComplete(id);
                                        if (reply.Equals(true))
                                        {
                                            status = 2;
                                        }
                                    }
                                    else
                                    {
                                        status = UpdateCart(NewCartId, CartContent.ProductId, Quantity, CartContent.UnitPrice,
                                                 employeeId, SupplierType, CartGrpNo);
                                    }
                                }
                            }
                        }
                        else if (ExistingInternalActiveCartEntries.Equals(true))
                        {
                            int prodId = _repository.GetProductIdForLastItemInInternalCart(employeeId);
                            bool match = CheckIfSupplierTypeMatches(prodId, CartContent.ProductId);
                            if (!match)
                            {
                                status = GetAlertStatus(prodId, CartContent.ProductId);
                                return new JsonResult { Data = new { status = status } };
                            }
                            else
                            {
                                status = 4;
                            }
                        }
                        else
                        {
                            status = UpdateCart(NewCartId, CartContent.ProductId, Quantity, CartContent.UnitPrice,
                            employeeId, SupplierType, CartGrpNo);
                        }
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }
            }
            else
            {
                ViewBag.Message = "Failed to add item!";
                status = 0;
            }
            return new JsonResult { Data = new { status = status} };
        }

        public ActionResult RenderFileUploadView()
        {
            return PartialView("_FileUpload");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewOrderDetails([Bind(Include = "Quote")] FileViewModel File)
        {
            if (ModelState.IsValid)
            {
                string employeeId = "79a0e692-d36e-455a-bfd9-32ed77c066b4";
                string cartId = _repository.GetActiveCartIdByEmployeeId(employeeId);
                Random rnd = new Random();
                var fileData = new MemoryStream();
                File.Quote.InputStream.CopyTo(fileData);

                int requisitionNo;
                int cartGrpNo = _repository.GetCartGroupNoByCartId(cartId);
                if (cartGrpNo > 1)
                {
                    requisitionNo = _repository.GetRequisitionNoByEmployeeIdGrpNo(employeeId, cartGrpNo);
                }
                else
                {
                    requisitionNo = rnd.Next(100000, 9999999);
                }

                var NewEntry = new OrderDetail
                {
                    CartId = cartId,
                    GrandTotal = _repository.GetCartTotalByCartId(cartId),
                    RequisitionNo = requisitionNo,
                    QuotationFile = fileData.ToArray()
                };

                db.OrderDetails.Add(NewEntry);
                try
                {
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        _repository.ChangeCartCompletionStatus(cartId);
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }
                return RedirectToAction("CartList");
            }

            return View();
        }

        // GET: Carts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = await db.Carts.FindAsync(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Cart cart = await db.Carts.FindAsync(id);
            db.Carts.Remove(cart);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public FileStreamResult DownloadFile(string id)
        {
            int OrderDetailsNo = Convert.ToInt32(id);
            string connectionString = @"Data Source=JAS;Initial Catalog=RAS;Integrated Security=True;
                                      MultipleActiveResultSets=True;Application Name=EntityFramework";
            string commandText = @"
                     SELECT QuotationFile.PathName(),
                     GET_FILESTREAM_TRANSACTION_CONTEXT()
                     FROM RAS.dbo.OrderDetails
                     WHERE OrderDetailsNo = @OrderDetailsNo;";

            string serverPath;
            byte[] serverTxn;
            byte[] buffer = new Byte[1024 * 512];

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {

                try
                {
                    sqlConnection.Open();

                    SqlTransaction transaction = sqlConnection.BeginTransaction();
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.Transaction = transaction;
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = commandText;
                    sqlCommand.Parameters.Add("@OrderDetailsNo", SqlDbType.Int).Value = OrderDetailsNo;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        reader.Read();
                        serverPath = reader.GetSqlString(0).Value;
                        serverTxn = reader.GetSqlBinary(1).Value;
                        reader.Close();
                    }

                    using (SqlFileStream sqlFileStream = new SqlFileStream(serverPath, serverTxn, FileAccess.Read))
                    {
                        buffer = new Byte[sqlFileStream.Length];
                        sqlFileStream.Read(buffer, 0, buffer.Length);
                        sqlFileStream.Close();
                    }

                    sqlCommand.Transaction.Commit();

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                }

            }//End of SQL Connection Block

            return File(new MemoryStream(buffer), "application/pdf");
        }//End of DownloadFile Method

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        int UpdateCart(string cartId, int productId, int quantity,decimal unitPrice, 
                                         string employeeId, string cartType,int cartGrpNo)
        {
            int status = 0;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Cart NewItem = new Cart
                    {
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = quantity,
                        SubTotal = unitPrice,
                        DateCreated = DateTime.Now,
                    };

                    db.Carts.Add(NewItem);
                    var result = db.SaveChanges();

                    if (result > 0)
                    {
                        bool CartPropertyExistsForId = _repository.CheckIfCartPropertyExistsForThisId(cartId);
                        if (CartPropertyExistsForId.Equals(false))
                        {

                            CartProperty NewCartProperty = new CartProperty
                            {
                                CartId = cartId,
                                EmployeeId = employeeId,
                                CartType = cartType,
                                GroupNum = cartGrpNo,
                                IsComplete = false,
                                IsActive = true
                            };
                            db.CartProperties.Add(NewCartProperty);
                            db.SaveChanges();
                        }

                        ViewBag.Message = "Successfully added to cart!";
                        transaction.Commit();
                        status = 1;

                    }
                    else
                    {
                        transaction.Rollback();
                    }
                    return status;
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                    transaction.Rollback();
                    return status;
                }
            }//End of Transaction Block
        }//End of UpdateCart()

        bool CheckIfSupplierTypeMatches(int productIdInCart, int productIdSelected)
        {
            string supplierType1 = _repository.GetSupplierTypeByProductId(productIdInCart);
            string supplierType2 = _repository.GetSupplierTypeByProductId(productIdSelected);
            bool match = supplierType1.Equals(supplierType2, StringComparison.Ordinal);

            if (!match)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        int GetAlertStatus(int productIdInCart, int productIdSelected)
        {
            int status = 0;
            string supplierType1 = _repository.GetSupplierTypeByProductId(productIdInCart);
            string supplierType2 = _repository.GetSupplierTypeByProductId(productIdSelected);

            if (supplierType1.Equals("External") && supplierType2.Equals("Internal"))
            {
                status = 5;
            }
            else
            {
                status = 6;
            }
            return status;
        }
    }
}
