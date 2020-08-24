﻿using eShopSolution.ViewModel.Catalog.Products;
using eShopSolution.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalogs.Products
{
    public interface IPublicProductService
    {
        Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetPublicProductPaginhRequest request);
        Task<List<ProductViewModel>> GetAll();
    }
}
