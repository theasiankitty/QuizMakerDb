using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;
using System.Data;

namespace QuizMakerDb.Pages.SchoolYears
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
        public SchoolYearVM SchoolYearVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schoolYear = await _context.SchoolYears.FirstOrDefaultAsync(m => m.Id == id);

            if (schoolYear == null)
            {
                return NotFound();
            }

            SchoolYearVM = new SchoolYearVM
            {
                Id = schoolYear.Id,
                Name = schoolYear.Name,
                Active = schoolYear.Active,
                CreatedBy = schoolYear.CreatedBy,
                CreatedDate = schoolYear.CreatedDate,
                UpdatedBy = schoolYear.UpdatedBy,
                UpdatedDate = schoolYear.UpdatedDate,
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

            var schoolYear = await _context.SchoolYears.FindAsync(id);

            if (schoolYear == null)
            {
                return NotFound();
            }

            schoolYear.Active = false;
            schoolYear.UpdatedBy = updater.Id;
            schoolYear.UpdatedDate = DateTime.Now;
            _context.SchoolYears.Update(schoolYear);

            // get section by their school year id
            var sections = await _context.Sections.Where(m => m.SchoolYearId == schoolYear.Id).ToListAsync();

            if (sections.Any())
            {
                foreach (var section in sections)
                {
                    section.Active = false;
                    section.UpdatedBy = updater.Id;
                    section.UpdatedDate = DateTime.Now;
                    _context.Sections.Update(section);
                }
            }

            // get section student by their school year id
            var sectionStudents = await _context.SectionStudents.Where(m => m.SchoolYearId == schoolYear.Id).ToListAsync();

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
