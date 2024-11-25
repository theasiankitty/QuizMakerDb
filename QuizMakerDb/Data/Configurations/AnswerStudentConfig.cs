using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Data.Configurations
{
	public class AnswerStudentConfig : IEntityTypeConfiguration<AnswerStudent>
	{
		public void Configure(EntityTypeBuilder<AnswerStudent> builder)
		{
			//builder.HasKey(e => e.Id);

			//builder.HasOne(m => m.QuizQuestionInfo)
			//	.WithOne()
			//	.OnDelete(DeleteBehavior.NoAction);

			//builder.HasOne(m => m.QuizTakeInfo)
			//	.WithOne()
			//	.OnDelete(DeleteBehavior.NoAction);

			//builder.HasOne(m => m.StudentInfo)
			//	.WithOne()
			//	.OnDelete(DeleteBehavior.NoAction);

			builder.HasKey(e => e.Id);

			builder.HasOne(m => m.QuizQuestionInfo)
				.WithMany()
				.HasForeignKey(m => m.QuizQuestionId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(m => m.QuizTakeInfo)
				.WithMany()
				.HasForeignKey(m => m.QuizTakeId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(m => m.StudentInfo)
				.WithMany()
				.HasForeignKey(m => m.StudentId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
