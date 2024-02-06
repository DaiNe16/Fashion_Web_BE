using AutoMapper;
using Fashion.Services.CouponAPI.Models;
using Fashion.Services.CouponAPI.Models.Dto;

namespace Fashion.Services.CouponAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<CouponDto, Coupon>().ReverseMap();
			});
			return mappingConfig;
		}
	}
}
