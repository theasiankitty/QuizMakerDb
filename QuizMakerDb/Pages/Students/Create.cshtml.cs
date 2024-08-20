using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Students
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
        public StudentVM StudentVM { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
			ModelState.Remove("StudentVM.UserId");
			ModelState.Remove("StudentVM.Active");

            if (!ModelState.IsValid)
			{
				return Page();
			}

			var hasher = new PasswordHasher<AppUser>();

			Guid userId = Guid.NewGuid();
			AppUser user = new()
			{
				Id = userId,
				UserName = StudentVM.UserName,
				NormalizedUserName = StudentVM.UserName.ToUpper(),
				Email = StudentVM.Email,
				NormalizedEmail = StudentVM.Email.ToUpper(),
				EmailConfirmed = true,
				SecurityStamp = Guid.NewGuid().ToString()
			};
			user.PasswordHash = hasher.HashPassword(user, StudentVM.UserName);
			await _userManager.CreateAsync(user);
			await _userManager.AddToRoleAsync(user, Constants.ROLE_STUDENT);

			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return NotFound();
			}

			var student = new Student
			{
				FirstName = StudentVM.FirstName,
				MiddleName = StudentVM.MiddleName,
				LastName = StudentVM.LastName,
				Sex = StudentVM.Sex,
				Email = StudentVM.Email,
				UserName = StudentVM.UserName,
				UserId = userId,
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.Now,
				UpdatedBy = null,
				UpdatedDate = null
			};

			_context.Students.Add(student);
			await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
