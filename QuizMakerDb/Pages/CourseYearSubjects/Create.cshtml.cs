using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.CourseYearSubjects
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

		public class CourseYearSubjectData
		{
            public int SubjectId { get; set; }

            public int CourseYearId { get; set; }
        }

        public async Task<JsonResult> OnPostAsync([FromBody] IList<CourseYearSubjectData> courseYearSubjects)
		{
			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return new JsonResult("NOT FOUND");
			}

<<<<<<< Updated upstream
			var courseYearSubjects = JsonConvert.DeserializeObject<List<CourseYearSubject>>(data.ToString());

<<<<<<< HEAD
			if (courseYearSubjects.Any())
=======
			if (courseYearSubjects == null)
>>>>>>> Stashed changes
=======
			if (courseYearSubjects == null || !courseYearSubjects.Any())
>>>>>>> 64989993402aa9d467121889e5da6b3cb58f9ffd
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
						foreach (var subject in courseYearSubjects)
						{
							var sectionSubject = new CourseYearSubject
							{
								SubjectId = subject.SubjectId,
								CourseYearId = subject.CourseYearId,
								Active = true,
								CreatedBy = creator.Id,
								CreatedDate = DateTime.Now
							};

							_context.CourseYearSubjects.Add(sectionSubject);
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
