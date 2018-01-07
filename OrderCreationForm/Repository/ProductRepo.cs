using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderCreationForm.Models;

namespace OrderCreationForm.Repository
{
    public class ProductRepo:IProductRepo
    {
        private RASEntities _dataContext;

        public ProductRepo()
        {
            _dataContext = new RASEntities();
        }

        public IList<SupplierType> GetAllSupplierTypes()
        {
            var query = from i in _dataContext.SupplierTypes
                        select i;
            var content = query.ToList<SupplierType>();
            return content;
        }
        public IList<Supplier> GetAllSuppliersBySupplierTypeId(int supplierTypeId)
        {
            var query = from supplier in _dataContext.Suppliers
                        where supplier.SupplierTypeId == supplierTypeId
                        select supplier;
            var content = query.ToList<Supplier>();
            return content;
        }

        public IList<ProductCategory> GetAllProductCategories()
        {
            var query = from i in _dataContext.ProductCategories
                        select i;
            var content = query.ToList<ProductCategory>();
            return content;
        }

        public IList<Product> GetAllProductByProductCategoryId(int productCategoryId)
        {
            var query = from product in _dataContext.Products
                        where product.ProductCatId == productCategoryId
                        select product;
            var content = query.ToList<Product>();
            return content;
        }

        public IList<Product> GetAllProductBySupplierId(int? supplierId)
        {
            var query = from product in _dataContext.Products
                        where product.SupplierId == supplierId
                        select product;
            var content = query.ToList<Product>();
            return content;
        }

        public IList<Product> GetAllProductsBySupplierIdAndSearchString(int? supplierId, string searchString)
        {
            var query = from product in _dataContext.Products
                        where product.SupplierId == supplierId && (product.ProductDesc.ToUpper().Contains(searchString.ToUpper()) ||
                        product.ProductCategory.ProductCatDesc.ToUpper().Contains(searchString.ToUpper()))
                        select product;
            var content = query.ToList<Product>();
            return content;
        }
    }
}