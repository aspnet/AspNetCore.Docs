using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;
using RazorPagesMovie.Utilities;

namespace RazorPagesMovie.Pages.Schedules
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Models.MovieContext _context;

        public IndexModel(RazorPagesMovie.Models.MovieContext context)
        {
            _context = context;
        }

        #region snippet1
        [BindProperty]
        public FileUpload FileUpload { get; set; }
        #endregion

        #region snippet2
        public IList<Schedule> Schedules { get; private set; }
        #endregion

        #region snippet3
        public async Task OnGetAsync()
        {
            Schedules = await _context.Schedules.AsNoTracking().ToListAsync();
        }
        #endregion

        #region snippet4
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Schedules = await _context.Schedules.AsNoTracking().ToListAsync();
                return Page();
            }

            var PublicScheduleScheduleData = 
                await FileHelpers.ProcessSchedule(FileUpload.UploadPublicSchedule);

            var PrivateScheduleScheduleData = 
                await FileHelpers.ProcessSchedule(FileUpload.UploadPrivateSchedule);

            var document = new Schedule() 
                { 
                    PublicSchedule = PublicScheduleScheduleData.RawText, 
                    PublicScheduleSize = PublicScheduleScheduleData.Size, 
                    PrivateSchedule = PrivateScheduleScheduleData.RawText, 
                    PrivateScheduleSize = PrivateScheduleScheduleData.Size, 
                    Title = FileUpload.Title, 
                    UploadDT = DateTime.UtcNow
                };

            _context.Schedules.Add(document);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        #endregion
    }
}
