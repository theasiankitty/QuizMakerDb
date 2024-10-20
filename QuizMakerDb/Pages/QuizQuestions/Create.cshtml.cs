using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Data.Models;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Http.HttpResults;
using System;

namespace QuizMakerDb.Pages.QuizQuestions
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

		public class ItemData
		{
			public string Name { get; set; } = string.Empty!;

			public string Order { get; set; } = string.Empty!;

			public bool Active { get; set; }
		}

		public class AnswerData
		{
			public string Answer { get; set; } = string.Empty!;

			public bool IsCorrect { get; set; }

			public bool ShowAnswer { get; set; }

			public string Order { get; set; } = string.Empty!;

			public byte[]? Image { get; set; }

			public bool Active { get; set; }
		}

		public class QuestionData
		{
			public string Description { get; set; } = string.Empty;

			public int Order { get; set; }

			public byte QuestionType { get; set; }

			public byte Points { get; set; }

			public byte[]? Image { get; set; }

			public IList<AnswerData>? Answers { get; set; }

			public IList<ItemData>? Items { get; set; }

			public int QuizId { get; set; }

			public bool Active { get; set; }
		}

		public async Task<JsonResult> OnPostAsync([FromBody] QuestionData questionData)
		{
			try
			{
				var creator = await _userManager.GetUserAsync(User);

				if (creator == null)
				{
					return new JsonResult("NOT FOUND");
				}

				if (questionData == null)
				{
					return new JsonResult("Invalid question data") { StatusCode = 400 };
				}

				int maxOrder = 0;

				var lastQuestion = await _context.QuizQuestions
					.OrderByDescending(m => m.Order)
					.FirstOrDefaultAsync();

				if (lastQuestion != null)
				{
					maxOrder = lastQuestion.Order;
				}

				maxOrder++;

				var question = new QuizQuestion
				{
					Description = questionData.Description,
					Order = maxOrder,
					Points = questionData.Points,
					QuestionType = questionData.QuestionType,
					Image = questionData.Image,
					QuizId = questionData.QuizId,
					Active = true,
					CreatedBy = creator.Id,
					CreatedDate = DateTime.Now,
				};

				_context.QuizQuestions.Add(question);

				await _context.SaveChangesAsync(); // Save question to get its ID

				if (questionData.Answers != null)
				{
					foreach (var answer in questionData.Answers)
					{
						var questionAnswer = new QuestionAnswer
						{
							Answer = answer.Answer,
							isCorrect = answer.IsCorrect,
							ShowAnswer = answer.ShowAnswer,
							Order = answer.Order,
							Image = answer.Image,
							Active = answer.Active,
							QuizQuestionId = question.Id, // Now question.Id has a valid value
							CreatedBy = creator.Id,
							CreatedDate = DateTime.Now
						};

						await _context.QuestionAnswers.AddAsync(questionAnswer); // Add to QuestionAnswers
					}
				}

				if (questionData.Items != null)
				{
					foreach (var item in questionData.Items)
					{
						var questionItem = new QuestionItem
						{
							Name = item.Name,
							Order = item.Order,
							QuizQuestionId = question.Id,
							Active = item.Active,
							CreatedBy = creator.Id,
							CreatedDate = DateTime.Now,
						};

						await _context.QuestionItems.AddAsync(questionItem);
					}
				}

				await _context.SaveChangesAsync(); // Save all changes at once

				return new JsonResult("OK");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving data: {ex.Message}");
				return new JsonResult("An error occurred while saving data") { StatusCode = 500 };
			}
		}
	}
}
