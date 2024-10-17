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
        First = 0,

        Second = 1,

        Third = 2,

        Fourth = 3
    }

    public enum Semester : byte
    {
        First = 0,

		Second = 1
    }

    public enum QuestionType : byte
    {
        [Display(Name = "Multiple Choice")]
        MultipleChoice = 0,

        [Display(Name = "True Or False")]
        TrueOrFalse = 1,

        [Display(Name = "Matching Or Ordering")]
		MatchingOrOrdering = 2,

        Numeric = 3,

        [Display(Name = "Fill In The Blank")]
        FillInTheBlank = 4
    }
}
