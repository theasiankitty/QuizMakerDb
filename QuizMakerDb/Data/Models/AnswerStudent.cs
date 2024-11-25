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

        public int QuizQuestionId { get; set; }

		public int QuizTakeId { get; set; }

		public int StudentId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(QuizQuestionId))]
		public QuizQuestion QuizQuestionInfo { get; set; } = null!;

		[ForeignKey(nameof(QuizTakeId))]
		public QuizTake QuizTakeInfo { get; set; } = null!;

		[ForeignKey(nameof(StudentId))]
		public Student StudentInfo { get; set; } = null!;
    }
}
