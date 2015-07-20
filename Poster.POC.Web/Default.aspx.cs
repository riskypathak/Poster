using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
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
    public class AAGoogleAuthorizationCodeRequestUrl : GoogleAuthorizationCodeRequestUrl
    {
        [Google.Apis.Util.RequestParameterAttribute("request_visible_actions", Google.Apis.Util.RequestParameterType.Query)]
        public string VisibleActions { get; set; }

        public AAGoogleAuthorizationCodeRequestUrl(Uri authorizationServerUrl)
            : base(authorizationServerUrl)
        {
        }
    }

    public class AAGoogleAuthorizationCodeFlow : AuthorizationCodeFlow
    {
        /// <summary>Constructs a new Google authorization code flow.</summary>
        public AAGoogleAuthorizationCodeFlow(AuthorizationCodeFlow.Initializer initializer)
            : base(initializer)
        {
        }

        public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
        {
            return new AAGoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl))
            {
                ClientId = ClientSecrets.ClientId,
                Scope = string.Join(" ", Scopes),
                RedirectUri = redirectUri,
                VisibleActions = "http://schemas.google.com/AddActivity"
            };
        }
    }

    public partial class Default : System.Web.UI.Page
    {
        // These come from the APIs console:
        //   https://code.google.com/apis/console
        public static ClientSecrets secrets = new ClientSecrets()
        {
            ClientId = "9231245845-kmqb43s9q0o8apppcvg0d4in7c4em291.apps.googleusercontent.com",
            ClientSecret = "Vxg-lRDr6rN6ncwCm73mLKh2"
        };

        static UserCredential credential = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //POstGooglePlus();
            //POstGooglePlus(credential);

            Run();
        }

        private static void Run()
        {
            UserCredential credential;

            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { PlusService.Scope.PlusLogin }
            };
            var flow = new AAGoogleAuthorizationCodeFlow(initializer);
            credential = new AuthorizationCodeInstalledApp(flow, new LocalServerCodeReceiver()).AuthorizeAsync
                ("user", CancellationToken.None).Result;

            // Create the service.
            var service = new PlusService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gus API",
            });

            Moment body = new Moment();
            ItemScope target = new ItemScope();
            target.Url = "https://developers.google.com/+/web/snippet/examples/widget";
            target.Image = "http://picpaste.com/pics/001.1437292069.jpg";
            //target.Type = "http://schema.org/Thing";
            target.Description = "The description for the action";
            target.Name = "An example of add activity";
            body.Target = target;
            body.Type = "http://schemas.google.com/AddActivity";

            //PeopleResource.GetRequest personRequest = service.People.Get("me");
            //Person _me = personRequest.Execute();

            MomentsResource.InsertRequest insert =

                service.Moments.Insert(body, "me", MomentsResource.InsertRequest.CollectionEnum.Vault);

            //new MomentsResource.InsertRequest(
            //    service,
            //    body,
            //    "me",
            //    MomentsResource.InsertRequest.CollectionEnum.Vault);

            Moment wrote = insert.Execute();


            MomentsResource.ListRequest ls = service.Moments.List("me", MomentsResource.ListRequest.CollectionEnum.Vault);
            MomentsFeed feeds = ls.Execute();

        }
        private void POstGooglePlus(UserCredential credential)
        {
            PlusService service = new PlusService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });

            Moment body = new Moment();
            body.Type = "http://schema.org/AddAction";

            ItemScope itemScope = new ItemScope();
            itemScope.Id = "target-id-1";
            itemScope.Type = "http://schema.org/AddAction";
            itemScope.Name = "The Google+ Platform";
            itemScope.Description = "A page that describes just how awesome Google+ is!";
            itemScope.Image = "https://developers.google.com/+/plugins/snippet/examples/thing.png";
            body.object__ = itemScope;

            var l = service.Moments.Insert(body, "me", MomentsResource.InsertRequest.CollectionEnum.Vault);
            l.Execute();

            PeopleResource.GetRequest personRequest = service.People.Get("me");
            Person _me = personRequest.Execute();


        }

        public static void POstGooglePlus()
        {
            string[] scopes = new string[] {PlusService.Scope.PlusLogin,
 PlusService.Scope.UserinfoEmail,
 PlusService.Scope.UserinfoProfile};
            // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            credential =
                    GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                    {
                        ClientId = "9231245845-kmqb43s9q0o8apppcvg0d4in7c4em291.apps.googleusercontent.com",
                        ClientSecret = "Vxg-lRDr6rN6ncwCm73mLKh2",
                    },
                                                                scopes,
                                                                Environment.UserName,
                                                                CancellationToken.None,
                                                            new FileDataStore("Daimto.GooglePlus.Auth.Store")
                                                                ).Result;

            //// Now we create a Google service. All of our requests will be run though this.
            //PlusService service = new PlusService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //});

            //PeopleResource.GetRequest personRequest = service.People.Get("104915057916004137172");
            //Person _me = personRequest.Execute();
        }
    }
}