using BusinessLayer.ClassHelpers;
using BusinessLayer.ClassHelpers.Extensions;
using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BusinessLayer.Implementation.Admin
{
    public class ProductOrderServiceAdmin(IProductDataAccess productDataAccess) : IProductOrderServiceAdmin
    {
        readonly IProductDataAccess _productDataAccess = productDataAccess;

        public (decimal DebitAmount, decimal CreditAmount, string Message)? UpdateAfterSale(Order order)
        {
            var error = new StringBuilder();
            decimal creditAmmount = 0;
            decimal debitAmmount = 0;
            CheckProductsOnOrder(order);
            var productIds = order.Products.Select(p => (Guid)p.AltId);
            var products = this._productDataAccess.GetAll(productIds);
            var groupedProducts = order.Products.GroupProducts();
            creditAmmount = this.PrcessProductsCredit(groupedProducts, products, error);
            _productDataAccess.UpdateAll(products);
            order.Amount = order.Products.Sum(p => p.Price);
            var paid = order.PaymentDetails.Sum(p => p.Amount);
            var paymentDiscrepancy = order.Amount - paid;
            if(paymentDiscrepancy > 0)
            {
                debitAmmount += paymentDiscrepancy;
                error.AppendLine($"Customer paid {paid} for an order of {order.Amount}, overpaid by {debitAmmount}");
            } 
            else if(paymentDiscrepancy < 0)
            {
                creditAmmount += -paymentDiscrepancy;
                error.AppendLine($"Customer paid {paid} for an order of {order.Amount}, underpaid by {creditAmmount}");
            }
            if(error.Length == 0)
            {
                return null;
            }
            return (debitAmmount, creditAmmount, error.ToString());
        }

        private decimal PrcessProductsCredit(IEnumerable<SimpleProduct> groupedProducts, IEnumerable<Product>? products, StringBuilder error)
        {
            decimal creditAmount = 0; 
            foreach (var p in groupedProducts)
            {
                var prod = products.FirstOrDefault(prod => prod.Id == p.Id);
                var instock = prod.NumberInStock;
                if (p.Quantity > instock)
                {
                    var missingAmount = (decimal)(p.Quantity - instock);
                    creditAmount += p.Price * missingAmount;
                    error.AppendLine($"{p.Name} has {missingAmount} out of stock, order unfulfilled. Price {p.Price}, refund is {creditAmount}");
                    prod.NumberInStock = 0;
                }
                else
                {
                    prod.NumberInStock = instock - p.Quantity;
                }
            }
            return creditAmount;
        }

        private void CheckProductsOnOrder(Order order)
        {
            if (order.Products == null)
            {
                throw new ArgumentException("Order products are null");
            }
            if(order.Products.Any(p => p.AltId == null))
            {
                throw new ArgumentException("Order contains products with an invalid Guid Id");
            }
        }
    }
}
