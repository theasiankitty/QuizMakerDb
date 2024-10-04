using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
	public class SchoolYearVM
	{
		[DisplayFormat(DataFormatString = "{0:000000000#}")]
		public int Id { get; set; }

		[Display(Name = "School Year")]
		public string Name { get; set; } = string.Empty!;

		public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
	}
}
