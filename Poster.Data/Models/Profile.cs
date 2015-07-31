using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Data.Models
{
    public class Profile : DBEntity
    {

        [References(typeof(ProfileType))]
        public int ProfileTypeID { get; set; }

        [Reference]
        public ProfileType ProfileName { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string UserId { get; set; }
        public string PageId { get; set; }
        public string TokenSecret { get; set; }
        public string RefreshToken { get; set; }
        public int Interval { get; set; }
    }
}
