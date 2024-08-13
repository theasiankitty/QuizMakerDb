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
            ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");

            return Page();
        }

        [BindProperty]
        public CourseYearVM CourseYearVM { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
			if (!ModelState.IsValid)
            {
                return Page();
            }

            var creator = await _userManager.GetUserAsync(User);

            if (creator == null)
            {
                return NotFound();
            }

            var courseYear = new CourseYear
            {
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

            return RedirectToPage("./Index");
        }
    }
}
