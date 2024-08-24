using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class Subject
	{
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; } = string.Empty!;

		[StringLength(30)]
		public string Code { get; set; } = string.Empty!;

        public byte Semester { get; set; }

        public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }
    }
}
