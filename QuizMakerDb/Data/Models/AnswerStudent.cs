using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class AnswerStudent
	{
		[Key]
		public int Id { get; set; }

		[StringLength(100)]
		public string Answer { get; set; } = string.Empty!;

        public bool isCorrect { get; set; }

        public int QuestionAnswerId { get; set; }

        public int StudentId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(QuestionAnswerId))]
		public QuestionAnswer QuestionAnswerInfo { get; set; } = null!;

		[ForeignKey(nameof(StudentId))]
		public Student StudentInfo { get; set; } = null!;
    }
}
