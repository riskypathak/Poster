using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster.Data.Models
{
    public class DBEntity
    {
        [AutoIncrement]
        public int Id { get; set; }
    }
}
