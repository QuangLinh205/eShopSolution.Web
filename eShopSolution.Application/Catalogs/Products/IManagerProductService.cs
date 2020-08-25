using eShopSolution.ViewModel.Catalog.Products;
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
        Task<int> AddImages(int ProductId, List<IFormFile> files); // thêm ảnh vào sản phẩm nào, thêm vào 1 danh sách file
        Task<int> RemoveImages(int ImageId);
        Task<int> UpdateImages(int ImageId, string Caption, string isDefault);
        Task<List<ProductViewModel>> Getvalue();
        Task<string> SaveFile(IFormFile file);





    }
}
