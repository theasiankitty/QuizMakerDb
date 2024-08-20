using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Subjects
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
        public SubjectVM SubjectVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            SubjectVM = new SubjectVM
            {
                Id = subject.Id,
                Title = subject.Title,
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

            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            subject.Active = false;
            subject.UpdatedBy = updater.Id;
            subject.UpdatedDate = DateTime.Now;

            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
