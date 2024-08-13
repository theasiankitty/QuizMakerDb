﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;
using QuizMakerDb.Infrastructure;

namespace QuizMakerDb.Pages.Teachers
{
	[Authorize(Roles = Constants.ROLE_ADMIN)]
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedList<TeacherVM> Teachers { get; set; } = null!;
        public string SortColumn { get; set; } = string.Empty!;
        public string SortOrder { get; set; } = string.Empty!;
        public string SearchTeacher { get; set; } = string.Empty!;
        public int TotalItems { get; set; }

        public async Task OnGetAsync(string? sortColumn, string? sortOrder, int? pageIndex, string? searchTeacher)
        {
            SortColumn = string.IsNullOrEmpty(sortColumn) ? "" : sortColumn;
            SortOrder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
            searchTeacher = string.IsNullOrEmpty(searchTeacher) ? "" : searchTeacher;

            if (_context.Teachers != null)
            {
                IQueryable<Teacher> teachers = _context.Teachers.AsQueryable();

                teachers = teachers.OrderByDescending(o => o.Id);

                if (!string.IsNullOrEmpty(searchTeacher))
                {
                    teachers = teachers.Where(m => (m.FirstName + " " + m.LastName)
                    .ToLower()
                    .Contains(searchTeacher.ToLower()));
                }

                switch (sortColumn)
                {
                    case "id":
                        teachers = SortOrder == "asc" ? teachers.OrderBy(o => o.Id)
                            : teachers.OrderByDescending(o => o.Id);
                        break;
                    case "teacher":
                        teachers = SortOrder == "asc" ? teachers.OrderBy(o => o.FirstName + " " + o.LastName)
                            : teachers.OrderByDescending(o => o.FirstName + " " + o.LastName);
                        break;
					case "username":
						teachers = SortOrder == "asc" ? teachers.OrderBy(o => o.UserName)
							: teachers.OrderByDescending(o => o.UserName);
						break;
					case "sex":
						teachers = SortOrder == "asc" ? teachers.OrderBy(o => ((Sex)o.Sex))
							: teachers.OrderByDescending(o => ((Sex)o.Sex));
						break;
					case "email":
						teachers = SortOrder == "asc" ? teachers.OrderBy(o => o.Email)
							: teachers.OrderByDescending(o => o.Email);
						break;
				}

                int pageSize = 10;

                TotalItems = teachers.Count();

                Teachers = await PaginatedList<TeacherVM>.CreateAsync(
                    teachers.Select(m => new TeacherVM
                    {
                        Id = m.Id,
                        UserName = m.UserName,
                        Teacher = m.FirstName + " " + m.LastName,
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
