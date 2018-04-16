using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmandaFE.Models
{
    public class UserViewModel
    {
        public List<User> users;
        public SelectList name;
    }
}
