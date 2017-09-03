using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManBox.Model.ViewModels
{
    public class BlogFeed
    {
        public List<BlogPost> Posts { get; set; }
    }

    public class BlogPost
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Excerpt { get; set; }
    }
}
