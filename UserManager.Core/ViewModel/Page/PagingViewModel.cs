using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Core.ViewModel.Page
{
    public class PagingViewModel
    {
        public string PageLink { get; set; }
        public string PageText { get; set; }
        public string Class { get; set; }
        public bool Active { get; set; }
        public bool Enable { get; set; }
    }

    public class PageViewModel
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public bool SortDesc { get; set; }
        public string Sort { get; set; }
        public string Search { get; set; }
        public int Take { get; set; }
        public int Count { get; set; }
    }
}
