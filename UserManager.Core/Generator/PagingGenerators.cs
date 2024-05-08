using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManager.Core.ViewModel.Page;

namespace UserManager.Core.Generator
{
    public class PagingGenerators
    {
        public static List<PagingViewModel> Paging(int PageCount, int Page)
        {
            List<PagingViewModel> model = new List<PagingViewModel>();
            if (Page > 1)
            {
                model.Add(new PagingViewModel { PageText = "", PageLink = (Page - 1).ToString(), Active = false, Class = "bx bx-chevron-left", Enable = true });
            }
            else
            {
                model.Add(new PagingViewModel { PageText = "", PageLink = "", Active = false, Class = "bx bx-chevron-left", Enable = false });
            }
            if ((PageCount + 1) < 10)
            {
                for (int i = 1; i <= (PageCount + 1); i++)
                {
                    model.Add(new PagingViewModel { PageText = i.ToString(), PageLink = i.ToString(), Active = Page == i ? true : false, Class = " ", Enable = true });
                }
            }
            else
            {
                if (Page > 5 && Page < ((PageCount + 1) - 4))
                {
                    model.Add(new PagingViewModel { PageText = "1", PageLink = "1", Active = Page == 1 ? true : false, Class = " ", Enable = true });
                    model.Add(new PagingViewModel { PageText = "...", PageLink = "", Active = false, Class = " ", Enable = false });
                    model.Add(new PagingViewModel { PageText = (Page - 3).ToString(), PageLink = (Page - 3).ToString(), Active = Page == (Page - 3) ? true : false, Class = " ", Enable = true });
                    model.Add(new PagingViewModel { PageText = (Page - 2).ToString(), PageLink = (Page - 2).ToString(), Active = Page == (Page - 2) ? true : false, Class = " ", Enable = true });
                    model.Add(new PagingViewModel { PageText = (Page - 1).ToString(), PageLink = (Page - 1).ToString(), Active = Page == (Page - 1) ? true : false, Class = " ", Enable = true });
                    model.Add(new PagingViewModel { PageText = Page.ToString(), PageLink = Page.ToString(), Active = true, Class = " ", Enable = true });
                    model.Add(new PagingViewModel { PageText = (Page + 1).ToString(), PageLink = (Page + 1).ToString(), Active = Page == (Page + 1) ? true : false, Class = " ", Enable = true });
                    model.Add(new PagingViewModel { PageText = (Page + 2).ToString(), PageLink = (Page + 2).ToString(), Active = Page == (Page + 2) ? true : false, Class = " ", Enable = true });
                    model.Add(new PagingViewModel { PageText = "...", PageLink = "", Active = false, Class = " ", Enable = false });
                    model.Add(new PagingViewModel { PageText = (PageCount + 1).ToString(), PageLink = (PageCount + 1).ToString(), Active = Page == (PageCount + 1) ? true : false, Class = " ", Enable = true });
                }
                else if (Page <= 5)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        model.Add(new PagingViewModel { PageText = i.ToString(), PageLink = i.ToString(), Active = Page == i ? true : false, Class = " ", Enable = true });
                    }
                    model.Add(new PagingViewModel { PageText = "...", PageLink = "", Active = false, Class = " ", Enable = false });
                    model.Add(new PagingViewModel { PageText = (PageCount + 1).ToString(), PageLink = (PageCount + 1).ToString(), Active = Page == (PageCount + 1) ? true : false, Class = " ", Enable = true });
                }
                else if (Page > (PageCount + 1) - 5)
                {
                    model.Add(new PagingViewModel { PageText = "1", PageLink = "1", Active = Page == 1 ? true : false, Class = " ", Enable = true });
                    model.Add(new PagingViewModel { PageText = "...", PageLink = "", Active = false, Class = " ", Enable = false });
                    for (int i = 1; i <= 8; i++)
                    {
                        model.Add(new PagingViewModel { PageText = (((PageCount + 1) - 8) + i).ToString(), PageLink = (((PageCount + 1) - 8) + i).ToString(), Active = Page == (((PageCount + 1) - 8) + i) ? true : false, Class = " ", Enable = true });
                    }
                }
            }
            if (Page < (PageCount + 1))
            {
                model.Add(new PagingViewModel { PageText = "", PageLink = (Page + 1).ToString(), Active = false, Class = "bx bx-chevron-right", Enable = true });
            }
            else
            {
                model.Add(new PagingViewModel { PageText = "", PageLink = "", Active = false, Class = "bx bx-chevron-right", Enable = false });
            }
            return model;
        }
    }
}
