using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Data.Models
{
    public class ProfileImageGroup : DBEntity
    {
        [ForeignKey(typeof(Profile), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public int ProfileID { get; set; }
        [ForeignKey(typeof(Group), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public int GroupID { get; set; }
    }
}
