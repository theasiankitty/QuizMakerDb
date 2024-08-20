using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class CourseYearVM
	{
		[DisplayFormat(DataFormatString = "{0:000000000#}")]
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty!;

		public string Year { get; set; } = string.Empty!;

		[Display(Name = "Course")]
		public int CourseId { get; set; }

		[Display(Name = "Course")]
		public string CourseName { get; set; } = string.Empty!;

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
