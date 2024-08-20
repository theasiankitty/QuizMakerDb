using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.SchoolYears
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
            return Page();
        }

        [BindProperty]
        public SchoolYearVM SchoolYearVM { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("SchoolYearVM.Active");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var creator = await _userManager.GetUserAsync(User);

            if (creator == null)
            {
                return NotFound();
            }

            var schoolYear = new SchoolYear
            {
                Name = SchoolYearVM.Name,
                Active = true,
                CreatedBy = creator.Id,
                CreatedDate = DateTime.Now,
                UpdatedBy = null,
                UpdatedDate = null
            };

            _context.SchoolYears.Add(schoolYear);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
