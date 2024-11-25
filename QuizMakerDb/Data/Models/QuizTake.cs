using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class QuizTake
	{
		[Key]
		public int Id { get; set; }

        public int StudentId { get; set; }

        public int QuizId { get; set; }

        public DateTime StartTime { get; set; }

        public int Duration { get; set; }

		public bool isFinished { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(StudentId))]
		public Student StudentInfo { get; set; } = null!;

		[ForeignKey(nameof(QuizId))]
		public Quiz QuizInfo { get; set; } = null!;
    }
}
