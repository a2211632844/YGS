using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGS
{
    public class Page
    {
        public string accesstoken { get; set; }
        public string apicode { get; set; }
        public Test data { get; set; }
    }
    public class Test
        {
            public string pageindex { get; set; }
            public string pagesize { get; set; }
        }
}
