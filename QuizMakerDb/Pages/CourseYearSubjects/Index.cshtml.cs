using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;

namespace QuizMakerDb.Pages.CourseYearSubjects
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public PaginatedList<CourseYearSubjectVM> CourseYearSubjects { get; set; } = default!;
		public CourseYearVM CourseYearVM { get; set; } = default!;
		public IList<Subject> Subjects { get; set; } = new List<Subject>();
		public string SearchSubject { get; set; } = string.Empty!;
        public string SearchCode { get; set; } = string.Empty!;
        public string SearchSemester { get; set; } = string.Empty!;
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

		public async Task<IActionResult> OnGetAsync(int? courseYearId, string? sortColumn, string? sortOrder, int? pageIndex, string? searchSubject, string? searchCode, string? searchSemester)
		{
			if (courseYearId == null)
			{
				return NotFound();
			}

			var courseYear = await _context.CourseYears
				.Include(m => m.CourseInfo)
				.Where(m => m.Active)
				.SingleOrDefaultAsync(m => m.Id == courseYearId);

			if (courseYear == null)
			{
				return NotFound();
			}

			CourseYearVM = new CourseYearVM
			{
				Id = courseYear.Id,
				CourseName = courseYear.Name
			};

			SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
			SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;

			if (_context.CourseYearSubjects != null)
			{
				IQueryable<CourseYearSubject> courseYearSubject = _context.CourseYearSubjects.AsQueryable();

				courseYearSubject = courseYearSubject
					.Include(m => m.CourseYearInfo)
					.Include(m => m.SubjectInfo)
					.Where(m => m.CourseYearId == courseYear.Id
						&& m.CourseYearInfo.Active == true
						&& m.Active == true)
					.OrderByDescending(o => o.Id);

				if (!string.IsNullOrEmpty(searchSubject))
				{
					courseYearSubject = courseYearSubject.Where(m => (m.SubjectInfo.Title)
					.ToLower()
					.Contains(searchSubject.ToLower()));
				}

				if (!string.IsNullOrEmpty(searchCode))
				{
					courseYearSubject = courseYearSubject.Where(m => (m.SubjectInfo.Code)
					.ToLower()
					.Contains(searchCode.ToLower()));
				}

				if (!string.IsNullOrEmpty(searchSemester))
				{
					courseYearSubject = courseYearSubject.Where(m => m.SubjectInfo.Semester == byte.Parse(searchSemester));
				}

				switch (sortColumn)
				{
					case "id":
						courseYearSubject = SortOrder == "asc" ? courseYearSubject.OrderBy(o => o.Id)
							: courseYearSubject.OrderByDescending(o => o.Id);
						break;
					case "subject":
						courseYearSubject = SortOrder == "asc" ? courseYearSubject.OrderBy(o => o.SubjectInfo.Title)
							: courseYearSubject.OrderByDescending(o => o.SubjectInfo.Title);
						break;
					case "code":
						courseYearSubject = SortOrder == "asc" ? courseYearSubject.OrderBy(o => o.SubjectInfo.Code)
							: courseYearSubject.OrderByDescending(o => o.SubjectInfo.Code);
						break;
					case "semester":
						courseYearSubject = SortOrder == "asc" ? courseYearSubject.OrderBy(o => o.SubjectInfo.Semester)
							: courseYearSubject.OrderByDescending(o => o.SubjectInfo.Semester);
						break;
				}

				int pageSize = 10;

				TotalItems = courseYearSubject.Count();

				CourseYearSubjects = await PaginatedList<CourseYearSubjectVM>.CreateAsync(
					courseYearSubject.Select(m => new CourseYearSubjectVM
					{
						Id = m.Id,
						Subject = m.SubjectInfo.Title,
						Code = m.SubjectInfo.Code,
						Semester = ((Semester)m.SubjectInfo.Semester).ToString()
					}).AsNoTracking(),
					pageIndex ?? 1,
					pageSize
				);
			}

			return Page();
		}
	}
}
