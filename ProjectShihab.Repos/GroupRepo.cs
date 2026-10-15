using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class GroupRepo(AsDbContext context)
    {
        public Result<List<Group>> GetAll()
        {
            var result = new Result<List<Group>>();
            try
            {
                result.Data = context.Groups.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Group> GetById(int id)
        {
            var result = new Result<Group>();
            try
            {
                result.Data = context.Groups.Find(id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<Group> Save(Group model)
        {
            var result = new Result<Group>();

            try
            {
                if (context.Groups.Any(e => e.Title.ToLower() == model.Title.ToLower() && e.Id != model.Id))
                {
                    result.HasError = true;
                    result.Message = "Group Title already exists.";
                    return result;
                }

                var objToSave = context.Groups.Find(model.Id);
                if (objToSave == null)
                {
                    objToSave = new Group();
                    context.Groups.Add(objToSave);
                }

                objToSave.Title = model.Title;

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

        public Result<Group> Delete(int id)
        {
            var result = new Result<Group>();

            try
            {
                var entity = context.Groups.Find(id);

                if (entity == null)
                {
                    result.HasError = true;
                    result.Message = $"Group Title with ID {id} was not found.";
                    return result;
                }

                context.Groups.Remove(entity);
                context.SaveChanges();

                result.Data = entity;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = "An error occurred while deleting the Group Title.";
            }

            return result;
        }
    }
}
