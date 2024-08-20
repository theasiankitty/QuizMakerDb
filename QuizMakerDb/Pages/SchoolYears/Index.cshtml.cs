using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;

namespace QuizMakerDb.Pages.SchoolYears
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<SchoolYearVM> SchoolYears { get; set; } = null!;
        public string SortColumn { get; set; } = string.Empty!;
        public string SortOrder { get; set; } = string.Empty!;
        public string SearchName { get; set; } = string.Empty!;
        public int TotalItems { get; set; }

        public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex, string? searchName)
        {
            SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
            SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
            SearchName = string.IsNullOrEmpty(searchName) ? "" : searchName;

            if (_context.SchoolYears != null)
            {
                IQueryable<SchoolYear> schoolYears = _context.SchoolYears.AsQueryable();

                schoolYears = schoolYears
                    .Where(m => m.Active == true)
                    .OrderByDescending(o => o.Id);

                if (!string.IsNullOrEmpty(searchName))
                {
                    schoolYears = schoolYears.Where(m => (m.Name)
                    .ToLower()
                    .Contains(searchName.ToLower()));
                }

                switch (sortColumn)
                {
                    case "id":
                        schoolYears = SortOrder == "asc" ? schoolYears.OrderBy(o => o.Id)
                            : schoolYears.OrderByDescending(o => o.Id);
                        break;
                    case "name":
                        schoolYears = SortOrder == "asc" ? schoolYears.OrderBy(o => o.Name)
                            : schoolYears.OrderByDescending(o => o.Name);
                        break;
                }

                int pageSize = 10;

                TotalItems = schoolYears.Count();

                SchoolYears = await PaginatedList<SchoolYearVM>.CreateAsync(
                    schoolYears.Select(m => new SchoolYearVM
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
