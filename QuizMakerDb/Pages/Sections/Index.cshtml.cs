using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuizMakerDb.Pages.Sections
{
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

		public PaginatedList<SectionVM> Sections { get; set; } = null!;
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public string SearchSection { get; set; } = string.Empty!;
		public string SearchSchoolYear { get; set; } = string.Empty!;
		public string SearchCourse { get; set; } = string.Empty!;
		public string SearchYear { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

        public static string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex, string? searchSection, string? searchSchoolYear, string? searchCourse, string? searchYear)
		{
			ViewData["SchoolYears"] = new SelectList(_context.SchoolYears, "Id", "Name");
			ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");

			SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
			SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
			searchSection = string.IsNullOrEmpty(searchSection) ? "" : searchSection;
			searchSchoolYear = string.IsNullOrEmpty(searchSchoolYear) ? "" : searchSchoolYear;
			searchCourse = string.IsNullOrEmpty(searchCourse) ? "" : searchCourse;
			searchYear = string.IsNullOrEmpty(searchYear) ? "" : searchYear;


			if (_context.Sections != null)
			{
				IQueryable<Section> sections = _context.Sections.AsQueryable();

				sections = sections
					.Include(m => m.SchoolYearInfo)
					.OrderByDescending(o => o.Id);

				if (!string.IsNullOrEmpty(searchSection))
				{
					sections = sections.Where(m => (m.Name)
					.ToLower()
					.Contains(searchSection.ToLower()));
				}

				if (!string.IsNullOrEmpty(searchSchoolYear))
				{
					sections = sections.Where(m => m.SchoolYearInfo.Id == int.Parse(searchSchoolYear));
				}

				if (!string.IsNullOrEmpty(searchCourse))
				{
					sections = sections.Where(m => m.CourseInfo.Id == int.Parse(searchCourse));
				}

				if (!string.IsNullOrEmpty(searchYear))
				{
					sections = sections.Where(m => m.Year == byte.Parse(searchYear));
				}

				switch (sortColumn)
				{
					case "id":
						sections = SortOrder == "asc" ? sections.OrderBy(o => o.Id)
							: sections.OrderByDescending(o => o.Id);
						break;
					case "section":
						sections = SortOrder == "asc" ? sections.OrderBy(o => o.Name)
							: sections.OrderByDescending(o => o.Name);
						break;
					case "school_year":
						sections = SortOrder == "asc" ? sections.OrderBy(o => o.SchoolYearInfo.Name)
							: sections.OrderByDescending(o => o.SchoolYearInfo.Name);
						break;
					case "course":
						sections = SortOrder == "asc" ? sections.OrderBy(o => o.CourseInfo.Name)
							: sections.OrderByDescending(o => o.CourseInfo.Name);
						break;
					case "year":
						sections = SortOrder == "asc" ? sections.OrderBy(o => o.Year)
							: sections.OrderByDescending(o => o.Year);
						break;
				}

				int pageSize = 10;

				TotalItems = sections.Count();

				Sections = await PaginatedList<SectionVM>.CreateAsync(
					sections.Select(m => new SectionVM
					{
						Id = m.Id,
						Name = m.Name,
						SchoolYearName = m.SchoolYearInfo.Name,
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
