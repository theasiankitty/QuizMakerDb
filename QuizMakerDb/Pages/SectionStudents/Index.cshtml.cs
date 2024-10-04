using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuizMakerDb.Pages.SectionStudents
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public IndexModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[BindProperty]
		public PaginatedList<SectionStudentVM> StudentSections { get; set; } = default!;
		public SectionVM SectionVM { get; set; } = default!;
		public IList<Student> Students { get; set; } = new List<Student>();
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public string SearchStudentInSection { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

		public async Task<IActionResult> OnGetAsync(int? sectionId, string? sortColumn, string? sortOrder, int? pageIndex, string? searchStudentInSection)
		{
			if (sectionId == null)
			{
				return NotFound();
			}

			var section = await _context.Sections
				.Include(m => m.CourseYearInfo)
				.Include(m => m.SchoolYearInfo)
				.FirstOrDefaultAsync(s => s.Id == sectionId);

			if (section == null)
			{
				return NotFound();
			}

			SectionVM = new SectionVM
			{
				Id = section.Id,
				Name = section.Name + " - " + section.CourseYearInfo.Name,
				SchoolYearId = section.SchoolYearId,
				SchoolYearName = section.SchoolYearInfo.Name,
				Year = section.CourseYearInfo.Year.ToString()
			};

			SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
			SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
			SearchStudentInSection = string.IsNullOrEmpty(searchStudentInSection) ? "" : searchStudentInSection;

			if (_context.SectionStudents != null)
			{
				IQueryable<SectionStudent> studentSection = _context.SectionStudents.AsQueryable();

				studentSection = studentSection
					.Include(m => m.StudentInfo)
					.Where(m => m.SectionId == section.Id
						&& m.SectionInfo.SchoolYearId == section.SchoolYearId
						&& m.StudentInfo.Active == true
						&& m.Active == true)
					.OrderByDescending(o => o.Id);

				if (!string.IsNullOrEmpty(searchStudentInSection))
				{
					int studentNumber = 0;

					try
					{
						int.TryParse(searchStudentInSection, out studentNumber);
					}
					catch (Exception ex)
					{
						var err = ex.Message;
					}

					if (studentNumber == 0)
					{
						studentSection = studentSection
							.Where(m => (m.StudentInfo.FirstName + " " + m.StudentInfo.LastName)
							.ToLower()
							.Contains(searchStudentInSection.ToLower()));
					}
					else
					{
						studentSection = studentSection
							.Where(m => m.StudentInfo.UserName == searchStudentInSection);
					}
				}

				switch (sortColumn)
				{
					case "id":
						studentSection = SortOrder == "asc" ? studentSection.OrderBy(o => o.Id)
							: studentSection.OrderByDescending(o => o.Id);
						break;
					case "name":
						studentSection = SortOrder == "asc" ? studentSection.OrderBy(o => o.StudentInfo.FirstName + " " + o.StudentInfo.LastName)
							: studentSection.OrderByDescending(o => o.StudentInfo.FirstName + " " + o.StudentInfo.LastName);
						break;
					case "username":
						studentSection = SortOrder == "asc" ? studentSection.OrderBy(o => o.StudentInfo.UserName)
							: studentSection.OrderByDescending(o => o.StudentInfo.UserName);
						break;
					case "sex":
						studentSection = SortOrder == "asc" ? studentSection.OrderByDescending(o => o.StudentInfo.Sex)
							: studentSection.OrderBy(o => o.StudentInfo.Sex);
						break;
				}

				int pageSize = 10;

				TotalItems = studentSection.Count();

				StudentSections = await PaginatedList<SectionStudentVM>.CreateAsync(
					studentSection.Select(m => new SectionStudentVM
					{
						Id = m.Id,
						Student = m.StudentInfo.FirstName + " " + m.StudentInfo.LastName,
						StudentUserName = m.StudentInfo.UserName,
						SexDescription = ((Sex)m.StudentInfo.Sex).ToString()
					}).AsNoTracking(),
					pageIndex ?? 1,
					pageSize
				);
			}

			return Page();
		}
	}
}
