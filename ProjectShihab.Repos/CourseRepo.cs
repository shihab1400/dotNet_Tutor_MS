using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class CourseRepo(AsDbContext context, CurrentUserHelper currentUserHelper, LogRepo logRepo)
    {
        public Result<List<Course>> GetAll()
        {
            try
            {
                return Result<List<Course>>.Ok(context.Courses.Include(e => e.Group).Include(e => e.User)
                    .OrderBy(e => e.Group!.Title).ThenBy(e => e.Subject).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Course>>.Fail(e.Message);
            }
        }

        public Result<Course> GetById(int id)
        {
            try
            {
                return Result<Course>.Ok(context.Courses.Include(e => e.Group).FirstOrDefault(e => e.Id == id));
            }
            catch (Exception e)
            {
                return Result<Course>.Fail(e.Message);
            }
        }

        public Result<Course> Save(Course model)
        {
            try
            {
                if (context.Courses.Any(e => e.Subject.ToLower() == model.Subject.ToLower() && e.GroupId == model.GroupId && e.Id != model.Id))
                    return Result<Course>.Fail("Subject already exists in this group.");

                var objToSave = context.Courses.Find(model.Id);
                var isNew = objToSave == null;
                if (objToSave == null)
                {
                    objToSave = new Course { CreatedAt = DateTime.Now, CreatedBy = currentUserHelper.UserId };
                    context.Courses.Add(objToSave);
                }

                objToSave.GroupId = model.GroupId;
                objToSave.Subject = model.Subject;
                objToSave.BasePrice = model.BasePrice;
                context.SaveChanges();

                logRepo.Add(isNew ? "Create Course" : "Update Course", "Course", objToSave.Id, objToSave.Subject);
                return Result<Course>.Ok(objToSave);
            }
            catch (Exception e)
            {
                return Result<Course>.Fail(e.Message);
            }
        }

        public Result<Course> Delete(int id)
        {
            try
            {
                var entity = context.Courses.Find(id);
                if (entity == null)
                    return Result<Course>.Fail($"Course with ID {id} was not found.");

                context.Courses.Remove(entity);
                context.SaveChanges();
                logRepo.Add("Delete Course", "Course", id, entity.Subject);
                return Result<Course>.Ok(entity);
            }
            catch (Exception)
            {
                return Result<Course>.Fail("This course is in use and cannot be deleted.");
            }
        }
    }
}
