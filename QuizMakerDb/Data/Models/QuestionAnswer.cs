using QuizMakerDb.Data.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
    public class QuestionAnswer
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Answer { get; set; } = string.Empty!;

        public bool isCorrect { get; set; }

        public bool ShowAnswer { get; set; }

        public string? Order { get; set; }

        public byte[]? Image { get; set; }

        public int QuizQuestionId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(QuizQuestionId))]
		public QuizQuestion QuizQuestionInfo { get; set; } = null!;
	}
}
