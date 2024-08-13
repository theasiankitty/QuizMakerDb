using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Teachers
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public TeacherVM TeacherVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FirstOrDefaultAsync(m => m.Id == id);

            if (teacher == null)
            {
                return NotFound();
            }

            TeacherVM = new TeacherVM
            {
                Id = teacher.Id,
                Teacher = teacher.FirstName + " " + teacher.LastName,
                SexDescription = ((Sex)teacher.Sex).ToString(),
                Email = teacher.Email,
                UserName = teacher.UserName,
                UserId = teacher.UserId,
                Active = teacher.Active,
                CreatedBy = teacher.CreatedBy,
                CreatedDate = teacher.CreatedDate,
                UpdatedBy = teacher.UpdatedBy,
                UpdatedDate = teacher.UpdatedDate,
            };

            return Page();
        }
    }
}
