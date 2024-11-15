using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.QuizQuestions
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public IList<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
		public QuizVM QuizVM { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? quizId)
		{
			if (quizId == null)
			{
				return NotFound();
			}

			var quiz = await _context.Quizzes
				.Include(m => m.TeacherInfo)
				.Where(m => m.Active)
				.SingleOrDefaultAsync(m => m.Id == quizId);

			if (quiz == null)
			{
				return NotFound();
			}

			QuizVM = new QuizVM
			{
				Id = quiz.Id,
				Title = quiz.Title,
				Introduction = quiz.Introduction,
			};

			return Page();
		}
	}
}