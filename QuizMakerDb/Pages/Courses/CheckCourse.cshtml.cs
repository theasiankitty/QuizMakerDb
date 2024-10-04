using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;

namespace QuizMakerDb.Pages.Courses
{
    public class CheckCourseModel : PageModel
    {
		private readonly ApplicationDbContext _context;

		public CheckCourseModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetCheckCourseAsync([FromQuery] string name)
		{
			var courseName = await _context.Courses
				.Where(m => m.Active)
				.FirstOrDefaultAsync(m => m.Name == name);

			if (courseName != null)
			{
				return new JsonResult("NOT OK");
			}

			return new JsonResult("OK");
		}

		public async Task<JsonResult> OnGetCheckCourseByIdAsync([FromQuery] int id, string name)
		{
			var course = await _context.Courses
				.Where(m => m.Active)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (course != null)
			{
				if (course.Name == name)
				{
					return new JsonResult("OK");
				}
				else
				{
					var courseName = await _context.Courses
						.Where(m => m.Active)
						.FirstOrDefaultAsync(m => m.Name == name);

					if (courseName != null)
					{
						return new JsonResult("NOT OK");
					}

					return new JsonResult("OK");
				}
			}

			return new JsonResult("Course not found");
		}
	}
}


