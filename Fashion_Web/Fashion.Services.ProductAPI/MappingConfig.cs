using AutoMapper;
using Fashion.Services.ProductAPI.Models;
using Fashion.Services.ProductAPI.Models.Dto;

namespace Fashion.Services.ProductAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<CategoryDto, Category>().ReverseMap();
				config.CreateMap<SubCategoryDto, SubCategory>().ReverseMap();
				config.CreateMap<ProductDto, Product>().ReverseMap();
			});
			return mappingConfig;
		}
	}
}
