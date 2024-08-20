using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;

namespace QuizMakerDb.Pages.Students
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<StudentVM> Students { get; set; } = null!;
        public string SortColumn { get; set; } = string.Empty!;
        public string SortOrder { get; set; } = string.Empty!;
        public string SearchStudent { get; set; } = string.Empty!;
		public string SearchUserName { get; set; } = string.Empty!;
		public string SearchSex { get; set; } = string.Empty!;
		public int TotalItems { get; set; }

        public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex, string? searchStudent, string? searchUserName, string? searchSex)
        {
            SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
            SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
            SearchStudent = string.IsNullOrEmpty(searchStudent) ? "" : searchStudent;
			SearchUserName = string.IsNullOrEmpty(searchUserName) ? "" : searchUserName;
			SearchSex = string.IsNullOrEmpty(searchSex) ? "" : searchSex;

			if (_context.Students != null)
            {
                IQueryable<Student> students = _context.Students.AsQueryable();

				students = students
                    .Where(m => m.Active == true)
                    .OrderByDescending(o => o.Id);

                if (!string.IsNullOrEmpty(searchStudent))
                {
                    students = students.Where(m => (m.FirstName + " " + m.LastName)
                    .ToLower()
                    .Contains(searchStudent.ToLower()));
                }

				if (!string.IsNullOrEmpty(searchUserName))
				{
                    students = students.Where(m => m.UserName == searchUserName);
				}

				if (!string.IsNullOrEmpty(searchSex))
				{
					students = students.Where(m => m.Sex == byte.Parse(searchSex));
				}

				switch (sortColumn)
                {
                    case "id":
                        students = SortOrder == "asc" ? students.OrderBy(o => o.Id)
                            : students.OrderByDescending(o => o.Id);
                        break;
                    case "student":
                        students = SortOrder == "asc" ? students.OrderBy(o => o.FirstName + " " + o.LastName)
                            : students.OrderByDescending(o => o.FirstName + " " + o.LastName);
                        break;
                    case "username":
                        students = SortOrder == "asc" ? students.OrderBy(o => o.UserName)
                            : students.OrderByDescending(o => o.UserName);
                        break;
                    case "sex":
                        students = SortOrder == "asc" ? students.OrderBy(o => ((Sex)o.Sex))
                            : students.OrderByDescending(o => ((Sex)o.Sex));
                        break;
                    case "email":
                        students = SortOrder == "asc" ? students.OrderBy(o => o.Email)
                            : students.OrderByDescending(o => o.Email);
                        break;
                }

                int pageSize = 10;

                TotalItems = students.Count();

                Students = await PaginatedList<StudentVM>.CreateAsync(
                    students.Select(m => new StudentVM
                    {
                        Id = m.Id,
                        UserName = m.UserName,
                        Student = m.FirstName + " " + m.LastName,
                        SexDescription = ((Sex)m.Sex).ToString(),
                        Email = m.Email,
                    }).AsNoTracking(),
                    pageIndex ?? 1,
                    pageSize
                );
            }
        }
    }
}
