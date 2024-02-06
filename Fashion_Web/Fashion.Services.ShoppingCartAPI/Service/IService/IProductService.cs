using Fashion.Services.ShoppingCartAPI.Models.Dto;

namespace Fashion.Services.ShoppingCartAPI.Service.IService
{
	public interface IProductService
	{
		Task<IEnumerable<ProductDto>> GetProducts();
	}
}
