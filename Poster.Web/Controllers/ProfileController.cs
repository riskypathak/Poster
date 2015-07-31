using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Poster.Data;
using System.Data.Entity.Infrastructure;
using System.Configuration;
using Poster.Data.Models;
using System.Data;
using ServiceStack.OrmLite;
using System.Web.Script.Serialization;
using ServiceStack.Data;
using ServiceStack.Host;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Dynamic;
using Facebook;
namespace Poster.Web.Controllers
{
    public class ProfileController : Controller
    {
        private string FacebookClientID = string.Empty;
        private string AppSecret = string.Empty;
        private string AccessToken = string.Empty;
        private string PageId = string.Empty;
        private int ProfileTypeId = 0;
        private int ServiceInterval = 10;
        //
        // GET: /Profile/
        [Authorize]
        public ActionResult Index(string access_token)
        {
            //getpageTokens();

            FacebookClientID = ConfigurationManager.AppSettings["FacebookClientID"].ToString();
            AppSecret = ConfigurationManager.AppSettings["AppSecret"].ToString();
            try
            {
                if (Request.QueryString["access_token"] != null)
                {
                    AccessToken = Request.QueryString["access_token"];
                    var querystingValues = Request.QueryString["pageid"];
                    PageId = querystingValues.ToString().Split('|')[0];
                    ServiceInterval = Convert.ToInt32(querystingValues.ToString().Split('|')[1]);
                    ProfileTypeId = 1;
                    AccessToken = GetLongLivedToken(AccessToken);
                    Save();
                }

                Profile model = new Profile();
                ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                using (IDbConnection db = dbFactory.Open())
                {
                    var items = db.Select<ProfileType>().Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                    ViewBag.ProfileTypeList = items;

                    var result = db.Select<Profile>().Select(x => new Profile
                    {
                        Id = x.Id,
                        Username = x.Username,
                        ProfileName = db.LoadSelect<ProfileType>().Where(u => u.Id == x.ProfileTypeID).FirstOrDefault()
                    }).ToList();
                    ViewBag.userProfileList = result;
                    //var serviceInterval = db.Select<Config>();
                    //if (serviceInterval.Count > 0)
                    //{
                    //    ViewBag.ServiceInterval = serviceInterval.FirstOrDefault().ServiceInterval;
                    //}
                    //else
                    //{
                    //    ViewBag.ServiceInterval = 10;
                    //}
                    // model.Interval = 10;

                }

                return View(model);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [Authorize]
        [HttpPost]
        public ActionResult Index(FormCollection frm)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add()
        {
            try
            {
                if (Request.Form["ProfileTypeId"] == "3")
                {

                    return RedirectToAction("Begin", "Twitter", new { serviceInterval = Convert.ToInt32(Request.Form["Interval"]) });
                }
                else if (Request.Form["ProfileTypeId"] == "2")
                {
                    return RedirectToAction("Index", "Google");
                }
                Session["ProfileTypeId"] = Request.Form["ProfileTypeId"];
                PageId = Request.Form["PageId"];
                ServiceInterval = Convert.ToInt32(Request.Form["Interval"]);
                SetPagePermissions();
                return Json("Success");

            }
            catch (Exception ex)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return Json("Fail");
                // throw ex;
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
                    db.DeleteById<Profile>(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region NEED TO REMOVE THIS CODE AFTER FINAL TESTING
        //public void getpageTokens()
        //{
        //    // User Access Token  we got After authorization
        //    string UserAccesstoken = "CAACEdEose0cBAPrRVrAn6ASZCB0RAgn5IrM3kQEvb7OfRVYLVVpq0fpnuWM1vR4aaDzOXygZAV6ry5QnKN3Rzlp6j2mGiaINZBuEWSeZBmqdHytlif4CNzwwn9MZA5VcABHSRL2HucyL8zyRdYLWFrgFNPz6XTKyBGQQkHW5QXqRSiFWzqM5IA4SArvR6Cdm5Sw2mVmJmyYwfWt4NlnwaOtGZAoqrjNL4ZD";
        //    string url = string.Format("https://graph.facebook.com/" + "me/accounts?access_token={0}", UserAccesstoken);
        //    WebRequest webRequest = WebRequest.Create(url);
        //    webRequest.ContentType = "application/x-www-form-urlencoded";
        //    webRequest.Method = "Get";
        //    var webResponse = webRequest.GetResponse();
        //    StreamReader sr = null;

        //    sr = new StreamReader(webResponse.GetResponseStream());
        //    var vdata = new Dictionary<string, string>();
        //    string returnvalue = sr.ReadToEnd();

        //    //using Jobject to parse result
        //    JObject mydata = JObject.Parse(returnvalue);

        //    JArray data = (JArray)mydata["data"];
        //    dynamic filterData = new ExpandoObject();
        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        if (data[i].ToList()[4].Last.ToString() == "738024112987033")
        //        {
        //            FacebookPost(data[i]);
        //        }
        //    }

        //}

        //public void FacebookPost(dynamic obj)
        //{
        //    string mainaccess = "CAACEdEose0cBAIZC4VCx5bJcdrRdT7DsykmGxFdWgf9UHGImssZARWIHVT5XoCNQfxZAVjK6KGMmIQfOdPzUMnZBt8if5SmWNl1tItJsMZCARwc255OsJ6omk8AdiuQQ1s2ZCJmwmSg4Bt5E0DWf8QtY0Uzkl6MLtoGakKfsVOpjLYymh788ohJyNweZCBmRZBOeMJDUoAUmsX2WkWZAhaPWXv9zz4O94ut4ZD";
        //    string name = obj.name;
        //    string Accesstoken = obj.access_token;
        //    string category = obj.category;
        //    string id = obj.id;
        //    dynamic messagePost = new ExpandoObject();
        //    messagePost.access_token = Accesstoken;
        //    messagePost.picture = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQlKBchID7G2ISdIL4n-L2mmp8TuGZ_7CX26pr8usH2jUGa_0sgQTTomhY";
        //    messagePost.message = "Facebook Client";
        //    FacebookClient app = new FacebookClient(mainaccess);
        //    try
        //    {
        //        var result = app.Post("/" + id + "/feed", messagePost);
        //    }
        //    catch (FacebookOAuthException ex)
        //    {
        //        //handle something
        //    }
        //    catch (FacebookApiException ex)
        //    {
        //        //handle something else
        //    }
        //}
        #endregion

        private void SetPagePermissions()
        {
#if DEBUG
            string url = "https://www.facebook.com/dialog/oauth?client_id=1643946982507030&redirect_uri=http://localhost:20659/Profile/Index?pageid=" + PageId + "|" + ServiceInterval + "&profiletype=1&response_type=token&scope=manage_pages,publish_pages,publish_actions";
#else
            string url = "https://www.facebook.com/dialog/oauth?client_id=1643946982507030&redirect_uri=http://188.42.227.39/Poster/Profile/Index?pageid=" + PageId + "&profiletype=1&response_type=token&scope=manage_pages,publish_pages,publish_actions";
#endif
            Response.Redirect(url);


        }

        public void Save()
        {
            try
            {

                string userInfo = getUserDetail();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var obj = (IDictionary<string, object>)jss.DeserializeObject(userInfo);
                Profile model = new Data.Models.Profile();
                model.AccessToken = AccessToken;
                model.UserId = obj["id"].ToString();
                model.Username = obj["name"].ToString();
                model.ProfileTypeID = ProfileTypeId;
                model.PageId = PageId;// "738024112987033";
                model.Interval = ServiceInterval;
                ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                using (IDbConnection db = dbFactory.Open())
                {
                    //Check user Exits
                    var alreadyExist = db.Select<Profile>().Where(u => u.UserId == model.UserId).FirstOrDefault();
                    if (alreadyExist != null)
                    {
                        Response.Redirect("index");
                    }
                    db.Insert<Profile>(model);

                }
                Response.Redirect("index");
            }
            catch (Exception ex)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Response.Redirect("index");
                // throw ex;
            }

        }

        public string getUserDetail()
        {
            // User Access Token  we got After authorization
            string UserAccesstoken = AccessToken;
            string url = string.Format("https://graph.facebook.com/" + "me?access_token={0}", UserAccesstoken);
            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "Get";
            var webResponse = webRequest.GetResponse();
            StreamReader sr = null;
            sr = new StreamReader(webResponse.GetResponseStream());
            var vdata = new Dictionary<string, string>();
            string returnvalue = sr.ReadToEnd();
            return returnvalue;

        }

        //SAVE SERVICE INTERVAL TIME
        [HttpPost]
        public ActionResult SaveInterval(string inertvaltime)
        {
            try
            {
                Config model = new Config();
                model.ServiceInterval = Convert.ToInt32(inertvaltime);
                ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                using (IDbConnection db = dbFactory.Open())
                {
                    var serviceInterval = db.Select<Config>().ToList();
                    if (serviceInterval.Count > 0)
                    {
                        var intervalmodel = serviceInterval.FirstOrDefault();
                        intervalmodel.ServiceInterval = Convert.ToInt32(inertvaltime);
                        db.Update<Config>(intervalmodel);
                    }
                    else
                    {
                        db.Insert<Config>(model);
                    }
                    return Json("Success");

                }

            }
            catch (Exception)
            {

                // throw;
                return Json("Fail");
            }
        }

        //GENERATE THE LONG LIVED ACCESS TOKEN FROM SHORT LIVED
        private string GetLongLivedToken(string Access_Token)
        {
            string url = "https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=" + FacebookClientID + "&client_secret=" + AppSecret + "&fb_exchange_token=" + Access_Token;
            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "Get";
            var webResponse = webRequest.GetResponse();
            StreamReader sr = null;
            sr = new StreamReader(webResponse.GetResponseStream());
            var vdata = new Dictionary<string, string>();
            string returnvalue = sr.ReadToEnd();
            returnvalue = returnvalue.Split('&')[0].Replace("access_token=", "");
            return returnvalue;
        }

        [Authorize]
        public ActionResult Post()
        {
            Post model = new Post();
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                ViewBag.PostList = db.Select<Post>().ToList();
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Post(HttpPostedFileBase PhotoPath, Post model)
        {
            if (PhotoPath != null && PhotoPath.ContentLength > 0)
                try
                {
                    string path = Path.Combine("/Images", DateTime.Now.ToString("ddMMyyyyhhmm") + Path.GetFileName(PhotoPath.FileName));
                    ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                    using (IDbConnection db = dbFactory.Open())
                    {
                        var existingRecord = db.Select<Post>().Where(x => x.Id == model.Id).FirstOrDefault();
                        if (existingRecord != null)
                        {
                            System.IO.File.Delete(Server.MapPath(existingRecord.PhotoPath)); //FIRST DELETE THE EXISTING PHOTO
                            SaveImage(PhotoPath, path);
                            model.PhotoPath = path;
                            UpdatePost(model);
                        }
                        else
                        {
                            SaveImage(PhotoPath, path);
                            model.PhotoPath = path;
                            db.Save<Post>(model);
                        }



                    }
                    return RedirectToAction("Post");
                }
                catch (Exception ex)
                {

                }
            else
            {
                UpdatePost(model);
                return RedirectToAction("Post");

            }
            return View(model);
        }

        private void SaveImage(HttpPostedFileBase PhotoPath, string path)
        {

            string fullpath = Server.MapPath(path);
            PhotoPath.SaveAs(fullpath);
        }

        private void UpdatePost(Post model)
        {
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                var existingRecord = db.Select<Post>().Where(x => x.Id == model.Id).FirstOrDefault();
                existingRecord.Text = model.Text;
                if (model.PhotoPath != null)
                    existingRecord.PhotoPath = model.PhotoPath;

                db.Update<Post>(existingRecord);

            }
        }

        [Authorize]
        public ActionResult DeletePost(int id = 0)
        {
            try
            {
                ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                using (IDbConnection db = dbFactory.Open())
                {
                    var post = db.Select<Post>();
                    if (post != null)
                    {
                        string photoPath = post.FirstOrDefault().PhotoPath;
                        System.IO.File.Delete(Server.MapPath(photoPath));
                        db.DeleteById<Post>(id);
                    }
                }
                return RedirectToAction("Post");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}