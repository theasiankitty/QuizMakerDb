﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizMakerDb.Data.Models;

namespace QuizMakerDb.Data.Configurations
{
	public class AnswerStudentConfig : IEntityTypeConfiguration<AnswerStudent>
	{
		public void Configure(EntityTypeBuilder<AnswerStudent> builder)
		{
			builder.HasKey(e => e.Id);

			builder.HasOne(m => m.QuestionAnswerInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(m => m.StudentInfo)
				.WithOne()
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
