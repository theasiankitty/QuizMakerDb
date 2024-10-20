using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.CourseYears
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
	public class CreateModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public CreateModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public IActionResult OnGet()
		{
<<<<<<< Updated upstream

			ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");
=======
			ViewData["Courses"] = new SelectList(_context.Courses.Where(m => m.Active == true), "Id", "Name");
>>>>>>> Stashed changes

			ViewData["SchoolYears"] = new SelectList(_context.SchoolYears.Where(m => m.Active == true), "Id", "Name");

			return Page();
		}

		[BindProperty]
		public CourseYearVM CourseYearVM { get; set; } = default!;

		public async Task<IActionResult> OnPostAsync()
		{
            ModelState.Remove("CourseYearVM.Active");

            if (!ModelState.IsValid)
			{
				return Page();
			}

			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				TempData["Message"] = "User not found. Course Year could not be created.";
				TempData["MessageType"] = "error";
				return NotFound();
			}

			try
			{
				var courseYear = new CourseYear
				{
					Name = CourseYearVM.Name,
					Year = byte.Parse(CourseYearVM.Year),
					CourseId = CourseYearVM.CourseId,
					Active = true,
					CreatedBy = creator.Id,
					CreatedDate = DateTime.Now,
					UpdatedBy = null,
					UpdatedDate = null
				};

				_context.CourseYears.Add(courseYear);
				await _context.SaveChangesAsync();

				TempData["Message"] = "Course Year successfully created!";
				TempData["MessageType"] = "success";
				TempData["CourseYearId"] = courseYear.Id;

				return RedirectToPage();
			}
			catch (Exception)
			{
				TempData["Message"] = "An error occurred while creating the course year. Please try again.";
				TempData["MessageType"] = "error";

				return RedirectToPage();
			}
		}
	}
}
