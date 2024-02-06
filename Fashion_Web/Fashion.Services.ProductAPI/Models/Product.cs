using System.ComponentModel.DataAnnotations.Schema;

namespace Fashion.Services.ProductAPI.Models
{
	public class Product
	{
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public string ProductUrl { get; set; }
        public string Material { get; set; }
        public string Color { get; set; }
        public string ProductDescription { get; set; }
        public int ProductRate { get; set; }

        [ForeignKey("Subcategory")]
        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }
    }
}
