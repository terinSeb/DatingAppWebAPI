using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingAppCore.Extensions
{
    public static class ClaimPrincipalExtention
    {
        public static string GetUsername(this ClaimsPrincipal User)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
