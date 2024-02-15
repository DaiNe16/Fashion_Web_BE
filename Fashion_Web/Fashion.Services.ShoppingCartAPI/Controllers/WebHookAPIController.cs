using Fashion.Services.ShoppingCartAPI.Data;
using Fashion.Services.ShoppingCartAPI.Models.Dto;
using Fashion.Services.ShoppingCartAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using AutoMapper;
using Fashion.Services.ShoppingCartAPI.Service.IService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Fashion.Services.ShoppingCartAPI.RabbitMQSender;

namespace Fashion.Services.ShoppingCartAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WebHookAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private IMapper _mapper;
		private ResponseDto _response;
		private IProductService _productService;
		private ICouponService _couponService;
		private IRabbitMQCartMessageSender _rabbitMQCartMessageSender;
		public WebHookAPIController(AppDbContext appDbContext, IMapper mapper, IProductService productService, ICouponService couponService, IRabbitMQCartMessageSender rabbitMQCartMessageSender)
        {
			_db = appDbContext;
			_mapper = mapper;
			_response = new ResponseDto();
			_productService = productService;
			_couponService = couponService;
			_rabbitMQCartMessageSender = rabbitMQCartMessageSender;
		}

        [HttpPost("/webhook")]
		public async Task<IActionResult> Index()
		{
			string endpointSecret = "whsec_e22247eb900a5dd9379ccec2d74280b2494ddf5b98587f0acc55d2f845bf4dd7";
			string message = "";
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], endpointSecret);

				if (stripeEvent.Type == Events.CheckoutSessionCompleted)
				{
					var session = stripeEvent.Data.Object as Session;

					if (session != null && session.Metadata.TryGetValue("userId", out var userId))
					{
						message += userId + "@123@";
						// Success
						var cartHeader = _db.CartHeader.FirstOrDefault(u => u.UserId == userId);
						var cartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(u => u.CartHeaderId == cartHeader.CartHeaderId));

						// Step 1: Remove CartHeaders
						_db.Remove(cartHeader);

						// Step 2: Create Order
						IEnumerable<ProductDto> listProductDto = await _productService.GetProducts();

						foreach (var cartDetail in cartDetails)
						{
							cartDetail.Product = listProductDto.FirstOrDefault(u => u.ProductId == cartDetail.ProductId);
							cartHeader.CartTotal += (cartDetail.Product.ProductPrice * cartDetail.Count);
						}

						double discount = 0;
						if(!string.IsNullOrEmpty(cartHeader.CouponCode))
						{
							CouponDto coupon = await _couponService.GetCouponByCode(cartHeader.CouponCode);
							if(cartHeader.CartTotal > coupon.MinAmount)
							{
								discount = cartHeader.CartTotal * coupon.DiscountAmount / 100;
								cartHeader.CartTotal -= (cartHeader.CartTotal * coupon.DiscountAmount / 100);
							}
						}

						Order order = new Order
						{
							UserId = userId,
							DiscountAmount = discount,
							Total = cartHeader.CartTotal,
						};
						_db.Add(order);
						_db.SaveChanges();

						var orderCreated = _db.Orders.FirstOrDefault(o => o.UserId == userId);
						foreach (var cartDetail in cartDetails)
						{
							OrderDetail orderDetail = new OrderDetail
							{
								OrderId = orderCreated.OrderId,
								ProductName = cartDetail.Product.ProductName,
								Price = cartDetail.Product.ProductPrice,
								Count = cartDetail.Count
							};
							_db.OrderDetails.Add(orderDetail);
						}
						_db.SaveChanges();
						message += "Your order is placed with price of " + order.Total+"$";
						_rabbitMQCartMessageSender.SendMessage(message, "I just send fanout with exchange: not need queuename");
						// Example of using userId:
						Console.WriteLine($"User ID: {userId}");
					}
					else
					{
						// Failed
					}
				}
				return Ok();
			}
			catch (StripeException e)
			{
				return BadRequest();
			}
		}
	}
}
