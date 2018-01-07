using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderCreationForm.ViewModels;
using OrderCreationForm.Models;
using OrderCreationForm.Repository;
using System.IO;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace OrderCreationForm.Controllers
{
    public class OrderDetailsController : Controller
    {
        private ICartRepo _repository;
        private RASEntities db = new RASEntities();

        public OrderDetailsController() : this(new CartRepo())
        {
        }

        public OrderDetailsController(ICartRepo repository)
        {
            _repository = repository;
        }

         //GET: OrderDetails
        public ActionResult FileUploadHandler()
        {
            return PartialView("_FileUpload");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileUploadHandler([Bind(Include = "Quote")] FileViewModel File)
        {
            if (ModelState.IsValid)
            {
                bool status = false;
                string employeeId = "79a0e692-d36e-455a-bfd9-32ed77c066b4";
                string cartId = _repository.GetActiveCartIdByEmployeeId(employeeId);
                Random rnd = new Random();
                var fileData = new MemoryStream();
                File.Quote.InputStream.CopyTo(fileData);

                var NewEntry = new OrderDetail
                {
                    CartId = cartId,
                    GrandTotal = _repository.GetCartTotalByCartId(cartId),
                    RequisitionNo = rnd.Next(100000, 9999999),
                    QuotationFile = fileData.ToArray()
                };

                db.OrderDetails.Add(NewEntry);
                try
                {
                    var result = db.SaveChanges();
                    if (result > 0)
                    {

                        status = true;
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
                return RedirectToAction("Index");
            }

            return View(File);
        }
    }
}