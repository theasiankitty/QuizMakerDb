using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class QuizVM
	{
		[DisplayFormat(DataFormatString = "{0:000000000#}")]
		public int Id { get; set; }

        public string Title { get; set; } = string.Empty!;

        public string Introduction { get; set; } = string.Empty!;

		public bool isQuestionRandomized { get; set; }

		public string ConclusionDescription { get; set; } = string.Empty!;

		public byte Minutes { get; set; }

		public byte Takes { get; set; }

		public bool ShowResults { get; set; }

		public string Date { get; set; } = string.Empty!;

        public int TeacherId { get; set; }

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
