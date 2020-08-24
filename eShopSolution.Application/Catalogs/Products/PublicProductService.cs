
using eShopSolution.Data.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.Utilities.Exceptions;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModel.Catalog.Products;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.Catalog.Products.Public;

namespace eShopSolution.Application.Catalogs.Products
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDBContext _context;
        public PublicProductService(EShopDBContext context)
        {
            _context = context;
        }

        public async Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetProductPaginhRequest request)
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
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
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
    }
    }

