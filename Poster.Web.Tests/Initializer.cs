using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poster.Data.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Web.Tests
{
    [TestClass]
    public class Initializer
    {
        [TestMethod]
        public void Database()
        {
            IDbConnectionFactory dbFactory =
                new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["db"].ConnectionString, MySqlDialect.Provider);

            using (IDbConnection db = dbFactory.Open())
            {
                db.DropAndCreateTable<ProfileType>();
                db.DropAndCreateTable<Profile>();
                db.InsertAll<ProfileType>(GetProfileTypes());
                db.DropAndCreateTable<Config>();
                db.DropAndCreateTable<Post>();
                //db.InsertAll<Post>(GetPhotos());
            }
        }
        private IEnumerable<ProfileType> GetProfileTypes()
        {
            return new List<ProfileType>()
            {
                new ProfileType()
                {
                    Name = "Facebook"
                },
                new ProfileType()
                {
                    Name = "GooglePlus"
                   
                },
                new ProfileType()
                {
                    Name = "Twitter"
                    
                }
                
            };
        }
        private IEnumerable<Post> GetPhotos()
        {
            return new List<Post>()
            {
                new Post()
                {
                    PhotoPath="https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcSC8XYrmXzYOWP4KYxZI7D92X-aIsQNzFMS9r0pZ4RqBby8TFLKuQ"
                    ,Text="Success"
                    
                },
                new Post()
                {
                     PhotoPath="http://www.apisanet.com/nnh-content/uploads/th/this-is-a-test-funny-quotes-funny-sayings-and-quote-on-black.jpg"
                    ,Text="Test"
                   
                },
                new Post()
                {
                     PhotoPath="http://t0.gstatic.com/images?q=tbn:ANd9GcRO0w7NZtvEPTaLf1EnoWBobzirZGW4AATKaQKe6rcPykrJ08ma"
                    ,Text="Minion"
                   
                },
                new Post()
                {
                     PhotoPath="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSlk96Sf7gimDAm5oafwdfrHNpdn1V_IXqmt6t1qRPBq5LjnKwo"
                    ,Text="Information Technology"
                   
                },
                new Post()
                {
                     PhotoPath="https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcRuDLM3PSmQ7pu03wOLVh0VAfLOIis_intH0mtsgyh-tNlmX7o4"
                    ,Text="Experince"
                   
                },
                new Post()
                {
                     PhotoPath="http://api.ning.com/files/x4oQmCnQ4B7YcQj8lBEBb5Z5Fiu0CI2SzVv-n4x*c3moTkqVWal4Ip1LFmdzAS3HspVtIoQbzz3F*GKnyjoLl5pBGu5VDAmT/knowledge3.png"
                    ,Text="Knowledge"
                   
                },
                new Post()
                {
                    PhotoPath="https://encrypted-tbn1.gstatic.com/images?q=tbn:ANd9GcTmziU-gLHfPjoLDVY5AdckWNBLwVKNkLm1NBiDr8dImEbyleIF"
                    ,Text="Software"
                    
                }
                
            };
        }
    }
}
