using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;

namespace QuizMakerDb.Pages.QuizSections
{
	public class SearchSectionModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public SearchSectionModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetAllSectionAsync([FromQuery] int quizId, int teacherId, int currentPage, int pageSize)
		{
			var unassignedSections = await _context.Sections
				.Where(m => m.Active)
				.OrderByDescending(o => o.Id)
				.ToListAsync();

			var dataCount = unassignedSections.Count;

			if (quizId != 0 && teacherId != 0)
			{
				//var assignedSections = await _context.QuizSections
				//	.Where(m => m.QuizId == quizId
				//		&& m.QuizInfo.TeacherId == teacherId
				//		&& m.Active)
				//	.Select(m => m.Id)
				//	.ToListAsync();

				//unassignedSections = unassignedSections
				//	.Where(m => !assignedSections.Contains(m.Id))
				//	.Skip(currentPage * pageSize)
				//	.Take(pageSize)
				//	.ToList();

				dataCount = unassignedSections.Count;
			}

			if (unassignedSections == null)
			{
				return new JsonResult(new { message = "Section Not Found." });
			}

			return new JsonResult(new { message = "OK", sections = unassignedSections, count = dataCount });
		}
	}
}
