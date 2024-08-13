using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Sections
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

        public static string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        public IActionResult OnGet()
		{
			ViewData["SchoolYears"] = new SelectList(_context.SchoolYears, "Id", "Name");

			var courseYears = _context.CourseYears
				.Select(m => new CourseYearVM
				{
					Id = m.Id,
					CourseYearName = m.CourseInfo.Name + " - " + GetEnumDisplayName((YearLevel)m.Year)
                });

			ViewData["CourseYears"] = new SelectList(courseYears, "Id", "CourseYearName");

			return Page();
		}

		[BindProperty]
		public SectionVM SectionVM { get; set; } = default!;

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

			var section = new Section
			{
				Name = SectionVM.Name,
				SchoolYearId = SectionVM.SchoolYearId,
				CourseYearId = SectionVM.CourseYearId,
				Active = true,
				CreatedBy = creator.Id,
				CreatedDate = DateTime.Now,
				UpdatedBy = null,
				UpdatedDate = null
			};

			_context.Sections.Add(section);
			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}
