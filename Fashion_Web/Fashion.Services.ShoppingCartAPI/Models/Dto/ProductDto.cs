namespace Fashion.Services.ShoppingCartAPI.Models.Dto
{
	public class ProductDto
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public double ProductPrice { get; set; }
		public string ProductUrl { get; set; }
		public string Material { get; set; }
		public string Color { get; set; }
		public string ProductDescription { get; set; }
		public int ProductRate { get; set; }
		public int SubCategoryId { get; set; }
	}
}
