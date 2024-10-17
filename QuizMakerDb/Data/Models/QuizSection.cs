using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class QuizSection
	{
		[Key]
		public int Id { get; set; }

		public int QuizId { get; set; }

		public int SectionId { get; set; }

        public Guid Code { get; set; }

        public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		[ForeignKey(nameof(QuizId))]
		public Quiz QuizInfo { get; set; } = null!;

		[ForeignKey(nameof(SectionId))]
		public Section SectionInfo { get; set; } = null!;
    }
}
