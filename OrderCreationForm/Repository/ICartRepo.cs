using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderCreationForm.Models;

namespace OrderCreationForm.Repository
{
    public interface ICartRepo
    {
        string GetCartTypeByEmployeeId(string employeeId);
        int GetProductIdForLastItemInInternalCart(string employeeId);
        int GetProductIdForLastItemInExternalCart(string employeeId);
        bool CheckIfCartIsComplete(string cartId);
        int GetSupplierIdByCartId(string cartId);
        bool CheckIfSupplierAlreadySelected(int productId);
        int GetRequisitionNoByEmployeeIdGrpNo(string employeeId,int grpNo);
        List<CartProperty> GetAllPassiveCartIdsByEmployeeId(string employeeId);
        int GetOrderDetailsNoByCartId(string cartId);
        List<Cart> GetCartItemsByCartId(string cartId);
        IList<Cart> GetAllActiveCartItemsByEmployeeId(string employeeId);
        int GetCartGroupNoByCartId(string cartId);
        string GetActiveCartIdByEmployeeId(string employeeId);
        string GetCartIdOfLastEntryOfExtActiveCartItem(string employeeId);
        IList<Cart> GetAllActiveInternalCartEntriesByEmployeeId(string employeeId);
        bool CheckIfUserHasActiveCartItems(string employeeId);
        bool CheckIfUserHasInternalActiveCartItems(string employeeId);
        bool CheckIfUserHasExternalActiveCartItems(string employeeId);
        bool CheckIfCartPropertyExistsForThisId(string cartId);
        string GetSupplierTypeByProductId(int productId);
        int GetSupplierIdByProductId(int productId);
        decimal GetCartTotalByCartId(string cartId);
        bool ChangeCartCompletionStatus(string cartId);
    }
}
