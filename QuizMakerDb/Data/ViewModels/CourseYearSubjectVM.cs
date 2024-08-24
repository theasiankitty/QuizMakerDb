using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class CourseYearSubjectVM
	{
		[DisplayFormat(DataFormatString = "{0:000000000#}")]
		public int Id { get; set; }

		[Display(Name = "Subject")]
		public int SubjectId { get; set; }

		public string Subject { get; set; } = string.Empty!;

		[Display(Name = "Course Year")]
		public int CourseYearId { get; set; }

		[Display(Name = "Course Year")]
		public string CourseYear { get; set; } = string.Empty!;

		public string Code { get; set; } = string.Empty!;

		public string Semester { get; set; } = string.Empty!;

        public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
