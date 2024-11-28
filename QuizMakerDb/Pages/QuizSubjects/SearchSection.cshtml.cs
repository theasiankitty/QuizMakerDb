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

		//public async Task<JsonResult> OnGetSearchSectionBySubjectAsync([FromQuery] int courseYearId, int quizId, int teacherId)
		//{
		//	var teacherSubjects = await _context.TeacherSubjects
		//		.Where(m => m.TeacherId == teacherId && m.Active)
		//		.Select(m => m.CourseYearSubjectInfo.CourseYearId)
		//		.ToListAsync();

		//	var assignedSubjects = await _context.QuizSubjects
		//		.Where(m => m.QuizId == quizId)
		//		.Select(m => m.SubjectId)
		//		.ToListAsync();

		//	var unassignedSubjects = await _context.CourseYearSubjects
		//		.Where(m => !assignedSubjects.Contains(m.SubjectId) && m.CourseYearId == courseYearId && m.Active)
		//		.Select(m => new
		//		{
		//			Id = m.CourseYearId,
		//			Name = m.SubjectInfo.Title,
		//			CourseYear = m.CourseYearInfo.Name
		//		})
		//		.ToListAsync();

		//	if (unassignedSubjects == null || !unassignedSubjects.Any())
		//	{
		//		return new JsonResult(new { message = "Subject Not Found." });
		//	}

		//	return new JsonResult(new { message = "OK", sections = unassignedSubjects });
		//}

		public async Task<JsonResult> OnGetSearchSectionBySubjectAsync([FromQuery] int courseYearId, int quizId, int teacherId)
		{
			// Step 1: Get Teacher's Subject IDs (linked via TeacherSubjects and CourseYearSubjects)
			var teacherSubjectIds = await _context.TeacherSubjects
				.Where(ts => ts.TeacherId == teacherId && ts.Active)
				.Select(ts => ts.CourseYearSubjectInfo.SubjectId)
				.ToListAsync();

			// Step 2: Get the assigned section IDs for the quiz
			var assignedSectionIds = await _context.QuizSubjects
				.Where(qs => qs.QuizId == quizId && qs.QuizInfo.TeacherId == teacherId)
				.Select(qs => qs.SectionId)
				.ToListAsync();

			// Step 3: Get valid CourseYearIds linked to Teacher's SubjectIds
			var validCourseYearIds = await _context.CourseYearSubjects
				.Where(cys => teacherSubjectIds.Contains(cys.SubjectId))
				.Select(cys => cys.CourseYearId)
				.ToListAsync();

			// Step 4: Fetch unassigned sections
			var unassignedSections = await _context.Sections
				.Include(s => s.CourseYearInfo) // For accessing the course year details
				.Where(s => validCourseYearIds.Contains(s.CourseYearId) && // Match CourseYearId
							s.Active &&
							!assignedSectionIds.Contains(s.Id)) // Exclude already assigned sections
				.Select(s => new
				{
					s.Id,
					s.Name,
					CourseYear = s.CourseYearInfo.Name
				})
				.OrderByDescending(s => s.Id)
				.ToListAsync();

			// Return the result
			if (!unassignedSections.Any())
			{
				return new JsonResult(new { message = "No unassigned sections found." });
			}

			return new JsonResult(new { message = "OK", sections = unassignedSections });
		}

		//public async Task<JsonResult> OnGetSearchSectionBySubjectAsync([FromQuery] int courseYearId, int quizId, int teacherId)
		//{
		//	// Step 1: Get CourseYearSubjectIds assigned to the teacher
		//	var teacherCourseYearSubjectIds = await _context.TeacherSubjects
		//		.Where(ts => ts.TeacherId == teacherId && ts.Active)
		//		.Select(ts => ts.CourseYearSubjectId)
		//		.ToListAsync();

		//	if (!teacherCourseYearSubjectIds.Any())
		//	{
		//		return new JsonResult(new { message = "Teacher has no assigned CourseYearSubjects." });
		//	}

		//	// Step 2: Get CourseYearIds linked to the teacher's CourseYearSubjects
		//	var teacherCourseYearIds = await _context.CourseYearSubjects
		//		.Where(cys => teacherCourseYearSubjectIds.Contains(cys.Id))
		//		.Select(cys => cys.CourseYearId)
		//		.ToListAsync();

		//	if (!teacherCourseYearIds.Contains(courseYearId))
		//	{
		//		return new JsonResult(new { message = "Course Year is not assigned to this teacher." });
		//	}

		//	// Step 3: Get sections already assigned to the quiz
		//	var assignedSectionIds = await _context.QuizSubjects
		//		.Where(qs => qs.QuizId == quizId && qs.QuizInfo.TeacherId == teacherId)
		//		.Select(qs => qs.SectionId)
		//		.ToListAsync();

		//	// Step 4: Fetch unassigned sections for the given CourseYearId
		//	var unassignedSections = await _context.Sections
		//		.Include(s => s.CourseYearInfo)
		//		.Where(s => s.CourseYearId == courseYearId &&
		//					s.Active &&
		//					!assignedSectionIds.Contains(s.Id)) // Exclude already assigned sections
		//		.Select(s => new
		//		{
		//			s.Id,
		//			s.Name,
		//			CourseYear = s.CourseYearInfo.Name
		//		})
		//		.OrderByDescending(s => s.Id)
		//		.ToListAsync();

		//	if (!unassignedSections.Any())
		//	{
		//		return new JsonResult(new { message = "No unassigned sections found." });
		//	}

		//	return new JsonResult(new { message = "OK", sections = unassignedSections });
		//}

		//public async Task<JsonResult> OnGetSearchSectionBySubjectAsync([FromQuery] int courseYearId, int quizId, int teacherId)
		//{
		//	// Step 1: Get all course years associated with the teacher
		//	var teacherSubjects = await _context.TeacherSubjects
		//		.Include(m => m.CourseYearSubjectInfo)
		//		.Where(m => m.TeacherId == teacherId && m.Active)
		//		.Select(m => m.CourseYearSubjectInfo.CourseYearId)
		//		.Distinct()
		//		.ToListAsync();

		//	// Ensure the teacher has subjects in the specified course year
		//	if (!teacherSubjects.Contains(courseYearId))
		//	{
		//		return new JsonResult(new { message = "Teacher is not assigned to this course year." });
		//	}

		//	// Step 2: Get sections already assigned to the quiz
		//	var assignedSections = await _context.QuizSubjects
		//		.Where(m => m.QuizId == quizId && m.QuizInfo.TeacherId == teacherId)
		//		.Select(m => m.SectionId)
		//		.ToListAsync();

		//	// Step 3: Find unassigned sections
		//	var unassignedSections = await _context.Sections
		//		.Include(m => m.CourseYearInfo)
		//		.Where(m => m.CourseYearId == courseYearId &&
		//					teacherSubjects.Contains(m.CourseYearId) &&
		//					m.Active &&
		//					!assignedSections.Contains(m.Id))
		//		.Select(m => new
		//		{
		//			m.Id,
		//			m.Name,
		//			CourseYear = m.CourseYearInfo.Name
		//		})
		//		.OrderByDescending(o => o.Id)
		//		.ToListAsync();

		//	// Step 4: Check if there are unassigned sections
		//	if (!unassignedSections.Any())
		//	{
		//		return new JsonResult(new { message = "No unassigned sections found." });
		//	}

		//	// Step 5: Return the unassigned sections
		//	return new JsonResult(new { message = "OK", sections = unassignedSections });
		//}

		//public async Task<JsonResult> OnGetSearchSectionBySubjectAsync([FromQuery] int courseYearId , int quizId, int teacherId)
		//{
		//	var teacherSubjects = await _context.TeacherSubjects
		//		.Include(m => m.CourseYearSubjectInfo)
		//		.Where(m => m.TeacherId == teacherId && m.Active)
		//		.Select(m => m.CourseYearSubjectInfo.CourseYearId)
		//		.ToListAsync();

		//	//var courseYear = await _context.CourseYears
		//	//	.Where(m => teacherSubjects.Contains(m.CourseId))
		//	//	.Select(m => m.Id)
		//	//	.ToListAsync();

		//	var assignedSections = await _context.QuizSubjects
		//		.Where(m => m.QuizId == quizId && m.QuizInfo.TeacherId == teacherId)
		//		.Select(m => m.SectionId)
		//		.ToListAsync();

		//	var unassignedSections = await _context.TeacherSubjects


		//	return new JsonResult(new { message = "OK", sections = teacherSubjects });

		//	//var unassignedTeacherSections = await _context.Sections
		//	//	.Include(m => m.CourseYearInfo)
		//	//	.Where(m => m.CourseYearId == courseYearId && teacherSubjects.Contains(m.CourseYearId) && m.Active)
		//	//	.Select(m => new
		//	//	{
		//	//		m.Id,
		//	//		m.Name,
		//	//		CourseYear = m.CourseYearInfo.Name,
		//	//	})
		//	//	.OrderByDescending(o => o.Id)
		//	//	.ToListAsync();

		//	//var dataCount = unassignedTeacherSections.Count;

		//	//if (quizId != 0 && teacherId != 0)
		//	//{
		//	//	var assignedSections = await _context.QuizSubjects
		//	//		.Where(m => m.QuizId == quizId && m.QuizInfo.TeacherId == teacherId)
		//	//		.Select(m => m.SectionId)
		//	//		.ToListAsync();

		//	//	unassignedTeacherSections = unassignedTeacherSections
		//	//		.Where(m => !assignedSections.Contains(m.Id))
		//	//		.ToList();

		//	//	dataCount = unassignedTeacherSections.Count;
		//	//}

		//	//if (unassignedTeacherSections == null || !unassignedTeacherSections.Any())
		//	//{
		//	//	return new JsonResult(new { message = "Subject Not Found." });
		//	//}

		//	//return new JsonResult(new { message = "OK", sections = unassignedTeacherSections });
		//}
	}
}
