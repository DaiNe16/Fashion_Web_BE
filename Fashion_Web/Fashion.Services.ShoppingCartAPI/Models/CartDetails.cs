using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Fashion.Services.ShoppingCartAPI.Models.Dto;

namespace Fashion.Services.ShoppingCartAPI.Models
{
	public class CartDetails
	{
		[Key]
		public int CartDetailId { get; set; }
		public int CartHeaderId { get; set; }
		[ForeignKey("CartHeaderId")]
		public CartHeader CartHeader { get; set; }
		public int ProductId { get; set; }
		[NotMapped]
		public ProductDto Product { get; set; }
		public int Count { get; set; }
	}
}
