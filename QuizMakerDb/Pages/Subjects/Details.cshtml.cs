using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Subjects
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SubjectVM SubjectVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(m => m.CourseYearInfo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            SubjectVM = new SubjectVM
            {
                Id = subject.Id,
                Title = subject.Title,
                Code = subject.Code,
                CourseYear = subject.CourseYearInfo.Name,
                Semester = ((Semester)subject.Semester).ToString(),
            };

            return Page();
        }
    }
}
