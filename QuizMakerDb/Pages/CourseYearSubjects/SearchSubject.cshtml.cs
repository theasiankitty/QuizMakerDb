using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.CourseYearSubjects
{
	public class SearchSubjectModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public SearchSubjectModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetAsync([FromQuery] string searchSubject, int courseYearId)
		{
			if (string.IsNullOrEmpty(searchSubject))
			{
				return new JsonResult(new { message = "Invalid Subject." });
			}

			var unassignedSubjects = await _context.Subjects
					.Where(m => (m.Code).ToLower().Contains(searchSubject)
					&& m.Active == true)
					.ToListAsync();

			var assignedSubject = await _context.CourseYearSubjects
					.Where(m => m.CourseYearId == courseYearId
					&& m.Active == true)
					.Select(m => m.SubjectId)
					.ToListAsync();

			if (unassignedSubjects.Any())
			{
				unassignedSubjects = unassignedSubjects
							.Where(m => !assignedSubject.Contains(m.Id)).ToList();
			}

			if (unassignedSubjects == null)
			{
				return new JsonResult(new { message = "Subject Not Found." });
			}

			return new JsonResult(new { message = "OK", subjects = unassignedSubjects });
		}
	}
}
