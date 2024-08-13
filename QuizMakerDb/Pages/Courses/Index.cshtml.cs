using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;

namespace QuizMakerDb.Pages.Courses
{
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<CourseVM> Courses { get; set; } = null!;
        public string SortColumn { get; set; } = string.Empty!;
        public string SortOrder { get; set; } = string.Empty!;
        public string SearchName { get; set; } = string.Empty!;
        public int TotalItems { get; set; }

        public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex, string? searchName)
        {
            SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
            SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
            searchName = string.IsNullOrEmpty(searchName) ? "" : searchName;

            if (_context.Courses != null)
            {
                IQueryable<Course> courses = _context.Courses.AsQueryable();

                courses = courses.OrderByDescending(o => o.Id);

                if (!string.IsNullOrEmpty(searchName))
                {
                    courses = courses.Where(m => (m.Name)
                    .ToLower()
                    .Contains(searchName.ToLower()));
                }

                switch (sortColumn)
                {
                    case "id":
                        courses = SortOrder == "asc" ? courses.OrderBy(o => o.Id)
                            : courses.OrderByDescending(o => o.Id);
                        break;
                    case "name":
                        courses = SortOrder == "asc" ? courses.OrderBy(o => o.Name)
                            : courses.OrderByDescending(o => o.Name);
                        break;
                }

                int pageSize = 10;

                TotalItems = courses.Count();

                Courses = await PaginatedList<CourseVM>.CreateAsync(
                    courses.Select(m => new CourseVM
                    {
                        Id = m.Id,
                        Name = m.Name,
                    }).AsNoTracking(),
                    pageIndex ?? 1,
                    pageSize
                );
            }
        }
    }
}
