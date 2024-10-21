using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Data;
using QuizMakerDb.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuizMakerDb.Pages.QuizSubjects
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public PaginatedList<QuizSubjectVM> QuizSubjects { get; set; } = default!;
		public QuizVM QuizVM { get; set; } = default!;
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public string SearchSubject { get; set; } = string.Empty!;
		public string SearchSection { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

		public class TeacherSubjectData
		{
			public int Id { get; set; }
			public int SubjectId { get; set; }
			public string Name { get; set; } = string.Empty!;
		}

		public async Task<IActionResult> OnGetAsync(int? quizId, string? sortColumn, string? sortOrder, int? pageIndex, string? searchSubject, string? searchSection)
		{
			try
			{
				if (quizId == null)
				{
					return NotFound();
				}

				var quiz = await _context.Quizzes.FirstOrDefaultAsync(m => m.Id == quizId);

				if (quiz == null)
				{
					return NotFound();
				}

				var teacherSubjects = _context.TeacherSubjects
					.Include(m => m.CourseYearSubjectInfo)
						.ThenInclude(m => m.SubjectInfo)
					.Include(m => m.CourseYearSubjectInfo)
						.ThenInclude(c => c.CourseYearInfo)
					.Where(m => m.TeacherId == quiz.TeacherId && m.Active)
					.Select(m => new TeacherSubjectData
					{
						Id = m.CourseYearSubjectInfo.CourseYearId,
						Name = m.CourseYearSubjectInfo.CourseYearInfo.Name + " - (" + m.CourseYearSubjectInfo.SubjectInfo.Code + ")",
						SubjectId = m.CourseYearSubjectInfo.SubjectId
					})
					.ToList();

				ViewData["TeacherSubjects"] = teacherSubjects;

				QuizVM = new QuizVM
				{
					Id = quiz.Id,
					Title = quiz.Title,
					TeacherId = quiz.TeacherId
				};

				SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
				SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
				SearchSubject = string.IsNullOrEmpty(searchSubject) ? "" : searchSubject;
				SearchSection = string.IsNullOrEmpty(searchSection) ? "" : searchSection;

				if (_context.QuizSubjects != null)
				{
					IQueryable<QuizSubject> quizSubjects = _context.QuizSubjects.AsQueryable();

					quizSubjects = quizSubjects
						.Include(m => m.QuizInfo)
						.Include(m => m.SubjectInfo)
						.Include(m => m.SectionInfo)
						.ThenInclude(s => s.CourseYearInfo)
						.Where(m => m.QuizId == quiz.Id);

					if (!string.IsNullOrEmpty(searchSection))
					{
						quizSubjects = quizSubjects
							.Where(m => (m.SectionInfo.Name)
							.ToLower()
							.Contains(searchSection.ToLower()));
					}

					switch (sortColumn)
					{
						case "id":
							quizSubjects = SortOrder == "asc" ? quizSubjects.OrderBy(o => o.Id)
								: quizSubjects.OrderByDescending(o => o.Id);
							break;
						case "subject":
							quizSubjects = SortOrder == "asc" ? quizSubjects.OrderBy(o => o.SubjectInfo.Code)
								: quizSubjects.OrderByDescending(o => o.SubjectInfo.Code);
							break;
						case "course_year":
							quizSubjects = SortOrder == "asc" ? quizSubjects.OrderBy(o => o.SectionInfo.CourseYearInfo.Name)
								: quizSubjects.OrderByDescending(o => o.SectionInfo.CourseYearInfo.Name);
							break;
						case "section":
							quizSubjects = SortOrder == "asc" ? quizSubjects.OrderBy(o => o.SectionInfo.Name)
								: quizSubjects.OrderByDescending(o => o.SectionInfo.Name);
							break;
						case "code":
							quizSubjects = SortOrder == "asc" ? quizSubjects.OrderByDescending(o => o.Code)
								: quizSubjects.OrderBy(o => o.Code);
							break;
					}

					int pageSize = 10;

					TotalItems = quizSubjects.Count();

					QuizSubjects = await PaginatedList<QuizSubjectVM>.CreateAsync(
						quizSubjects.Select(m => new QuizSubjectVM
						{
							Id = m.Id,
							Subject = m.SubjectInfo.Code,
							CourseYear = m.SectionInfo.CourseYearInfo.Name,
							Section = m.SectionInfo.Name,
							Code = m.Code,
							Active = m.Active
						}).AsNoTracking(),
						pageIndex ?? 1,
						pageSize
					);
				}

				return Page();
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error");
			}
		}
	}
}
