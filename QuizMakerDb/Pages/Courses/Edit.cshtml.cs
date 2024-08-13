using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Courses
{
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
        public CourseVM CourseVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course =  await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var editor = await _userManager.GetUserAsync(User);

            if (editor == null)
            {
                return NotFound();
            }

            var course = new Course
            {
                Id = CourseVM.Id,
                Name = CourseVM.Name,
                Active = CourseVM.Active,
                CreatedBy = CourseVM.CreatedBy,
                CreatedDate = CourseVM.CreatedDate,
                UpdatedBy = editor.Id,
                UpdatedDate = DateTime.Now,
            };

            _context.Attach(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(CourseVM.Id))
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

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
