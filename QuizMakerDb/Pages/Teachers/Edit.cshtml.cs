using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Teachers
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
		public TeacherVM TeacherVM { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var teacher = await _context.Teachers.FirstOrDefaultAsync(m => m.Id == id);

			if (teacher == null)
			{
				return NotFound();
			}

			TeacherVM = new TeacherVM
			{
				Id = teacher.Id,
				FirstName = teacher.FirstName,
				MiddleName = teacher.MiddleName,
				LastName = teacher.LastName,
				Sex = teacher.Sex,
				Email = teacher.Email,
				UserName = teacher.UserName,
				UserId = teacher.UserId,
				Active = teacher.Active,
				CreatedBy = teacher.CreatedBy,
				CreatedDate = teacher.CreatedDate,
				UpdatedBy = teacher.UpdatedBy,
				UpdatedDate = teacher.UpdatedDate,
			};

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ModelState.Remove("TeacherVM.UserId");
			ModelState.Remove("TeacherVM.Active");

			if (!ModelState.IsValid)
			{
				return Page();
			}

			var editor = await _userManager.GetUserAsync(User);

			if (editor == null)
			{
				return NotFound();
			}

			var teacher = new Teacher
			{
				Id = TeacherVM.Id,
				FirstName = TeacherVM.FirstName,
				MiddleName = TeacherVM.MiddleName,
				LastName = TeacherVM.LastName,
				Sex = TeacherVM.Sex,
				Email = TeacherVM.Email,
				UserName = TeacherVM.UserName,
				UserId = TeacherVM.UserId,
				Active = true,
				CreatedBy = TeacherVM.CreatedBy,
				CreatedDate = TeacherVM.CreatedDate,
				UpdatedBy = editor.Id,
				UpdatedDate = DateTime.Now,
			};

			var teacherIdentity = await _userManager.FindByIdAsync(teacher.UserId.ToString());

			if (teacherIdentity == null)
			{
				return NotFound();
			}

            var hasher = new PasswordHasher<AppUser>();

            teacherIdentity.UserName = teacher.UserName;
			teacherIdentity.NormalizedUserName = teacher.UserName.ToUpper();
			teacherIdentity.Email = teacher.Email;
			teacherIdentity.NormalizedEmail = teacher.Email.ToUpper();
            teacherIdentity.PasswordHash = hasher.HashPassword(teacherIdentity, teacher.UserName);

            await _userManager.UpdateAsync(teacherIdentity);

			_context.Attach(teacher).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TeacherExists(TeacherVM.Id))
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

		private bool TeacherExists(int id)
		{
			return _context.Teachers.Any(e => e.Id == id);
		}
	}
}
