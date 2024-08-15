using QuizMakerDb.Data.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
    public class SectionStudent
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int SectionId { get; set; }

        public int SchoolYearId { get; set; }

        public bool Active { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student StudentInfo { get; set; } = null!;

        [ForeignKey(nameof(SectionId))]
        public Section SectionInfo { get; set; } = null!;
    }
}
