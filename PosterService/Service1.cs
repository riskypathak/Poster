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

namespace PosterService
{
    public partial class Service1 : ServiceBase
    {
        private Timer timmer = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timmer = new Timer();
            this.timmer.Interval = 60000;
            timmer.Enabled = true;
            GetpageTokens();
        }

        protected override void OnStop()
        {
            timmer.Enabled = false;
        }

        public void GetpageTokens()
        {
            // User Access Token  we got After authorization
            string UserAccesstoken = "CAAXXKTvsAhYBABO95ezCqSnKcAiHaZC3spJa8ljgZBSzxfCcyx7dpW1wZCTFN1110uIPUAlEAIDFeHtY5o9qORwTNJAy3tKMIPAR2ClQyL0QgiuWkYZC5xBvON74W7lhwy6EsyoIfKnoxtPdzGUwZAnis6IMJiPEMoVwECirsuKUO5IMeMTTVtx1eKtH8GTNrh0MPrwRZCmZA02l6uHSvLzx9ATq0VaC24ZD";
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
            FacebookPost(data);

        }

        public void PostOnFBPage(JArray obj)
        {
            
            string mainaccess = "CAAXXKTvsAhYBABO95ezCqSnKcAiHaZC3spJa8ljgZBSzxfCcyx7dpW1wZCTFN1110uIPUAlEAIDFeHtY5o9qORwTNJAy3tKMIPAR2ClQyL0QgiuWkYZC5xBvON74W7lhwy6EsyoIfKnoxtPdzGUwZAnis6IMJiPEMoVwECirsuKUO5IMeMTTVtx1eKtH8GTNrh0MPrwRZCmZA02l6uHSvLzx9ATq0VaC24ZD";
            string name = (string)obj[0]["name"];
            string Accesstoken = (string)obj[0]["access_token"];
            string category = (string)obj[0]["category"];
            string id = (string)obj[0]["id"];
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
    }
}
