using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.Models
{
	public class QuestionItem
	{
		[Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty!;

		public string Order { get; set; } = string.Empty!;

        public int QuizQuestionId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
