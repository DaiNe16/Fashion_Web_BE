namespace Fashion.Services.ProductAPI.Models
{
	public class Category
	{
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public virtual IEnumerable<SubCategory> SubCategories { get; set; }

    }
}
