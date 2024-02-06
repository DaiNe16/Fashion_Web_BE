using Fashion.Services.ShoppingCartAPI.Models.Dto;

namespace Fashion.Services.ShoppingCartAPI.Service.IService
{
	public interface ICouponService
	{
		Task<CouponDto> GetCouponByCode(string code);
	}
}
