﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Subjects
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
			ViewData["CourseYears"] = new SelectList(_context.CourseYears, "Id", "Name");

            return Page();
		}

		[BindProperty]
		public SubjectVM SubjectVM { get; set; } = default!;

		public async Task<IActionResult> OnPostAsync()
		{
            ModelState.Remove("SubjectVM.Active");

            if (!ModelState.IsValid)
			{
				return Page();
			}

			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return NotFound();
			}

			var subject = new Subject
			{
				Title = SubjectVM.Title,
				Code = SubjectVM.Code,
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.Now
			};

			_context.Subjects.Add(subject);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}
