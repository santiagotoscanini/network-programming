using System.Collections.Generic;

namespace Domain
{
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Image> Images { get; set; } = new List<Image>();
        
        public override bool Equals(object obj)
        {
            var result = false;

            if (obj is User user)
            {
                result = this.Email == user.Email;
            }

            return result;
        }
    }
}
