using LinqToTwitter;
using Poster.Data.Models;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Poster.Web.Controllers
{
    public class TwitterController : AsyncController
    {
        private int ServiceInterval = 10;
        // GET: Twitter
        public ActionResult Index()
        {

            return View();
        }

        public async Task<ActionResult> BeginAsync(int? serviceInterval)
        {
            //  ServiceInterval = serviceInterval??10;

            //var auth = new MvcSignInAuthorizer
            var auth = new MvcAuthorizer
            {

                CredentialStore = new SessionStateCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"].ToString(),// "0vbuhbtd8Zz7M121MtxtrA",
                    ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"].ToString()// "5aj5te5ygcpPCBOMrwvGcjI8GAoAfAZFMlpLhyt2U"
                }
            };
            string twitterCallbackUrl = Request.Url.ToString().Replace("Begin", "Complete");
            return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));
        }

        public async Task<ActionResult> CompleteAsync()
        {
            if (Request.QueryString["serviceInterval"] != null)
            {
                ServiceInterval = Convert.ToInt32(Request.QueryString["serviceInterval"]);
            }
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"].ToString(),
                    ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"].ToString()
                }
            };

            await auth.CompleteAuthorizeAsync(Request.Url);
            //
            Profile model = new Data.Models.Profile();
            var credentials = auth.CredentialStore;
            model.AccessToken = credentials.OAuthToken;
            model.TokenSecret = credentials.OAuthTokenSecret;
            model.Username = credentials.ScreenName;
            model.UserId = credentials.UserID.ToString();
            model.ProfileTypeID = 3;
            model.Interval = ServiceInterval;
            SaveTwitterInfo(model);
            return RedirectToAction("Index", "Profile");
        }

        private void SaveTwitterInfo(Profile model)
        {
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

        //private static async void PosterTwitterFromMVC()
        //{
        //    IAuthorizer auth = new PinAuthorizer()
        //    {
        //        CredentialStore = new InMemoryCredentialStore
        //        {
        //            ConsumerKey = "0vbuhbtd8Zz7M121MtxtrA",
        //            ConsumerSecret = "5aj5te5ygcpPCBOMrwvGcjI8GAoAfAZFMlpLhyt2U",
        //            OAuthToken = "164334839-o5Dq8UKqZCLqooAHBOeuGDKgcx9ExTfO2FMvMxti",
        //            OAuthTokenSecret = "9vy702RGpq4sGPJEGAuQy0tsVOIsN3gu53MImpyPizI1f"
        //        }
        //    };

        //    var ctx = new TwitterContext(auth);

        //    //await ctx.TweetAsync("Testing from Application. MV Tweet!");

        //    await ctx.TweetWithMediaAsync("Test", false, ReadImageFile(Path.GetFullPath("image.png")));
        //}

        //public static byte[] ReadImageFile(string imageLocation)
        //{
        //    byte[] imageData = null;
        //    FileInfo fileInfo = new FileInfo(imageLocation);
        //    long imageFileLength = fileInfo.Length;
        //    FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
        //    BinaryReader br = new BinaryReader(fs);
        //    imageData = br.ReadBytes((int)imageFileLength);
        //    return imageData;
        //}
    }
}