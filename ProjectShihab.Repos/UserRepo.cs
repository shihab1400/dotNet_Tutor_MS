using Microsoft.AspNetCore.Identity;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class UserRepo(AsDbContext context, LogRepo logRepo)
    {
        private readonly PasswordHasher<User> hasher = new();

        public Result<List<User>> GetAll(string? role = null)
        {
            try
            {
                var query = context.Users.AsQueryable();
                if (!string.IsNullOrEmpty(role))
                    query = query.Where(e => e.Role == role);
                return Result<List<User>>.Ok(query.OrderBy(e => e.Role).ThenBy(e => e.UserName).ToList());
            }
            catch (Exception e)
            {
                return Result<List<User>>.Fail(e.Message);
            }
        }

        public Result<User> GetById(int id)
        {
            try
            {
                return Result<User>.Ok(context.Users.Find(id));
            }
            catch (Exception e)
            {
                return Result<User>.Fail(e.Message);
            }
        }

        public Result<User> Authenticate(string email, string password)
        {
            try
            {
                var user = context.Users.FirstOrDefault(e => e.Email == email);
                if (user == null || hasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
                {
                    logRepo.AddFor(user, "Login Failed", $"Email: {email}");
                    return Result<User>.Fail("Invalid Credential");
                }

                if (!user.Status)
                {
                    logRepo.AddFor(user, "Login Blocked", "Account is deactivated");
                    return Result<User>.Fail("Your account is deactivated.");
                }

                logRepo.AddFor(user, "Login");
                return Result<User>.Ok(user);
            }
            catch (Exception e)
            {
                return Result<User>.Fail(e.Message);
            }
        }

        // Public self-registration: only Student and Tutor are allowed.
        public Result<User> Register(User model, string password)
        {
            if (model.Role != Roles.Student && model.Role != Roles.Tutor)
                return Result<User>.Fail("Only Student or Tutor accounts can be registered.");

            var result = Create(model, password);
            if (!result.HasError && result.Data != null)
                logRepo.AddFor(result.Data, "Register", $"Role: {result.Data.Role}");
            return result;
        }

        // Admin only: trainers cannot self-register.
        public Result<User> CreateTrainer(User model, string password)
        {
            model.Role = Roles.Trainer;
            var result = Create(model, password);
            if (!result.HasError && result.Data != null)
                logRepo.Add("Create Trainer", "User", result.Data.Id, result.Data.Email);
            return result;
        }

        public Result<User> SetStatus(int id, bool status)
        {
            try
            {
                var user = context.Users.Find(id);
                if (user == null)
                    return Result<User>.Fail($"User with ID {id} was not found.");
                if (user.Role == Roles.Admin)
                    return Result<User>.Fail("Admin accounts cannot be deactivated.");

                user.Status = status;
                context.SaveChanges();
                logRepo.Add(status ? "Activate User" : "Deactivate User", "User", user.Id, user.Email);
                return Result<User>.Ok(user);
            }
            catch (Exception e)
            {
                return Result<User>.Fail(e.Message);
            }
        }

        private Result<User> Create(User model, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                    return Result<User>.Fail("Password must be at least 6 characters.");
                if (context.Users.Any(e => e.Email.ToLower() == model.Email.ToLower()))
                    return Result<User>.Fail("Email already exists.");

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Gender = model.Gender,
                    ContactNumber = model.ContactNumber,
                    Dob = model.Dob,
                    Address = model.Address,
                    Role = model.Role,
                    Status = true,
                    CreatedAt = DateTime.Now
                };
                user.PasswordHash = hasher.HashPassword(user, password);

                context.Users.Add(user);
                context.SaveChanges();
                return Result<User>.Ok(user);
            }
            catch (Exception e)
            {
                return Result<User>.Fail(e.Message);
            }
        }
    }
}
