using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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
            ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");

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
                TempData["Message"] = "User not found. Section could not be created.";
                TempData["MessageType"] = "error";
                return RedirectToPage("./Index");
            }

            try
            {
                var section = new Section
                {
                    Name = SectionVM.Name,
                    SchoolYearId = SectionVM.SchoolYearId,
                    CourseId = SectionVM.CourseId,
                    Year = byte.Parse(SectionVM.Year),
                    Active = true,
                    CreatedBy = creator.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = null,
                    UpdatedDate = null
                };

                _context.Sections.Add(section);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Section successfully created!";
                TempData["MessageType"] = "success";
                TempData["SectionId"] = section.Id;

                return RedirectToPage();
            }
            catch (Exception)
            {
                TempData["Message"] = "An error occurred while creating the section. Please try again.";
                TempData["MessageType"] = "error";

                return RedirectToPage();
            }
        }
    }
}
