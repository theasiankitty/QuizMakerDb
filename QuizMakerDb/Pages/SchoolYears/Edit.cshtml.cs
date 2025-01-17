﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.SchoolYears
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
        public SchoolYearVM SchoolYearVM { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schoolYear =  await _context.SchoolYears.FirstOrDefaultAsync(m => m.Id == id);

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
        
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("SchoolYearVM.Active");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var editor = await _userManager.GetUserAsync(User);

            if (editor == null)
            {
                return NotFound();
            }

            var schoolYear = new SchoolYear
            {
                Id = SchoolYearVM.Id,
                Name = SchoolYearVM.Name,
                Active = true,
                CreatedBy = SchoolYearVM.CreatedBy,
                CreatedDate = SchoolYearVM.CreatedDate,
                UpdatedBy = editor.Id,
                UpdatedDate = DateTime.Now,
            };

            _context.Attach(schoolYear).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SchoolYearExists(SchoolYearVM.Id))
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

        private bool SchoolYearExists(int id)
        {
            return _context.SchoolYears.Any(e => e.Id == id);
        }
    }
}
