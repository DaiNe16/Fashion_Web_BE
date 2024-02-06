using AutoMapper;
using Fashion.Services.CouponAPI.Data;
using Fashion.Services.CouponAPI.Models;
using Fashion.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Services.CouponAPI.Controllers
{
	[Route("api/coupon")]
	[ApiController]
	[Authorize]
	public class CouponAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private ResponseDto _response;
		private IMapper _mapper;
		public CouponAPIController(AppDbContext appDbContext, IMapper mapper)
		{
			_db = appDbContext;
			_mapper = mapper;
			_response = new ResponseDto();

		}

		[HttpGet("GetAll")]
		public ResponseDto Get()
		{
			try
			{
				IEnumerable<Coupon> listCoupon = _db.Coupons.ToList();
				List<CouponDto> listCouponDto = new List<CouponDto>();
				foreach (var item in listCoupon)
				{
					listCouponDto.Add(new CouponDto()
					{
						CouponId = item.CouponId,
						CouponCode = item.CouponCode,
						DiscountAmount = item.DiscountAmount,
						MinAmount = item.MinAmount,
					});
				}
				_response.Result = listCouponDto;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet("GetById/{id}")]
		public ActionResult<ResponseDto> Get(int id)
		{
			try
			{
				CouponDto couponDto = _mapper.Map<CouponDto>(_db.Coupons.FirstOrDefault(u => u.CouponId == id));
				if (couponDto == null)
				{
					return NotFound();
				}
				_response.Result = couponDto;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet("GetByCode/{code}")]
		public ActionResult<ResponseDto> Get(string code)
		{
			try
			{
				CouponDto couponDto = _mapper.Map<CouponDto>(_db.Coupons.FirstOrDefault(u => u.CouponCode == code));
				if (couponDto == null)
				{
					return NotFound();
				}
				_response.Result = couponDto;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost("CreateCoupon")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Post(CouponDto couponDto)
		{
			try
			{
				Coupon coupon = _mapper.Map<Coupon>(couponDto);
				coupon.CouponId = 0;
				_db.Coupons.Add(coupon);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPut("UpdateCoupon")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Put(CouponDto couponDto)
		{
			try
			{
				Coupon coupon = _db.Coupons.AsNoTracking().FirstOrDefault(u => u.CouponId == couponDto.CouponId);
				if (coupon == null) { return NotFound(); }
				Coupon couponUpdate = _mapper.Map<Coupon>(couponDto);
				_db.Coupons.Update(couponUpdate);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpDelete("DeleteCoupon/{id}")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Delete(int id)
		{
			try
			{
				Coupon coupon = _db.Coupons.FirstOrDefault(u => u.CouponId == id);
				if (coupon == null) { return NotFound(); }
				_db.Coupons.Remove(coupon);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

	}
}
