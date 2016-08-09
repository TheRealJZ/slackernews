using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class HomePageModel
    {
        public List<article> TopArticles { get; set; }
        public List<DisplaySectionModel> Data { get; set; }
    }
}