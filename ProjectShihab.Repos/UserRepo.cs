using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class UserRepo(AsDbContext context)
    {
        public Result<List<User>> GetAll()
        {
            var result = new Result<List<User>>();
            try
            {
                result.Data = context.Users.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<User> GetById(int id)
        {
            var result = new Result<User>();
            try
            {
                result.Data = context.Users.Find(id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<User> Authenticate(string email, string password)
        {
            var result = new Result<User>();
            try
            {
                result.Data = context.Users.FirstOrDefault(e => e.Email == email && e.Password == password);
                if(result.Data==null)
                {
                    result.HasError = true;
                    result.Message = "Invalid Credential";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<User> Save(User model)
        {
            var result = new Result<User>();

            try
            {
                if (context.Users.Any(e => e.Email.ToLower() == model.Email.ToLower() && e.Id != model.Id))
                {
                    result.HasError = true;
                    result.Message = "Email already exists.";
                    return result;
                }

                var objToSave = context.Users.Find(model.Id);
                if (objToSave == null)
                {
                    objToSave = new User();
                    context.Users.Add(objToSave);
                }

                objToSave.UserName = model.UserName;
                objToSave.Email = model.Email;
                objToSave.Gender = model.Gender;

                objToSave.ContactNumber = model.ContactNumber;
                objToSave.Dob = model.Dob;
                objToSave.Role = model.Role;
                objToSave.Address = model.Address;
                objToSave.Password = model.Password;
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

        public Result<User> Delete(int id)
        {
            var result = new Result<User>();

            try
            {
                var entity = context.Users.Find(id);

                if (entity == null)
                {
                    result.HasError = true;
                    result.Message = $"User with ID {id} was not found.";
                    return result;
                }

                context.Users.Remove(entity);
                context.SaveChanges();

                result.Data = entity;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = "An error occurred while deleting the User.";
            }

            return result;
        }
    }
}
