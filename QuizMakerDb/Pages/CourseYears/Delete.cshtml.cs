using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuizMakerDb.Pages.CourseYears
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

        public static string GetDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        [BindProperty]
        public CourseYearVM CourseYearVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseYear = await _context.CourseYears
                .Include(m => m.CourseInfo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (courseYear == null)
            {
                return NotFound();
            }

            CourseYearVM = new CourseYearVM
            {
                Id = courseYear.Id,
                Year = GetDisplayName((YearLevel)courseYear.Year),
                CourseName = courseYear.CourseInfo?.Name ?? "Unknown",
                Active = courseYear.Active,
                CreatedBy = courseYear.CreatedBy,
                CreatedDate = courseYear.CreatedDate,
                UpdatedBy = courseYear.UpdatedBy,
                UpdatedDate = courseYear.UpdatedDate,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseYear = await _context.CourseYears.FindAsync(id);
            if (courseYear != null)
            {
                _context.CourseYears.Remove(courseYear);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
