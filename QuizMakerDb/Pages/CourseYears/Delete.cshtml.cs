using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.CourseYears
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class DeleteModel : PageModel
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public DeleteModel(ApplicationDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[BindProperty]
		public CourseYearVM CourseYearVM { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var courseYear = await _context.CourseYears.FirstOrDefaultAsync(m => m.Id == id);

			if (courseYear == null)
			{
				return NotFound();
			}

			CourseYearVM = new CourseYearVM
			{
				Id = courseYear.Id,
				Name = courseYear.Name,
				CourseId = courseYear.Id,
				Year = courseYear.Year.ToString(),
				Active = courseYear.Active,
				CreatedBy = courseYear.CreatedBy,
				CreatedDate = courseYear.CreatedDate,
				UpdatedBy = courseYear.UpdatedBy,
				UpdatedDate = courseYear.UpdatedDate
			};

			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var updater = await _userManager.GetUserAsync(User);

			if (updater == null)
			{
				return NotFound();
			}

			var courseYear = await _context.CourseYears.FindAsync(id);

			if (courseYear == null)
			{
				return NotFound();
			}

			courseYear.Active = false;
			courseYear.UpdatedBy = updater.Id;
			courseYear.UpdatedDate = DateTime.Now;
			_context.CourseYears.Update(courseYear);

			// get section by their course id
			var sections = await _context.Sections
				.Where(m => m.CourseYearId == courseYear.Id && m.Active == true).ToListAsync();

			if (sections.Any())
			{
				foreach (var section in sections)
				{
					section.Active = false;
					section.UpdatedBy = updater.Id;
					section.UpdatedDate = DateTime.Now;
					_context.Sections.Update(section);

					// get section students by their section id
					var sectionStudents = await _context.SectionStudents
						.Where(m => m.SectionId == section.Id && m.Active == true).ToListAsync();

					foreach (var student in sectionStudents)
					{
						student.Active = false;
						student.UpdatedBy = updater.Id;
						student.UpdatedDate = DateTime.Now;
						_context.SectionStudents.Update(student);
					}
				}
			}

			var subjects = await _context.CourseYearSubjects
				.Where(m => m.CourseYearId == courseYear.Id && m.Active == true).ToListAsync();

			if (subjects.Any())
			{
				foreach (var subject in subjects)
				{
					subject.Active = false;
					subject.UpdatedBy = updater.Id;
					subject.UpdatedDate = DateTime.Now;
					_context.CourseYearSubjects.Update(subject);
				}
			}

			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
		}
	}
}
