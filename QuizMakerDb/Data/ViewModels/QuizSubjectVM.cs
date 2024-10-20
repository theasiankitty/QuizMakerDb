using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class QuizSubjectVM
	{
		public int Id { get; set; }

		public int QuizId { get; set; }

        public int SubjectId { get; set; }

        public string Subject { get; set; } = string.Empty!;

        public int SectionId { get; set; }

		public string Section { get; set; } = string.Empty!;

		[Display(Name = "Course Year")]
		public string CourseYear { get; set; } = string.Empty!;

        public string Code { get; set; } = string.Empty!;

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }
	}
}
