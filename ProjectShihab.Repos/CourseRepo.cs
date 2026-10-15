using EventManagement.Shared;
using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class CourseRepo(AsDbContext context, CurrentUserHelper currentUserHelper)
    {
        public Result<List<Course>> GetAll()
        {
            var result = new Result<List<Course>>();
            try
            {
                result.Data = context.Courses.Include(e=>e.Group).Include(e=>e.User).ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Course> GetById(int id)
        {
            var result = new Result<Course>();
            try
            {
                result.Data = context.Courses.Find(id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Course> Save(Course model)
        {
            var result = new Result<Course>();

            try
            {
                if (context.Courses.Any(e => e.Subject.ToLower() == model.Subject.ToLower() && e.GroupId == model.GroupId && e.Id != model.Id))
                {
                    result.HasError = true;
                    result.Message = "Title already exists.";
                    return result;
                }

                var objToSave = context.Courses.Find(model.Id);
                if (objToSave == null)
                {
                    objToSave = new Course();
                    context.Courses.Add(objToSave);
                }

                objToSave.GroupId = model.GroupId;
                objToSave.Subject = model.Subject;

                objToSave.CreatedAt = DateTime.Now;
                objToSave.CreatedBy = currentUserHelper.UserId;

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

        public Result<Course> Delete(int id)
        {
            var result = new Result<Course>();

            try
            {
                var entity = context.Courses.Find(id);

                if (entity == null)
                {
                    result.HasError = true;
                    result.Message = $"Course with ID {id} was not found.";
                    return result;
                }

                context.Courses.Remove(entity);
                context.SaveChanges();

                result.Data = entity;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = "An error occurred while deleting the Course.";
            }

            return result;
        }
    }
}
