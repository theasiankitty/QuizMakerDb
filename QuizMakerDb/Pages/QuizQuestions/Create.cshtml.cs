using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Data.Models;
using Newtonsoft.Json;

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

		public class QuestionRequest
		{
			public object QuestionData { get; set; } = null!;
			public byte QuestionType { get; set; }
		}

		public async Task<JsonResult> OnPostAsync([FromBody] QuestionRequest request, [FromQuery] string operation)
		{
			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return new JsonResult("NOT FOUND");
			}

			switch (request.QuestionType)
			{
				case 0:
					return await ProcessMultipleChoice(request.QuestionData, request.QuestionType, creator.Id);

				case 1:
				case 3:
				case 4:
					return await ProcessOneAnswer(request.QuestionData, request.QuestionType, creator.Id);

				case 2:
					return await ProcessMatchingOrOrdering(request.QuestionData, request.QuestionType, creator.Id);

				default:
					return new JsonResult("Invalid question type");
			}
		}

		private async Task<JsonResult> ProcessMultipleChoice(object questionData, byte questionType, Guid creator)
		{
			var executionStrategy = _context.Database.CreateExecutionStrategy();

			return await executionStrategy.ExecuteAsync(async () =>
			{
				using (var transaction = await _context.Database.BeginTransactionAsync())
				{
					try
					{
						var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(questionData.ToString());

						if (data == null)
						{
							return new JsonResult("Invalid question data") { StatusCode = 400 };
						}

						if (!data.TryGetValue("question", out var questionObj) ||
							!data.TryGetValue("order", out var orderObj) ||
							!data.TryGetValue("image", out var imageObj) ||
							!data.TryGetValue("points", out var pointsObj) ||
							!data.TryGetValue("quizId", out var quizIdObj) ||
							!data.TryGetValue("choices", out var choicesObj))
						{
							return new JsonResult("Missing required fields") { StatusCode = 400 };
						}

						var choices = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(choicesObj.ToString());

						if (choices == null || !choices.Any())
						{
							return new JsonResult("No choices provided") { StatusCode = 400 };
						}

						var lastQuestion = _context.QuizQuestions
							.Where(m => m.QuizId == Convert.ToInt32(quizIdObj) && m.Active)
							.OrderByDescending(m => m.Order)
							.FirstOrDefault();

						var maxOrder = lastQuestion?.Order ?? 0;

						var quizQuestion = new QuizQuestion
						{
							Description = questionObj?.ToString() ?? string.Empty,
							Order = maxOrder + 1,
							QuestionType = questionType,
							Points = Convert.ToByte(pointsObj),
							QuizId = Convert.ToInt32(quizIdObj),
							Active = true,
							CreatedBy = creator,
							CreatedDate = DateTime.Now
						};

						if (imageObj is string imageBase64)
						{
							quizQuestion.Image = Convert.FromBase64String(imageBase64);
						}
						else
						{
							quizQuestion.Image = null;
						}

						_context.QuizQuestions.Add(quizQuestion);

						await _context.SaveChangesAsync();

						foreach (var choice in choices)
						{
							var answer = new QuestionAnswer
							{
								Answer = choice["choice"]?.ToString() ?? string.Empty,
								Image = choice["image"] is string choiceImageBase64 ? Convert.FromBase64String(choiceImageBase64) : null,
								isCorrect = Convert.ToBoolean(choice["isCorrect"]),
								ShowAnswer = Convert.ToBoolean(choice["showAnswer"]),
								Order = choice["order"]?.ToString() ?? string.Empty,
								QuizQuestionId = quizQuestion.Id,
								Active = true,
								CreatedBy = creator,
								CreatedDate = DateTime.Now
							};

							_context.QuestionAnswers.Add(answer);
						}

						await _context.SaveChangesAsync();
						await transaction.CommitAsync();

						return new JsonResult("OK");
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						Console.WriteLine($"Error saving data: {ex.Message}");
						return new JsonResult("An error occurred while saving data") { StatusCode = 500 };
					}
				}
			});
		}

		private async Task<JsonResult> ProcessMatchingOrOrdering(object questionData, byte questionType, Guid creator)
		{
			var executionStrategy = _context.Database.CreateExecutionStrategy();

			return await executionStrategy.ExecuteAsync(async () =>
			{
				using (var transaction = await _context.Database.BeginTransactionAsync())
				{
					try
					{
						var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(questionData.ToString());

						if (data == null)
						{
							return new JsonResult("Invalid question data") { StatusCode = 400 };
						}

						if (!data.TryGetValue("instruction", out var instructionObj) ||
							!data.TryGetValue("order", out var orderObj) ||
							!data.TryGetValue("image", out var imageObj) ||
							!data.TryGetValue("points", out var pointsObj) ||
							!data.TryGetValue("quizId", out var quizIdObj) ||
							!data.TryGetValue("items", out var itemsObj) ||
							!data.TryGetValue("promptLists", out var promptListsObj))
						{
							return new JsonResult("Missing required fields") { StatusCode = 400 };
						}

						var lastQuestion = _context.QuizQuestions
							.Where(m => m.QuizId == Convert.ToInt32(quizIdObj) && m.Active)
							.OrderByDescending(m => m.Order)
							.FirstOrDefault();

						var maxOrder = lastQuestion?.Order ?? 0;

						var quizQuestion = new QuizQuestion
						{
							Description = instructionObj?.ToString() ?? string.Empty,
							Order = maxOrder + 1,
							QuestionType = questionType,
							Points = Convert.ToByte(pointsObj),
							QuizId = Convert.ToInt32(quizIdObj),
							Active = true,
							CreatedBy = creator,
							CreatedDate = DateTime.Now
						};

						if (imageObj is string imageBase64)
						{
							quizQuestion.Image = Convert.FromBase64String(imageBase64);
						}
						else
						{
							quizQuestion.Image = null;
						}

						_context.QuizQuestions.Add(quizQuestion);

						await _context.SaveChangesAsync();

						var items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(itemsObj.ToString());

						if (items != null)
						{
							foreach (var item in items)
							{
								var questionItem = new QuestionItem
								{
									Name = item["name"]?.ToString() ?? string.Empty,
									Order = item["order"]?.ToString() ?? string.Empty,
									QuizQuestionId = quizQuestion.Id,
									Active = true,
									CreatedBy = creator,
									CreatedDate = DateTime.Now
								};

								_context.QuestionItems.Add(questionItem);
							}

							await _context.SaveChangesAsync();
						}

						var prompts = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(promptListsObj.ToString());

						if (prompts != null)
						{
							foreach (var prompt in prompts)
							{
								var answer = new QuestionAnswer
								{
									Answer = prompt["answer"]?.ToString() ?? string.Empty,
									Image = prompt["image"] is string choiceImageBase64 ? Convert.FromBase64String(choiceImageBase64) : null,
									isCorrect = Convert.ToBoolean(prompt["isCorrect"]),
									ShowAnswer = Convert.ToBoolean(prompt["showAnswer"]),
									Order = prompt["order"]?.ToString() ?? string.Empty,
									QuizQuestionId = quizQuestion.Id,
									Active = true,
									CreatedBy = creator,
									CreatedDate = DateTime.Now
								};

								_context.QuestionAnswers.Add(answer);
							}
						}

						await _context.SaveChangesAsync();
						await transaction.CommitAsync();

						return new JsonResult("OK");
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						Console.WriteLine($"Error saving data: {ex.Message}");
						return new JsonResult("An error occurred while saving data") { StatusCode = 500 };
					}
				}
			});
		}

		private async Task<JsonResult> ProcessOneAnswer(object questionData, byte questionType, Guid creator)
		{
			var executionStrategy = _context.Database.CreateExecutionStrategy();

			return await executionStrategy.ExecuteAsync(async () =>
			{
				using (var transaction = await _context.Database.BeginTransactionAsync())
				{
					try
					{
						var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(questionData.ToString());

						if (data == null)
						{
							return new JsonResult("Invalid question data") { StatusCode = 400 };
						}

						if (!data.TryGetValue("question", out var questionObj) ||
							!data.TryGetValue("order", out var orderObj) ||
							!data.TryGetValue("image", out var imageObj) ||
							!data.TryGetValue("points", out var pointsObj) ||
							!data.TryGetValue("quizId", out var quizIdObj) ||
							!data.TryGetValue("answer", out var answerObj) ||
							!data.TryGetValue("answerOrder", out var answerOrderObj) ||
							!data.TryGetValue("isCorrect", out var isCorrectObj) ||
							!data.TryGetValue("showAnswer", out var showAnswerObj) ||
							!data.TryGetValue("alternativeAnswers", out var alternativeAnswersObj))
						{
							return new JsonResult("Missing required fields") { StatusCode = 400 };
						}

						var lastQuestion = _context.QuizQuestions
							.Where(m => m.QuizId == Convert.ToInt32(quizIdObj) && m.Active)
							.OrderByDescending(m => m.Order)
							.FirstOrDefault();

						var maxOrder = lastQuestion?.Order ?? 0;

						var quizQuestion = new QuizQuestion
						{
							Description = questionObj?.ToString() ?? string.Empty,
							Order = maxOrder + 1,
							QuestionType = questionType,
							Points = Convert.ToByte(pointsObj),
							QuizId = Convert.ToInt32(quizIdObj),
							Active = true,
							CreatedBy = creator,
							CreatedDate = DateTime.Now
						};

						if (imageObj is string imageBase64)
						{
							quizQuestion.Image = Convert.FromBase64String(imageBase64);
						}
						else
						{
							quizQuestion.Image = null;
						}

						_context.QuizQuestions.Add(quizQuestion);

						await _context.SaveChangesAsync();

						var answer = new QuestionAnswer
						{
							Answer = answerObj?.ToString() ?? string.Empty,
							Image = null,
							isCorrect = Convert.ToBoolean(isCorrectObj),
							ShowAnswer = Convert.ToBoolean(showAnswerObj),
							Order = null,
							QuizQuestionId = quizQuestion.Id,
							Active = true,
							CreatedBy = creator,
							CreatedDate = DateTime.Now
						};

						_context.QuestionAnswers.Add(answer);

						await _context.SaveChangesAsync();

						var alternativeAnswers = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(alternativeAnswersObj.ToString());

						if (alternativeAnswers != null && alternativeAnswers.Count > 0)
						{
							foreach (var alternativeAnswer in alternativeAnswers)
							{
								var otherAnswer = new QuestionAnswer
								{
									Answer = alternativeAnswer["answer"]?.ToString() ?? string.Empty,
									Image = null,
									isCorrect = Convert.ToBoolean(alternativeAnswer["isCorrect"]),
									ShowAnswer = false,
									Order = null,
									QuizQuestionId = quizQuestion.Id,
									Active = true,
									CreatedBy = creator,
									CreatedDate = DateTime.Now
								};

								_context.QuestionAnswers.Add(otherAnswer);
							}

							await _context.SaveChangesAsync();
						}

						await transaction.CommitAsync();

						return new JsonResult("OK");
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						Console.WriteLine($"Error saving data: {ex.Message}");
						return new JsonResult("An error occurred while saving data") { StatusCode = 500 };
					}
				}
			});
		}
	}
}
