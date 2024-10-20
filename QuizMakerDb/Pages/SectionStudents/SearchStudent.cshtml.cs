using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using System.Drawing.Printing;

namespace QuizMakerDb.Pages.SectionStudents
{
	public class SearchStudentModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public SearchStudentModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<JsonResult> OnGetSearchStudentAsync([FromQuery] string searchStudent, int sectionId, int schoolYearId, byte year)
		{
			if (string.IsNullOrEmpty(searchStudent))
			{
				return new JsonResult(new { message = "Invalid Student." });
			}

			var unassignedStudents = await _context.Students
					.Where(m => m.Active)
					.OrderByDescending(o => o.Id)
					.ToListAsync();

			bool isNumeric = int.TryParse(searchStudent, out int studentNumber);

			if (isNumeric)
			{
				unassignedStudents = unassignedStudents.Where(m => m.UserName == searchStudent).ToList();
			}
			else
			{
				string searchLower = searchStudent.ToLower();
				unassignedStudents = unassignedStudents
					.Where(m => (m.FirstName + " " + m.LastName).ToLower().Contains(searchLower))
					.ToList();
			}

			if (sectionId != 0)
			{
				if (year != 0)
				{
					List<int> assignedIrregularStudents;
					List<int> assignedStudents;

					var irregularStudentsInThisYear = await _context.SectionStudents
						.Where(m => m.StudentInfo.isIrregular
							&& m.SectionId != 0
							&& m.Active
							&& m.SectionInfo.CourseYearInfo.Year == year)
						.Select(m => m.StudentId)
						.ToListAsync();

					if (irregularStudentsInThisYear.Count == 0)
					{
						assignedIrregularStudents = await _context.SectionStudents
							.Where(m => m.SectionId != 0
								&& m.SchoolYearId == schoolYearId
								&& m.Active == true
								&& m.StudentInfo.isIrregular)
							.Select(m => m.StudentId)
							.ToListAsync();
					}
					else
					{
						assignedIrregularStudents = await _context.SectionStudents
							.Where(m => m.SectionId != 0
								&& m.SchoolYearId == schoolYearId
								&& m.Active == true
								&& m.StudentInfo.isIrregular
								&& !irregularStudentsInThisYear.Contains(m.StudentId))
							.Select(m => m.StudentId)
							.ToListAsync();
					}

					assignedStudents = await _context.SectionStudents
							.Where(m => m.SectionId != 0
								&& m.SchoolYearId == schoolYearId
								&& m.Active == true
								&& !assignedIrregularStudents.Contains(m.StudentId))
							.Select(m => m.StudentId)
							.ToListAsync();


					unassignedStudents = unassignedStudents
						.Where(m => !assignedStudents.Contains(m.Id))
						.OrderBy(m => m.LastName + " " + m.FirstName)
						.ToList();

					// if the student is irregular, and it is already assigned in a section
					// then get the section course year
					// if the section course year is less than the section course year passed parameter
					// then we can still assigned the irregular student
					// else we can't or don't show
					//var irregularStudents = await _context.SectionStudents
					//	.Where(m => m.StudentInfo.isIrregular
					//	&& m.SectionId != 0
					//	&& m.Active
					//	&& m.SectionInfo.CourseYearInfo.Year == year)
					//	.Select(m => m.StudentId)
					//	.ToListAsync();

					//List<int> assignedStudents;

					//if (irregularStudents.Count == 0)
					//{
					//	assignedStudents = await _context.SectionStudents
					//	.Where(m => m.SectionId != 0
					//		&& m.SchoolYearId == schoolYearId
					//		&& m.Active == true
					//		&& !m.StudentInfo.isIrregular)
					//	.Select(m => m.StudentId)
					//	.ToListAsync();
					//}
					//else
					//{
					//	assignedStudents = await _context.SectionStudents
					//	.Where(m => m.SectionId != 0
					//		&& m.SchoolYearId == schoolYearId
					//		&& m.Active == true)
					//	.Select(m => m.StudentId)
					//	.ToListAsync();
					//}

					//unassignedStudents = unassignedStudents
					//	.Where(m => !assignedStudents.Contains(m.Id))
					//	.OrderBy(m => m.LastName + " " + m.FirstName)
					//	.ToList();
				}
			}

			if (unassignedStudents == null)
			{
				return new JsonResult(new { message = "Student Not Found." });
			}

			return new JsonResult(new { message = "OK", students = unassignedStudents });
		}

		public async Task<JsonResult> OnGetAllStudentAsync([FromQuery] int sectionId, int schoolYearId, byte? year, int currentPage, int pageSize)
		{
			var unassignedStudents = await _context.Students
					.Where(m => m.Active)
					.OrderByDescending(o => o.Id)
					.ToListAsync();

			var dataCount = unassignedStudents.Count;

			if (sectionId != 0)
			{
				if (year != null)
				{
					List<int> assignedIrregularStudents;
					List<int> assignedStudents;

					var irregularStudentsInThisYear = await _context.SectionStudents
						.Where(m => m.StudentInfo.isIrregular
							&& m.SectionId != 0 // it's either this to test
							&& m.Active
							&& m.SectionInfo.CourseYearInfo.Year == year)
						.Select(m => m.StudentId)
						.ToListAsync();

					if (irregularStudentsInThisYear.Count == 0)
					{
						assignedIrregularStudents = await _context.SectionStudents
							.Where(m => m.SectionId != 0
								&& m.SchoolYearId == schoolYearId
								&& m.Active == true
								&& m.StudentInfo.isIrregular)
							.Select(m => m.StudentId)
							.ToListAsync();
					}
					else
					{
						assignedIrregularStudents = await _context.SectionStudents
							.Where(m => m.SectionId != 0
								&& m.SchoolYearId == schoolYearId
								&& m.Active == true
								&& m.StudentInfo.isIrregular
								&& !irregularStudentsInThisYear.Contains(m.StudentId))
							.Select(m => m.StudentId)
							.ToListAsync();
					}

					assignedStudents = await _context.SectionStudents
						.Where(m => m.SectionId != 0
							&& m.SchoolYearId == schoolYearId
							&& m.Active == true
							&& !assignedIrregularStudents.Contains(m.StudentId))
						.Select(m => m.StudentId)
						.ToListAsync();

					unassignedStudents = unassignedStudents
						.Where(m => !assignedStudents.Contains(m.Id))
						.OrderBy(m => m.LastName + " " + m.FirstName)
						.Skip(currentPage * pageSize)
						.Take(pageSize)
						.ToList();

					dataCount = unassignedStudents
						.Where(m => !assignedStudents.Contains(m.Id))
						.ToList()
						.Count;
				}
			}

			if (unassignedStudents == null)
			{
				return new JsonResult(new { message = "Student Not Found." });
			}

			return new JsonResult(new { message = "OK", students = unassignedStudents, count = dataCount });
		}
	}
}
