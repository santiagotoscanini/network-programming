using System.Collections.Generic;

namespace Domain
{
    public class Image
    {
        public string Name { get; set; }
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
    }
}
