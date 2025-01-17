using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;

namespace QuizMakerDb.Pages.Quizzes
{
	public class IndexModel : PageModel
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ApplicationDbContext _context;

		public IndexModel(UserManager<AppUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public PaginatedList<QuizVM> Quizzes { get; set; } = null!;
		public string SortColumn { get; set; } = string.Empty!;
		public string SortOrder { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

		public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex)
		{
			SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
			SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;

			var user = await _userManager.GetUserAsync(User);

			if (user == null) 
			{
				return;
			}

			if (_context.Quizzes != null)
			{
				IQueryable<Quiz> quizzes = _context.Quizzes.AsQueryable();

				quizzes = quizzes
					.Where(m => m.TeacherInfo.UserId == user.Id && m.Active)
					.OrderByDescending(o => o.Id);

				int pageSize = 10;

				TotalItems = quizzes.Count();

				Quizzes = await PaginatedList<QuizVM>.CreateAsync(
					quizzes.Select(m => new QuizVM
					{
						Id = m.Id,
						Title = m.Title,
						Minutes = m.Minutes,
						Takes = m.Takes,
						Date = m.CreatedDate.ToString("d"),
					}).AsNoTracking(),
					pageIndex ?? 1,
					pageSize
				);
			}
		}
	}
}