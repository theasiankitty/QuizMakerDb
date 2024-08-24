using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class Quiz
	{
		[Key]
        public int Id { get; set; }

		[StringLength(100)]
        public string Title { get; set; } = string.Empty!;

		[StringLength(300)]
		public string Introduction { get; set; } = string.Empty!;

        public bool isQuestionRandomized { get; set; }

		[StringLength(300)]
		public string ConclusionDescription { get; set; } = string.Empty!;

        public byte Minutes { get; set; }

        public byte Takes { get; set; }

        public int TeacherId { get; set; }

        public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(TeacherId))]
		public Teacher TeacherInfo { get; set; } = null!;
    }
}
