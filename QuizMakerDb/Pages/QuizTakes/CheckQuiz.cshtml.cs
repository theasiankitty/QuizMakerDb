using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Pages.QuizTakes
{
	public class CheckQuizModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public CheckQuizModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_context = context;
		}

		public class StudentData
		{
            public int StudentId { get; set; }

            public string Code { get; set; } = string.Empty!;
        }

        public async Task<JsonResult> OnGetQuizAsync([FromQuery] StudentData studentData)
		{
            if (studentData == null)
            {
                return new JsonResult(new { message = "Invalid Student" });
            }

            try
			{
				var sectionStudent = await _context.SectionStudents
					.Where(m => m.StudentId == studentData.StudentId && m.Active)
					.FirstOrDefaultAsync();

				if (sectionStudent == null)
				{
                    return new JsonResult(new { message = "Invalid Section" });
                }

				var quizSubject = await _context.QuizSubjects
					.FirstOrDefaultAsync(m => m.Code == studentData.Code
						&& m.SectionId == sectionStudent.SectionId
						&& m.Active);

				if (quizSubject == null)
				{
					return new JsonResult(new { message = "ERROR" });
				}
				else
				{
					var quizData = new List<object>();

					var quiz = await _context.Quizzes
						.Where(m => m.Id == quizSubject.QuizId)
                        .Select(m => new
                        {
                            m.Id,
                            m.Title,
                            m.Introduction,
                            m.isQuestionRandomized,
							m.AllowEmptyAnswers,
							m.Minutes,
							m.Takes
                        })
                        .FirstOrDefaultAsync();

					if (quiz == null)
					{
                        return new JsonResult(new { message = "Quiz is Expired" });
                    }

					quizData.Add(quiz);

                    return new JsonResult(new { message = "OK", quizData });
				}
			}
			catch (Exception ex)
			{
                return new JsonResult(new { message = "An error occurred", error = ex.Message });
            }
		}
	}
}
