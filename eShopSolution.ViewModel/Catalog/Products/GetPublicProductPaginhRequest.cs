using eShopSolution.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModel.Catalog.ProductImages
{
    public class GetPublicProductPaginhRequest : PagingRequestBase // kế thừa 2 thuộc tính PageIndex, PageSize
    {
        public int? CategoryId { get; set; }
    }
}
