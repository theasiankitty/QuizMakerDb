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
}
