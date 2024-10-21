using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Pages.SectionStudents
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

		public class SectionStudentData
		{
            public int SectionId { get; set; }

            public int StudentId { get; set; }

            public int SchoolYearId { get; set; }
        }

        public async Task<JsonResult> OnPostAsync([FromBody] IList<SectionStudentData> studentSections)
		{
			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return new JsonResult("NOT FOUND");
			}

			if (studentSections == null || !studentSections.Any())
			{
				return new JsonResult("No student sections provided");
			}

			var executionStrategy = _context.Database.CreateExecutionStrategy();

			await executionStrategy.ExecuteAsync(async () =>
			{
				using (var transaction = await _context.Database.BeginTransactionAsync())
				{
					try
					{
						foreach (var student in studentSections)
						{
							var sectionStudent = new SectionStudent
							{
								StudentId = student.StudentId,
								SectionId = student.SectionId,
								SchoolYearId = student.SchoolYearId,
								Active = true,
								CreatedBy = creator.Id,
								CreatedDate = DateTime.Now
							};

							_context.SectionStudents.Add(sectionStudent);
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
