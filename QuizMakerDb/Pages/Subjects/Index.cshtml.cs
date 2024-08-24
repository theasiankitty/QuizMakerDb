using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;

namespace QuizMakerDb.Pages.Subjects
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public PaginatedList<SubjectVM> Subjects { get; set; } = null!;
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public string SearchTitle { get; set; } = string.Empty!;
		public string SearchCode { get; set; } = string.Empty!;
		public string SearchSemester { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

		public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex, string? searchTitle, string? searchCode, string? searchSemester)
		{
			SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
			SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
			SearchTitle = string.IsNullOrEmpty(searchTitle) ? "" : searchTitle;
			SearchCode = string.IsNullOrEmpty(searchCode) ? "" : searchCode;
			SearchSemester = string.IsNullOrEmpty(searchSemester) ? "" : searchSemester;

			if (_context.Subjects != null)
			{
				IQueryable<Subject> subjects = _context.Subjects.AsQueryable();

				subjects = subjects
					.Where(m => m.Active == true)
					.OrderByDescending(o => o.Id);

				if (!string.IsNullOrEmpty(searchTitle))
				{
					subjects = subjects.Where(m => (m.Title)
					.ToLower()
					.Contains(searchTitle.ToLower()));
				}

				if (!string.IsNullOrEmpty(searchCode))
				{
					subjects = subjects.Where(m => (m.Code)
					.ToLower()
					.Contains(searchCode.ToLower()));
				}

				if (!string.IsNullOrEmpty(searchSemester))
				{
					subjects = subjects.Where(m =>
						m.Semester == byte.Parse(searchSemester));
				}

				switch (sortColumn)
				{
					case "id":
						subjects = SortOrder == "asc" ? subjects.OrderBy(o => o.Id)
							: subjects.OrderByDescending(o => o.Id);
						break;
					case "title":
						subjects = SortOrder == "asc" ? subjects.OrderBy(o => o.Title)
							: subjects.OrderByDescending(o => o.Title);
						break;
					case "code":
						subjects = SortOrder == "asc" ? subjects.OrderBy(o => o.Code)
							: subjects.OrderByDescending(o => o.Code);
						break;
					case "semester":
						subjects = SortOrder == "asc" ? subjects.OrderBy(o => o.Semester)
							: subjects.OrderByDescending(o => o.Semester);
						break;
				}

				int pageSize = 10;

				TotalItems = subjects.Count();

				Subjects = await PaginatedList<SubjectVM>.CreateAsync(
					subjects.Select(m => new SubjectVM
					{
						Id = m.Id,
						Title = m.Title,
						Code = m.Code,
						Semester = ((Semester)m.Semester).ToString(),
					}).AsNoTracking(),
					pageIndex ?? 1,
					pageSize
				);
			}
		}
	}
}
