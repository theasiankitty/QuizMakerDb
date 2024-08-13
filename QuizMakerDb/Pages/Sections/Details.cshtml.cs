using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Pages.Sections
{
    public class DetailsModel : PageModel
    {
        private readonly QuizMakerDb.Data.ApplicationDbContext _context;

        public DetailsModel(QuizMakerDb.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Section Section { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections.FirstOrDefaultAsync(m => m.Id == id);
            if (section == null)
            {
                return NotFound();
            }
            else
            {
                Section = section;
            }
            return Page();
        }
    }
}
