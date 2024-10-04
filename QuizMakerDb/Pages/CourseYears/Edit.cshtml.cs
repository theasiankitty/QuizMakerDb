using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.CourseYears
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
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
		public CourseYearVM CourseYearVM { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			ViewData["Courses"] = new SelectList(_context.Courses.Where(m => m.Active == true), "Id", "Name");

			if (id == null)
			{
				return NotFound();
			}

			var courseYear = await _context.CourseYears.FirstOrDefaultAsync(m => m.Id == id);

			if (courseYear == null)
			{
				return NotFound();
			}

			CourseYearVM = new CourseYearVM
			{
				Id = courseYear.Id,
				Name = courseYear.Name,
				CourseId = courseYear.Id,
				Year = courseYear.Year.ToString(),
				Active = courseYear.Active,
				CreatedBy = courseYear.CreatedBy,
				CreatedDate	= courseYear.CreatedDate,
				UpdatedBy = courseYear.UpdatedBy,
				UpdatedDate = courseYear.UpdatedDate
			};

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ModelState.Remove("CourseYearVM.Active");

			if (!ModelState.IsValid)
			{
				return Page();
			}

			var editor = await _userManager.GetUserAsync(User);

			if (editor == null)
			{
				return NotFound();
			}

			var courseYear = new CourseYear
			{
				Id = CourseYearVM.Id,
				Name = CourseYearVM.Name,
				CourseId = CourseYearVM.CourseId,
				Year = byte.Parse(CourseYearVM.Year),
				Active = true,
				CreatedBy = CourseYearVM.CreatedBy,
				CreatedDate = CourseYearVM.CreatedDate,
				UpdatedBy = editor.Id,
				UpdatedDate = DateTime.Now,
			};

			_context.Attach(courseYear).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CourseYearExists(CourseYearVM.Id))
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

		private bool CourseYearExists(int id)
		{
			return _context.CourseYears.Any(e => e.Id == id);
		}
	}
}
