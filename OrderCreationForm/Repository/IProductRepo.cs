using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderCreationForm.Models;

namespace OrderCreationForm.Repository
{
    public interface IProductRepo
    {
        IList<SupplierType> GetAllSupplierTypes();
        IList<Supplier> GetAllSuppliersBySupplierTypeId(int supplierTypeId);
        IList<ProductCategory> GetAllProductCategories();
        IList<Product> GetAllProductByProductCategoryId(int productCategoryId);
        IList<Product> GetAllProductBySupplierId(int? supplierId);
        IList<Product> GetAllProductsBySupplierIdAndSearchString(int? supplierId, string searchString);
    }
}
