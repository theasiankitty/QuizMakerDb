using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class SectionVM
	{
		[DisplayFormat(DataFormatString = "{0:000000000#}")]
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty!;

		[Display(Name = "School Year")]
		public int SchoolYearId { get; set; }

		[Display(Name = "School Year")]
		public string SchoolYearName { get; set; } = string.Empty;

		[Display(Name = "Course Year")]
		public int CourseYearId { get; set; }

		[Display(Name = "Course")]
		public string CourseName { get; set; } = string.Empty;

		[Display(Name = "Course Year")]
		public string CourseYear { get; set; } = string.Empty;

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }
	}
}
