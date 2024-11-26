using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.QuizTakes
{
    public class StudentQuizResultModel : PageModel
    {
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public StudentQuizResultModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_context = context;
			_userManager = userManager;
		}

		public QuizTakeResultVM QuizTakeResultVM { get; set; } = default!;
		public int QuizTakeId { get; set; }
		public class QuizQuestionDetailVM
		{
			public string QuestionText { get; set; } = string.Empty;
			public int Points { get; set; }
			public bool IsCorrect { get; set; }
			public List<string> StudentAnswers { get; set; } = new();
			public List<string> CorrectAnswers { get; set; } = new();
		}
		public class AnswerDetailVM
		{
			public string AnswerText { get; set; } = string.Empty!;
			public int? Order { get; set; }
		}

		public List<QuizQuestionDetailVM> QuizDetails { get; set; } = new();

		public async Task<IActionResult> OnGetAsync(int quizTakeId)
		{
			QuizTakeId = quizTakeId;

			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound();
			}

			var student = await _context.Students
				.FirstOrDefaultAsync(m => m.UserId == user.Id);

			if (student == null)
			{
				return NotFound();
			}

			var quizTake = await _context.QuizTakes
				.Include(m => m.QuizInfo)
				.FirstOrDefaultAsync(m => m.StudentId == student.Id && m.Active);

			if (quizTake == null)
			{
				return NotFound();
			}

			var quizResults = await _context.QuizResults
				.Where(m => m.Active && m.AnswerStudentInfo.QuizTakeId == quizTakeId)
				.Include(m => m.AnswerStudentInfo)
				.ThenInclude(m => m.QuizQuestionInfo)
				.ToListAsync();

			var groupedResults = quizResults
				.GroupBy(r => r.AnswerStudentInfo.QuizQuestionId)
				.ToList();

			int totalScore = 0;

			foreach (var group in groupedResults)
			{
				bool allCorrect = group.All(r => r.isCorrect);
				int quizQuestionPoints = group.First().AnswerStudentInfo.QuizQuestionInfo.Points;

				if (allCorrect)
				{
					totalScore += quizQuestionPoints;
				}

				//In the answers, if the questionType is 2 or 3 please include the order and display them beside the answer
				QuizDetails.Add(new QuizQuestionDetailVM
				{
					QuestionText = group.First().AnswerStudentInfo.QuizQuestionInfo.Description,
					Points = quizQuestionPoints,
					IsCorrect = allCorrect,
					StudentAnswers = group.Select(r => r.AnswerStudentInfo.Answer).ToList(),
					CorrectAnswers = _context.QuestionAnswers
						.Where(a => a.QuizQuestionId == group.Key && a.isCorrect)
						.Select(a => a.Answer)
						.ToList()
				});

			}

			QuizTakeResultVM = new QuizTakeResultVM
			{
				Score = totalScore,
				Title = quizTake.QuizInfo.Title,
				Introduction = quizTake.QuizInfo.Introduction,
				ShowResults = quizTake.QuizInfo.ShowResults
			};

			// What if I want to show all the Quiz Take Result here in one page?

			return Page();
		}
	}
}
