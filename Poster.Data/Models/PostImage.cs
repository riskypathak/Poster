using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Data.Models
{
    public class PostImage:DBEntity
    {
        public string Group { get; set; }
        public string Imagelink { get; set; }
    }
}
