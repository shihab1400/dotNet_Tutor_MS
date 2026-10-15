using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Shared
{
    public class CurrentUserHelper(IHttpContextAccessor accessor)
    {
        public bool IsAuthenticated 
        { 
            get 
            {
                try
                {
                    return (bool)accessor.HttpContext?.User?.Identity?.IsAuthenticated;
                }
                catch (Exception ex)
                {
                    return false;
                }
            } 
        }
        public int UserId 
        { 
            get 
            {
                try
                {
                    var id = accessor.HttpContext?.User?.FindFirst("UserId").Value;
                    return id != null ? int.Parse(id) : -1;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            } 
        }
        public string Email 
        { 
            get 
            {
                try
                {
                    var email = accessor.HttpContext?.User?.FindFirst("Email").Value;
                    return email != null ? email : "-";
                }
                catch (Exception ex)
                {
                    return "-";
                }
            } 
        }
        
    }
}
