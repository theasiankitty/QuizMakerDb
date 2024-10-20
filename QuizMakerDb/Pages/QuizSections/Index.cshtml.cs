using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Pages.QuizSections
{
	public class IndexModel : PageModel
	{
		//private readonly ApplicationDbContext _context;

		//public IndexModel(ApplicationDbContext context)
		//{
		//	_context = context;
		//}

		//[BindProperty]
		//public PaginatedList<QuizSectionVM> QuizSections { get; set; } = default!;
		//public QuizVM QuizVM { get; set; } = default!;
  //      public string SortColumn { get; set; } = string.Empty!;
		//public string SortOrder { get; set; } = string.Empty!;
		//public string SearchSection { get; set; } = string.Empty!;
		//public int TotalItems { get; set; }

		//public async Task<IActionResult> OnGetAsync(int? quizId, string? sortColumn, string? sortOrder, int? pageIndex, string? searchSection)
		//{
		//	if (quizId == null)
		//	{
		//		return NotFound();
		//	}

		//	var quiz = await _context.Quizzes.FirstOrDefaultAsync(m => m.Id == quizId);

		//	if (quiz == null)
		//	{
		//		return NotFound();
		//	}

		//	QuizVM = new QuizVM
		//	{
		//		Id = quiz.Id,
		//		Title = quiz.Title,
		//		TeacherId = quiz.TeacherId
		//	};

		//	SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
		//	SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
		//	SearchSection = string.IsNullOrEmpty(searchSection) ? "" : searchSection;

		//	if (_context.QuizSections != null)
		//	{
		//		IQueryable<QuizSection> quizSections = _context.QuizSections.AsQueryable();

		//		quizSections = quizSections
		//			.Include(m => m.SectionInfo)
		//			.Where(m => m.QuizId == quiz.Id && m.Active);

		//		if (!string.IsNullOrEmpty(searchSection))
		//		{
		//			quizSections = quizSections
		//				.Where(m => (m.SectionInfo.Name)
		//				.ToLower()
		//				.Contains(searchSection.ToLower()));
		//		}

		//		switch (sortColumn)
		//		{
		//			case "id":
		//				quizSections = SortOrder == "asc" ? quizSections.OrderBy(o => o.Id)
		//					: quizSections.OrderByDescending(o => o.Id);
		//				break;
		//			case "section":
		//				quizSections = SortOrder == "asc" ? quizSections.OrderBy(o => o.SectionInfo.Name)
		//					: quizSections.OrderByDescending(o => o.SectionInfo.Name);
		//				break;
		//			case "code":
		//				quizSections = SortOrder == "asc" ? quizSections.OrderByDescending(o => o.Code)
		//					: quizSections.OrderBy(o => o.Code);
		//				break;
		//		}

		//		int pageSize = 10;

		//		TotalItems = quizSections.Count();

		//		QuizSections = await PaginatedList<QuizSectionVM>.CreateAsync(
		//			quizSections.Select(m => new QuizSectionVM
		//			{
		//				Id = m.Id,
		//				Section = m.SectionInfo.CourseYearInfo.Name + " - " + m.SectionInfo.Name,
		//				Code = m.Code
		//			}).AsNoTracking(),
		//			pageIndex ?? 1,
		//			pageSize
		//		);
		//	}

		//	return Page();
		//}
	}
}
