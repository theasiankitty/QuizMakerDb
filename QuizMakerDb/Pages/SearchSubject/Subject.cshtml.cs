using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.SearchSubject
{
	public class SubjectModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public SubjectModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetSearchSubjectAsync([FromQuery] string searchSubject, int courseYearId)
		{
			if (string.IsNullOrEmpty(searchSubject))
			{
				return new JsonResult(new { message = "Invalid Subject." });
			}

			var unassignedSubjects = await _context.Subjects
					.Where(m => (m.Code).ToLower().Contains(searchSubject)
					&& m.Active)
					.ToListAsync();

			var assignedSubject = await _context.CourseYearSubjects
					.Where(m => m.Active
					&& m.CourseYearId == courseYearId)
					.Select(m => m.SubjectId)
					.ToListAsync();

			if (unassignedSubjects.Any())
			{
				unassignedSubjects = unassignedSubjects
					.Where(m => !assignedSubject.Contains(m.Id)).ToList();
			}

			if (unassignedSubjects == null)
			{
				return new JsonResult(new { message = "Subject Not Found." });
			}

			return new JsonResult(new { message = "OK", subjects = unassignedSubjects });
		}

		public async Task<JsonResult> OnGetAllSubjectByCourseYearAsync([FromQuery] int courseYearId, int currentPage, int pageSize)
		{
			var unassignedSubjects = await _context.Subjects
					.Where(m => m.Active)
					.ToListAsync();

			var dataCount = unassignedSubjects.Count;

			var assignedSubject = await _context.CourseYearSubjects
					.Where(m => m.Active
					&& m.CourseYearId == courseYearId)
					.Select(m => m.SubjectId)
					.ToListAsync();

			if (unassignedSubjects.Any())
			{
				dataCount = unassignedSubjects
					.Where(m => !assignedSubject.Contains(m.Id))
					.ToList()
					.Count;

				unassignedSubjects = unassignedSubjects
					.Where(m => !assignedSubject.Contains(m.Id))
					.Skip(currentPage * pageSize)
					.Take(pageSize)
					.ToList();
			}

			if (unassignedSubjects == null)
			{
				return new JsonResult(new { message = "Subject Not Found." });
			}

			return new JsonResult(new { message = "OK", subjects = unassignedSubjects, count = dataCount });
		}

		public async Task<JsonResult> OnGetAllSubjects([FromQuery] int teacherId, int currentPage, int pageSize)
		{
			var unassignedSubjects = await _context.CourseYearSubjects
					.Include(m => m.SubjectInfo)
					.Where(m => m.Active)
					.Select(m => new CourseYearSubjectVM
					{
						Id = m.Id,
						Subject = m.SubjectInfo.Title,
						Code = m.SubjectInfo.Code,
						SubjectId = m.SubjectId,
						CourseYearId = m.CourseYearId,
						CourseYear = m.CourseYearInfo.Name,
					})
					.OrderBy(m => m.Code)
					.ToListAsync();

			var assignedSubject = await _context.TeacherSubjects
					.Where(m => m.TeacherId == teacherId
					&& m.Active)
					.Select(m => m.CourseYearSubjectId)
					.ToListAsync();

			if (unassignedSubjects.Any())
			{
				unassignedSubjects = unassignedSubjects
					.Where(m => !assignedSubject.Contains(m.Id))
					.Skip(currentPage * pageSize)
					.Take(pageSize)
					.ToList();
			}

			var dataCount = unassignedSubjects.Count;

			if (unassignedSubjects == null)
			{
				return new JsonResult(new { message = "Subject Not Found." });
			}

			return new JsonResult(new { message = "OK", subjects = unassignedSubjects, count = dataCount });
		}

		public async Task<JsonResult> OnGetSearchSubjectByCourseYearAsync([FromQuery] int teacherId, int courseYearId)
		{
			var unassignedSubjects = await _context.CourseYearSubjects
					.Include(m => m.SubjectInfo)
					.Where(m => m.CourseYearId == courseYearId
					&& m.Active)
					.Select(m => new CourseYearSubjectVM
					{
						Id = m.Id,
						Subject = m.SubjectInfo.Title,
						Code = m.SubjectInfo.Code,
						SubjectId = m.SubjectId,
						CourseYearId = m.CourseYearId,
						CourseYear = m.CourseYearInfo.Name,
					})
					.OrderBy(m => m.Code)
					.ToListAsync();

			var assignedSubject = await _context.TeacherSubjects
					.Where(m => m.Active
					&& m.TeacherId == teacherId
					&& m.CourseYearSubjectId == courseYearId)
					.Select(m => m.CourseYearSubjectId)
					.ToListAsync();


			if (unassignedSubjects.Any())
			{
				unassignedSubjects = unassignedSubjects
					.Where(m => !assignedSubject.Contains(m.Id))
					.ToList();
			}

			if (unassignedSubjects == null || !unassignedSubjects.Any())
			{
				return new JsonResult(new { message = "Subject Not Found." });
			}

			return new JsonResult(new { message = "OK", subjects = unassignedSubjects });
		}
	}
}