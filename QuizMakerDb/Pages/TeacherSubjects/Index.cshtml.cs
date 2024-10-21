using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;

namespace QuizMakerDb.Pages.TeacherSubjects
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public IndexModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public PaginatedList<TeacherSubjectVM> TeacherSubjects { get; set; } = null!;
		public TeacherVM TeacherVM { get; set; } = default!;
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public string SearchSubject { get; set; } = string.Empty!;
		public string SearchCode { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

		public async Task<IActionResult> OnGetAsync(int? teacherId, string? sortColumn, string? sortOrder, int? pageIndex, string? searchSubject, string? searchCode)
		{
			ViewData["CourseYears"] = new SelectList(_context.CourseYears.Where(m => m.Active == true), "Id", "Name");

			if (teacherId == null)
			{
				return NotFound();
			}

			var teacher = await _context.Teachers
				.Where(m => m.Active)
				.SingleOrDefaultAsync(m => m.Id == teacherId);

			if (teacher == null)
			{
				return NotFound();
			}

			TeacherVM = new TeacherVM
			{
				Id = teacher.Id,
				Teacher = teacher.FirstName + " " + teacher.LastName,
			};

			SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
			SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
			SearchSubject = string.IsNullOrEmpty(searchSubject) ? "" : searchSubject;

			if (_context.TeacherSubjects != null)
			{
				IQueryable<TeacherSubject> teacherSubjects = _context.TeacherSubjects.AsQueryable();

				teacherSubjects = teacherSubjects
					.Include(m => m.TeacherInfo)
					.Where(m => m.TeacherId == teacher.Id && m.Active == true)
					.OrderByDescending(o => o.Id);

				if (!string.IsNullOrEmpty(searchSubject))
				{
					teacherSubjects = teacherSubjects.Where(m => (m.CourseYearSubjectInfo.SubjectInfo.Title)
					.ToLower()
					.Contains(searchSubject.ToLower()));
				}

				if (!string.IsNullOrEmpty(searchCode))
				{
					teacherSubjects = teacherSubjects.Where(m => (m.CourseYearSubjectInfo.SubjectInfo.Code)
					.ToLower()
					.Contains(searchCode.ToLower()));
				}

				switch (sortColumn)
				{
					case "id":
						teacherSubjects = SortOrder == "asc" ? teacherSubjects.OrderBy(o => o.Id)
							: teacherSubjects.OrderByDescending(o => o.Id);
						break;
					case "subject":
						teacherSubjects = SortOrder == "asc" ? teacherSubjects.OrderBy(o => o.CourseYearSubjectInfo.SubjectInfo.Title)
							: teacherSubjects.OrderByDescending(o => o.CourseYearSubjectInfo.SubjectInfo.Title);
						break;
					case "code":
						teacherSubjects = SortOrder == "asc" ? teacherSubjects.OrderBy(o => o.CourseYearSubjectInfo.SubjectInfo.Code)
							: teacherSubjects.OrderByDescending(o => o.CourseYearSubjectInfo.SubjectInfo.Code);
						break;
				}

				int pageSize = 10;

				TotalItems = teacherSubjects.Count();

				TeacherSubjects = await PaginatedList<TeacherSubjectVM>.CreateAsync(
					teacherSubjects.Select(m => new TeacherSubjectVM
					{
						Id = m.Id,
						Subject = m.CourseYearSubjectInfo.SubjectInfo.Title,
						Code = m.CourseYearSubjectInfo.SubjectInfo.Code,
						CourseYear = m.CourseYearSubjectInfo.CourseYearInfo.Name
					}).AsNoTracking(),
					pageIndex ?? 1,
					pageSize
				);
			}

			return Page();
		}
	}
}