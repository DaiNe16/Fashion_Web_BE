﻿using AutoMapper;
using Fashion.Services.ShoppingCartAPI.Data;
using Fashion.Services.ShoppingCartAPI.Models;
using Fashion.Services.ShoppingCartAPI.Models.Dto;
using Fashion.Services.ShoppingCartAPI.Service;
using Fashion.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Stripe;
using Stripe.Checkout;

namespace Fashion.Services.ShoppingCartAPI.Controllers
{
	[Route("api/cart")]
	[ApiController]
	[Authorize]
	public class ShoppingCartAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private IMapper _mapper;
		private ResponseDto _response;
		private IProductService _productService;
		private ICouponService _couponService;
		private StripeSettings _stripeSettings;

		public ShoppingCartAPIController(AppDbContext appDbContext, IMapper mapper, IProductService productService, ICouponService couponService, IOptions<StripeSettings> stripeSettings)
		{
			_db = appDbContext;
			_mapper = mapper;
			_response = new ResponseDto();
			_productService = productService;
			_couponService = couponService;
			_stripeSettings = stripeSettings.Value;
		}

		[HttpGet("CheckOut")]
		public async Task<ResponseDto> CheckOut(string userId)
		{
			try
			{
				//Get cart
				var cartHeader = _db.CartHeader.FirstOrDefault(u => u.UserId == userId);
				if (cartHeader == null)
				{
					_response.Message = "Please add item to cart.";
					_response.IsSuccess = false;
					return _response;
				}
				CartDto cartDto = new CartDto()
				{
					CartHeader = _mapper.Map<CartHeaderDto>(cartHeader),
				};
				cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(u => u.CartHeaderId == cartHeader.CartHeaderId));
				IEnumerable<ProductDto> listProductDto = await _productService.GetProducts();

				foreach (var cartDetail in cartDto.CartDetails)
				{
					cartDetail.Product = listProductDto.FirstOrDefault(u => u.ProductId == cartDetail.ProductId);
					cartDto.CartHeader.CartTotal += (cartDetail.Product.ProductPrice * cartDetail.Count);
				}
				//Apply coupon 
				if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
				{
					CouponDto coupon = await _couponService.GetCouponByCode(cartDto.CartHeader.CouponCode);
					if (cartDto.CartHeader.CartTotal > coupon.MinAmount)
					{
						cartDto.CartHeader.CartTotal -= (cartDto.CartHeader.CartTotal * coupon.DiscountAmount / 100);
					}
				}

				//Payment
				StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
				var domain = "http://localhost:4242";

				var options = new SessionCreateOptions
				{
					//List item
					LineItems = cartDto.CartDetails.Select(cartDetail => new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							Currency = "usd",
							UnitAmount = long.Parse(cartDetail?.Product?.ProductPrice + ""),
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = cartDetail.Product.ProductName,
								Description = cartDetail.Product.ProductDescription
							}
						},
						Quantity = cartDetail.Count,
					}).ToList(),
					// Other options...
					Metadata = new Dictionary<string, string>
					{
						{ "userId", userId }
					},
					Mode = "payment",
					SuccessUrl = "https://translate.google.com/?hl=vi&sl=en&tl=vi&text=success&op=translate",
					CancelUrl = "https://translate.google.com/?hl=vi&sl=en&tl=vi&text=cance&op=translate",
				};
				var service = new SessionService();
				Session session = service.Create(options);

				_response.Result = session.Url;
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpGet("GetAllOrder")]
		public async Task<ResponseDto> GetAllOrder(string userId)
		{
			// Handle the event
			try
			{
				var orders = _db.Orders.Where(u => u.UserId == userId).ToList();
				_response.Result = orders;
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpPut("UpdateQuantityOfCartDetail")]
		public async Task<ResponseDto> UpdateQuantityOfCartDetail(int cartDetailId, int quantity)
		{
			// Handle the event
			try
			{
				var cartDetail = _db.CartDetails.FirstOrDefault(o => o.CartDetailId == cartDetailId);
				cartDetail.Count = quantity;
				_db.Update(cartDetail);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpGet("GetAllOrderDetailByOrderId")]
		public async Task<ResponseDto> GetAllOrderDetailByOrderId(int orderId)
		{
			// Handle the event
			try
			{
				var orderDetails = _db.OrderDetails.Where(u => u.OrderId == orderId).ToList();
				_response.Result = orderDetails;
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpGet("GetCart")]
		public async Task<ResponseDto> GetCart(string userId)
		{
			// Handle the event
			try
			{
				var cartHeader = _db.CartHeader.FirstOrDefault(u => u.UserId == userId);
				if (cartHeader == null)
				{
					_response.Message = "Please add item to cart.";
					_response.IsSuccess = false;
					return _response;
				}
				CartDto cartDto = new CartDto()
				{
					CartHeader = _mapper.Map<CartHeaderDto>(cartHeader),
				};
				cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(u => u.CartHeaderId == cartHeader.CartHeaderId));
				IEnumerable<ProductDto> listProductDto = await _productService.GetProducts();

				foreach (var cartDetail in cartDto.CartDetails)
				{
					cartDetail.Product = listProductDto.FirstOrDefault(u => u.ProductId == cartDetail.ProductId);
					cartDto.CartHeader.CartTotal += (cartDetail.Product.ProductPrice * cartDetail.Count);
				}
				//Apply coupon 
				if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
				{
					CouponDto coupon = await _couponService.GetCouponByCode(cartDto.CartHeader.CouponCode);
					if (cartDto.CartHeader.CartTotal > coupon.MinAmount)
					{
						cartDto.CartHeader.CartTotal -= (cartDto.CartHeader.CartTotal * coupon.DiscountAmount / 100);
					}
				}
				_response.Result = cartDto;
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpPost("CartUpsert")]
		public async Task<ResponseDto> CartUpsert(CartDto cartDto)
		{
			try
			{
				var cartHeaderFromDb = await _db.CartHeader.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
				if (cartHeaderFromDb == null)
				{
					//TODO: Create CartHeader
					//TODO: Create CartDetails
					CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
					_db.Add(cartHeader);
					await _db.SaveChangesAsync();
					// Associate CartDetails with the existing CartHeader
					var cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
					cartDetails.CartHeaderId = cartHeader.CartHeaderId;
					_db.Add(cartDetails);
					await _db.SaveChangesAsync();
				}
				else
				{
					var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
						u => u.CartHeaderId == cartHeaderFromDb.CartHeaderId
						&& u.ProductId == cartDto.CartDetails.First().ProductId
					);
					if (cartDetailsFromDb == null)
					{
						//TODO: Create CartDetails
						cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
						_db.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
						await _db.SaveChangesAsync();
					}
					else
					{
						//TODO: Update count in CartDetails
						cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
						cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
						cartDto.CartDetails.First().CartDetailId = cartDetailsFromDb.CartDetailId;
						_db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
						await _db.SaveChangesAsync();
					}
				}
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpPost("RemoveCart")]
		public async Task<ResponseDto> RemoveCart(int cartDetailId)
		{
			try
			{
				//Case 1: CartHeader has more than one CartDetails => just Remove CartDetails
				//Case 2: CartHeader has only one CartDetails => Remove CartHeader and CartDetails
				var cartDetails = _db.CartDetails.FirstOrDefault(u => u.CartDetailId == cartDetailId);
				if (cartDetails == null)
				{
					_response.Message = "Cartdetails is not found";
					_response.IsSuccess = false;
					return _response;
				}
				var cartDetailCount = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
				_db.Remove(cartDetails);
				if (cartDetailCount == 1)
				{
					_db.Remove(_db.CartHeader.FirstOrDefault(u => u.CartHeaderId == cartDetails.CartHeaderId));
					_db.SaveChanges();
					return _response;
				}
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpPost("ApplyCoupon")]
		public async Task<ResponseDto> ApplyCoupon(CartDto cartDto)
		{
			try
			{
				var cartHeader = _db.CartHeader.FirstOrDefault(u => u.UserId == cartDto.CartHeader.UserId);
				if (cartHeader == null)
				{
					_response.Message = "User has no cart.";
					_response.IsSuccess = false;
					return _response;
				}
				cartHeader.CouponCode = cartDto.CartHeader.CouponCode;
				_db.Update(cartHeader);
				_db.SaveChanges();
				_response.Result = true;
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message.ToString();
				_response.IsSuccess = false;
			}
			return _response;
		}


	}
}
