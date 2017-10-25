﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ModelProvidersSample.Pages.OtherPages
{
    [ReplaceRouteValueFilter]
    public class Page3Model : PageModel
    {
        public string Message { get; private set; }

        public string RouteDataGlobalAttribute { get; private set; }

        public string RouteDataOtherPagesAttribute { get; private set; }

        public void Get()
        {
            Message = "Your application Page3 page.";

            if (RouteData.Values["globalAttribute"] != null)
            {
                RouteDataGlobalAttribute = $"Route data for 'globalAttribute' was provided: {RouteData.Values["globalAttribute"]}";
            }

            if (RouteData.Values["otherPagesAttribute"] != null)
            {
                RouteDataOtherPagesAttribute = $"Route data for 'otherPagesAttribute' was provided: {RouteData.Values["otherPagesAttribute"]}";
            }
        }
    }
}
