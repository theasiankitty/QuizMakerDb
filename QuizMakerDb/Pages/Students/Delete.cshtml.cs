using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Students
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
        public StudentVM StudentVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            StudentVM = new StudentVM
            {
                Id = student.Id,
                Student = student.FirstName + " " + student.LastName,
                SexDescription = ((Sex)student.Sex).ToString(),
                Email = student.Email,
                UserName = student.UserName,
                UserId = student.UserId,
                Active = student.Active,
                CreatedBy = student.CreatedBy,
                CreatedDate = student.CreatedDate,
                UpdatedBy = student.UpdatedBy,
                UpdatedDate = student.UpdatedDate,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(student.UserId);

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            else
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}
