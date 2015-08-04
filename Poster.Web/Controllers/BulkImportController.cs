using Poster.Data.Models;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Poster.Web.Controllers
{
    public class BulkImportController : Controller
    {
        //
        // GET: /BulkImport/
        [HttpGet]
        public ActionResult Index(string msg)
        {
            ViewBag.Message = msg;
            return View();
        }

        [HttpPost]
        public ActionResult ImagePost()
        {
            int groupid = CheckAndCreateGroup(Request.Form["imggroupname"], "img");
            if (groupid == 0)
            {
                return RedirectToAction("Index", new { msg = "Exists" });
            }

            string subPath = "/UploadImage"; // your code goes here
            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
            string GroupName = Request.Form["imggroupname"];

            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            string directory = subPath + "/" + GroupName + "_" + DateTime.Now.ToString("yyyyMMddhhmm");
            System.IO.Directory.CreateDirectory(Server.MapPath(directory));
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase text = Request.Files[i];
                if (text != null && text.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(text.FileName);
                    text.SaveAs(Path.Combine(Server.MapPath(directory), fileName));
                    SaveImagesPathInDB(directory, groupid);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult TextPost()
        {
            int groupid = CheckAndCreateGroup(Request.Form["textgroupname"], "txt");
            if (groupid == 0)
            {
                return RedirectToAction("Index", new { msg = "Exists" });
            }
            string subPath = "/UploadText"; // your code goes here
            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
            string GroupName = Request.Form["textgroupname"];

            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            string directory = subPath + "/" + GroupName + "_" + DateTime.Now.ToString("yyyyMMddhhmm");
            System.IO.Directory.CreateDirectory(Server.MapPath(directory));
            HttpPostedFileBase text = Request.Files["textupload"];
            if (text != null && text.ContentLength > 0)
            {
                var fileName = Path.GetFileName(text.FileName);
                text.SaveAs(Path.Combine(Server.MapPath(directory), fileName));
                SaveTextInDB(directory, groupid);
            }
            return RedirectToAction("Index");
        }

        private void SaveTextInDB(string directory, int GroupId)
        {
            var model = new PostText();
            model.GroupId = GroupId;
            model.Text = directory;
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                db.Save<PostText>(model);
            }
        }

        private void SaveImagesPathInDB(string directory, int GroupId)
        {
            var model = new PostImage();
            model.GroupId = GroupId;
            model.Imagelink = directory;
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                db.Save<PostImage>(model);
            }
        }

        private int CheckAndCreateGroup(string GroupName, string Type)
        {
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                var isExist = db.Where<Group>("name", GroupName);
                if (isExist.Count > 0)
                {
                    return 0;
                }
                else
                {
                    var model = new Group();
                    model.Name = GroupName;
                    model.Type = Type;
                    db.Save(model);
                    return model.Id;
                }
            }
        }
    }
}
