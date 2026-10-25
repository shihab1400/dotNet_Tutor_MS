using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class LogRepo(AsDbContext context, CurrentUserHelper currentUser)
    {
        public const int PageSize = 25;

        // Never throws: a failed log write must not break the action being logged.
        public void Add(string action, string? entity = null, int? entityId = null, string? details = null)
        {
            try
            {
                context.Logs.Add(new Log
                {
                    UserId = currentUser.IsAuthenticated ? currentUser.UserId : null,
                    UserName = currentUser.UserName,
                    Role = currentUser.Role,
                    Action = action,
                    Entity = entity,
                    EntityId = entityId,
                    Details = details?.Length > 1000 ? details[..1000] : details,
                    IpAddress = currentUser.IpAddress,
                    CreatedAt = DateTime.Now
                });
                context.SaveChanges();
            }
            catch
            {
            }
        }

        // Used when the actor is not yet in the cookie (login / registration).
        public void AddFor(User? user, string action, string? details = null)
        {
            try
            {
                context.Logs.Add(new Log
                {
                    UserId = user?.Id,
                    UserName = user?.UserName ?? "-",
                    Role = user?.Role ?? "Guest",
                    Action = action,
                    Entity = "User",
                    EntityId = user?.Id,
                    Details = details,
                    IpAddress = currentUser.IpAddress,
                    CreatedAt = DateTime.Now
                });
                context.SaveChanges();
            }
            catch
            {
            }
        }

        public Result<List<Log>> Search(string? search, string? role, DateTime? from, DateTime? to, int page, out int totalPages)
        {
            totalPages = 1;
            try
            {
                var query = context.Logs.AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(e => e.UserName.Contains(search) || e.Action.Contains(search)
                                             || (e.Entity != null && e.Entity.Contains(search))
                                             || (e.Details != null && e.Details.Contains(search)));
                if (!string.IsNullOrWhiteSpace(role))
                    query = query.Where(e => e.Role == role);
                if (from != null)
                    query = query.Where(e => e.CreatedAt >= from);
                if (to != null)
                    query = query.Where(e => e.CreatedAt < to.Value.AddDays(1));

                var total = query.Count();
                totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
                page = Math.Clamp(page, 1, totalPages);

                return Result<List<Log>>.Ok(query.OrderByDescending(e => e.Id).Skip((page - 1) * PageSize).Take(PageSize).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Log>>.Fail(e.Message);
            }
        }
    }
}
