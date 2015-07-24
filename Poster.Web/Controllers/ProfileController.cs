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
        private string AccessToken = string.Empty;
        //
        // GET: /Profile/
        [Authorize]
        public ActionResult Index(string access_token)
        {
           
            try
            {
                if (Request.QueryString["access_token"] != null)
                {
                    AccessToken = Request.QueryString["access_token"];
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
        //public void getpageTokens()
        //{
        //    // User Access Token  we got After authorization
        //    string UserAccesstoken = "CAAXXKTvsAhYBABO95ezCqSnKcAiHaZC3spJa8ljgZBSzxfCcyx7dpW1wZCTFN1110uIPUAlEAIDFeHtY5o9qORwTNJAy3tKMIPAR2ClQyL0QgiuWkYZC5xBvON74W7lhwy6EsyoIfKnoxtPdzGUwZAnis6IMJiPEMoVwECirsuKUO5IMeMTTVtx1eKtH8GTNrh0MPrwRZCmZA02l6uHSvLzx9ATq0VaC24ZD";
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
        //    FacebookPost(data);

        //}

        //public void FacebookPost(JArray obj)
        //{
        //    string mainaccess = "CAAXXKTvsAhYBABO95ezCqSnKcAiHaZC3spJa8ljgZBSzxfCcyx7dpW1wZCTFN1110uIPUAlEAIDFeHtY5o9qORwTNJAy3tKMIPAR2ClQyL0QgiuWkYZC5xBvON74W7lhwy6EsyoIfKnoxtPdzGUwZAnis6IMJiPEMoVwECirsuKUO5IMeMTTVtx1eKtH8GTNrh0MPrwRZCmZA02l6uHSvLzx9ATq0VaC24ZD";
        //    string name = (string)obj[0]["name"];
        //    string Accesstoken = (string)obj[0]["access_token"];
        //    string category = (string)obj[0]["category"];
        //    string id = (string)obj[0]["id"];
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
            string url = "https://www.facebook.com/dialog/oauth?client_id=1643946982507030&redirect_uri=http://localhost:20659/Profile/Index&response_type=token&scope=manage_pages,publish_pages,publish_actions";
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
                model.ProfileTypeID = 1;
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
    }
}