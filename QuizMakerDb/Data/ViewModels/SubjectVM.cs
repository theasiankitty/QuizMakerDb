using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class SubjectVM
	{
        [DisplayFormat(DataFormatString = "{0:000000000#}")]
        public int Id { get; set; }

		public string Title { get; set; } = string.Empty!;

		public string Code { get; set; } = string.Empty!;
		
		public int SubjectId { get; set; }

		[Display(Name = "Course Year")]
		public string CourseYear { get; set; } = string.Empty!;

		public int CourseYearId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
