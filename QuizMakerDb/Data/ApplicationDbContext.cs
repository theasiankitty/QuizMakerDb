﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuizMakerDb.Data.Configurations;
using QuizMakerDb.Data.Identity;
using QuizMakerDb.Data.Models;
using QuizMakerDb.Data.ViewModels;

namespace QuizMakerDb.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options)
        {
        }

		public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizSubject> QuizSubjects { get; set; }
        public DbSet<QuestionItem> QuestionItems { get; set; }
		public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public DbSet<QuizTake> QuizTakes { get; set; }
        public DbSet<AnswerStudent> AnswerStudents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
		public DbSet<TeacherSubject> TeacherSubjects { get; set; }
		public DbSet<SchoolYear> SchoolYears { get; set; }
        public DbSet<Course> Courses { get; set; }
		public DbSet<CourseYear> CourseYears { get; set; }
		public DbSet<CourseYearSubject> CourseYearSubjects { get; set; }
		public DbSet<Section> Sections { get; set; }
        public DbSet<SectionStudent> SectionStudents { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

			builder.ApplyConfiguration(new AnswerStudentConfig());
			builder.ApplyConfiguration(new QuizSubjectConfig());
			builder.ApplyConfiguration(new QuizTakeConfig());
            builder.ApplyConfiguration(new QuizResultConfig());

            builder.Entity<AppUser>().ToTable("IdentityUsers");
            builder.Entity<AppRole>().ToTable("IdentityRoles");
            builder.Entity<AppUserClaim>().ToTable("IdentityUserClaims");
            builder.Entity<AppUserRole>().ToTable("IdentityUserRoles");
            builder.Entity<AppUserLogin>().ToTable("IdentityUserLogins");
            builder.Entity<AppRoleClaim>().ToTable("IdentityRoleClaims");
            builder.Entity<AppUserToken>().ToTable("IdentityUserTokens");

            //Seed(builder);
        }

		protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			builder.ConfigureWarnings(delegate (WarningsConfigurationBuilder warnings)
			{
				warnings.Ignore(CoreEventId.ForeignKeyAttributesOnBothNavigationsWarning);
			});
		}

		private void Seed(ModelBuilder builder)
        {
            Guid adminUserId = Guid.NewGuid();
            Guid adminRoleId = Guid.NewGuid();
            Guid teacherUserId = Guid.NewGuid();
            Guid teacherRoleId = Guid.NewGuid();
            Guid studentId = Guid.NewGuid();
            Guid studentRoleId = Guid.NewGuid();

            SeedRoles(builder, adminRoleId, teacherRoleId, studentRoleId);
            SeedUser(builder, adminUserId, teacherUserId, studentId);
            SeedUserRoles(builder, adminUserId, adminRoleId, teacherUserId, teacherRoleId, studentId, studentRoleId);
        }

        private void SeedRoles(ModelBuilder builder, Guid adminRoleId, Guid teacherRoleId, Guid studentRoleId)
        {
            string role = "Admin";
            AppRole adminRole = new()
            {
                Id = adminRoleId,
                Name = role,
                NormalizedName = role.ToUpper()
            };

            role = "Teacher";
            AppRole teacherRole = new()
            {
                Id = teacherRoleId,
                Name = role,
                NormalizedName = role.ToUpper()
            };

            role = "Student";
            AppRole studentRole = new()
            {
                Id = studentRoleId,
                Name = role,
                NormalizedName = role.ToUpper()
            };

            builder.Entity<AppRole>().HasData(adminRole, teacherRole, studentRole);
        }

        private void SeedUser(ModelBuilder builder, Guid adminUserId, Guid teacherUserId, Guid studentId)
        {
            var hasher = new PasswordHasher<AppUser>();

            string userName = "SysAdmin";
            string userEmail = "sysadmin@domain.com";
            AppUser adminUser = new()
            {
                Id = adminUserId,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = userEmail,
                NormalizedEmail = userEmail.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                Active = true
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@1234");

            userName = "Teacher";
            userEmail = "teacher@domain.com";
            AppUser teacherUser = new()
            {
                Id = teacherUserId,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = userEmail,
                NormalizedEmail = userEmail.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                Active = true
            };
            teacherUser.PasswordHash = hasher.HashPassword(teacherUser, "Teacher@1234");

            userName = "Student";
            userEmail = "student@domain.com";
            AppUser student = new()
            {
                Id = studentId,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = userEmail,
                NormalizedEmail = userEmail.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                Active = true
            };
            student.PasswordHash = hasher.HashPassword(student, "Student@1234");

            builder.Entity<AppUser>().HasData(adminUser, teacherUser, student);
        }

        private void SeedUserRoles(ModelBuilder builder, Guid adminUserId, Guid adminRoleId, Guid teacherUserId, Guid teacherRoleId, Guid studentId, Guid studentRoleId)
        {
            AppUserRole userRoleAdmin = new()
            {
                UserId = adminUserId,
                RoleId = adminRoleId
            };

            AppUserRole userRoleTeacher = new()
            {
                UserId = teacherUserId,
                RoleId = teacherRoleId
            };

            AppUserRole userRoleStudent = new()
            {
                UserId = studentId,
                RoleId = studentRoleId
            };

            builder.Entity<AppUserRole>().HasData(userRoleAdmin, userRoleTeacher, userRoleStudent);
        }
    }
}
