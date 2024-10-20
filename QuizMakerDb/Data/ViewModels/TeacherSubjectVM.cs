using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
    public class TeacherSubjectVM
    {
        [DisplayFormat(DataFormatString = "{0:000000000#}")]
        public int Id { get; set; }

        public string Teacher { get; set; } = string.Empty!;

        public string Subject { get; set; } = string.Empty!;

        public string Code { get; set; } = string.Empty!;

        [Display(Name = "Course Year")]
        public string CourseYear { get; set; } = string.Empty!;

        public string Title { get; set; } = string.Empty!;
    }
}
