using Microsoft.AspNetCore.Authorization;
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
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public PaginatedList<SectionVM> Sections { get; set; } = null!;
		public SectionStudentVM SectionStudentVM { get; set; } = default!;
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public string SearchSection { get; set; } = string.Empty!;
		public string SearchSchoolYear { get; set; } = string.Empty!;
		public string SearchCourse { get; set; } = string.Empty!;
		public string SearchYear { get; set; } = string.Empty!;
		public string SearchStudentUserName { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

		public static string GetEnumDisplayName(Enum value)
		{
			var field = value.GetType().GetField(value.ToString());
			var attribute = field?.GetCustomAttribute<DisplayAttribute>();
			return attribute?.Name ?? value.ToString();
		}

		public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex, string? searchSection, string? searchSchoolYear, string? searchCourse, string? searchYear, string? searchStudentUserName)
		{
			ViewData["SchoolYears"] = new SelectList(_context.SchoolYears, "Id", "Name");
			ViewData["CourseYears"] = new SelectList(_context.CourseYears, "Id", "Name");

			SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
			SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
			SearchSection = string.IsNullOrEmpty(searchSection) ? "" : searchSection;
			SearchSchoolYear = string.IsNullOrEmpty(searchSchoolYear) ? "" : searchSchoolYear;
			SearchCourse = string.IsNullOrEmpty(searchCourse) ? "" : searchCourse;
			SearchYear = string.IsNullOrEmpty(searchYear) ? "" : searchYear;
			SearchStudentUserName = string.IsNullOrEmpty(searchStudentUserName) ? "" : searchStudentUserName;

			if (_context.Sections != null)
			{
				IQueryable<Section> sections = _context.Sections.AsQueryable();

				sections = sections
					.Include(m => m.SchoolYearInfo)
					.Where(m => m.Active == true)
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
					sections = sections.Where(m => m.CourseYearInfo.Id == int.Parse(searchCourse));
				}

				if (!string.IsNullOrEmpty(searchStudentUserName))
				{
					var sectionIds = await _context.SectionStudents
						.Where(m => m.StudentInfo.UserName == searchStudentUserName && m.Active == true)
						.Select(m => m.SectionId)
						.Distinct()
						.ToListAsync();

					sections = sections.Where(m => sectionIds.Contains(m.Id));
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
						sections = SortOrder == "asc" ? sections.OrderBy(o => o.CourseYearInfo.Name)
							: sections.OrderByDescending(o => o.CourseYearInfo.Name);
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
						CourseYearName = m.CourseYearInfo.Name,
					}).AsNoTracking(),
					pageIndex ?? 1,
					pageSize
				);
			}
		}
	}
}
