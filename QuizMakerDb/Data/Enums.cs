using System.ComponentModel.DataAnnotations;

namespace QuizMakerDb.Data
{
    public enum Sex : byte
    {
        Male = 0,
        Female = 1
    }

    public enum YearLevel : byte
    {
        [Display(Name = "1st Year")]
        FirstYear = 0,

        [Display(Name = "2nd Year")]
        SecondYear = 1,

        [Display(Name = "3rd Year")]
        ThirdYear = 2,

        [Display(Name = "4th Year")]
        FourthYear = 3,
    }
}
