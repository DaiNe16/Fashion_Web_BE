using AutoMapper;
using Fashion.Services.ProductAPI.Data;
using Fashion.Services.ProductAPI.Models;
using Fashion.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fashion.Services.ProductAPI.Controllers
{
	[Route("api/SubCategory")]
	[ApiController]
	public class SubCategoryAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private ResponseDto _response;
		private IMapper _mapper;
		public SubCategoryAPIController(AppDbContext appDbContext, IMapper mapper)
		{
			_db = appDbContext;
			_mapper = mapper;
			_response = new ResponseDto();

		}

		[HttpGet("GetAllSubCategoryByCategoryId/{categoryId}")]
		public ResponseDto GetAllSubCategoryByCategoryId(int categoryId)
		{
			try
			{
				var SubCategories = _mapper.Map<IEnumerable<SubCategoryDto>>(_db.SubCategories.Where(s => s.CategoryId == categoryId).ToList());
				_response.Result = SubCategories;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet("GetAll")]
		public ResponseDto Get()
		{
			try
			{
				var SubCategories = _mapper.Map<IEnumerable<SubCategoryDto>>(_db.SubCategories);
				_response.Result = SubCategories;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet("GetSubCategoryById")]
		public ResponseDto GetSubCategoryById(int SubCategoryId)
		{
			try
			{
				var SubCategory = _mapper.Map<SubCategoryDto>(_db.SubCategories.FirstOrDefault(u => u.SubCategoryId == SubCategoryId));
				if(SubCategory == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Can not Found";
					return _response;	
				}
				_response.Result = SubCategory;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost("CreateSubCategory")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Post(SubCategoryDto SubCategoryDto)
		{
			try
			{
				SubCategory SubCategory = _mapper.Map<SubCategory>(SubCategoryDto);
				SubCategory.SubCategoryId = 0;
				_db.SubCategories.Add(SubCategory);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPut("UpdateSubCategory")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Put(SubCategoryDto SubCategoryDto)
		{
			try
			{
				SubCategory SubCategory = _db.SubCategories.AsNoTracking().FirstOrDefault(u => u.SubCategoryId == SubCategoryDto.SubCategoryId);
				if (SubCategory == null) { return NotFound(); }
				SubCategory SubCategoryUpdate = _mapper.Map<SubCategory>(SubCategoryDto);
				_db.SubCategories.Update(SubCategoryUpdate);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpDelete("DeleteSubCategory/{id}")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Delete(int id)
		{
			try
			{
				SubCategory SubCategory = _db.SubCategories.FirstOrDefault(u => u.SubCategoryId == id);
				if (SubCategory == null) { return NotFound(); }
				_db.SubCategories.Remove(SubCategory);
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
