using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace RSSFeed.Models
{
    public class Rss
    {
        public int ID { get; set; }       
        public string Title { get; set; }
        public string RssTitle { get; set; }
        public string Link { get; set; }        
        public string Description { get; set; }
        [Display(Name = "Publish Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PublishDate { get; set; }
    }

    public class RssDBContext : DbContext
    {
        public DbSet<Rss> Rsses { get; set; }
    }
}