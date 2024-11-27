using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Pages.AnswerStudents
{
	public class CreateModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public CreateModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public class AnswerWithOrder
		{
			public string Answer { get; set; } = string.Empty!;

			public string? Order { get; set; }
		}

		public class AnswerObject
		{
			public string Answer { get; set; } = string.Empty!;

			public string? Order { get; set; }
		}

		public class AnswerData
		{
			public List<AnswerObject> Answers { get; set; } = new List<AnswerObject>();

			public int QuizQuestionId { get; set; }

			public int QuizTakeId { get; set; }

			public int StudentId { get; set; }

			[ForeignKey(nameof(QuizQuestionId))]
			public QuizQuestion QuizQuestionInfo { get; set; } = null!;
		}

		public async Task<JsonResult> OnPostAsync([FromBody] AnswerData answerData)
		{
			try
			{
				var creator = await _userManager.GetUserAsync(User);

				if (creator == null)
				{
					return new JsonResult("User not found") { StatusCode = 404 };
				}

				if (answerData == null || answerData.Answers == null || !answerData.Answers.Any())
				{
					return new JsonResult("Invalid answer data") { StatusCode = 400 };
				}

				var quizQuestion = await _context.QuizQuestions
					.Where(m => m.Id == answerData.QuizQuestionId && m.Active)
					.FirstOrDefaultAsync();

				if (quizQuestion == null)
				{
					return new JsonResult("Quiz question not found") { StatusCode = 404 };
				}

				var correctAnswersWithOrder = await _context.QuestionAnswers
					.Where(m => m.QuizQuestionId == answerData.QuizQuestionId && m.Active && m.isCorrect)
					.Select(m => new AnswerWithOrder
					{
						Answer = m.Answer,
						Order = m.Order
					})
					.ToListAsync();

				if (correctAnswersWithOrder == null || !correctAnswersWithOrder.Any())
				{
					return new JsonResult("Correct answers not found") { StatusCode = 404 };
				}

				switch (quizQuestion.QuestionType)
				{
					case 0:
					case 1:
					case 4:
						if (answerData.Answers.Count > 1)
						{
							return new JsonResult("Only one answer is allowed for this question type") { StatusCode = 400 };
						}
						await HandleSingleAnswerAsync(answerData, correctAnswersWithOrder, creator);
						break;

					case 6:
						await HandleAlternativeAnswersAsync(answerData, correctAnswersWithOrder, creator);
						break;

					case 2:
					case 3:
						await HandleMultipleAnswersAsync(answerData, correctAnswersWithOrder, creator);
						break;
					case 5:
						await HandleEnumerationAnswersAsync(answerData, correctAnswersWithOrder, creator);
						break;

					default:
						return new JsonResult("Unsupported question type") { StatusCode = 400 };
				}

				return new JsonResult(new { status = "OK" });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving data: {ex.Message}");
				return new JsonResult("An error occurred while saving data") { StatusCode = 500 };
			}
		}

		private async Task HandleSingleAnswerAsync(AnswerData answerData, List<AnswerWithOrder> correctAnswersWithOrder, AppUser creator)
		{
			var submittedAnswer = answerData.Answers.First();
			var isAnswerCorrect = correctAnswersWithOrder
				.Any(correctAnswer =>
					string.Equals(correctAnswer.Answer, submittedAnswer.Answer, StringComparison.Ordinal));

			await SaveAnswerAndResultAsync(answerData, submittedAnswer, isAnswerCorrect, creator);
		}

		private async Task HandleAlternativeAnswersAsync(AnswerData answerData, List<AnswerWithOrder> correctAnswersWithOrder, AppUser creator)
		{
			foreach (var submittedAnswer in answerData.Answers)
			{
				var isAnswerCorrect = correctAnswersWithOrder
					.Any(correctAnswer =>
						string.Equals(correctAnswer.Answer, submittedAnswer.Answer, StringComparison.Ordinal) &&
						correctAnswer.Order == submittedAnswer.Order);

				await SaveAnswerAndResultAsync(answerData, submittedAnswer, isAnswerCorrect, creator);
			}
		}

		private async Task HandleMultipleAnswersAsync(AnswerData answerData, List<AnswerWithOrder> correctAnswersWithOrder, AppUser creator)
		{
			foreach (var submittedAnswer in answerData.Answers)
			{
				var isAnswerCorrect = correctAnswersWithOrder
					.Any(correctAnswer =>
						string.Equals(correctAnswer.Answer, submittedAnswer.Answer, StringComparison.Ordinal) &&
						correctAnswer.Order == submittedAnswer.Order);

				await SaveAnswerAndResultOfMultipleAnswerAsync(answerData, submittedAnswer, isAnswerCorrect, creator);
			}
		}

		private async Task HandleEnumerationAnswersAsync(AnswerData answerData, List<AnswerWithOrder> correctAnswersWithOrder, AppUser creator)
		{
			foreach (var submittedAnswer in answerData.Answers)
			{
				var isAnswerCorrect = correctAnswersWithOrder
					.Any(correctAnswer =>
						string.Equals(correctAnswer.Answer, submittedAnswer.Answer, StringComparison.Ordinal) &&
						correctAnswer.Order == submittedAnswer.Order);

				await SaveAnswerAndResultAsync(answerData, submittedAnswer, isAnswerCorrect, creator);
			}
		}

		private async Task SaveAnswerAndResultAsync(AnswerData answerData, AnswerObject submittedAnswer, bool isAnswerCorrect, AppUser creator)
		{
			var answerStudent = new AnswerStudent
			{
				Answer = submittedAnswer.Answer,
				QuizQuestionId = answerData.QuizQuestionId,
				QuizTakeId = answerData.QuizTakeId,
				StudentId = answerData.StudentId,
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.UtcNow,
			};

			_context.AnswerStudents.Add(answerStudent);
			await _context.SaveChangesAsync(); // Save to generate AnswerStudent.Id

			var quizResult = new QuizResult
			{
				isCorrect = isAnswerCorrect,
				AnswerStudentId = answerStudent.Id, // Ensure ID is available
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.UtcNow,
			};

			_context.QuizResults.Add(quizResult);
			await _context.SaveChangesAsync();
		}

		private async Task SaveAnswerAndResultOfMultipleAnswerAsync(AnswerData answerData, AnswerObject submittedAnswer, bool isAnswerCorrect, AppUser creator)
		{
			var answerStudent = new AnswerStudent
			{
				Answer = submittedAnswer.Order ?? "", // order may be null here
				QuizQuestionId = answerData.QuizQuestionId,
				QuizTakeId = answerData.QuizTakeId,
				StudentId = answerData.StudentId,
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.UtcNow,
			};

			_context.AnswerStudents.Add(answerStudent);
			await _context.SaveChangesAsync(); // Save to generate AnswerStudent.Id

			var quizResult = new QuizResult
			{
				isCorrect = isAnswerCorrect,
				AnswerStudentId = answerStudent.Id, // Ensure ID is available
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.UtcNow,
			};

			_context.QuizResults.Add(quizResult);
			await _context.SaveChangesAsync();
		}
	}
}
