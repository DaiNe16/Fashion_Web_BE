namespace Fashion.Services.ShoppingCartAPI.Models
{
	public class Order
	{
		public int OrderId { get; set; }
        public string UserId { get; set; }
        public double DiscountAmount { get; set; }
        public double Total { get; set; }
		public virtual IEnumerable<OrderDetail> OrderDetails { get; set; }
	}
}
