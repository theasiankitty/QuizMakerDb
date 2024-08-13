using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuizMakerDb.Pages.CourseYears
{
	public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public CourseYearVM CourseYearVM { get; set; } = default!;

        public static string GetDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

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
    }
}
