using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Data;
using Microsoft.AspNetCore.Identity;
using QuizMakerDb.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.QuizTakes
{
	public class IndexModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public IndexModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public StudentVM StudentVM { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound();
			}

			var student = await _context.Students
				.FirstOrDefaultAsync(m => m.UserId == user.Id);

			if (student == null)
			{
				return NotFound();
			}

			StudentVM = new StudentVM
			{
				Id = student.Id,
				Student = student.FirstName + " " + student.LastName
			};

			return Page();
		}
	}
}
