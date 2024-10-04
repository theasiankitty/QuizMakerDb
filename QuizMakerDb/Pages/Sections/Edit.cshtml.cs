using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QuizMakerDb.Pages.Sections
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
		public SectionVM SectionVM { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			ViewData["SchoolYears"] = new SelectList(_context.SchoolYears.Where(m => m.Active == true), "Id", "Name");
			ViewData["CourseYears"] = new SelectList(_context.CourseYears.Where(m => m.Active == true), "Id", "Name");

			if (id == null)
			{
				return NotFound();
			}

			var section = await _context.Sections.FirstOrDefaultAsync(m => m.Id == id);

			if (section == null)
			{
				return NotFound();
			}

			SectionVM = new SectionVM
			{
				Id = section.Id,
				Name = section.Name,
				SchoolYearId = section.SchoolYearId,
				CourseYearId = section.CourseYearId,
				Active = section.Active,
				CreatedBy = section.CreatedBy,
				CreatedDate = section.CreatedDate,
				UpdatedBy = section.UpdatedBy,
				UpdatedDate = section.UpdatedDate,
			};

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			ModelState.Remove("SectionVM.Active");

			if (!ModelState.IsValid)
			{
				return Page();
			}

			var editor = await _userManager.GetUserAsync(User);

			if (editor == null)
			{
				return NotFound();
			}

			var section = new Section
			{
				Id = SectionVM.Id,
				Name = SectionVM.Name,
				SchoolYearId = SectionVM.SchoolYearId,
				CourseYearId = SectionVM.CourseYearId,
				Active = true,
				CreatedBy = SectionVM.CreatedBy,
				CreatedDate = SectionVM.CreatedDate,
				UpdatedBy = editor.Id,
				UpdatedDate = DateTime.Now,
			};

			// get section student by their section id
			var sectionStudents = await _context.SectionStudents
				.Where(m => m.SectionId == SectionVM.Id 
					&& m.Active)
				.ToListAsync();

			if (sectionStudents.Any())
			{
				foreach (var sectionStudent in sectionStudents)
				{
					sectionStudent.SchoolYearId = SectionVM.SchoolYearId;
					sectionStudent.UpdatedBy = editor.Id;
					sectionStudent.UpdatedDate = DateTime.Now;
					_context.SectionStudents.Update(sectionStudent);
				}
			}

			_context.Attach(section).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!SectionExists(SectionVM.Id))
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

		private bool SectionExists(int id)
		{
			return _context.Sections.Any(e => e.Id == id);
		}
	}
}
