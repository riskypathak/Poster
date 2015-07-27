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
                    PageId = Request.QueryString["pageid"];
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

                    return RedirectToAction("Begin", "Twitter");
                }
                else if (Request.Form["ProfileTypeId"] == "2")
                {
                    return RedirectToAction("Index", "Google");
                }
                Session["ProfileTypeId"] = Request.Form["ProfileTypeId"];
                PageId = Request.Form["PageId"];
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

        private void Authorization()
        {
            //     Your Website Url which needs to Redirected
            string callbackUrl = "http://localhost:20659/Profile/Index";


            string FacebookClientId = "1643946982507030";
            var s = string.Format("https://graph.facebook.com/oauth/" +
               "authorize?client_id={0}&redirect_uri={1}&scope=offline_access," +
               "publish_stream,read_stream,publish_actions,manage_pages",
               FacebookClientId, callbackUrl);
        }


        public void GetShortLiveURl()
        {
            string appid = "1643946982507030";
            string appSceret = "92852f8f8b352ffe6486717c32211d7a";
            string url = "https://graph.facebook.com/oauth/access_token?client_id=" + appid + "&client_secret=" + appSceret + "&code=<CODE>";
        }

        #region NEED TO REMOVE THIS CODE AFTER FINAL TESTING
        public void getpageTokens()
        {
            // User Access Token  we got After authorization
            string UserAccesstoken = "CAACEdEose0cBAPrRVrAn6ASZCB0RAgn5IrM3kQEvb7OfRVYLVVpq0fpnuWM1vR4aaDzOXygZAV6ry5QnKN3Rzlp6j2mGiaINZBuEWSeZBmqdHytlif4CNzwwn9MZA5VcABHSRL2HucyL8zyRdYLWFrgFNPz6XTKyBGQQkHW5QXqRSiFWzqM5IA4SArvR6Cdm5Sw2mVmJmyYwfWt4NlnwaOtGZAoqrjNL4ZD";
            string url = string.Format("https://graph.facebook.com/" + "me/accounts?access_token={0}", UserAccesstoken);
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "Get";
            var webResponse = webRequest.GetResponse();
            StreamReader sr = null;

            sr = new StreamReader(webResponse.GetResponseStream());
            var vdata = new Dictionary<string, string>();
            string returnvalue = sr.ReadToEnd();

            //using Jobject to parse result
            JObject mydata = JObject.Parse(returnvalue);

            JArray data = (JArray)mydata["data"];
            dynamic filterData = new ExpandoObject();
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].ToList()[4].Last.ToString() == "738024112987033")
                {
                    FacebookPost(data[i]);
                }
            }

        }

        public void FacebookPost(dynamic obj)
        {
            string mainaccess = "CAACEdEose0cBAIZC4VCx5bJcdrRdT7DsykmGxFdWgf9UHGImssZARWIHVT5XoCNQfxZAVjK6KGMmIQfOdPzUMnZBt8if5SmWNl1tItJsMZCARwc255OsJ6omk8AdiuQQ1s2ZCJmwmSg4Bt5E0DWf8QtY0Uzkl6MLtoGakKfsVOpjLYymh788ohJyNweZCBmRZBOeMJDUoAUmsX2WkWZAhaPWXv9zz4O94ut4ZD";
            string name = obj.name;
            string Accesstoken = obj.access_token;
            string category = obj.category;
            string id = obj.id;
            dynamic messagePost = new ExpandoObject();
            messagePost.access_token = Accesstoken;
            messagePost.picture = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQlKBchID7G2ISdIL4n-L2mmp8TuGZ_7CX26pr8usH2jUGa_0sgQTTomhY";
            messagePost.message = "Facebook Client";
            FacebookClient app = new FacebookClient(mainaccess);
            try
            {
                var result = app.Post("/" + id + "/feed", messagePost);
            }
            catch (FacebookOAuthException ex)
            {
                //handle something
            }
            catch (FacebookApiException ex)
            {
                //handle something else
            }
        }
        #endregion

        private void SetPagePermissions()
        {

            string url = "https://www.facebook.com/dialog/oauth?client_id=1643946982507030&redirect_uri=http://localhost:20659/Profile/Index?pageid=" + PageId + "&profiletype=1&response_type=token&scope=manage_pages,publish_pages,publish_actions";
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
                    db.Insert<Config>(model);
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
            returnvalue = returnvalue.Split('&')[0].Replace("access_token=","");
            return returnvalue;
        }
    }
}