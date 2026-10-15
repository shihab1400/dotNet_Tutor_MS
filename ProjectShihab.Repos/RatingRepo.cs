using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class RatingRepo(AsDbContext context)
    {
        public Result<List<Rating>> GetAll()
        {
            var result = new Result<List<Rating>>();
            try
            {
                result.Data = context.Ratings.Include(e=>e.Course).Include(e=>e.User).ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Rating> GetById(int id)
        {
            var result = new Result<Rating>();
            try
            {
                result.Data = context.Ratings.Find(id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Rating> Save(Rating model)
        {
            var result = new Result<Rating>();

            try
            {
                if (context.Ratings.Any(e => e.Id != model.Id))
                {
                    result.HasError = true;
                    result.Message = "Already exists.";
                    return result;
                }

                var objToSave = context.Ratings.Find(model.Id);
                if (objToSave == null)
                {
                    objToSave = new Rating();
                    context.Ratings.Add(objToSave);
                }

                objToSave.UserId = model.UserId;
                objToSave.CourseType = model.CourseType;
                objToSave.CourseId = model.CourseId;
                objToSave.RatingValue = model.RatingValue;
                objToSave.Feedback = model.Feedback;

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

        public Result<Rating> Delete(int id)
        {
            var result = new Result<Rating>();

            try
            {
                var entity = context.Ratings.Find(id);

                if (entity == null)
                {
                    result.HasError = true;
                    result.Message = $"Rating with ID {id} was not found.";
                    return result;
                }

                context.Ratings.Remove(entity);
                context.SaveChanges();

                result.Data = entity;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = "An error occurred while deleting the Rating.";
            }

            return result;
        }
    }
}
