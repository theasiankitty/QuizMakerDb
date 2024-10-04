using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data;
using Microsoft.EntityFrameworkCore;

namespace QuizMakerDb.Pages.CheckUserName
{
    public class UserNameModel : PageModel
    {
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public UserNameModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task<JsonResult> OnGetUserNameAsync([FromQuery] string userName)
		{
			var user = await _userManager.FindByNameAsync(userName);

			if (user != null)
			{
				return new JsonResult("NOT OK");
			}

			return new JsonResult("OK");
		}

		public async Task<JsonResult> OnGetUserNameByTeacherIdAsync([FromQuery] int teacherId, string userName)
		{
			var teacher = await _context.Teachers
				.Where(m => m.Active)
				.FirstOrDefaultAsync(m => m.Id == teacherId);

			if (teacher != null)
			{
				if (teacher.UserName == userName)
				{
					return new JsonResult("OK");
				}
				else
				{
					var user = await _userManager.FindByNameAsync(userName);

					if (user != null)
					{
						return new JsonResult("NOT OK");
					}
				}

				return new JsonResult("OK");
			}

			return new JsonResult("Teacher Not Found");
		}

        public async Task<JsonResult> OnGetUserNameByStudentIdAsync([FromQuery] int studentId, string userName)
        {
            var student = await _context.Students.FirstOrDefaultAsync(m => m.Id == studentId);

            if (student != null)
            {
                if (student.UserName == userName)
                {
                    return new JsonResult("OK");
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(userName);

                    if (user != null)
                    {
                        return new JsonResult("NOT OK");
                    }
                }

                return new JsonResult("OK");
            }

            return new JsonResult("Student Not Found");
        }
    }
}
