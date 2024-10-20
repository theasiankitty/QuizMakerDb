using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Data.Configurations
{
	public class QuizSubjectConfig : IEntityTypeConfiguration<QuizSubject>
	{
		public void Configure(EntityTypeBuilder<QuizSubject> builder)
		{
			builder.HasKey(e => e.Id);

			builder.HasOne(m => m.QuizInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(m => m.SubjectInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(m => m.SectionInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
