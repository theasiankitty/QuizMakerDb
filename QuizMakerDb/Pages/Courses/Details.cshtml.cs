using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Courses
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public CourseVM CourseVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            CourseVM = new CourseVM
            {
                Id = course.Id,
                Name = course.Name,
                Active = course.Active,
                CreatedBy = course.CreatedBy,
                CreatedDate = course.CreatedDate,
                UpdatedBy = course.UpdatedBy,
                UpdatedDate = course.UpdatedDate,
            };

            return Page();
        }
    }
}
