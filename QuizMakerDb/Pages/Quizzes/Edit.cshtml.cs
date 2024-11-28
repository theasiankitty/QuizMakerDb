using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Quizzes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public EditModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[BindProperty]
        public QuizVM QuizVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz =  await _context.Quizzes.FirstOrDefaultAsync(m => m.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            QuizVM = new QuizVM
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Introduction = quiz.Introduction,
                isQuestionRandomized = quiz.isQuestionRandomized,
                AllowEmptyAnswers = quiz.AllowEmptyAnswers,
                isUnlimitedMinutes = quiz.isUnlimitedMinutes,
                Minutes = quiz.Minutes,
                isUnlimitedTakes = quiz.isUnlimitedTakes,
                Takes = quiz.Takes,
                TeacherId = quiz.TeacherId,
                Active = quiz.Active,
                CreatedBy = quiz.CreatedBy,
                CreatedDate = quiz.CreatedDate,
                UpdatedBy = quiz.UpdatedBy,
                UpdatedDate = quiz.UpdatedDate
            };

            // ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
			ModelState.Remove("QuizVM.Active");

			if (!ModelState.IsValid)
            {
				return Page();
			}

			var editor = await _userManager.GetUserAsync(User);

			if (editor == null)
			{
				return NotFound();
			}

            var quiz = new Quiz
            {
                Id = QuizVM.Id,
                Title = QuizVM.Title,
                Introduction = QuizVM.Introduction,
                isQuestionRandomized = QuizVM.isQuestionRandomized,
                AllowEmptyAnswers = QuizVM.AllowEmptyAnswers,
                isUnlimitedMinutes = QuizVM.isUnlimitedMinutes,
                Minutes = QuizVM.Minutes,
                isUnlimitedTakes = QuizVM.isUnlimitedTakes,
                Takes = QuizVM.Takes,
                TeacherId = QuizVM.TeacherId,
                Active = QuizVM.Active,
                CreatedBy = QuizVM.CreatedBy,
                CreatedDate = QuizVM.CreatedDate,
                UpdatedBy = editor.Id,
                UpdatedDate = DateTime.Now
            };

			_context.Attach(quiz).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuizExists(QuizVM.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.Id == id);
        }
    }
}
