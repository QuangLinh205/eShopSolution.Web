
using eShopSolution.ViewModel.Catalog.Products;
using eShopSolution.ViewModel.Catalog.Products.Manager;
using eShopSolution.ViewModel.Common;
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
        Task<PageResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);

    }
}
