using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using QuizMakerDb.Data.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

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

		public class QuestionItemData
		{
			public string Name { get; set; } = string.Empty!;
			public string Order { get; set; } = string.Empty!;
		}

		public class StudentAnswerData
		{
			public string? Answer { get; set; }
			public string? Order { get; set; }
		}

		public class CorrectAnswerData
		{
			public string Answer { get; set; } = string.Empty!;
			public string? Order { get; set; }
		}

		public class QuizQuestionData
		{
			public string QuestionText { get; set; } = string.Empty;
			public int Points { get; set; }
			public bool IsCorrect { get; set; }
			public List<StudentAnswerData> StudentAnswers { get; set; } = new();
			public List<CorrectAnswerData> CorrectAnswers { get; set; } = new();
			public List<QuestionItemData> QuestionItems { get; set; } = new();
			public List<string> Orders { get; set; } = new();
		}

		public class AnswerData
		{
			public string AnswerText { get; set; } = string.Empty!;
			public int? Order { get; set; }
		}

		public List<QuizQuestionData> QuizDetails { get; set; } = new();

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
				.FirstOrDefaultAsync(m => m.Id == quizTakeId 
					&& m.StudentId == student.Id
					&& m.Active);

			if (quizTake == null)
			{
				return NotFound();
			}

			if (quizTake.isFinished == false)
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

				QuizDetails.Add(new QuizQuestionData
				{
					QuestionText = group.First().AnswerStudentInfo.QuizQuestionInfo.Description,
					Points = quizQuestionPoints,
					IsCorrect = allCorrect,
					StudentAnswers = group
						.Select(r => new StudentAnswerData
						{
							Answer = _context.QuestionAnswers
								.Where(a => a.QuizQuestionId == r.AnswerStudentInfo.QuizQuestionId && a.Answer == r.AnswerStudentInfo.Answer)
								.Select(a => a.Answer)
								.FirstOrDefault(),
							Order = r.AnswerStudentInfo.Answer,
						}).ToList(),
					CorrectAnswers = _context.QuestionAnswers
						.Where(a => a.QuizQuestionId == group.Key && a.isCorrect)
						.Select(a => new CorrectAnswerData
						{
							Answer = a.Answer,
							Order = group.First().AnswerStudentInfo.QuizQuestionInfo.QuestionType == 0 ? null : a.Order
						})
						.ToList(),
					QuestionItems = _context.QuestionItems
						.Where(m => m.QuizQuestionId == group
							.Select(m => m.AnswerStudentInfo.QuizQuestionId)
							.FirstOrDefault())
						.Select(m => new QuestionItemData
						{
							Name = m.Name,
							Order = m.Order
						})
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
