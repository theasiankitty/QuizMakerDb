using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.QuizQuestions
{
	public class LoadQuestionsModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public LoadQuestionsModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetQuestionsAsync([FromQuery] int quizId)
		{
			if (quizId == 0)
			{
				return new JsonResult(new { message = "Invalid Quiz." });
			}

			try
			{
				var questions = new List<object>();

				var quizQuestions = await _context.QuizQuestions
					.Where(m => m.QuizId == quizId && m.Active)
					.OrderByDescending(m => m.Order)
					.ToListAsync();

				foreach (var quizQuestion in quizQuestions)
				{
					var questionsAnswers = await _context.QuestionAnswers
						.Where(m => m.QuizQuestionId == quizQuestion.Id && m.Active)
						.ToListAsync();

					var questionItems = await _context.QuestionItems
						.Where(m => m.QuizQuestionId == quizQuestion.Id && m.Active)
						.Select(m => new
						{
							m.Id,
							m.Name,
							m.Order,
							m.QuizQuestionId,
							m.Active
						})
						.ToListAsync();

					string quizQuestionImage = quizQuestion.Image != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(quizQuestion.Image)}" : "";

					var questionWithAnswers = new
					{
						quizQuestion.Id,
						quizQuestion.Description,
						quizQuestion.Order,
						quizQuestion.QuestionType,
						quizQuestion.Points,
						Image = quizQuestionImage,
						quizQuestion.Active,
						Answers = questionsAnswers.Select(answer => new
						{
							answer.Id,
							answer.Answer,
							answer.isCorrect,
							answer.ShowAnswer,
							answer.Order,
							Image = answer.Image != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(answer.Image)}" : "",
							answer.Active
						}).ToList(),
						Items = questionItems,
					};

					questions.Add(questionWithAnswers);
				}

				return new JsonResult(new { message = "OK", questions });
			}
			catch (Exception ex)
			{
				return new JsonResult(new { message = "An error occurred", error = ex.Message });
			}
		}
	}
}