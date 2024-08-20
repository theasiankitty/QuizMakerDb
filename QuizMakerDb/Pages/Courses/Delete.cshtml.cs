using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Courses
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
        public CourseVM CourseVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            CourseVM = new CourseVM
            {
                Id = course.Id,
                Name = course.Name,
                Active = course.Active,
                CreatedBy = course.CreatedBy,
                CreatedDate = course.CreatedDate,
                UpdatedBy = course.UpdatedBy,
                UpdatedDate = course.UpdatedDate,
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

            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            course.Active = false;
            course.UpdatedBy = updater.Id;
            course.UpdatedDate = DateTime.Now;
            _context.Courses.Update(course);

            // get section by their course id
            var sections = await _context.Sections.Where(m => m.CourseYearId == course.Id).ToListAsync();

            if (sections.Any())
            {
                foreach (var section in sections)
                {
                    section.Active = false;
                    section.UpdatedBy = updater.Id;
                    section.UpdatedDate = DateTime.Now;
                    _context.Sections.Update(section);

                    // get section students by their section id
                    var sectionStudents = await _context.SectionStudents.Where(m => m.SectionId == section.Id).ToListAsync();

                    foreach (var student in sectionStudents)
                    {
                        student.Active = false;
                        student.UpdatedBy = updater.Id;
                        student.UpdatedDate = DateTime.Now;
                        _context.SectionStudents.Update(student);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
