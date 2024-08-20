using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class CourseYear
	{
        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; } = string.Empty!;

        public byte Year { get; set; }

        public int CourseId { get; set; }

        public bool Active { get; set; }

		public Guid CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public Guid? UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course CourseInfo { get; set; } = null!;
    }
}
