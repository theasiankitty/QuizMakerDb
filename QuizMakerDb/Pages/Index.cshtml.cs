using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;

namespace QuizMakerDb.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public int TotalQuizzes { get; set; }
		public int TotalSections { get; set; }
		public int TotalTeachers { get; set; }
		public int TotalStudents { get; set; }

		public async Task OnGetAsync()
        {
			TotalQuizzes = 0;

			TotalSections = await _context.Sections
				.Where(m => m.Active)
				.CountAsync();

			TotalTeachers = await _context.Teachers
				.Where(m => m.Active)
				.CountAsync();

			TotalStudents = await _context.Students
				.Where(m => m.Active)
				.CountAsync();
		}
    }
}
