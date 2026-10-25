using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class PriceQuote
    {
        public decimal Original { get; set; }
        public decimal Discount { get; set; }
        public decimal Final { get; set; }
        public Coupon? Coupon { get; set; }
    }

    public class CouponRepo(AsDbContext context, LogRepo logRepo)
    {
        public Result<List<Coupon>> GetAll()
        {
            try
            {
                return Result<List<Coupon>>.Ok(context.Coupons.OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Coupon>>.Fail(e.Message);
            }
        }

        public Result<Coupon> GetById(int id)
        {
            try
            {
                return Result<Coupon>.Ok(context.Coupons.Find(id));
            }
            catch (Exception e)
            {
                return Result<Coupon>.Fail(e.Message);
            }
        }

        public Result<Coupon> Save(Coupon model)
        {
            try
            {
                if (model.Type != CouponTypes.Percentage && model.Type != CouponTypes.Fixed)
                    return Result<Coupon>.Fail("Invalid coupon type.");
                if (model.Type == CouponTypes.Percentage && model.Value > 100)
                    return Result<Coupon>.Fail("Percentage cannot exceed 100.");
                if (context.Coupons.Any(e => e.Code.ToLower() == model.Code.ToLower() && e.Id != model.Id))
                    return Result<Coupon>.Fail("Coupon code already exists.");

                var objToSave = context.Coupons.Find(model.Id);
                var isNew = objToSave == null;
                if (objToSave == null)
                {
                    objToSave = new Coupon();
                    context.Coupons.Add(objToSave);
                }

                objToSave.Code = model.Code.Trim().ToUpper();
                objToSave.Type = model.Type;
                objToSave.Value = model.Value;
                objToSave.ExpiryDate = model.ExpiryDate.Date;
                objToSave.MaxUses = model.MaxUses;
                objToSave.IsActive = model.IsActive;
                context.SaveChanges();

                logRepo.Add(isNew ? "Create Coupon" : "Update Coupon", "Coupon", objToSave.Id, objToSave.Code);
                return Result<Coupon>.Ok(objToSave);
            }
            catch (Exception e)
            {
                return Result<Coupon>.Fail(e.Message);
            }
        }

        // Single place where a coupon turns an amount into a discounted amount. Does not consume the coupon.
        public Result<PriceQuote> Quote(decimal amount, string? code)
        {
            var quote = new PriceQuote { Original = amount, Final = amount };
            if (string.IsNullOrWhiteSpace(code))
                return Result<PriceQuote>.Ok(quote);

            try
            {
                var normalized = code.Trim().ToUpper();
                var coupon = context.Coupons.FirstOrDefault(e => e.Code == normalized);
                if (coupon == null || !coupon.IsActive)
                    return Result<PriceQuote>.Fail("Invalid coupon.");
                if (coupon.ExpiryDate.Date < DateTime.Today)
                    return Result<PriceQuote>.Fail("Coupon has expired.");
                if (coupon.UsedCount >= coupon.MaxUses)
                    return Result<PriceQuote>.Fail("Coupon usage limit reached.");

                var discount = coupon.Type == CouponTypes.Percentage
                    ? Math.Round(amount * coupon.Value / 100, 2)
                    : coupon.Value;
                discount = Math.Min(discount, amount);

                quote.Coupon = coupon;
                quote.Discount = discount;
                quote.Final = amount - discount;
                return Result<PriceQuote>.Ok(quote);
            }
            catch (Exception e)
            {
                return Result<PriceQuote>.Fail(e.Message);
            }
        }
    }
}
