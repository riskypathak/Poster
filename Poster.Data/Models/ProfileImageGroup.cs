using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Data.Models
{
    public class ProfileImageGroup : DBEntity
    {
        public int ProfileID { get; set; }
        public int GroupID { get; set; }
    }
}
