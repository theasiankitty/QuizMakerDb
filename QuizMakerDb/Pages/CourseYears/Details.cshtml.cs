using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuizMakerDb.Pages.CourseYears
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public static string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

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
                Name = courseYear.Name,
                CourseId = courseYear.Id,
                CourseName = courseYear.CourseInfo.Name,
                Year = GetEnumDisplayName((YearLevel)courseYear.Year),
                Active = courseYear.Active,
                CreatedBy = courseYear.CreatedBy,
                CreatedDate = courseYear.CreatedDate,
                UpdatedBy = courseYear.UpdatedBy,
                UpdatedDate = courseYear.UpdatedDate
            };

            return Page();
        }
    }
}
