using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.QuizSubjects
{
	public class CreateModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;
		private static Random random = new Random();

		public CreateModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public class QuizSubjectData
		{
			public int QuizId { get; set; }
			public int SectionId { get; set; }
			public int SubjectId { get; set; }
		}

		public async Task<JsonResult> OnPostAsync([FromBody] IList<QuizSubjectData> sectionSubjects)
		{
			try
			{
				var creator = await _userManager.GetUserAsync(User);

				if (creator == null)
				{
					return new JsonResult("NOT FOUND");
				}

				if (sectionSubjects == null || !sectionSubjects.Any())
				{
					return new JsonResult("No subjects provided");
				}

				foreach (var subject in sectionSubjects)
				{
					bool exists = await _context.QuizSubjects
						.AnyAsync(q => q.QuizId == subject.QuizId &&
									   q.SectionId == subject.SectionId &&
									   q.SubjectId == subject.SubjectId);

					if (!exists)
					{
						var sectionSubject = new QuizSubject
						{
							QuizId = subject.QuizId,
							SectionId = subject.SectionId,
							SubjectId = subject.SubjectId,
							Code = GenerateRandomCode(),
							Active = false,
							CreatedBy = creator.Id,
							CreatedDate = DateTime.Now
						};

						_context.QuizSubjects.Add(sectionSubject);
					}
					else
					{
						Console.WriteLine($"Entry already exists for QuizId: {subject.QuizId}, SectionId: {subject.SectionId}, SubjectId: {subject.SubjectId}");
					}
				}

				await _context.SaveChangesAsync();

				return new JsonResult("OK");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving data: {ex.Message}");
				throw;
			}
		}

		private string GenerateRandomCode(int length = 6)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}

}
