using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data.ViewModels
{
<<<<<<< HEAD
    public class TeacherSubjectVM
    {
        [DisplayFormat(DataFormatString = "{0:000000000#}")]
        public int Id { get; set; }
=======
	public class TeacherSubjectVM
	{
		[DisplayFormat(DataFormatString = "{0:000000000#}")]
		public int Id { get; set; }
>>>>>>> 64989993402aa9d467121889e5da6b3cb58f9ffd

        public string Teacher { get; set; } = string.Empty!;

        public string Subject { get; set; } = string.Empty!;

        public string Code { get; set; } = string.Empty!;
<<<<<<< HEAD

        [Display(Name = "Course Year")]
        public string CourseYear { get; set; } = string.Empty!;

        public string Title { get; set; } = string.Empty!;
    }
=======
	}
>>>>>>> 64989993402aa9d467121889e5da6b3cb58f9ffd
}
