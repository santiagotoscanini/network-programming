using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Models
{
    public class UserModel
    {

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public User ToEntity()
        {
            return new User
            {
                Email = this.Email,
                Password = this.Password,
            };
        }
    }
}
