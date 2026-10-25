using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class PaymentRepo(AsDbContext context)
    {
        public Result<List<Payment>> GetAll(int? userId = null)
        {
            try
            {
                var query = context.Payments.Include(e => e.User).Include(e => e.Coupon).AsQueryable();
                if (userId != null)
                    query = query.Where(e => e.UserId == userId);
                return Result<List<Payment>>.Ok(query.OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Payment>>.Fail(e.Message);
            }
        }

        // Demo payment: always succeeds. Caller owns the transaction and SaveChanges.
        internal Payment Pay(int userId, string kind, int targetId, PriceQuote quote)
        {
            var payment = new Payment
            {
                UserId = userId,
                Kind = kind,
                TargetId = targetId,
                OriginalAmount = quote.Original,
                DiscountAmount = quote.Discount,
                FinalAmount = quote.Final,
                CouponId = quote.Coupon?.Id,
                TransactionRef = "TXN-" + Guid.NewGuid().ToString("N")[..12].ToUpper(),
                PaidAt = DateTime.Now
            };
            context.Payments.Add(payment);

            if (quote.Coupon != null)
                quote.Coupon.UsedCount++;

            return payment;
        }
    }
}
