using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Quizzes
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
        public QuizVM QuizVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(m => m.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

			QuizVM = new QuizVM
			{
				Id = quiz.Id,
				Title = quiz.Title,
				Introduction = quiz.Introduction,
				isQuestionRandomized = quiz.isQuestionRandomized,
				AllowEmptyAnswers = quiz.AllowEmptyAnswers,
				isUnlimitedMinutes = quiz.isUnlimitedMinutes,
				Minutes = quiz.Minutes,
				isUnlimitedTakes = quiz.isUnlimitedTakes,
				Takes = quiz.Takes,
				TeacherId = quiz.TeacherId,
				Active = quiz.Active,
				CreatedBy = quiz.CreatedBy,
				CreatedDate = quiz.CreatedDate,
				UpdatedBy = quiz.UpdatedBy,
				UpdatedDate = quiz.UpdatedDate
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

			var quiz = await _context.Quizzes.FindAsync(id);

			if (quiz == null)
			{
				return NotFound();
			}

			quiz.Active = false;
			quiz.UpdatedBy = updater.Id;
			quiz.UpdatedDate = DateTime.Now;
			_context.Quizzes.Update(quiz);

			var quizSubjects = await _context.QuizSubjects
				.Where(m => m.QuizId == quiz.Id && m.Active)
				.ToListAsync();

			if (quizSubjects.Any())
			{
				foreach (var subject in quizSubjects)
				{
					subject.Active = false;
					subject.UpdatedBy = updater.Id;
					subject.UpdatedDate = DateTime.Now;
					_context.QuizSubjects.Update(subject);
				}
			}

			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
        }
    }
}
