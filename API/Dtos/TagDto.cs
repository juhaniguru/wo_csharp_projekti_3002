
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class TagDto
    {
        public int Id { get; set; }
        public required string TagText { get; set; }
    }
}