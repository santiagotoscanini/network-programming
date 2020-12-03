﻿using Domain;
using System.ComponentModel.DataAnnotations;

namespace AdministratorServer.Models
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