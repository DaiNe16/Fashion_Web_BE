using System.ComponentModel.DataAnnotations.Schema;

namespace Fashion.Services.ShoppingCartAPI.Models
{
	public class OrderDetail
	{
        public int OrderDetailId { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
		public virtual Order Order { get; set; }
	}
}
