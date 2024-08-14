using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace QuizMakerDb.Pages.SectionStudents
{
	public class UpdateModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public UpdateModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task<JsonResult> OnPostAsync([FromBody] object data, [FromQuery] string operation)
		{
			var editor = await _userManager.GetUserAsync(User);

			if (editor == null)
			{
				return new JsonResult("NOT FOUND");
			}

			if (operation == "update")
			{
				var sectionStudents = JsonConvert.DeserializeObject<List<SectionStudent>>(data.ToString());

				if (sectionStudents.Any())
				{
					foreach (var item in sectionStudents)
					{
						SectionStudent? sectionStudent = await _context.SectionStudents
							.Where(m => m.StudentId == item.StudentId)
							.Where(m => m.SectionId == null)
							.FirstOrDefaultAsync();

						if (sectionStudent != null)
						{
							sectionStudent.SectionId = item.SectionId;
							sectionStudent.UpdatedBy = editor.Id;
							sectionStudent.UpdatedDate = DateTime.Now;
							_context.SectionStudents.Update(sectionStudent);
						}
					}

					await _context.SaveChangesAsync();
				}
			}
			else if (operation == "remove")
			{
				var studentId = JsonConvert.DeserializeObject<int>(data.ToString());

				SectionStudent? sectionStudent = await _context.SectionStudents
					.Where(m => m.StudentId == studentId)
					.Where(m => m.SectionId != null)
					.FirstOrDefaultAsync();

				if (sectionStudent != null)
				{
					sectionStudent.SectionId = null;
					sectionStudent.UpdatedBy = editor.Id;
					sectionStudent.UpdatedDate = DateTime.Now;
					_context.SectionStudents.Update(sectionStudent);
				}

				await _context.SaveChangesAsync();
			}

			return new JsonResult("OK");
		}
	}
}
