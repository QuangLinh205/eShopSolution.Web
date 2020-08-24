using eShopSolution.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModel.Catalog.Products
{
    public class GetManageProductPagingRequest : PagingRequestBase // kế thừa pagingRequestBase có 2 trường PageIndex và PageSize
    {
        public string Keyword { get; set; }
        public List<int> CategoryIds { get; set; }
        //public int PageIndex { get; set; }
        //public int PageSize { get; set; }
    }
}
