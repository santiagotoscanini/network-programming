using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminServer.Models
{
    public class UserUpdateModel
    {
        [Required]
        public string Passwword { get; set; }

        public User ToEntity(string email)
        {
            return new User
            {
                Email = email,
                Password = this.Passwword,
            };
        }
    }
}
