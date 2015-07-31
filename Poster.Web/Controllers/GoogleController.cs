using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Poster.Data;
using Poster.Data.Models;
using System.Data;
using ServiceStack.OrmLite;
using System.Configuration;
namespace Poster.Web.Controllers
{
    public class GoogleController : Controller
    {
        //
        // GET: /Google/
        private string GoogleClientID = string.Empty;
        private string ClientSecret = string.Empty;

        public ActionResult Index()
        {
            GoogleClientID = ConfigurationManager.AppSettings["ClientId"].ToString();
            ClientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();
            if (Request.QueryString["code"] != null)
            {
                GetToken();
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                SetGooglePagePermissions();
            }
            return View();
        }

        private void SetGooglePagePermissions()
        {
#if DEBUG
            string url = "https://accounts.google.com/o/oauth2/auth?request_visible_actions=http://schemas.google.com/AddActivity&access_type=offline&response_type=code&client_id=" + GoogleClientID + "&redirect_uri=http://localhost:20659/Google/Index/&scope=https://www.googleapis.com/auth/plus.login";
#else
            string url = "https://accounts.google.com/o/oauth2/auth?request_visible_actions=http://schemas.google.com/AddActivity&access_type=offline&response_type=code&client_id=" + GoogleClientID + "&redirect_uri=http://188.42.227.39/Poster/Google/Index&scope=https://www.googleapis.com/auth/plus.login";
#endif


            Response.Redirect(url);

        }
        private void GetToken()
        {
            string code = Request.QueryString["code"];
            var request = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");
            //
#if DEBUG
            var postData = "code=" + code + "&redirect_uri=http://localhost:20659/Google/Index/&scope=https://www.googleapis.com/auth/plus.login&grant_type=authorization_code&client_id=" + GoogleClientID + "&client_secret=" + ClientSecret;
#else
            var postData = "code=" + code + "&redirect_uri=http://188.42.227.39/Poster/Google/Index&scope=https://www.googleapis.com/auth/plus.login&grant_type=authorization_code&client_id=" + GoogleClientID + "&client_secret="+ClientSecret;
#endif
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var obj = (dynamic)jss.DeserializeObject(responseString);
            SaveGoogleUserInfo(obj);
        }
        private void SaveGoogleUserInfo(dynamic obj)
        {
            try
            {
                if (obj["access_token"] != null)
                {
                    var userInfo = GetUserAdditionalInfo(obj["access_token"]);
                    Profile model = new Profile();
                    model.AccessToken = obj["access_token"];
                    model.RefreshToken = obj["refresh_token"];
                    model.ProfileTypeID = 2;
                    model.UserId = userInfo["id"];
                    model.Username = userInfo["name"];
                    ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
                    using (IDbConnection db = dbFactory.Open())
                    {
                        //Check user Exits
                        var alreadyExist = db.Select<Profile>().Where(u => u.UserId == model.UserId).FirstOrDefault();
                        if (alreadyExist != null)
                        {
                            return;
                        }
                        db.Insert<Profile>(model);

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        private dynamic GetUserAdditionalInfo(string accessToken)
        {
            string url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + accessToken;
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "Get";
            var webResponse = webRequest.GetResponse();
            StreamReader sr = null;

            sr = new StreamReader(webResponse.GetResponseStream());
            var vdata = new Dictionary<string, string>();
            string responseString = sr.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var obj = (dynamic)jss.DeserializeObject(responseString);
            return obj;


        }


    }
}
