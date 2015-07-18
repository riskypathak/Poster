using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Plus.v1;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Plus.v1.Data;
using System.Collections.Generic;
using System.Linq;

namespace Poster.POC
{
    class Program
    {
        static void Main(string[] args)
        {
            POstGooglePlus();
            PostTwitter();

            System.Threading.Thread.Sleep(2000000);
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
                        ClientSecret = "AvHRdSwtyNSZ9YVm6bDMPO6o",
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
        public static async void PostTwitter()
        {
            string pl;

            //var auth = AuthorizeTwitter();

            var auth = AuthorizeTwitterWithToken();



            var ctx = new TwitterContext(auth);

            //var new Media
            //    {
            //        Data = Utilities.GetFileBytes(replaceThisWithYourImageLocation),
            //        FileName = "200xColor_2.png",
            //        ContentType = MediaContentType.Png
            //    };

            //await ctx.TweetWithMediaAsync("Testing from Application. Posting Image", false, ReadImageFile(Path.GetFullPath("image.png")));

            await ctx.TweetAsync("Testing from Application. Second Tweet!");
            //var twitterCtx = new TwitterContext(auth);

            await TwitterParameters(auth);
        }

        public async static Task TwitterParameters(PinAuthorizer auth)
        {

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

        private static PinAuthorizer AuthorizeTwitter()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = "0vbuhbtd8Zz7M121MtxtrA",
                    ConsumerSecret = "5aj5te5ygcpPCBOMrwvGcjI8GAoAfAZFMlpLhyt2U"
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };
            return auth;
        }

        private static PinAuthorizer AuthorizeTwitterWithToken()
        {
            return new PinAuthorizer()
           {
               CredentialStore = new InMemoryCredentialStore
               {
                   ConsumerKey = "0vbuhbtd8Zz7M121MtxtrA",
                   ConsumerSecret = "5aj5te5ygcpPCBOMrwvGcjI8GAoAfAZFMlpLhyt2U",
                   OAuthToken = "164334839-o5Dq8UKqZCLqooAHBOeuGDKgcx9ExTfO2FMvMxti",
                   OAuthTokenSecret = "9vy702RGpq4sGPJEGAuQy0tsVOIsN3gu53MImpyPizI1f"
               }
           };
        }
    }
}
