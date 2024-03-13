using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class Review
    {
        public int? Id {  get; set; }
        public int? ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Product? Product { get; set; }

        [EmailAddress]
        public string Owner { get; set; }
    }
}
