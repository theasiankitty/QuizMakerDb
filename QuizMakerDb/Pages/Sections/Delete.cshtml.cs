using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Sections
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
        public SectionVM SectionVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Sections
				.Include(m => m.SchoolYearInfo)
				.Include(m => m.CourseYearInfo)
				.FirstOrDefaultAsync(m => m.Id == id);

            if (section == null)
            {
                return NotFound();
            }

			SectionVM = new SectionVM
			{
				Id = section.Id,
				Name = section.Name,
                SchoolYearName = section.SchoolYearInfo.Name,
                CourseYearName = section.CourseYearInfo.Name,
				Active = section.Active,
				CreatedBy = section.CreatedBy,
				CreatedDate = section.CreatedDate,
				UpdatedBy = section.UpdatedBy,
				UpdatedDate = section.UpdatedDate,
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

			var section = await _context.Sections.FindAsync(id);

			if (section == null)
			{
				return NotFound();
			}

			section.Active = false;
			section.UpdatedBy = updater.Id;
			section.UpdatedDate = DateTime.Now;
			_context.Sections.Update(section);

			// get section student by their section id
			var sectionStudents = await _context.SectionStudents.Where(m => m.SectionId == id).ToListAsync();

			if (sectionStudents.Any())
			{
				foreach (var sectionStudent in sectionStudents)
				{
					sectionStudent.Active = false;
					sectionStudent.UpdatedBy = updater.Id;
					sectionStudent.UpdatedDate = DateTime.Now;
					_context.SectionStudents.Update(sectionStudent);
				}
			}

			await _context.SaveChangesAsync();

			return RedirectToPage("./Index");
        }
    }
}
