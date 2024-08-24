using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Subjects
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
		public SubjectVM SubjectVM { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			ViewData["CourseYears"] = new SelectList(_context.CourseYears, "Id", "Name");

			if (id == null)
			{
				return NotFound();
			}

			var subject = await _context.Subjects.FirstOrDefaultAsync(m => m.Id == id);

			if (subject == null)
			{
				return NotFound();
			}

			SubjectVM = new SubjectVM
			{
				Id = subject.Id,
				Title = subject.Title,
				Code = subject.Code,
				Semester = subject.Semester.ToString(),
				Active = subject.Active,
				CreatedBy = subject.CreatedBy,
				CreatedDate = subject.CreatedDate,
			};

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ModelState.Remove("SubjectVM.Active");

			if (!ModelState.IsValid)
			{
				return Page();
			}

			var editor = await _userManager.GetUserAsync(User);

			if (editor == null)
			{
				return NotFound();
			}

			var subject = new Subject
			{
				Id = SubjectVM.Id,
				Title = SubjectVM.Title,
				Code = SubjectVM.Code,
				Semester = byte.Parse(SubjectVM.Semester),
				Active = true,
				CreatedBy = SubjectVM.CreatedBy,
				CreatedDate = SubjectVM.CreatedDate,
				UpdatedBy = editor.Id,
				UpdatedDate = DateTime.Now
			};

			_context.Attach(subject).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SubjectExists(SubjectVM.Id))
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

		private bool SubjectExists(int id)
		{
			return _context.Subjects.Any(e => e.Id == id);
		}
	}
}
