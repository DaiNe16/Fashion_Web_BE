﻿using AutoMapper;
using Fashion.Services.ShoppingCartAPI.Models.Dto;
using Fashion.Services.ShoppingCartAPI.Models;

namespace Fashion.Services.ShoppingCartAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
				config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
			});
			return mappingConfig;
		}
	}
}
