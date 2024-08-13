using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Students
{
	public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<AppUser> userManager)
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

            var student =  await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            StudentVM = new StudentVM
            {
                Id = student.Id,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName,
                LastName = student.LastName,
                Sex = student.Sex,
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var editor = await _userManager.GetUserAsync(User);

            if (editor == null)
            {
                return NotFound();
            }

            var student = new StudentVM
            {
                Id = StudentVM.Id,
                FirstName = StudentVM.FirstName,
                MiddleName = StudentVM.MiddleName,
                LastName = StudentVM.LastName,
                Sex = StudentVM.Sex,
                Email = StudentVM.Email,
                UserName = StudentVM.UserName,
                UserId = StudentVM.UserId,
                Active = StudentVM.Active,
                CreatedBy = StudentVM.CreatedBy,
                CreatedDate = StudentVM.CreatedDate,
                UpdatedBy = editor.Id,
                UpdatedDate = DateTime.Now,
            };

            var studentIdentity = await _userManager.FindByIdAsync(student.UserId.ToString());

            if (studentIdentity == null)
            {
                return NotFound();
            }

            studentIdentity.UserName = student.UserName;

            _context.Attach(StudentVM).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(StudentVM.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
