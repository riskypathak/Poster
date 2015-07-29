using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Data.Models
{
    public class Post:DBEntity
    {
        public string PhotoPath { get; set; }
        public string Text { get; set; }
    }
}
