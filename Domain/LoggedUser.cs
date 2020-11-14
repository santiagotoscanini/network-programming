using System;

namespace Domain
{
    public class LoggedUser
    {
        public string Email { get; set; }
        public DateTime ConnectionDate { get; set; } = DateTime.Now;
    }
}