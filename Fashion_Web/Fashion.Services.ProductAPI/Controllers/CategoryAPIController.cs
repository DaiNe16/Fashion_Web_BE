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
	[Route("api/category")]
	[ApiController]
	public class CategoryAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private ResponseDto _response;
		private IMapper _mapper;
		public CategoryAPIController(AppDbContext appDbContext, IMapper mapper)
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
				var categories = _mapper.Map<IEnumerable<CategoryDto>>(_db.Categories);
				_response.Result = categories;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet("GetCategoryById")]
		public ResponseDto GetCategoryById(int CategoryId)
		{
			try
			{
				var category = _mapper.Map<CategoryDto>(_db.Categories.FirstOrDefault(u => u.CategoryId == CategoryId));
				if(category == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Can not Found";
					return _response;	
				}
				_response.Result = category;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost("CreateCategory")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Post(CategoryDto categoryDto)
		{
			try
			{
				Category category = _mapper.Map<Category>(categoryDto);
				category.CategoryId = 0;
				_db.Categories.Add(category);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPut("UpdateCategory")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Put(CategoryDto categoryDto)
		{
			try
			{
				Category category = _db.Categories.AsNoTracking().FirstOrDefault(u => u.CategoryId == categoryDto.CategoryId);
				if (category == null) { return NotFound(); }
				Category categoryUpdate = _mapper.Map<Category>(categoryDto);
				_db.Categories.Update(categoryUpdate);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpDelete("DeleteCategory/{id}")]
		[Authorize(Roles = "admin")]
		public ActionResult<ResponseDto> Delete(int id)
		{
			try
			{
				Category category = _db.Categories.FirstOrDefault(u => u.CategoryId == id);
				if (category == null) { return NotFound(); }
				_db.Categories.Remove(category);
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
