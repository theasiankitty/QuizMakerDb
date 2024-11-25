using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class QuizResult
	{
		[Key]
		public int Id { get; set; }

		public bool isCorrect { get; set; }

		public int AnswerStudentId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(AnswerStudentId))]
		public AnswerStudent AnswerStudentInfo { get; set; } = null!;
	}
}
