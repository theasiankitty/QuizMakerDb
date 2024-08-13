using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.SchoolYears
{
	public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schoolYear = await _context.SchoolYears.FindAsync(id);

            if (schoolYear != null)
            {
                _context.SchoolYears.Remove(schoolYear);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
