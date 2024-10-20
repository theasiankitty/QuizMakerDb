using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;

namespace QuizMakerDb.Pages.Students
{
    public class IrregularModel : PageModel
    {
		private readonly ApplicationDbContext _context;

		public IrregularModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetSearchSectionsAsync(int? schoolYearId)
		{
			if (schoolYearId == null)
			{
				return new JsonResult(new { message = "Invalid Course Year." }) { StatusCode = 400 };
			}

			var sections = await _context.Sections
				.Include(m => m.CourseYearInfo)
				.Where(m => m.SchoolYearId == schoolYearId && m.Active)
				.Select(m => new {
					m.Id,
					Section = m.CourseYearInfo.Name + " - " + m.Name
				}).ToListAsync();

			return new JsonResult(new { message = "OK", sections });
		}
	}
}
