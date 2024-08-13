using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuizMakerDb.Pages.CourseYears
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public PaginatedList<CourseYearVM> CourseYears { get; set; } = null!;
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public string SearchCourse { get; set; } = string.Empty!;
		public string SearchYear { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

		public static string GetEnumDisplayName(Enum value)
		{
			var field = value.GetType().GetField(value.ToString());
			var attribute = field?.GetCustomAttribute<DisplayAttribute>();
			return attribute?.Name ?? value.ToString();
		}

		public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex, string? searchCourse, string? searchYear)
		{
			ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");

			SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
			SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
			searchCourse = string.IsNullOrEmpty(searchCourse) ? "" : searchCourse;
			searchYear = string.IsNullOrEmpty(searchYear) ? "" : searchYear;

			if (_context.CourseYears != null)
			{
				IQueryable<CourseYear> courseYears = _context.CourseYears.AsQueryable();

				courseYears = courseYears
					.Include(m => m.CourseInfo)
					.OrderByDescending(o => o.Id);

				if (!string.IsNullOrEmpty(searchCourse))
				{
					courseYears = courseYears.Where(m => m.CourseId == int.Parse(searchCourse));
				}

				if (!string.IsNullOrEmpty(searchYear))
				{
					courseYears = courseYears.Where(m => m.Year == byte.Parse(searchYear));
				}

				switch (sortColumn)
				{
					case "id":
						courseYears = SortOrder == "asc" ? courseYears.OrderBy(o => o.Id)
							: courseYears.OrderByDescending(o => o.Id);
						break;
					case "course":
						courseYears = SortOrder == "asc" ? courseYears.OrderBy(o => o.CourseInfo.Name)
							: courseYears.OrderByDescending(o => o.CourseInfo.Name);
						break;
					case "year":
						courseYears = SortOrder == "asc" ? courseYears.OrderBy(o => o.Year)
							: courseYears.OrderByDescending(o => o.Year);
						break;
				}

				int pageSize = 10;

				TotalItems = courseYears.Count();

				CourseYears = await PaginatedList<CourseYearVM>.CreateAsync(
					courseYears.Select(m => new CourseYearVM
					{
						Id = m.Id,
						CourseName = m.CourseInfo.Name,
						Year = GetEnumDisplayName((YearLevel)m.Year),
					}).AsNoTracking(),
					pageIndex ?? 1,
					pageSize
				);
			}
		}
	}
}
