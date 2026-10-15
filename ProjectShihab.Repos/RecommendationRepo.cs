using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class RecommendationRepo(AsDbContext context)
    {
        public Result<List<Recommendation>> GetAll()
        {
            var result = new Result<List<Recommendation>>();
            try
            {
                result.Data = context.Recommendations.Include(e=>e.FromUser).Include(e=>e.ToUser).ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Recommendation> GetById(int id)
        {
            var result = new Result<Recommendation>();
            try
            {
                result.Data = context.Recommendations.Find(id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Recommendation> Save(Recommendation model)
        {
            var result = new Result<Recommendation>();

            try
            {
                if (context.Recommendations.Any(e => e.FromUserId == model.FromUserId && e.ToUserId == model.ToUserId && e.Id != model.Id))
                {
                    result.HasError = true;
                    result.Message = "Recommendation already exists.";
                    return result;
                }

                var objToSave = context.Recommendations.Find(model.Id);
                if (objToSave == null)
                {
                    objToSave = new Recommendation();
                    context.Recommendations.Add(objToSave);
                }

                objToSave.FromUserId = model.FromUserId;
                objToSave.ToUserId = model.ToUserId;

                objToSave.Message = model.Message;
                objToSave.Date = DateTime.Now;

                context.SaveChanges();
                result.Data = objToSave;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public Result<Recommendation> Delete(int id)
        {
            var result = new Result<Recommendation>();

            try
            {
                var entity = context.Recommendations.Find(id);

                if (entity == null)
                {
                    result.HasError = true;
                    result.Message = $"Recommendation with ID {id} was not found.";
                    return result;
                }

                context.Recommendations.Remove(entity);
                context.SaveChanges();

                result.Data = entity;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = "An error occurred while deleting the Recommendation.";
            }

            return result;
        }
    }
}
