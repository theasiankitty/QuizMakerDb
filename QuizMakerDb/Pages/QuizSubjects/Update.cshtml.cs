using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Pages.QuizSubjects
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

		public class QuizSubjectData
		{
			public int Id { get; set; }
			public bool Active { get; set; }
		}

		public async Task<JsonResult> OnPostAsync([FromBody] QuizSubjectData sectionSubjects)
		{
			try
			{
				var updater = await _userManager.GetUserAsync(User);

				if (updater == null)
				{
					return new JsonResult("NOT FOUND");
				}

				if (sectionSubjects == null)
				{
					return new JsonResult("No subjects provided");
				}

				var quizSubject = await _context.QuizSubjects.Where(m => m.Id == sectionSubjects.Id).FirstOrDefaultAsync();

				if (quizSubject == null)
				{
					return new JsonResult("Quiz subject is invalid.");
				}

				quizSubject.Active = sectionSubjects.Active;
				quizSubject.UpdatedBy = updater.Id;
				quizSubject.UpdatedDate = DateTime.Now;

				_context.QuizSubjects.Update(quizSubject);
				await _context.SaveChangesAsync();

				return new JsonResult("OK");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving data: {ex.Message}");
				throw;
			}
		}
	}
}
