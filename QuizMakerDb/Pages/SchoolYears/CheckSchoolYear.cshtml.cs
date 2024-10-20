using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;

namespace QuizMakerDb.Pages.SchoolYears
{
	public class CheckSchoolYearModel : PageModel
    {
		private readonly ApplicationDbContext _context;

		public CheckSchoolYearModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetCheckSchoolYearAsync([FromQuery] string name)
		{
			var schoolYearName = await _context.SchoolYears
				.Where(m => m.Active)
				.FirstOrDefaultAsync(m => m.Name == name);

			if (schoolYearName != null)
			{
				return new JsonResult("NOT OK");
			}

			return new JsonResult("OK");
		}

		public async Task<JsonResult> OnGetCheckSchoolYearByIdAsync([FromQuery] int id, string name)
		{
			var schoolYear = await _context.SchoolYears
				.Where(m => m.Active)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (schoolYear != null)
			{
				if (schoolYear.Name == name)
				{
					return new JsonResult("OK");
				}
				else
				{
					var searchSchoolYear = await _context.SchoolYears
						.Where(m => m.Active)
						.FirstOrDefaultAsync(m => m.Name == name);

					if (searchSchoolYear != null) 
					{
						return new JsonResult("NOT OK");
					}

					return new JsonResult("OK");
				}
			}

			return new JsonResult("School Year not found");
		}
	}
}
