using eShopSolution.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModel.Catalog.Products.Public
{
    public class GetProductPaginhRequest : PagingRequestBase // kế thừa 2 thuộc tính PageIndex, PageSize
    {
        public int? CategoryId { get; set; }
    }
}
