using DSharpPlus.Commands.ContextChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Attributes
{
    public class RequireRoleAttribute: ContextCheckAttribute
    {
        public ulong RequiredRoleId;
        public RequireRoleAttribute(ulong roleId) => RequiredRoleId = roleId;
    }
}
