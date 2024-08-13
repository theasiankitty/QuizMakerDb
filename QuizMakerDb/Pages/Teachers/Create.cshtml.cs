using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Teachers
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
		public TeacherVM TeacherVM { get; set; } = default!;

		public async Task<IActionResult> OnPostAsync()
		{
			ModelState.Remove("TeacherVM.UserId");

			if (!ModelState.IsValid)
			{
				return Page();
			}

			var hasher = new PasswordHasher<AppUser>();

			Guid userId = Guid.NewGuid();
			AppUser user = new()
			{
				Id = userId,
				UserName = TeacherVM.UserName,
				NormalizedUserName = TeacherVM.UserName.ToUpper(),
				Email = TeacherVM.Email,
				NormalizedEmail = TeacherVM.Email.ToUpper(),
				EmailConfirmed = true,
				SecurityStamp = Guid.NewGuid().ToString()
			};
			user.PasswordHash = hasher.HashPassword(user, "Password@1234");
			await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, Constants.ROLE_TEACHER);

            var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return NotFound();
			}

			var teacher = new Teacher
			{
				FirstName = TeacherVM.FirstName,
				MiddleName = TeacherVM.MiddleName,
				LastName = TeacherVM.LastName,
				Sex = TeacherVM.Sex,
				Email = TeacherVM.Email,
				UserName = TeacherVM.UserName,
				UserId = userId,
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.Now,
				UpdatedBy = null,
				UpdatedDate = null
			};

			_context.Teachers.Add(teacher);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}
