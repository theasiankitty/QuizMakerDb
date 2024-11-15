using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace QuizMakerDb.Pages.QuizTakes
{
    public class QuizModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public QuizModel(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class StudentData
        {
            public int StudentId { get; set; }

            public string Code { get; set; } = string.Empty!;
        }

        public int RemainingTime { get; set; }
        public int QuizId { get; set; }
        public int StudentId { get; set; }

        public async Task<IActionResult> OnGetAsync(StudentData studentData)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            if (studentData == null || studentData.StudentId == 0 || string.IsNullOrEmpty(studentData.Code))
            {
                return NotFound();
            }

            var sectionStudent = await _context.SectionStudents
                .Include(m => m.StudentInfo)
                .Where(m => m.StudentId == studentData.StudentId && m.Active)
                .FirstOrDefaultAsync();

            if (sectionStudent == null)
            {
                return NotFound();
            }

            if (sectionStudent.StudentInfo.UserId != user.Id)
            {
                return NotFound();
            }

            var quizSubject = await _context.QuizSubjects
                .Include(m => m.QuizInfo)
                .FirstOrDefaultAsync(m => m.Code == studentData.Code && m.SectionId == sectionStudent.SectionId && m.Active);

            if (quizSubject == null)
            {
                return NotFound();
            }

            QuizId = quizSubject.QuizId;
            StudentId = studentData.StudentId;

            var quizTake = await _context.QuizTakes
                .FirstOrDefaultAsync(m => m.QuizId == quizSubject.QuizId && m.StudentId == sectionStudent.StudentId && m.Active);

            if (quizTake == null)
            {
                quizTake = new QuizTake
                {
                    StudentId = studentData.StudentId,
                    QuizId = quizSubject.QuizId,
                    StartTime = DateTime.Now,
                    Duration = quizSubject.QuizInfo.Minutes,
                    //Takes = 1,
                    Active = true,
                    CreatedBy = user.Id,
                    CreatedDate = DateTime.Now
                };

                _context.QuizTakes.Add(quizTake);
            }
            else
            {
                var endTime = quizTake.StartTime.AddMinutes(quizTake.Duration);
                var remainingTime = endTime - DateTime.Now;

                if (remainingTime <= TimeSpan.Zero)
                {
                    // don't add takes if it is equal or more than the quizSubject.QuizInfo.Takes
                    //if (quizTake.Takes < quizSubject.QuizInfo.Takes)
                    //{
                    //    quizTake.StartTime = DateTime.Now;
                    //    //quizTake.Takes += 1;
                    //    quizTake.UpdatedBy = user.Id;
                    //    quizTake.UpdatedDate = DateTime.Now;
                    //}
                    //else
                    //{
                    //    // should show result if time is up and took the limit of the quizSubject.QuizInfo.Takes
                    //    RemainingTime = 0;
                    //    return Page();
                    //}
                }
                else
                {
                    RemainingTime = (int)remainingTime.TotalSeconds;
                    return Page();
                }
            }

            await _context.SaveChangesAsync();

            var currentEndTime = quizTake.StartTime.AddMinutes(quizSubject.QuizInfo.Minutes);
            RemainingTime = (int)(currentEndTime - DateTime.Now).TotalSeconds > 0
                ? (int)(currentEndTime - DateTime.Now).TotalSeconds
                : 0;

            return Page();
        }
    }
}
