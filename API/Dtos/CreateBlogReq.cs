
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class CreateBlogReq
    {
        public required string Title { get; set; }

        public required string Content { get; set; }
    }
}