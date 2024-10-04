using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Pages.Sections
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class DetailsModel : PageModel
	{
		private readonly ApplicationDbContext _context;

		public DetailsModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public SectionVM SectionVM { get; set; } = default!;
		public IList<SectionStudent> MaleStudents { get; set; } = new List<SectionStudent>();
		public IList<SectionStudent> FemaleStudents { get; set; } = new List<SectionStudent>();

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
			};

			IQueryable<SectionStudent> maleStudents = _context.SectionStudents.AsQueryable();

			maleStudents = _context.SectionStudents
				.Include(m => m.StudentInfo)
				.Where(m => m.StudentInfo.Sex == (byte)Sex.Male
					&& m.Active == true
					&& m.SectionId == SectionVM.Id)
				.OrderBy(o => o.StudentInfo.LastName + " " + o.StudentInfo.FirstName);

			MaleStudents = await maleStudents.ToListAsync();

			IQueryable<SectionStudent> femaleStudents = _context.SectionStudents.AsQueryable();

			femaleStudents = _context.SectionStudents
				.Include(m => m.StudentInfo)
				.Where(m => m.StudentInfo.Sex == (byte)Sex.Female
					&& m.Active == true
					&& m.SectionId == SectionVM.Id)
				.OrderBy(o => o.StudentInfo.LastName + " " + o.StudentInfo.FirstName);

			FemaleStudents = await femaleStudents.ToListAsync();

			return Page();
		}
	}
}
