using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Poster.POC.Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            POstGooglePlus();
        }

        public static void POstGooglePlus()
        {
            string[] scopes = new string[] {PlusService.Scope.PlusLogin,
 PlusService.Scope.UserinfoEmail,
 PlusService.Scope.UserinfoProfile};
            // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            UserCredential credential =
                    GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                    {
                        ClientId = "9231245845-r8j2rtmtteakfut8qigee9c39mf10j3v.apps.googleusercontent.com",
                        ClientSecret = "AAZWLPUk9ee40GX-Anl7xIbd",
                    },
                                                                scopes,
                                                                Environment.UserName,
                                                                CancellationToken.None,
                                                            new FileDataStore("Daimto.GooglePlus.Auth.Store")
                                                                ).Result;

            // Now we create a Google service. All of our requests will be run though this.
            PlusService service = new PlusService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Plus Sample",
            });
        }
    }
}