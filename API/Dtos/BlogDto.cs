
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class BlogDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }

        public required UserDto Owner { get; set; }

        public List<TagDto> Tags { get; set; } = [];
    }
}