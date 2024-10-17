using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using QuizMakerDb.Data.Models;
using System.Text.Json;

namespace QuizMakerDb.Pages.QuizQuestions
{
	public class UpdateModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public UpdateModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public class ItemData
		{
			public int Id { get; set; }

			public string Name { get; set; } = string.Empty!;

			public string Order { get; set; } = string.Empty!;

			public int QuizQuestionId { get; set; }

			public bool Active { get; set; }
		}

		public class AnswerData
		{
			public int Id { get; set; }

			public string Answer { get; set; } = string.Empty!;

			public bool IsCorrect { get; set; }

			public bool ShowAnswer { get; set; }

			public string Order { get; set; } = string.Empty!;

			public byte[]? Image { get; set; }

			public bool Active { get; set; }
		}

		public class QuestionData
		{
			public int Id { get; set; }

			public string Description { get; set; } = string.Empty;

			public byte Order { get; set; }

			public byte QuestionType { get; set; }

			public byte Points { get; set; }

			public byte[]? Image { get; set; }

			public IList<AnswerData>? Answers { get; set; }

			public IList<ItemData>? Items { get; set; }

			public bool Active { get; set; }
		}

		public async Task<JsonResult> OnPostAsync([FromBody] QuestionData questionData)
		{
			try
			{
				var updater = await _userManager.GetUserAsync(User);
				if (updater == null)
				{
					return new JsonResult("NOT FOUND");
				}

				if (questionData == null)
				{
					return new JsonResult("QUESTION DATA NOT FOUND") { StatusCode = 400 };
				}

				var existingQuestion = await _context.QuizQuestions
					.FirstOrDefaultAsync(m => m.Id == questionData.Id);

				if (existingQuestion == null)
				{
					return new JsonResult("QUESTION NOT FOUND");
				}

				existingQuestion.Description = questionData.Description;
				existingQuestion.Order = questionData.Order;
				existingQuestion.QuestionType = questionData.QuestionType;
				existingQuestion.Points = questionData.Points;
				existingQuestion.Image = questionData.Image;
				existingQuestion.Active = questionData.Active;
				existingQuestion.UpdatedBy = updater.Id;
				existingQuestion.UpdatedDate = DateTime.Now;

				if (questionData.Answers != null)
				{
					foreach (var answer in questionData.Answers)
					{
						var existingAnswer = await _context.QuestionAnswers.FirstOrDefaultAsync(m => m.Id == answer.Id && m.Active);

						if (existingAnswer != null)
						{
							existingAnswer.Answer = answer.Answer;
							existingAnswer.isCorrect = answer.IsCorrect;
							existingAnswer.ShowAnswer = answer.ShowAnswer;
							existingAnswer.Order = answer.Order;
							existingAnswer.Image = answer.Image;
							existingAnswer.Active = answer.Active;
							existingAnswer.UpdatedBy = updater.Id;
							existingAnswer.UpdatedDate = DateTime.Now;
						}
						else
						{
							var newAnswer = new QuestionAnswer
							{
								Answer = answer.Answer,
								isCorrect = answer.IsCorrect,
								ShowAnswer = answer.ShowAnswer,
								Order = answer.Order,
								Image = answer.Image,
								QuizQuestionId = questionData.Id,
								Active = answer.Active,
								CreatedBy = updater.Id,
								CreatedDate = DateTime.Now
							};

							await _context.QuestionAnswers.AddAsync(newAnswer);
						}
					}

					var existingAnswers = await _context.QuestionAnswers.Where(m => m.QuizQuestionId == questionData.Id && m.Active).ToListAsync();

					foreach (var existingAnswer in existingAnswers)
					{
						if (!questionData.Answers.Any(m => m.Id == existingAnswer.Id && m.Active))
						{
							existingAnswer.Active = false;
							existingAnswer.UpdatedBy = updater.Id;
							existingAnswer.UpdatedDate = DateTime.Now;
						}
					}
				}

				if (questionData.Items != null)
				{
					foreach (var item in questionData.Items)
					{
						var existingItem = await _context.QuestionItems.FirstOrDefaultAsync(m => m.Id == item.Id && m.Active);

						if (existingItem != null)
						{
							existingItem.Name = item.Name;
							existingItem.Order = item.Order;
							existingItem.QuizQuestionId = item.QuizQuestionId;
							existingItem.Active = item.Active;
							existingItem.UpdatedBy = updater.Id;
							existingItem.UpdatedDate = DateTime.Now;
						}
						else
						{
							var newItem = new QuestionItem
							{
								Name = item.Name,
								Order = item.Order,
								QuizQuestionId = item.QuizQuestionId,
								Active = item.Active,
								CreatedBy = updater.Id,
								CreatedDate = DateTime.Now,
							};

							await _context.QuestionItems.AddAsync(newItem);
						}
					}

					var existingItems = await _context.QuestionItems.Where(m => m.QuizQuestionId == questionData.Id && m.Active).ToArrayAsync();

					foreach (var existingItem in existingItems)
					{
						if (!questionData.Items.Any(m => m.Id == existingItem.Id))
						{
							existingItem.Active = false;
							existingItem.UpdatedBy = updater.Id;
							existingItem.UpdatedDate = DateTime.Now;
						}
					}
				}

				await _context.SaveChangesAsync();

				return new JsonResult("OK");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				return new JsonResult("AN ERROR OCCURRED") { StatusCode = 500 };
			}
		}
	}
}
