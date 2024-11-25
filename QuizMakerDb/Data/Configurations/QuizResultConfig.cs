using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Data.Configurations
{
	public class QuizResultConfig : IEntityTypeConfiguration<QuizResult>
	{
		public void Configure(EntityTypeBuilder<QuizResult> builder)
		{
			builder.HasKey(e => e.Id);

			builder.HasOne(m => m.AnswerStudentInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
