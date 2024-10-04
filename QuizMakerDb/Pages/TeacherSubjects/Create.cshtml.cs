using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.TeacherSubjects
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class CreateModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public CreateModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task<JsonResult> OnPostAsync([FromBody] object data, [FromQuery] string operation)
		{
			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return new JsonResult("NOT FOUND");
			}

			var teacherSubjects = JsonConvert.DeserializeObject<List<TeacherSubject>>(data.ToString());

			if (teacherSubjects == null || !teacherSubjects.Any())
			{
				return new JsonResult("No subjects provided");
			}

			var executionStrategy = _context.Database.CreateExecutionStrategy();

			await executionStrategy.ExecuteAsync(async () =>
			{
				using (var transaction = await _context.Database.BeginTransactionAsync())
				{
					try
					{
						foreach (var subject in teacherSubjects)
						{
							var courseYearSubject = new TeacherSubject
							{
								TeacherId = subject.TeacherId,
								SubjectId = subject.SubjectId,
								CourseYearId = subject.CourseYearId,
								Active = true,
								CreatedBy = creator.Id,
								CreatedDate = DateTime.Now
							};

							_context.TeacherSubjects.Add(courseYearSubject);
						}

						await _context.SaveChangesAsync();
						await transaction.CommitAsync();
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						Console.WriteLine($"Error saving data: {ex.Message}");
						throw;
					}
				}
			});

			return new JsonResult("OK");
		}
	}
}
