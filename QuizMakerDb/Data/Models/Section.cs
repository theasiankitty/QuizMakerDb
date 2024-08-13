using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; } = string.Empty!;

        public int SchoolYearId { get; set; }

        public int CourseYearId { get; set; }

        public bool Active { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(SchoolYearId))]
        public SchoolYear SchoolYearInfo { get; set; }

        [ForeignKey(nameof(CourseYearId))]
        public CourseYear CourseYearInfo { get; set; }
    }
}
