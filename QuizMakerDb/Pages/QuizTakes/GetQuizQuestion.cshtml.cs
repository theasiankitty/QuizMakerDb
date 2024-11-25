using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.QuizTakes
{
    public class GetQuizQuestionModel : PageModel
	{
        private readonly ApplicationDbContext _context;

		public GetQuizQuestionModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetQuestionAsync([FromQuery] int quizId, int quizTakeId, int studentId)
		{
			if (quizId == 0 || quizTakeId == 0)
			{
				return new JsonResult(new { message = "Something went wrong." });
			}

			var quiz = await _context.Quizzes.FirstOrDefaultAsync(m => m.Id == quizId);

			if (quiz == null)
			{
				return new JsonResult(new { message = "Invalid Quiz." });
			}

			var quizTake = await _context.QuizTakes.FirstOrDefaultAsync(m => m.Id == quizTakeId);

			if (quizTake == null)
			{
				return new JsonResult(new { message = "Invalid Quiz Take." });
			}

			try
			{
				// Get the list of answered question IDs for this quiz take
				var answeredQuestions = await _context.AnswerStudents
					.Where(m => m.QuizTakeId == quizTake.Id && m.Active)
					.Select(m => m.QuizQuestionId)
					.ToListAsync();

				// Get remaining unanswered questions
				var remainingQuestions = await _context.QuizQuestions
					.Where(q => q.QuizId == quizId && !answeredQuestions.Contains(q.Id))
					.ToListAsync();

				// Check if there are any unanswered questions
				if (!remainingQuestions.Any())
				{
					// update isFinished in QuizTake
					quizTake.isFinished = true;
					_context.QuizTakes.Update(quizTake);
					await _context.SaveChangesAsync();

					return new JsonResult(new { message = "All questions answered.", question = (object)null });
				}

				QuizQuestion? quizQuestion = null;
				IList<QuestionAnswer>? questionAnswer = null;
				IList<QuestionItem>? questionItem = null;

				// If questions are randomized, pick a random question
				if (quiz.isQuestionRandomized)
				{
					quizQuestion = remainingQuestions.OrderBy(q => Guid.NewGuid()).FirstOrDefault();

					if (quizQuestion == null)
					{
						return new JsonResult(new { message = "ERROR", question = (object)null });
					}

					questionAnswer = await _context.QuestionAnswers
						.Where(m => m.QuizQuestionId == quizQuestion.Id)
						.ToListAsync();
				}
				else
				{
					// If not randomized, pick the next question based on order
					quizQuestion = remainingQuestions.OrderBy(q => q.Order).FirstOrDefault();

					if (quizQuestion == null)
					{
						return new JsonResult(new { message = "ERROR", question = (object)null });
					}

					questionAnswer = await _context.QuestionAnswers
						.Where(m => m.QuizQuestionId == quizQuestion.Id)
						.ToListAsync();
				}

				// If quizQuestion is null, return an error
				if (quizQuestion == null)
				{
					return new JsonResult(new { message = "No question available.", question = (object)null });
				}

				questionItem = await _context.QuestionItems
					.Where(m => m.QuizQuestionId == quizQuestion.Id && m.Active)
					.ToListAsync();

				// Return the selected question and its answers
				return new JsonResult(new
				{
					message = "OK",
					question = new
					{
						id = quizQuestion.Id,
						description = quizQuestion.Description,
						order = quizQuestion.Order,
						questionType = quizQuestion.QuestionType,
						image = quizQuestion.Image,
						points = quizQuestion.Points,
						quizId = quizQuestion.QuizId
					},
					answers = questionAnswer.Select(answer => new
					{
						id = answer.Id,
						answer = answer.Answer,
						image = answer.Image,
					}).ToList(),
					items = questionItem.Select(item => new
					{
						name = item.Name,
						order = item.Order
					}),
					allowEmptyAnswers = quiz.AllowEmptyAnswers,
					questionRandomized = quiz.isQuestionRandomized
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new { message = "An error occurred", error = ex.Message });
			}
		}

		public async Task<JsonResult> OnGetNextQuestionAsync([FromQuery] int order, int quizId, int quizTakeId, int studentId)
		{
			if (quizId == 0 || quizTakeId == 0)
			{
				return new JsonResult(new { message = "Something went wrong." });
			}

			var quiz = await _context.Quizzes.FirstOrDefaultAsync(m => m.Id == quizId);

			if (quiz == null)
			{
				return new JsonResult(new { message = "Invalid Quiz." });
			}

			var quizTake = await _context.QuizTakes.FirstOrDefaultAsync(m => m.Id == quizTakeId);

			if (quizTake == null)
			{
				return new JsonResult(new { message = "Invalid Quiz Take." });
			}

			try
			{
				// Get the list of answered question IDs for this quiz take
				var answeredQuestions = await _context.AnswerStudents
					.Where(m => m.QuizTakeId == quizTakeId && m.Active)
					.Select(m => m.QuizQuestionId)
					.ToListAsync();

				// Get all questions for the quiz, excluding answered questions
				var allQuestions = await _context.QuizQuestions
					.Where(q => q.QuizId == quizId && !answeredQuestions.Contains(q.Id))
					.OrderBy(q => q.Order)
					.ToListAsync();

				// If there are no remaining questions, finish the quiz
				if (!allQuestions.Any())
				{
					quizTake.isFinished = true;
					_context.QuizTakes.Update(quizTake);
					await _context.SaveChangesAsync();

					return new JsonResult(new { message = "All questions answered.", question = (object)null });
				}

				// Find the next question based on the provided order
				QuizQuestion nextQuestion = allQuestions.FirstOrDefault(q => q.Order > order);

				// If no question with a higher order is found, loop back to the first question
				if (nextQuestion == null)
				{
					nextQuestion = allQuestions.First();
				}

				// Fetch related answers and items
				var questionAnswers = await _context.QuestionAnswers
					.Where(m => m.QuizQuestionId == nextQuestion.Id)
					.ToListAsync();

				var questionItems = await _context.QuestionItems
					.Where(m => m.QuizQuestionId == nextQuestion.Id && m.Active)
					.ToListAsync();

				// Return the next question and its related data
				return new JsonResult(new
				{
					message = "OK",
					question = new
					{
						id = nextQuestion.Id,
						description = nextQuestion.Description,
						order = nextQuestion.Order,
						questionType = nextQuestion.QuestionType,
						image = nextQuestion.Image,
						points = nextQuestion.Points,
						quizId = nextQuestion.QuizId
					},
					answers = questionAnswers.Select(answer => new
					{
						id = answer.Id,
						answer = answer.Answer,
						image = answer.Image,
					}).ToList(),
					items = questionItems.Select(item => new
					{
						name = item.Name,
						order = item.Order
					}),
					allowEmptyAnswers = quiz.AllowEmptyAnswers,
					questionRandomized = quiz.isQuestionRandomized
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new { message = "An error occurred", error = ex.Message });
			}
		}
	}
}
