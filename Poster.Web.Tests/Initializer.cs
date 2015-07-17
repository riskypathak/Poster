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
    }
}
