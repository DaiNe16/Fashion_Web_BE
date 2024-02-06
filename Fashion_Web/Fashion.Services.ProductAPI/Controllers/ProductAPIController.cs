using AutoMapper;
using Fashion.Services.ProductAPI.Data;
using Fashion.Services.ProductAPI.Models;
using Fashion.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Services.ProductAPI.Controllers
{
	[Route("api/product")]
	[ApiController]
	public class ProductAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private ResponseDto _response;
		private IMapper _mapper;
		private IWebHostEnvironment _environment;
		public ProductAPIController(AppDbContext appDbContext, IMapper mapper, IWebHostEnvironment environment)
		{
			_db = appDbContext;
			_mapper = mapper;
			_response = new ResponseDto();
			_environment = environment;
		}

		[HttpPost("UploadProductImage")]
		[Authorize(Roles = "admin")]
		public async Task<ResponseDto> UploadProductImage(int productId, IFormFile source)
		{
			try
			{
				var product = _db.Products.FirstOrDefault(u => u.ProductId == productId);
				if (product == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Product with id " + productId + " is invalid.";
					return _response;
				}
				string fileName = source.FileName;
				string folderPath = GetFolderPath(productId);
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}
				string imagePath = folderPath + "\\image.png";
				if (System.IO.File.Exists(imagePath))
				{
					System.IO.File.Delete(imagePath);
				}
				using (FileStream stream = System.IO.File.Create(imagePath))
				{
					await source.CopyToAsync(stream);
				}
				product.ProductUrl = GetImagePathByProductId(product.ProductId);
				_db.Products.Update(product);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpDelete("RemoveProductImage")]
		[Authorize(Roles = "admin")]
		public async Task<ResponseDto> RemoveProductImage(int productId)
		{
			try
			{
				var product = _db.Products.FirstOrDefault(u => u.ProductId == productId);
				if (product == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Product with id " + productId + " is invalid.";
					return _response;
				}
				string folderPath = GetFolderPath(productId);
				if (Directory.Exists(folderPath))
				{
					Directory.Delete(folderPath, true);
				}
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[NonAction]
		private string GetFolderPath(int productId)
		{
			return _environment.WebRootPath + "\\Uploads\\Product\\" + productId;
		}

		[NonAction]
		private string GetImagePathByProductId(int productId)
		{
			string hostPath = "https://localhost:7003";
			string imagePath = GetFolderPath(productId) + "\\image.png";
			string imageUrl = "";
			if (System.IO.File.Exists(imagePath))
			{
				imageUrl = hostPath + "/Uploads/Product/" + productId + "/image.png";
			}
			return imageUrl;
		}

		[HttpGet("GetAll")]
		public ResponseDto Get(int offset = 0, int limit = 1000)
		{
			try
			{
				IEnumerable<ProductDto> listProductDto = _mapper.Map<IEnumerable<ProductDto>>(_db.Products.Skip(offset).Take(limit).ToList());
				List<ProductDto> listPro = new List<ProductDto>(listProductDto);
				listPro.ForEach(p =>
				{
					p.ProductUrl = GetImagePathByProductId(p.ProductId);
				});
				
				_response.Result = listPro;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet("GetProductById/{id}")]
		[Authorize]
		public ResponseDto Get(int id)
		{
			try
			{
				var product = _mapper.Map<ProductDto>(_db.Products.FirstOrDefault(u => u.ProductId == id));
				if (product == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Can not found product.";
					return _response;
				}
				product.ProductUrl = GetImagePathByProductId(product.ProductId);
				_response.Result = product;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost("CreateProduct")]
		[Authorize(Roles = "admin")]
		public ResponseDto Post(ProductDto productDto)
		{
			try
			{
				Product product = _mapper.Map<Product>(productDto);
				product.ProductId = 0;
				_db.Add(product);
				_db.SaveChanges();
				_response.Message = "Successfully to create product";
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;

		}

		[HttpPut("UpdateProduct")]
		[Authorize(Roles = "admin")]
		public ResponseDto Put(ProductDto productDto)
		{
			try
			{
				var product_db = _db.Products.AsNoTracking().FirstOrDefault(u => u.ProductId == productDto.ProductId);
				if (product_db == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Product is not found";
					return _response;
				}
				Product product = _mapper.Map<Product>(productDto);
				_db.Update(product);
				_db.SaveChanges();
				_response.Message = "Successfully to update product";
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;

		}

		[HttpDelete("DeleteProduct/{id}")]
		[Authorize(Roles = "admin")]
		public ResponseDto Delete(int id)
		{
			try
			{
				var product_db = _db.Products.FirstOrDefault(u => u.ProductId == id);
				if (product_db == null)
				{
					_response.IsSuccess = false;
					_response.Message = "Product is not found";
					return _response;
				}
				string folderPath = GetFolderPath(id);
				if (Directory.Exists(folderPath))
				{
					Directory.Delete(folderPath, true);
				}
				_db.Remove(product_db);
				_db.SaveChanges();
				_response.Message = "Successfully to delete product";
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
