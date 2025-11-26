
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public required string TagText { get; set; }

        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    }
}