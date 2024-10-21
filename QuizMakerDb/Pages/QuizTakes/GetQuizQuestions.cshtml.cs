using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.QuizTakes
{
    public class GetQuizQuestionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public GetQuizQuestionsModel(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<JsonResult> OnGetQuestionsAsync([FromQuery] int quizId, int studentId)
        {
            if (quizId == 0)
            {
                return new JsonResult(new { message = "Invalid Quiz." });
            }

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(m => m.Id == quizId);

            if (quiz == null)
            {
                return new JsonResult(new { message = "Invalid Quiz." });
            }

            var allowEmptyAnswers = quiz.AllowEmptyAnswers;
            var isQuestionRandomized = quiz.isQuestionRandomized;

            try
            {
                var questions = new List<object>();

                var studentAnswered = _context.AnswerStudents
                    .Where(m => m.StudentId == studentId)
                    .Select(m => m.QuizQuestionId)
                    .ToList();

                var quizQuestionsQuery = _context.QuizQuestions
                    .Where(m => !studentAnswered.Contains(m.Id) && m.QuizId == quizId && m.Active)
                    .OrderByDescending(m => m.Order);

                var quizQuestions = isQuestionRandomized
                    ? await quizQuestionsQuery.ToListAsync().ContinueWith(t => t.Result.OrderBy(q => Guid.NewGuid()).ToList())
                    : await quizQuestionsQuery.ToListAsync();

                foreach (var quizQuestion in quizQuestions)
                {
                    var questionsAnswers = await _context.QuestionAnswers
                        .Where(m => m.QuizQuestionId == quizQuestion.Id && m.Active)
                        .ToListAsync();

                    var questionItems = await _context.QuestionItems
                        .Where(m => m.QuizQuestionId == quizQuestion.Id && m.Active)
                        .Select(m => new
                        {
                            m.Id,
                            m.Name,
                            m.Order,
                            m.QuizQuestionId,
                        })
                        .ToListAsync();

                    string quizQuestionImage = quizQuestion.Image != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(quizQuestion.Image)}" : "";

                    var questionWithAnswers = new
                    {
                        quizQuestion.Id,
                        quizQuestion.Description,
                        quizQuestion.Order,
                        quizQuestion.QuestionType,
                        quizQuestion.Points,
                        //Image = quizQuestionImage,
                        Answers = questionsAnswers.Select(answer => new
                        {
                            answer.Id,
                            answer.Answer,
                            answer.ShowAnswer,
                            allowEmptyAnswers,
                            //Image = answer.Image != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(answer.Image)}" : "",
                        }).ToList(),
                        Items = questionItems,
                    };

                    questions.Add(questionWithAnswers);
                }

                return new JsonResult(new { message = "OK", questions });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}
