using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdministratorServer.Models
{
    public class UserUpdateModel
    {
        [Required]
        public string Passwword { get; set; }
    }
}
