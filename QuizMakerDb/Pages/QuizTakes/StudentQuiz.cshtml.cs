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

		public bool isUnlimitedTime { get; set; }
		public int RemainingTime { get; set; }
		public int QuizId { get; set; }
		public int QuizTakeId { get; set; }
		public int StudentId { get; set; }

		public async Task<IActionResult> OnGetAsync(StudentData studentData)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null || studentData.StudentId <= 0 || string.IsNullOrWhiteSpace(studentData.Code))
			{
				return NotFound();
			}

			var sectionStudent = await GetSectionStudentAsync(studentData.StudentId);
			if (sectionStudent == null || sectionStudent.StudentInfo.UserId != user.Id)
			{
				return NotFound();
			}

			var quizSubject = await GetQuizSubjectAsync(studentData.Code, sectionStudent.SectionId);
			if (quizSubject == null)
			{
				return NotFound();
			}

			QuizId = quizSubject.QuizId;
			StudentId = studentData.StudentId;

			var quiz = await _context.Quizzes
				.Where(m => m.Id == QuizId && m.Active)
				.FirstOrDefaultAsync();

			if (quiz == null)
			{
				return NotFound();
			}

			isUnlimitedTime = quiz.isUnlimitedMinutes;

			// Get active quiz take
			var activeQuizTake = await GetActiveQuizTakeAsync(QuizId, sectionStudent.StudentId);

			// Handle existing quiz take
			if (activeQuizTake != null)
			{
				QuizTakeId = activeQuizTake.Id;

				var endTime = activeQuizTake.StartTime.AddMinutes(activeQuizTake.Duration);
				var now = DateTime.UtcNow;

				// If the quiz has time limits and time has expired
				if (!quiz.isUnlimitedMinutes && now >= endTime)
				{
					activeQuizTake.isFinished = true;
					activeQuizTake.Active = false;
					await _context.SaveChangesAsync();
					return RedirectToPage("StudentQuizResult", new { QuizTakeId });
				}

				// If the quiz is complete
				if (await IsQuizCompleteAsync(QuizId, activeQuizTake.Id))
				{
					activeQuizTake.isFinished = true;
					activeQuizTake.Active = false;
					await _context.SaveChangesAsync();
					return RedirectToPage("StudentQuizResult", new { QuizTakeId });
				}

				// If the quiz is not unlimited time, calculate remaining time
				RemainingTime = !quiz.isUnlimitedMinutes ? Math.Max(0, (int)(endTime - now).TotalSeconds) : 0;
				return Page();
			}

			// No active quiz take, determine if we should create a new one
			if (ShouldAddNewTake(activeQuizTake, quiz, quizSubject))
			{
				await AddQuizTakeAsync(studentData.StudentId, user.Id, quizSubject);
				RemainingTime = quiz.isUnlimitedMinutes ? 0 : quizSubject.QuizInfo.Minutes * 60;
				return Page();
			}

			// Redirect to results if maximum takes have been reached
			if (!quiz.isUnlimitedTakes && await HasReachedMaxTakesAsync(QuizId, sectionStudent.StudentId, quizSubject))
			{
				return RedirectToPage("StudentQuizResult", new { QuizTakeId });
			}

			// Default case
			return RedirectToPage("StudentQuizResult", new { QuizTakeId });
		}

		private async Task<SectionStudent?> GetSectionStudentAsync(int studentId)
		{
			return await _context.SectionStudents
				.Include(m => m.StudentInfo)
				.Where(m => m.StudentId == studentId && m.Active)
				.FirstOrDefaultAsync();
		}

		private async Task<QuizSubject?> GetQuizSubjectAsync(string code, int sectionId)
		{
			return await _context.QuizSubjects
				.Include(m => m.QuizInfo)
				.Where(m => m.Code == code && m.SectionId == sectionId && m.Active)
				.FirstOrDefaultAsync();
		}

		private async Task<QuizTake?> GetActiveQuizTakeAsync(int quizId, int studentId)
		{
			return await _context.QuizTakes
				.Where(m => m.QuizId == quizId && m.StudentId == studentId && m.Active && !m.isFinished)
				.OrderByDescending(m => m.StartTime)
				.FirstOrDefaultAsync();
		}

		private async Task<bool> IsQuizCompleteAsync(int quizId, int quizTakeId)
		{
			var quizQuestionCount = await _context.QuizQuestions
				.Where(m => m.QuizId == quizId)
				.CountAsync();

			var studentQuizResult = await _context.AnswerStudents
				.Where(m => m.QuizTakeId == quizTakeId && m.Active)
				.CountAsync();

			return quizQuestionCount == studentQuizResult;
		}

		private async Task<bool> HasReachedMaxTakesAsync(int quizId, int studentId, QuizSubject quizSubject)
		{
			var takeCount = await _context.QuizTakes
				.Where(qt => qt.QuizId == quizId && qt.StudentId == studentId && qt.isFinished)
				.CountAsync();

			return takeCount >= quizSubject.QuizInfo.Takes;
		}

		private bool ShouldAddNewTake(QuizTake? activeQuizTake, Quiz quiz, QuizSubject quizSubject)
		{
			if (quiz.isUnlimitedTakes)
			{
				// Allow retake only if the last take is finished
				return activeQuizTake == null || activeQuizTake.isFinished;
			}
			else
			{
				// Limited takes: allow if not finished and max takes not reached
				var totalTakes = _context.QuizTakes
					.Count(qt => qt.QuizId == QuizId && qt.StudentId == StudentId && qt.isFinished);

				return totalTakes < quizSubject.QuizInfo.Takes &&
					   (activeQuizTake == null || activeQuizTake.isFinished);
			}
		}

		private async Task AddQuizTakeAsync(int studentId, Guid userId, QuizSubject quizSubject)
		{
			var take = new QuizTake
			{
				StudentId = studentId,
				QuizId = QuizId,
				StartTime = DateTime.UtcNow,
				Duration = quizSubject.QuizInfo.Minutes,
				Active = true,
				isFinished = false, // New quiz takes are not finished by default
				CreatedBy = userId,
				CreatedDate = DateTime.UtcNow
			};

			_context.QuizTakes.Add(take);
			await _context.SaveChangesAsync();

			QuizTakeId = take.Id;
		}
	}
}
