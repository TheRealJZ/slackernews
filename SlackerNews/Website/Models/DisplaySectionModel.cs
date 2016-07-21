using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class DisplaySectionModel
    {
        public section Section { get; set; }
        public List<article> Articles { get; set; }
    }
}