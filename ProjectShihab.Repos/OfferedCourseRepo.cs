using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class OfferedCourseRepo(AsDbContext context)
    {
        public Result<List<OfferedCourse>> GetAll()
        {
            var result = new Result<List<OfferedCourse>>();
            try
            {
                result.Data = context.OfferedCourses.Include(e => e.Course).Include(e => e.User).ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<OfferedCourse> GetById(int id)
        {
            var result = new Result<OfferedCourse>();
            try
            {
                result.Data = context.OfferedCourses.Find(id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<OfferedCourse> Save(OfferedCourse model)
        {
            var result = new Result<OfferedCourse>();

            try
            {
                if (context.OfferedCourses.Any(e => e.CourseId == model.CourseId && e.TutorId == model.TutorId && e.Id != model.Id))
                {
                    result.HasError = true;
                    result.Message = "Already exists.";
                    return result;
                }

                var objToSave = context.OfferedCourses.Find(model.Id);
                if (objToSave == null)
                {
                    objToSave = new OfferedCourse();
                    context.OfferedCourses.Add(objToSave);
                }

                objToSave.CourseId = model.CourseId;
                objToSave.TutorId = model.TutorId;
                objToSave.Price = model.Price;

                objToSave.StartDate = model.StartDate;
                objToSave.EndDate = model.EndDate;

                objToSave.Status = model.Status;

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

        public Result<OfferedCourse> Delete(int id)
        {
            var result = new Result<OfferedCourse>();

            try
            {
                var entity = context.OfferedCourses.Find(id);

                if (entity == null)
                {
                    result.HasError = true;
                    result.Message = $"Offered Course with ID {id} was not found.";
                    return result;
                }

                context.OfferedCourses.Remove(entity);
                context.SaveChanges();

                result.Data = entity;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = "An error occurred while deleting the Offered Course.";
            }

            return result;
        }
    }
}
