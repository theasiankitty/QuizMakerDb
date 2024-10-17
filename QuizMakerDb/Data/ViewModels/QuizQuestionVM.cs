namespace QuizMakerDb.Data.ViewModels
{
	public class QuizQuestionVM
	{
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty!;

        public byte Order { get; set; }

        public byte QuestionType { get; set; }

        public byte[]? Image { get; set; }

        public byte Points { get; set; }

        public int QuizId { get; set; }
    }
}
