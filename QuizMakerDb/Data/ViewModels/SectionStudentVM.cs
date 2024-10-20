using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class SectionStudentVM
	{
		[DisplayFormat(DataFormatString = "{0:000000000#}")]
		public int Id { get; set; }

        public string Student { get; set; } = string.Empty!;

		[Display(Name = "Sex")]
		public string SexDescription { get; set; } = string.Empty!;

		[Display(Name = "Username")]
        public string StudentUserName { get; set; } = string.Empty!;

        public int SectionId { get; set; }

        public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
