using Microsoft.AspNetCore.Http;

namespace ProjectShihab.Shared
{
    public class CurrentUserHelper(IHttpContextAccessor accessor)
    {
        public bool IsAuthenticated => accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public int UserId
        {
            get
            {
                var id = accessor.HttpContext?.User?.FindFirst("UserId")?.Value;
                return int.TryParse(id, out var value) ? value : -1;
            }
        }

        public string Email => accessor.HttpContext?.User?.FindFirst("Email")?.Value ?? "-";

        public string UserName => accessor.HttpContext?.User?.Identity?.Name ?? "-";

        public string Role => accessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Guest";

        public string? IpAddress => accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
