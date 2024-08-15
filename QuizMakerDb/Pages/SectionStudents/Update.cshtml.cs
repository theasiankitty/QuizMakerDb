using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace QuizMakerDb.Pages.SectionStudents
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

		public async Task<JsonResult> OnPostAsync([FromBody] int sectionStudentId)
		{
			var editor = await _userManager.GetUserAsync(User);

			if (editor == null)
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
			sectionStudent.UpdatedBy = editor.Id;
			sectionStudent.UpdatedDate = DateTime.Now;

			_context.SectionStudents.Update(sectionStudent);
			await _context.SaveChangesAsync();

			return new JsonResult("OK");
		}
	}
}
