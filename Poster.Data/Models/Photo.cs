using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Data.Models
{
    public class Photo:DBEntity
    {
        public string PhotoPath { get; set; }
        public string PhotoText { get; set; }
    }
}
