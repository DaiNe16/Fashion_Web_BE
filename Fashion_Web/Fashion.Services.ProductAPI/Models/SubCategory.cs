using System.ComponentModel.DataAnnotations.Schema;

namespace Fashion.Services.ProductAPI.Models
{
	public class SubCategory
	{
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual IEnumerable<Product> Products { get; set; }
    }
}
