using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRworks.Models
{
    public class userroles
    {
        public string rolename { get; set; }
        public bool hasrole { get; set; }
    }
    public class RoleEdit
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public List<userroles> userroles { get; set; }
    }
}