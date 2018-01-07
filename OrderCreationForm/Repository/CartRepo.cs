using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderCreationForm.Models;
using System.Threading.Tasks;

namespace OrderCreationForm.Repository
{
    public class CartRepo:ICartRepo
    {
        private RASEntities _dataContext;
        public CartRepo()
        {
            _dataContext = new RASEntities();
        }

        public string GetCartTypeByEmployeeId(string employeeId)
        {
            var cartType = (from i in _dataContext.CartProperties
                            where i.EmployeeId.Equals(employeeId)
                            select i.CartType).First();

            return cartType;
        }

        public bool CheckIfCartIsComplete(string cartId)
        {
            bool flag = false;
            var item = (from i in _dataContext.CartProperties
                        where i.CartId.Equals(cartId) &&
                        i.IsComplete.Equals(true) &&
                        i.IsActive.Equals(true)
                        select i).ToList();

            if (item.Count > 0)
            {
                flag = true;
            }
            return flag;
        }

        public int GetSupplierIdByCartId(string cartId)
        {
            int supplierId = (from i in _dataContext.Carts
                            where i.CartId.Equals(cartId)
                            select i.Product.SupplierId).First();
            return supplierId;
        }

        public bool CheckIfSupplierAlreadySelected(int productId)
        {
            bool flag = false;
            var IdCollection = (from i in _dataContext.CartProperties
                        where i.IsActive.Equals(true)
                        select i.CartId).ToList();

            int count = 0;
            int supplierIdA = GetSupplierIdByProductId(productId);
            for (int i = 0; i < IdCollection.Count; i++)
            {
                var CId = IdCollection[i];
                int supplierIdB = GetSupplierIdByCartId(CId);
                if (supplierIdA == supplierIdB)
                {
                    count++;
                }
                
            }

            if(count > 0)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            return flag;

        }

        public int GetRequisitionNoByEmployeeIdGrpNo(string employeeId,int grpNo)
        {
            var cartId = (from i in _dataContext.CartProperties
                         where i.EmployeeId.Equals(employeeId)
                         && i.GroupNum < grpNo
                         && i.IsComplete.Equals(true)
                         && i.IsActive.Equals(true)
                         select i.CartId).First();
            int reqNo = (from i in _dataContext.OrderDetails
                         where i.CartId.Equals(cartId)
                         select i.RequisitionNo).First();
            return reqNo;
        }

        public List<Cart> GetCartItemsByCartId(string cartId)
        {
            var items = (from i in _dataContext.Carts
                         where i.CartId.Equals(cartId)
                         select i).ToList();
            return items;
        }
        public int GetOrderDetailsNoByCartId(string cartId)
        {
            int id = (from i in _dataContext.OrderDetails
                      where i.CartId.Equals(cartId)
                      select i.OrderDetailsNo).First();
            return id;
        }

        public List<CartProperty> GetAllPassiveCartIdsByEmployeeId(string employeeId)
        {
            var records = (from i in _dataContext.CartProperties
                           where i.EmployeeId.Equals(employeeId)
                           && i.IsComplete.Equals(true)
                           && i.IsActive.Equals(true)
                           select i).OrderBy(x => x.GroupNum).ToList();
            return records;
        }

        public bool CheckIfCartPropertyExistsForThisId(string cartId)
        {
            bool exists = false;
            int records = (from i in _dataContext.CartProperties
                           where i.CartId.Equals(cartId)
                           select i).Count();

            if(records > 0)
            {
                exists = true;
            }
            return exists;
        }

        public int GetCartGroupNoByCartId(string cartId)
        {
            int GroupNo = (from i in _dataContext.CartProperties
                           where i.CartId.Equals(cartId)
                           select i.GroupNum).First();
            return GroupNo;
        }
        public decimal GetCartTotalByCartId(string cartId)
        {
            decimal sumLineTotal = (from i in _dataContext.Carts
                                    where i.CartId.Equals(cartId)
                                    select i.SubTotal).Sum();
            return sumLineTotal;
        }

        public string GetActiveCartIdByEmployeeId(string employeeId)
        {
            var resultSet = (from i in _dataContext.CartProperties
                             where i.EmployeeId.Equals(employeeId) &&
                                   i.IsComplete.Equals(false) &&
                                   i.IsActive.Equals(true)
                             select i.CartId).ToList();

            string data = resultSet[0];
            return data;
        }
        public bool CheckIfUserHasActiveCartItems(string employeeId)
        {
            bool doesExist;
            var query = from i in _dataContext.CartProperties
                        where i.EmployeeId.Equals(employeeId) 
                        && i.IsComplete.Equals(false)
                        && i.IsActive.Equals(true)
                        select i;

            if (query.Count() > 0)
            {
                doesExist = true;
            }
            else
            {
                doesExist = false;
            }

            return doesExist;
        }

        public bool CheckIfUserHasInternalActiveCartItems(string employeeId)
        {
            bool doesExist;
            var query = from i in _dataContext.CartProperties
                        where i.EmployeeId.Equals(employeeId)
                        && i.CartType.Equals("Internal")
                        && i.IsComplete.Equals(true)
                        && i.IsActive.Equals(true)
                        select i;

            if (query.Count() > 0)
            {
                doesExist = true;
            }
            else
            {
                doesExist = false;
            }

            return doesExist;
        }

        public bool CheckIfUserHasExternalActiveCartItems(string employeeId)
        {
            bool doesExist;
            var query = from i in _dataContext.CartProperties
                        where i.EmployeeId.Equals(employeeId)
                        && i.CartType.Equals("External")
                        && i.IsComplete.Equals(true)
                        && i.IsActive.Equals(true)
                        select i;

            if (query.Count() > 0)
            {
                doesExist = true;
            }
            else
            {
                doesExist = false;
            }

            return doesExist;
        }
        public IList<Cart> GetAllActiveCartItemsByEmployeeId(string employeeId)
        {
            var cartId = (from i in _dataContext.CartProperties
                          where i.EmployeeId.Equals(employeeId)
                          && i.IsComplete.Equals(false)
                          && i.IsActive.Equals(true)
                          select i.CartId).First();
            List<Cart> content;
            var query = (from i in _dataContext.Carts
                        where i.CartId.Equals(cartId)
                        select i).ToList();
            if(query.Count() > 0)
            {
                content = query;               
            }
            else
            {
                content = null;
            }

            return content;
        }

        public int GetProductIdForLastItemInExternalCart(string employeeId){
            var id = (from i in _dataContext.CartProperties
                         where i.EmployeeId.Equals(employeeId) &&
                         i.CartType.Equals("External") &&
                         i.IsComplete.Equals(true) &&
                         i.IsActive.Equals(true)
                         select i.CartId).First();

            int prodId = (from i in _dataContext.Carts
                          where i.CartId.Equals(id)
                          select i.ProductId).First();

            return prodId;
        }

        public int GetProductIdForLastItemInInternalCart(string employeeId)
        {
            var id = (from i in _dataContext.CartProperties
                      where i.EmployeeId.Equals(employeeId) &&
                      i.CartType.Equals("Internal") &&
                      i.IsComplete.Equals(true) &&
                      i.IsActive.Equals(true)
                      select i.CartId).First();

            int prodId = (from i in _dataContext.Carts
                          where i.CartId.Equals(id)
                          select i.ProductId).First();

            return prodId;
        }

        public string GetCartIdOfLastEntryOfExtActiveCartItem(string employeeId)
        {
            var items = (from i in _dataContext.CartProperties
                          where i.EmployeeId.Equals(employeeId)
                          && i.CartType.Equals("External")
                          && i.IsComplete.Equals(true)
                          && i.IsActive.Equals(true)
                          select i).OrderBy(x=>x.GroupNum).ToList();
            int index = (items.Count()-1);

            string cartId = items[index].CartId;
            return cartId;
        }

        public IList<Cart> GetAllActiveInternalCartEntriesByEmployeeId(string employeeId)
        {
            var cartId = (from i in _dataContext.CartProperties
                          where i.EmployeeId.Equals(employeeId)
                          && i.CartType.Equals("Internal")
                          && i.IsComplete.Equals(true)
                          && i.IsActive.Equals(true)
                          select i.CartId).First();
            List<Cart> content;
            var query = (from i in _dataContext.Carts
                         where i.CartId.Equals(cartId)
                         select i).ToList();
            if (query.Count() > 0)
            {
                content = query;
            }
            else
            {
                content = null;
            }

            return content;
        }

        public string GetSupplierTypeByProductId(int productId)
        {
            var resultSet = (from i in _dataContext.Products
                             where i.ProductId.Equals(productId)
                             select i.Supplier.SupplierType.SupplierTypeDesc).ToList();

            string data = resultSet[0];
            return data;
        }

        public int GetSupplierIdByProductId(int productId)
        {
            var resultSet = (from i in _dataContext.Products
                             where i.ProductId.Equals(productId)
                             select i.Supplier.SupplierId).ToList();

            int data = resultSet[0];
            return data;
        }

        public bool ChangeCartCompletionStatus(string cartId)
        {
            bool flag = false;
            var RowToUpdate = (from i in _dataContext.CartProperties
                               where i.CartId.Equals(cartId)
                               select i).First();

            RowToUpdate.IsComplete = true;
            var result = _dataContext.SaveChanges();
            if(result > 0)
            {
                flag = true;
            }
            return flag;
        }
    }
}