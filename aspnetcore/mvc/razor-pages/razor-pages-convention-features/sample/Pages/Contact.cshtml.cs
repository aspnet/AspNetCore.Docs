﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ModelProvidersSample.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; private set; }

        public string RouteDataTextAttribute { get; private set; }

        public void Get()
        {
            Message = "Your contact page.";

            if (RouteData.Values["text"] != null)
            {
                RouteDataTextAttribute = $"Route data for 'text' was provided: {RouteData.Values["text"]}";
            }
        }
    }
}
