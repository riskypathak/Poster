using Facebook;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Services;
using LinqToTwitter;
using Newtonsoft.Json.Linq;
using Poster.Data.Models;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Timers;
namespace PosterService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer = new Timer();
        private Dictionary<Profile, int> profiles = new Dictionary<Profile, int>();
        private static string filename = "/Images/310720151115Lighthouse.jpg";
        private string filetext = string.Empty;
        private string baseurl = "http://188.42.227.39/Poster/";
        private static Random rand;
        public Service1()
        {
            InitializeComponent();
            rand = new Random();
            timer.Interval = 60 * 1000; //1 Minute

        }

        //public void Start(string[] args)
        //{
        //    OnStart(args);
        //}

        //public void Stop()
        //{
        //    OnStop();
        //}

        protected override void OnStart(string[] args)
        {

            Log.WriteLog("Service Statrted");
            timer.Start();
            timer.Elapsed += timer_Elapsed;

            Process();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Process();
        }

        private void Process()
        {
            try
            {

                Log.WriteLog("Process Statrted");
                //Update profile here
                UpdateProfiles();

                foreach (KeyValuePair<Profile, int> profile in this.profiles.ToList())
                {
                    //Time to post
                    if (profile.Value <= 0)
                    {
                        Log.WriteLog("Profile value:" + profile.Value);
                        Log.WriteLog("Posting Statrted");
                        SetPostData(profile.Key);
                        Posting(profile.Key);

                        this.profiles[profile.Key] = profile.Key.Interval;
                    }
                    else
                    {
                        Log.WriteLog(profile.Value.ToString());
                        this.profiles[profile.Key] = profile.Value - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());

            }

            //timer = new List<Timer>();

            // ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            // using (IDbConnection db = dbFactory.Open())
            // {
            //     db.Select<Profile>().Select(x => x.Interval).ToList().ForEach(i=>)
            // }


            //int[] interval;
            //using (IDbConnection db = dbFactory.Open())
            //{
            //    var photoList = db.Select<Post>().ToList();
            //    var rand = new Random();
            //    int number = rand.Next(photoList.Count);
            //    string filename = photoList[number].PhotoPath;
            //    string filetext = photoList[number].Text;

            //    interval = ;
            //}
            //return interval;
        }

        private void SetPostData(Profile objprofile)
        {
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {

                filename = PostImage(objprofile, db);
                Log.WriteLog(filename);
                // Log.WriteLog("Error:" + System.IO.File.ReadAllText(PostText(objprofile, db)));
                filetext = PostText(objprofile, db);

            }

        }
        private string PostImage(Profile objprofile, IDbConnection db)
        {
            Log.WriteLog("ID:"+objprofile.Id.ToString());
            var imageGroups = db.Where<ProfileImageGroup>("ProfileID", objprofile.Id).Select(x => x.GroupID).ToList();
            Log.WriteLog(imageGroups.Count.ToString());
            string file = string.Empty;
            int group = 0;
            if (imageGroups.Count > 0)
            {
                if (imageGroups.Count > 1)
                {
                    group = rand.Next(imageGroups.Count);
                }
                else
                {
                    group = 0;
                }

                var photoList = db.Where<PostImage>("GroupId", imageGroups[group]).ToList();

                if (photoList.Count > 0)
                {
                    int number = rand.Next(photoList.Count);
                    string path = photoList[number].Imagelink;
                    file = baseurl + path.Substring(3, path.Length - 3);
                }
            }
            return file;
        }
        private string PostText(Profile objprofile, IDbConnection db)
        {
            var textGroups = db.Where<ProfileTextGroup>("ProfileID", objprofile.Id).Select(x => x.GroupID).ToList();
            string text = string.Empty;
            int group = 0;
            if (textGroups.Count > 0)
            {
                if (textGroups.Count > 1)
                {
                    group = rand.Next(textGroups.Count);
                }
                else
                {
                    group = 0;
                }

                var textList = db.Where<PostText>("GroupId", textGroups[group]).ToList();
                string file = string.Empty;
                if (textList.Count > 0)
                {
                    int number = rand.Next(textList.Count);
                    string path = textList[number].Text;
                    file = baseurl + path.Substring(3, path.Length - 3);
                    Log.WriteLog("text file:" + file);
                    string destination = Path.GetTempPath() + Guid.NewGuid() + ".txt";
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFile(file, destination);
                    }
                    text = System.IO.File.ReadAllText(destination);
                    Log.WriteLog("text file:" + destination);
                    File.Delete(destination);
                }
            }
            return text;
        }


        private void UpdateProfiles()
        {
            ServiceStack.Data.IDbConnectionFactory dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);
            using (IDbConnection db = dbFactory.Open())
            {
                Log.WriteLog("Update profile Statrted");
                foreach (Profile newprofile in db.Select<Profile>())
                {
                    KeyValuePair<Profile, int> oldProfile = this.profiles.FirstOrDefault(p => p.Key.Id == newprofile.Id);


                    if (oldProfile.Key != null && oldProfile.Equals(default(KeyValuePair<Profile, int>)))
                    {
                        if (oldProfile.Key.Interval != newprofile.Interval)
                        {
                            this.profiles.Remove(oldProfile.Key);
                            this.profiles.Add(newprofile, newprofile.Interval);
                        }
                    }
                    else
                    {
                        this.profiles.Add(newprofile, newprofile.Interval);
                    }

                }
                Log.WriteLog("Update profile end");
            }
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            Log.WriteLog("Service Stoped");
        }

        private void Posting(Profile item)
        {
            //var registerUsers = GetRegisterUsers();
            //foreach (var item in registerUsers)
            //{
            try
            {
                var profileType = item.ProfileTypeID;
                switch (profileType)
                {
                    case 1: //FACEBOOK CASE
                        Log.WriteLog("FB Post Statrted");
                        GetpageTokens(item.AccessToken, item.PageId);
                        break;
                    case 2:  //GOOGLE CASE'
                        GooglePost();
                        break;
                    case 3: //TWITTER CASE
                        Log.WriteLog("Twitter Post Statrted");
                        PosterTwitterFromMVC(item);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Posting Method:" + ex.ToString());
            }

            //}
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
        public void GetpageTokens(string AccessToken, string pageId)
        {
            try
            {
                Log.WriteLog("GetpageTokens Statrted");
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
                    Log.WriteLog("pageecheck Statrted");
                    Log.WriteLog("pageId" + pageId);

                    if (data[i].ToList()[4].Last.ToString() == pageId)
                    {
                        Log.WriteLog("pageId matched");
                        PostOnFBPage(data[i], AccessToken);
                    }
                    Log.WriteLog("pageId match passed");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                throw;
            }


        }
        public void PostOnFBPage(dynamic obj, string UserAccessToken)
        {
            try
            {
                Log.WriteLog(UserAccessToken);
                Log.WriteLog("FileName:"+filename);
              //  Log.WriteLog(filename);
                // string mainaccess = "CAAXXKTvsAhYBABO95ezCqSnKcAiHaZC3spJa8ljgZBSzxfCcyx7dpW1wZCTFN1110uIPUAlEAIDFeHtY5o9qORwTNJAy3tKMIPAR2ClQyL0QgiuWkYZC5xBvON74W7lhwy6EsyoIfKnoxtPdzGUwZAnis6IMJiPEMoVwECirsuKUO5IMeMTTVtx1eKtH8GTNrh0MPrwRZCmZA02l6uHSvLzx9ATq0VaC24ZD";
                string name = obj.name;
                string Accesstoken = obj.access_token;
                string category = obj.category;
                string id = obj.id;
                dynamic messagePost = new ExpandoObject();
                messagePost.access_token = Accesstoken;
                messagePost.picture = filename;
                //  ReadImageFile(filename);// "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQlKBchID7G2ISdIL4n-L2mmp8TuGZ_7CX26pr8usH2jUGa_0sgQTTomhY";
                messagePost.message = filetext;
                FacebookClient app = new FacebookClient(UserAccessToken);
                try
                {
                    var result = app.Post("/" + id + "/feed", messagePost);
                    Log.WriteLog("FB Post Done");
                }
                catch (FacebookOAuthException ex)
                {
                    Log.WriteLog(ex.ToString());
                }
                catch (FacebookApiException ex)
                {
                    Log.WriteLog(ex.ToString());
                }

            }
            catch (Exception ex)
            {

                Log.WriteLog(ex.ToString());
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
            Log.WriteLog("Twitter Post Done");
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
            ClientId = ConfigurationManager.AppSettings["ClientId"].ToString(),
            ClientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString()
        };


        private static void GooglePost()
        {
            //var initializer = new GoogleAuthorizationCodeFlow.Initializer
            //{
            //    ClientSecrets = secrets,
            //    Scopes = new[] { PlusService.Scope.PlusLogin }
            //};

            //var flow = new AAGoogleAuthorizationCodeFlow(initializer);

            //UserCredential newCred = new UserCredential(flow, "user", new Google.Apis.Auth.OAuth2.Responses.TokenResponse()
            //{
            //    AccessToken = "ya29.vAElYZ1lP49v1R9UKZUZZzMh5V3Qq82Hd_JltmTDlgG58_rWfN9pTAE7Fuca-aHVZqGM",
            //    TokenType = "Bearer",
            //    RefreshToken = "1/BGm2Zm5Qy-THANxDvw4gHMohK9YBKuAgVNwymqEnjSo"
            //});

            //// Create the service.
            //var service = new PlusService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = newCred
            //});

            //Moment body = new Moment();
            //ItemScope target = new ItemScope();
            ////target.Type = "https://schemas.google.com/AddActivity";
            //target.Url = "https://developers.google.com/+/web/snippet/examples/widget";
            ////target.Description = "The description for the action";
            //target.Name = "An example of add activity";
            //body.Target = target;
            //body.Type = "http://schemas.google.com/AddActivity";

            //MomentsResource.InsertRequest insert =

            //                service.Moments.Insert(body, "me", MomentsResource.InsertRequest.CollectionEnum.Vault);

            //Moment wrote = insert.Execute();
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
