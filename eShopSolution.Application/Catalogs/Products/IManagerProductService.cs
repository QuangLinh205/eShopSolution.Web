using eShopSolution.ViewModel.Catalog.ProductImages;
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalogs.Products
{
    public interface IManagerProductService
    {
        Task<int> Create(ProductCreateRequest request); // phương thức create trả về kiểu int

        Task<int> Update(ProductUpdateRequest request);

        Task<bool> UpdatePrice(int ProductId, decimal newPrice);

        Task<bool> UpdateStoke(int ProductId, int addedQuantity);

        Task AddViewCount(int ProductId);

        Task<int> Delete(int productID);

        Task<PageResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request);

        Task<List<ProductViewModel>> Getvalue();

        Task<ProductViewModel> GetById(int ProductId, string LanguageId);

        Task<int> AddImage(int ProductId, ProductImageCreateRequest productImage);

        Task<int> RemoveImage(int imageId);

        Task<int> UpdateImage(int imageId, ProductImageUpdateRequest productImage);

        Task<List<ProductImageViewModel>> GetListImages(int productId);

        Task<ProductImageViewModel> GetImageById(int imageId);
    }
}