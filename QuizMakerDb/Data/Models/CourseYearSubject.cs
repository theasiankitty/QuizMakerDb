using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.Models
{
	public class CourseYearSubject
	{
		[Key]
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public int CourseYearId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
