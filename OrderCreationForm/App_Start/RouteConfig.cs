 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OrderCreationForm
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("GetCartTotal",
                "carts/getcarttotal/",
                new { controller = "Carts", action = "GetCartTotal" },
                new[] { "RequisitionApprovalSys.Controllers" });

            routes.MapRoute("SearchProducts",
                "items/searchproducts/",
                new { controller = "Items", action = "SearchProducts" },
                new[] { "RequisitionApprovalSys.Controllers" });

            routes.MapRoute("GetProductsBySupplierId",
              "items/getproductsbysupplierid/",
               new { controller = "Items", action = "GetProductsBySupplierId" },
               new[] { "RequisitionApprovalSys.Controllers" });

            routes.MapRoute("GetSuppliersBySupplierTypeId",
             "items/getsuppliersbysuppliertypeid/",
              new { controller = "Items", action = "GetSuppliersBySupplierTypeId" },
              new[] { "RequisitionApprovalSys.Controllers" });

            routes.MapRoute("AddToCart",
             "carts/addtocart/",
              new { controller = "Carts", action = "AddToCart" },
              new[] { "RequisitionApprovalSys.Controllers" });

            routes.MapRoute("GetSupplierInfoByProductId",
             "items/getsupplierinfobyproductid/",
              new { controller = "Items", action = "GetSupplierInfoByProductId" },
              new[] { "RequisitionApprovalSys.Controllers" });

            routes.MapRoute("GetAllPassiveCartsId",
             "carts/getallpassivecartsid/",
              new { controller = "Carts", action = "GetAllPassiveCartsId" },
              new[] { "RequisitionApprovalSys.Controllers" });

            routes.MapRoute("GetCartType",
             "carts/getcarttype/",
              new { controller = "Carts", action = "GetCartType" },
              new[] { "RequisitionApprovalSys.Controllers" });
                    }
    }
}
