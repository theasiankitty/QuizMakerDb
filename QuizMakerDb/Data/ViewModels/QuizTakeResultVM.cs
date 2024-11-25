namespace QuizMakerDb.Data.ViewModels
{
	public class QuizTakeResultVM
	{
		public int Id { get; set; }

		public int Score { get; set; }

		public string Title { get; set; } = string.Empty!;

		public string Introduction { get; set; } = string.Empty!;

		public bool ShowResults { get; set; }
	}
}
