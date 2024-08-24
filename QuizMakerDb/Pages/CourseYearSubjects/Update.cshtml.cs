using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.CourseYearSubjects
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class UpdateModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public UpdateModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task<JsonResult> OnPostAsync([FromBody] int courseYearSubjectId)
		{
			var updater = await _userManager.GetUserAsync(User);

			if (updater == null)
			{
				return new JsonResult("NOT FOUND");
			}

			var courseYearSubject = await _context.CourseYearSubjects
				.Where(m => m.Id == courseYearSubjectId)
				.FirstOrDefaultAsync();

			if (courseYearSubject == null)
			{
				return new JsonResult("NOT FOUND");
			}

			courseYearSubject.Active = false;
			courseYearSubject.UpdatedBy = updater.Id;
			courseYearSubject.UpdatedDate = DateTime.Now;

			_context.CourseYearSubjects.Update(courseYearSubject);
			await _context.SaveChangesAsync();

			return new JsonResult("OK");
		}
	}
}
