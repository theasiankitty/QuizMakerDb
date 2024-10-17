using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Quizzes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public QuizVM QuizVM { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
				return Page();
			}

			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return NotFound();
			}

            var teacherId = await _context.Teachers
                .Where(m => m.UserId == creator.Id)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();

            if (teacherId == 0)
            {
                return NotFound();
            }

			var quiz = new Quiz
            {
                Title = QuizVM.Title,
                Introduction = QuizVM.Introduction,
                isQuestionRandomized = QuizVM.isQuestionRandomized,
                ConclusionDescription = QuizVM.ConclusionDescription,
                Minutes = QuizVM.Minutes,
                Takes = QuizVM.Takes,
                ShowResults = QuizVM.ShowResults,
                TeacherId = teacherId,
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.Now,
				UpdatedBy = null,
				UpdatedDate = null
			};

			_context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
