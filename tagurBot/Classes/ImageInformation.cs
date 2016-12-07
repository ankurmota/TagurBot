using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tagurBot
{
    public class ImageInformation
    {
        public Guid Id { get; set; }
        public string Caption { get; set; }

        public List<string> Tags { get; set; }
    }
}