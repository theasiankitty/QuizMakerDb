using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace QuizMakerDb.Pages.SectionStudents
{
	public class CreateModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public CreateModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task<JsonResult> OnPostAsync([FromBody] object data, [FromQuery] string operation)
		{
			var creator = await _userManager.GetUserAsync(User);

			if (creator == null)
			{
				return new JsonResult("NOT FOUND");
			}

			var studentSections = JsonConvert.DeserializeObject<List<SectionStudent>>(data.ToString());

			if (studentSections.Any())
			{
				foreach (var student in studentSections)
				{
					var sectionStudent = new SectionStudent
					{
						StudentId = student.StudentId,
						SectionId = student.SectionId,
						SchoolYearId = student.SchoolYearId,
						Active = true,
						CreatedBy = creator.Id,
						CreatedDate = DateTime.Now
					};

					_context.SectionStudents.Add(sectionStudent);
				}
			}

			await _context.SaveChangesAsync();

			return new JsonResult("OK");
		}
	}
}
