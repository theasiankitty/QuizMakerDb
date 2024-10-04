using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using static System.Collections.Specialized.BitVector32;

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
            ModelState.Remove("TeacherVM.Active");

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
				SecurityStamp = Guid.NewGuid().ToString(),
				Active = true,
			};
			user.PasswordHash = hasher.HashPassword(user, TeacherVM.UserName);
			await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, Constants.ROLE_TEACHER);

            var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				TempData["Message"] = "User not found. Section could not be created.";
				TempData["MessageType"] = "error";
				return RedirectToPage("./Index");
			}

			try
			{
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

				TempData["Message"] = "Teacher successfully created!";
				TempData["MessageType"] = "success";
				TempData["TeacherId"] = teacher.Id;

				return RedirectToPage();
			}
			catch (Exception)
			{
				TempData["Message"] = "An error occurred while creating the teacher. Please try again.";
				TempData["MessageType"] = "error";

				return RedirectToPage();
			}
		}
	}
}
