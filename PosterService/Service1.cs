using Facebook;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Poster.Data;
using Poster.Data.Models;
using ServiceStack.OrmLite;
using System.Configuration;
using LinqToTwitter;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Plus.v1;
using Google.Apis.Services;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
namespace PosterService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timmer = null;
        //738024112987033
        private static string filename = string.Empty;
        private static string filetext = string.Empty;
        public Service1()
        {
            InitializeComponent();
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }
        public void Stop()
        {
            OnStop();
        }


        protected override void OnStart(string[] args)
        {
            int interval = 180;
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                var photoList = db.Select<Photo>().ToList();
                var rand = new Random();
                int number = rand.Next(photoList.Count);
                filename = photoList[number].PhotoPath;
                filetext = photoList[number].PhotoText;
                var configValue = db.Select<Config>().ToList();
                if (configValue.Count > 0)
                    interval = configValue.FirstOrDefault().ServiceInterval;
            }
            timmer = new Timer();
            this.timmer.Interval = interval;
            timmer.Enabled = true;
            Posting();

        }

        protected override void OnStop()
        {
            timmer.Enabled = false;
        }

        private void Posting()
        {
            var registerUsers = GetRegisterUsers();
            foreach (var item in registerUsers)
            {
                var profileType = item.ProfileTypeID;
                switch (profileType)
                {
                    case 1: //FACEBOOK CASE
                        GetpageTokens(item.AccessToken);
                        break;
                    case 2:  //GOOGLE CASE'
                        GooglePost();
                        break;
                    case 3: //TWITTER CASE
                        PosterTwitterFromMVC(item);
                        break;
                    default:
                        break;
                }

            }


        }

        private List<Profile> GetRegisterUsers()
        {
            Profile model = new Profile();
            List<Profile> profilesList = new List<Profile>();
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                profilesList = db.Select<Profile>().ToList();
            }
            return profilesList;
        }

        #region FACEBOOK CODE
        public void GetpageTokens(string AccessToken)
        {
            // User Access Token  we got After authorization
            string UserAccesstoken = AccessToken;
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
                    PostOnFBPage(data[i], AccessToken);
                }
            }


        }
        public void PostOnFBPage(dynamic obj, string UserAccessToken)
        {

            // string mainaccess = "CAAXXKTvsAhYBABO95ezCqSnKcAiHaZC3spJa8ljgZBSzxfCcyx7dpW1wZCTFN1110uIPUAlEAIDFeHtY5o9qORwTNJAy3tKMIPAR2ClQyL0QgiuWkYZC5xBvON74W7lhwy6EsyoIfKnoxtPdzGUwZAnis6IMJiPEMoVwECirsuKUO5IMeMTTVtx1eKtH8GTNrh0MPrwRZCmZA02l6uHSvLzx9ATq0VaC24ZD";
            string name = obj.name;
            string Accesstoken = obj.access_token;
            string category = obj.category;
            string id = obj.id;
            dynamic messagePost = new ExpandoObject();
            messagePost.access_token = Accesstoken;
            messagePost.picture = filename;
            //  ReadImageFile(filename);// "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQlKBchID7G2ISdIL4n-L2mmp8TuGZ_7CX26pr8usH2jUGa_0sgQTTomhY";
            messagePost.message = "Facebook Client";
            FacebookClient app = new FacebookClient(UserAccessToken);
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

        #region TWITTER CODE
        private static async void PosterTwitterFromMVC(Profile item)
        {
            IAuthorizer auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"].ToString(),// "0vbuhbtd8Zz7M121MtxtrA",
                    ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"].ToString(),// "5aj5te5ygcpPCBOMrwvGcjI8GAoAfAZFMlpLhyt2U"
                    OAuthToken = item.AccessToken,// "164334839-o5Dq8UKqZCLqooAHBOeuGDKgcx9ExTfO2FMvMxti",
                    OAuthTokenSecret = item.TokenSecret // "9vy702RGpq4sGPJEGAuQy0tsVOIsN3gu53MImpyPizI1f"

                }
            };

            var ctx = new TwitterContext(auth);

            //await ctx.TweetAsync("Testing from Application. MV Tweet!");
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(filename);
            await ctx.TweetWithMediaAsync("Test", false, imageBytes);
        }
        public static byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
        }
        #endregion

        #region GOOGLE PLUS CODE

        public static ClientSecrets secrets = new ClientSecrets()
        {
            ClientId = "9231245845-kmqb43s9q0o8apppcvg0d4in7c4em291.apps.googleusercontent.com",
            ClientSecret = "Vxg-lRDr6rN6ncwCm73mLKh2"
        };


        private static void GooglePost()
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { PlusService.Scope.PlusLogin }
            };

            var flow = new AAGoogleAuthorizationCodeFlow(initializer);

            UserCredential newCred = new UserCredential(flow, "user", new Google.Apis.Auth.OAuth2.Responses.TokenResponse()
            {
                AccessToken = "ya29.vAElYZ1lP49v1R9UKZUZZzMh5V3Qq82Hd_JltmTDlgG58_rWfN9pTAE7Fuca-aHVZqGM",
                TokenType = "Bearer",
                RefreshToken = "1/BGm2Zm5Qy-THANxDvw4gHMohK9YBKuAgVNwymqEnjSo"
            });

            // Create the service.
            var service = new PlusService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = newCred
            });

            Moment body = new Moment();
            ItemScope target = new ItemScope();
            //target.Type = "https://schemas.google.com/AddActivity";
            target.Url = "https://developers.google.com/+/web/snippet/examples/widget";
            //target.Description = "The description for the action";
            target.Name = "An example of add activity";
            body.Target = target;
            body.Type = "http://schemas.google.com/AddActivity";

            MomentsResource.InsertRequest insert =

                            service.Moments.Insert(body, "me", MomentsResource.InsertRequest.CollectionEnum.Vault);

            Moment wrote = insert.Execute();
        }

        public class AAGoogleAuthorizationCodeFlow : AuthorizationCodeFlow
        {
            /// <summary>Constructs a new Google authorization code flow.</summary>
            public AAGoogleAuthorizationCodeFlow(AuthorizationCodeFlow.Initializer initializer)
                : base(initializer)
            {
            }

            //public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
            ////{
            ////    return new AAGoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl))
            ////    {
            ////        ClientId = ClientSecrets.ClientId,
            ////        Scope = string.Join(" ", Scopes),
            ////        RedirectUri = redirectUri,
            ////        VisibleActions = "http://schemas.google.com/AddActivity"
            ////    };
            //}
        }

        #endregion


    }
}
