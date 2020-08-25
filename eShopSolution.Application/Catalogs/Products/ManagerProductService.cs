using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModel.Catalog.Products;
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using eShopSolution.Application.Common;

namespace eShopSolution.Application.Catalogs.Products
{
    public class ManagerProductService : IManagerProductService
    {
        private readonly EShopDBContext _context;
        private readonly IStorageService _storageService;
        public ManagerProductService(EShopDBContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = _storageService;
        }

        public Task<int> AddImages(int ProductId, List<IFormFile> files)
        {
            throw new NotImplementedException();
        }

        public async Task AddViewCount(int ProductId)
        {
            var product = await _context.Products.FindAsync(ProductId);
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
                var product = new Product()
                {
                    Price = request.Price,
                    OriginalPrice =  request.OriginalPrice,
                    Stock = request.Stock,
                    ViewCount = 0,
                    DateCreated = DateTime.Now,
                    ProductTranslations = new List<ProductTranslation>()
                    {
                        new ProductTranslation{
                            Name = request.Name,
                            Description = request.Description,
                            Details = request.Details,
                            SeoDescription = request.SeoDescription,
                            SeoAlias = request.SeoAlias,
                            SeoTitle = request.SeoTitle,
                            LanguageId = request.LanguageId
                        }
                    }

                };
            // save Image
            if(request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Caption ="thumbnailImage",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
                _context.Products.Add(product);
                return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int productID)
        {
            var product = await _context.Products.FindAsync(productID);
            if (product == null) throw new EShopException($"khong tim thay idproduct{productID}");
            var images = _context.productImages.Where(i => i.ProductId == productID);
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<PageResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
        {
            // 1. cau lenh query
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };
            /*
                 p in _context.Products
                 pt in _context.ProductTranslations
                 Pic in _context.ProductInCategories
                 c in _context.Categories
            câu lệnh có nghĩa: from p join pt (p.Id = pt.ProductId)
                                    p join pic (p.Id = pic.ProductId)
                                    pic join c (pic.CategoryId = c.Id)
                                select new {p, pt, pic}
             */
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword)) //nếu keyword KHÔNG rỗng
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));

            if(request.CategoryIds.Count > 0) { 
                query = query.Where(x => request.CategoryIds.Contains(x.pic.CategoryId));
            }
            //3. paging
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductViewModel 
                { 
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount
                })
                .ToListAsync();
            //4. select and projection
            var pageResult = new PageResult<ProductViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };
            return pageResult;
        }

        public async Task<List<ProductViewModel>> Getvalue()
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };
            var data = await query.Select(x => new ProductViewModel()
            {
                Id = x.p.Id,
                Name = x.pt.Name,
                DateCreated = x.p.DateCreated,
                Description = x.pt.Description,
                Details = x.pt.Details,
                LanguageId = x.pt.LanguageId,
                OriginalPrice = x.p.OriginalPrice,
                Price = x.p.Price,
                SeoAlias = x.pt.SeoAlias,
                SeoDescription = x.pt.SeoDescription,
                SeoTitle = x.pt.SeoTitle,
                Stock = x.p.Stock,
                ViewCount = x.p.ViewCount
            })
                .ToListAsync();
            return data;
        }

        public Task<int> RemoveImages(int ImageId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product =await _context.Products.FindAsync(request.Id);
            var productTranslations =await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id 
            && x.LanguageId == request.LanguageId);
            if(product == null || productTranslations == null)
            {
                throw new EShopException($"khong tim thay product voi id{request.Id}");
            }
            productTranslations.Name = request.Name;
            productTranslations.SeoAlias = request.SeoAlias;
            productTranslations.SeoDescription = request.SeoDescription;
            productTranslations.SeoTitle = request.SeoTitle;
            productTranslations.Description = request.Description;
            productTranslations.Details = request.Details;
            // save image
            if(request.ThumbnailImage != null)
            {
                var thumbnailImage = await _context.productImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);
                if(thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _context.productImages.Update(thumbnailImage);
                }
            }
            return await _context.SaveChangesAsync();


        }

        public Task<int> UpdateImages(int ImageId, string Caption, string isDefault)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePrice(int ProductId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(ProductId);
            if(product == null)
            {
                throw new EShopException($"khong tim thay product voi id{ProductId}");
            }
            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStoke(int ProductId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(ProductId);
            if (product == null)
            {
                throw new EShopException($"khong tim thay product voi id{ProductId}");
            }
            product.Stock += addedQuantity;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<string> SaveFile(IFormFile file)
        {
            var originanlFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originanlFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        
    }
}
