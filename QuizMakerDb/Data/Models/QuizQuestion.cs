using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class QuizQuestion
	{
		[Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty!;

        public int Order { get; set; }

		public byte QuestionType { get; set; }

		public byte[]? Image { get; set; }

        public byte Points { get; set; }

        public int QuizId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(QuizId))]
		public Quiz QuizInfo { get; set; } = null!;

	}
}
