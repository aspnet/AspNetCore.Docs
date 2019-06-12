﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PartialViewsSample.Pages
{
    public class DiscoveryModel : PageModel
    {
        public void OnGet() => Page();

        #region snippet_OnGetPartial
        //public IActionResult OnGetPartial() =>
        //    Partial("_AuthorPartialRP");
        public IActionResult OnGetPartial() =>
            new PartialViewResult
            {
                ViewName = "_AuthorPartialRP",
                ViewData = ViewData,
            };
        #endregion
    }
}
