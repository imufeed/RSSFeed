using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.ServiceModel.Syndication;
using RSSFeed.Models;
using Newtonsoft.Json;

// Example RSS pages.
//http://www.nytimes.com/services/xml/rss/nyt/International.xml
//https://blogs.msdn.microsoft.com/martinkearn/feed/
//http://www.aljazeera.com/xml/rss/all.xml

namespace RSSFeed.Controllers
{
    public class ReadRSSController : Controller
    {
        private RssDBContext db = new RssDBContext();

        /// <summary>
        /// Read all RSS feed from a valid link and display it in HTML view.
        /// </summary>
        /// <param name="link">link to RSS feed</param>
        /// <returns>HTML view of RSS feed from the link</returns>
        // GET: ReadRSS
        public ActionResult Index(string link)
        {
            //List of RSS
            var rssList = new List<Rss>();

            if (!String.IsNullOrEmpty(link))
            {                
                try
                {
                    var reader = XmlReader.Create(link);
                    var feed = SyndicationFeed.Load(reader);
                    ViewBag.FeedTitle = feed.Title.Text;
                    ViewBag.Link = link;

                    foreach (var item in feed.Items)
                    {
                        var rss = new Rss();
                        rss.Title = item.Title.Text;
                        rss.Link = item.Links[0].Uri.ToString();
                        rss.PublishDate = item.PublishDate.DateTime;
                        rss.Description = item.Summary.Text;
                        rss.RssTitle = feed.Title.Text;
                        rssList.Add(rss);
                    }
                }
                catch
                {
                    TempData["EXCEPTION"] = "You provided invalid link for RSS feed ";
                    return RedirectToAction("Index");
                }
            }
            return View(rssList);          
        }
        
        /// <summary>
        /// Store all the RSS from the link into the DB if they are not already stored.
        /// They are checked aginst RSS channel title and item title.
        /// </summary>
        /// <param name="link"></param>
        /// <returns>back to HTML view page of index page</returns>
        // GET: ReadRSS/Store
        public ActionResult Store(string link)
        {
            int success = 0;
            int fail = 0;
            if (!String.IsNullOrEmpty(link))
            {
                var reader = XmlReader.Create(link);
                var feed = SyndicationFeed.Load(reader);

                ViewBag.FeedTitle = feed.Title.Text;                

                foreach (var item in feed.Items)
                {
                    var rss = new Rss();
                    rss.Title = item.Title.Text;
                    rss.Link = item.Links[0].Uri.ToString();
                    rss.PublishDate = item.PublishDate.DateTime;
                    rss.Description = item.Summary.Text;
                    rss.RssTitle = feed.Title.Text;

                    // Make sure there is no RSS from the same channel and same Title in the DB. We can also add PublishDate.
                    var rsses =
                        from m in db.Rsses
                        where m.RssTitle == rss.RssTitle
                        where m.Title == rss.Title
                        select m;
                                       
                    if (ModelState.IsValid && rsses.Count() == 0)
                    {                        
                        db.Rsses.Add(rss);                        
                        success += 1;
                    }
                    else
                    {
                        fail += 1;
                    }
                }                
            }
            db.SaveChanges();
            TempData["SUCCESS"] = success;
            TempData["FAIL"] = fail;
            return RedirectToAction("Index", "Rsses", null);
        }

        /// <summary>
        /// Retrieve all the RSS containing the searchString in their title or description.
        /// </summary>
        /// <param name="searchString">the string to search against in titles and description in the DB.</param>
        /// <returns>Download of a text file contains the RSS in JSON</returns>        
        public FileResult DownloadJSON(string searchString)
        {
            var rsses = from m in db.Rsses select new { m.Title, m.Description, m.Link, m.PublishDate };

            if (!String.IsNullOrEmpty(searchString))
            {
                rsses = rsses.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));                
            }

            string json = JsonConvert.SerializeObject(rsses);
            byte[] toBytes = System.Text.Encoding.ASCII.GetBytes(json);
            return File(toBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "rss.txt");
        }

        /// <summary>
        /// Retrieve all the RSS containing the searchString in their title or description.
        /// </summary>
        /// <param name="searchString">the string to search against in titles and description in the DB.</param>
        /// <returns>Download of a text file contains the RSS in JSON</returns>        
        // GET: ReadRSS/DisplayJSON
        public ActionResult DisplayJSON(string searchString)
        {
            var rsses = from m in db.Rsses select new { m.Title, m.Description, m.Link, m.PublishDate };

            if (!String.IsNullOrEmpty(searchString))
            {
                rsses = rsses.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));                
            }

            string json = JsonConvert.SerializeObject(rsses);                      

            ViewBag.JSON = json;
            return View();
        }

        /// <summary>
        /// Delete all the RSS that match the searchString, or all RSS if no searchString is selected.
        /// </summary>
        /// <param name="searchString">the string to search against in titles and description in the DB.</param>
        /// <returns>Redirect back to search HTML index page.</returns>
        public ActionResult DeleteMany(string searchString)
        {
            var counter = 0;
            var rsses = from m in db.Rsses select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                rsses = rsses.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));               
            }

            foreach(var item in rsses)
            {
                Rss rss = db.Rsses.Find(item.ID);
                db.Rsses.Remove(rss);
                counter += 1;

            }

            TempData["DELETED"] = counter;
            db.SaveChanges();
            return RedirectToAction("Index", "Rsses", null);
        }

    }
}