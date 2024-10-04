using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.TeacherSubjects
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

		public async Task<JsonResult> OnPostAsync([FromBody] int teacherSubjectId)
		{
			var updater = await _userManager.GetUserAsync(User);

			if (updater == null)
			{
				return new JsonResult("NOT FOUND");
			}

			var teacherSubjects = await _context.TeacherSubjects
				.Where(m => m.Id == teacherSubjectId)
				.FirstOrDefaultAsync();

			if (teacherSubjects == null)
			{
				return new JsonResult("NOT FOUND");
			}

			teacherSubjects.Active = false;
			teacherSubjects.UpdatedBy = updater.Id;
			teacherSubjects.UpdatedDate = DateTime.Now;

			_context.TeacherSubjects.Update(teacherSubjects);
			await _context.SaveChangesAsync();

			return new JsonResult("OK");
		}
	}
}
