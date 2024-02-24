using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Review
    {
        public int? Id {  get; set; }
        public int? ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Product? Product { get; set; }
    }
}
