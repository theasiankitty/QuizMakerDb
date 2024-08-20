using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.SchoolYears
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SchoolYearVM SchoolYearVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schoolYear = await _context.SchoolYears.FirstOrDefaultAsync(m => m.Id == id);
            if (schoolYear == null)
            {
                return NotFound();
            }

            SchoolYearVM = new SchoolYearVM
            {
                Id = schoolYear.Id,
                Name = schoolYear.Name,
                Active = schoolYear.Active,
                CreatedBy = schoolYear.CreatedBy,
                CreatedDate = schoolYear.CreatedDate,
                UpdatedBy = schoolYear.UpdatedBy,
                UpdatedDate = schoolYear.UpdatedDate,
            };

            return Page();
        }
    }
}
