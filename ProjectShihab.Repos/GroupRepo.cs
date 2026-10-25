using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class GroupRepo(AsDbContext context, LogRepo logRepo)
    {
        public Result<List<Group>> GetAll()
        {
            try
            {
                return Result<List<Group>>.Ok(context.Groups.OrderBy(e => e.Title).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Group>>.Fail(e.Message);
            }
        }

        public Result<Group> GetById(int id)
        {
            try
            {
                return Result<Group>.Ok(context.Groups.Find(id));
            }
            catch (Exception e)
            {
                return Result<Group>.Fail(e.Message);
            }
        }

        public Result<Group> Save(Group model)
        {
            try
            {
                if (context.Groups.Any(e => e.Title.ToLower() == model.Title.ToLower() && e.Id != model.Id))
                    return Result<Group>.Fail("Title already exists.");

                var objToSave = context.Groups.Find(model.Id);
                var isNew = objToSave == null;
                if (objToSave == null)
                {
                    objToSave = new Group();
                    context.Groups.Add(objToSave);
                }

                objToSave.Title = model.Title;
                context.SaveChanges();

                logRepo.Add(isNew ? "Create Group" : "Update Group", "Group", objToSave.Id, objToSave.Title);
                return Result<Group>.Ok(objToSave);
            }
            catch (Exception e)
            {
                return Result<Group>.Fail(e.Message);
            }
        }

        public Result<Group> Delete(int id)
        {
            try
            {
                var entity = context.Groups.Find(id);
                if (entity == null)
                    return Result<Group>.Fail($"Group with ID {id} was not found.");
                if (context.Courses.Any(e => e.GroupId == id))
                    return Result<Group>.Fail("This group still has courses.");

                context.Groups.Remove(entity);
                context.SaveChanges();
                logRepo.Add("Delete Group", "Group", id, entity.Title);
                return Result<Group>.Ok(entity);
            }
            catch (Exception)
            {
                return Result<Group>.Fail("An error occurred while deleting the Group.");
            }
        }
    }
}
