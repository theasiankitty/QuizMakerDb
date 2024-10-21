using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizMakerDb.Data.Models
{
	public class TeacherSubject
	{
		[Key]
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public int CourseYearSubjectId { get; set; }

        public bool Active { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public Teacher TeacherInfo { get; set; } = null!;

        [ForeignKey(nameof(CourseYearSubjectId))]
        public CourseYearSubject CourseYearSubjectInfo { get; set; } = null!;
    }
}
