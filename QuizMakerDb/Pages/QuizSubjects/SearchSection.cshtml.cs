using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using System.Drawing.Printing;

namespace QuizMakerDb.Pages.QuizSubjects
{
    public class SearchSectionModel : PageModel
    {
		private readonly ApplicationDbContext _context;

		public SearchSectionModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetSearchSectionBySubjectAsync([FromQuery] int courseYearId , int quizId, int teacherId)
		{
			var teacherSubjects = await _context.TeacherSubjects
				.Include(m => m.CourseYearSubjectInfo)
				.Where(m => m.TeacherId == teacherId && m.Active)
				.Select(m => m.CourseYearSubjectInfo.CourseYearId)
				.ToListAsync();

			var unassignedTeacherSections = await _context.Sections
				.Include(m => m.CourseYearInfo)
				.Where(m => m.CourseYearId == courseYearId && teacherSubjects.Contains(m.CourseYearId) && m.Active)
				.Select(m => new
				{
					m.Id,
					m.Name,
					CourseYear = m.CourseYearInfo.Name,
				})
				.OrderByDescending(o => o.Id)
				.ToListAsync();

			var dataCount = unassignedTeacherSections.Count;

			if (quizId != 0 && teacherId != 0)
			{
				var assignedSections = await _context.QuizSubjects
					.Where(m => m.QuizId == quizId
						&& m.QuizInfo.TeacherId == teacherId
						&& m.Active)
					.Select(m => m.SectionId)
					.ToListAsync();

				unassignedTeacherSections = unassignedTeacherSections
					.Where(m => !assignedSections.Contains(m.Id))
					.ToList();

				dataCount = unassignedTeacherSections.Count;
			}

			if (unassignedTeacherSections == null || !unassignedTeacherSections.Any())
			{
				return new JsonResult(new { message = "Subject Not Found." });
			}

			return new JsonResult(new { message = "OK", sections = unassignedTeacherSections });
		}
	}
}
