using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.Teachers
{
    public class RetrieveModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RetrieveModel(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var updater = await _userManager.GetUserAsync(User);

            if (updater == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            else
            {
                teacher.Active = true;
                teacher.UpdatedBy = updater.Id;
                teacher.UpdatedDate = DateTime.Now;
            }

            var teacherIdentity = await _userManager.FindByIdAsync(teacher.UserId.ToString());

            if (teacherIdentity == null)
            {
                return NotFound();
            }
            else
            {
                teacherIdentity.Active = true;
                await _userManager.UpdateAsync(teacherIdentity);
            }

            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

