using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Data.Models
{
    public class PostImage:DBEntity
    {
        [ForeignKey(typeof(Group), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
        public int GroupId { get; set; }
        public string Imagelink { get; set; }
    }
}
