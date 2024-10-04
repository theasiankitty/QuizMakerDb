using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Students
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
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

            var updater = await _userManager.GetUserAsync(User);

            if (updater == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            else
            {
                student.Active = false;
                student.UpdatedBy = updater.Id;
                student.UpdatedDate = DateTime.Now;
            }

            var studentIdentity = await _userManager.FindByIdAsync(student.UserId.ToString());

            if (studentIdentity == null)
            {
                return NotFound();
            }
            else
            {
                studentIdentity.Active = false;
                await _userManager.UpdateAsync(studentIdentity);
            }

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
