﻿using Domain;
using System.ComponentModel.DataAnnotations;

namespace AdministratorServer.Models
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