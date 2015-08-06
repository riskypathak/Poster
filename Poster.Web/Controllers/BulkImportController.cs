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
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                ViewBag.GroupList = db.Select<Group>().ToList();
            }
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

            string subPath = "../UploadImage"; // your code goes here
            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
            string GroupName = Request.Form["imggroupname"].Trim();

            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            string directory = subPath + "/" + GroupName;
            System.IO.Directory.CreateDirectory(Server.MapPath(directory));
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase text = Request.Files[i];
                if (text != null && text.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(text.FileName);
                    text.SaveAs(Path.Combine(Server.MapPath(directory), fileName));
                    SaveImagesPathInDB(Path.Combine(directory, fileName), groupid);
                }
            }
            return RedirectToAction("Index", new { msg = "Saved" });
        }

        [HttpPost]
        public ActionResult TextPost()
        {
            int groupid = CheckAndCreateGroup(Request.Form["textgroupname"], "txt");
            if (groupid == 0)
            {
                return RedirectToAction("Index", new { msg = "Exists" });
            }
            string subPath = "../UploadText"; // your code goes here
            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));
            string GroupName = Request.Form["textgroupname"].Trim();

            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            string directory = subPath + "/" + GroupName;
            System.IO.Directory.CreateDirectory(Server.MapPath(directory));
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase text = Request.Files[i];
                if (text != null && text.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(text.FileName);
                    text.SaveAs(Path.Combine(Server.MapPath(directory), fileName));
                    SaveTextInDB(Path.Combine(directory, fileName), groupid);
                }
            }
            //if (text != null && text.ContentLength > 0)
            //{
            //    var fileName = Path.GetFileName(text.FileName);
            //    text.SaveAs(Path.Combine(Server.MapPath(directory), fileName));
            //    SaveTextInDB(directory, groupid);
            //}
            return RedirectToAction("Index", new { msg = "Saved" });
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


        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            try
            {
                ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                using (IDbConnection db = dbFactory.Open())
                {
                    DeleteGroupFolder(db, id);
                    db.DeleteById<Group>(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteGroupFolder(IDbConnection db, int id)
        {
            var selectedGroup = db.Where<Group>("Id", id).FirstOrDefault();
            string groupName = selectedGroup.Name.Trim();
            string type = selectedGroup.Type;
            if (type == "img")
            {
                string groupPath = Server.MapPath("~/UploadImage/" + groupName);
                if (Directory.Exists(groupPath))
                    Directory.Delete(groupPath, true);
            }
            else if (type == "txt")
            {
                string groupPath = Server.MapPath("~/UploadText/" + groupName);
                if (Directory.Exists(groupPath))
                    Directory.Delete(groupPath, true);
            }


        }


    }
}
