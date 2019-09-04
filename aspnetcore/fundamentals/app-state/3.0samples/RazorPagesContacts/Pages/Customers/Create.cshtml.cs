﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesContacts.Models;
using System.Threading.Tasks;

namespace RazorPagesContacts.Pages.Customers
{
    #region snippet
    public class CreateModel : PageModel
    {
        private readonly RazorPagesContacts.Data.RazorPagesContactsContext _context;

        public CreateModel(RazorPagesContacts.Data.RazorPagesContactsContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [TempData]
        public string Message { get; set; }

        [BindProperty]
        public Customer Customer { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Customer.Add(Customer);
            await _context.SaveChangesAsync();
            Message = $"Customer {Customer.Name} added";

            return RedirectToPage("./IndexPeek");
        }
    }
    #endregion
}