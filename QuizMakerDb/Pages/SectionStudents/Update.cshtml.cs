using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;

namespace QuizMakerDb.Pages.SectionStudents
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

		public async Task<JsonResult> OnPostAsync([FromBody] int sectionStudentId)
		{
			var updater = await _userManager.GetUserAsync(User);

			if (updater == null)
			{
				return new JsonResult("NOT FOUND");
			}

			var sectionStudent = await _context.SectionStudents
				.Where(m => m.Id == sectionStudentId)
				.FirstOrDefaultAsync();

			if (sectionStudent == null)
			{
				return new JsonResult("NOT FOUND");
			}

			sectionStudent.Active = false;
			sectionStudent.UpdatedBy = updater.Id;
			sectionStudent.UpdatedDate = DateTime.Now;

			_context.SectionStudents.Update(sectionStudent);
			await _context.SaveChangesAsync();

			return new JsonResult("OK");
		}
	}
}
