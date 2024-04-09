using Microsoft.EntityFrameworkCore;
using Learning_App.Models;
using Learning_App.Models.Response;

namespace Learning_App
{
    public class DatabaseContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DatabaseContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseAssignments> CourseAssignments { get; set; }
        public virtual DbSet<CourseEnrollment> CourseEnrollment { get; set; }
        public virtual DbSet<CourseResource> CourseResources { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<StudentCourseAssignment> StudentCourseAssignments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DbConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(e => e.UserId)
                .HasName("users_pk");

                entity.Property(e => e.UserId)
                .HasColumnName("UserId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.FirstName)
                .HasColumnName("First_Name")
                .IsRequired();

                entity.Property(e => e.LastName)
                .HasColumnName("Last_Name")
                .IsRequired();

                entity.Property(e => e.Email)
                .HasColumnName("Email")
                .IsRequired();

                entity.Property(e => e.Password)
                .HasColumnName("Password")
                .IsRequired();

                entity.Property(e => e.Role)
                .HasColumnName("Role")
                .IsRequired()
                .HasConversion<int>();
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");

                entity.HasKey(e => e.CourseId)
                .HasName("courses_pk");

                entity.Property(e => e.CourseId)
                .HasColumnName("CourseId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                .HasColumnName("Title")
                .IsRequired();

                entity.Property(e => e.Description)
                .HasColumnName("Description")
                .IsRequired();

                entity.Property(e => e.UserId)
                .HasColumnName("UserId")
                .IsRequired();

                entity.Property(e => e.ImageUrl)
                .HasColumnName("ImageUrl");

                entity.HasOne(e => e.Instructor)
                    .WithMany(u => u.InstructorCourses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("course_user_fk");
            });

            modelBuilder.Entity<StudentCourseAssignment>(entity =>
            {
                entity.ToTable("StudentCourseAssignment");

                entity.HasKey(e => e.StudentCourseAssignmentId)
                .HasName("studentcourseassignment_pk");

                entity.Property(e => e.StudentCourseAssignmentId)
                .HasColumnName("StudentCourseAssignmentId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                .HasColumnName("UserId")
                .IsRequired();

                entity.Property(e => e.AssignmentId)
                .HasColumnName("AssignmentId")
                .IsRequired();

                entity.Property(e => e.Score)
                .HasColumnName("Score")
                .IsRequired();

                entity.HasOne(e => e.Student)
                    .WithOne()
                    .HasForeignKey<StudentCourseAssignment>(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("student_course_assignment_fk");

                entity.HasOne(e => e.Assignment)
                    .WithMany()
                    .HasForeignKey(e => e.AssignmentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("course_assignment_fk");
            });

            modelBuilder.Entity<CourseEnrollment>(entity =>
            {
                entity.ToTable("CourseEnrollment");

                entity.HasKey(e => e.CourseEnrollmentId)
                .HasName("courseenrollment_pk");

                entity.Property(e => e.CourseEnrollmentId)
                .HasColumnName("CourseEnrollmentId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.StudentId)
                .HasColumnName("StudentId")
                .IsRequired();

                entity.Property(e => e.CourseId)
                .HasColumnName("CourseId")
                .IsRequired();

                entity.Property(e => e.Grade)
                .HasColumnName("Grade")
                .IsRequired();

                entity.Property(e => e.LastCompletedLessonId)
                .HasColumnName("LastCompletedLessonId");
               

                entity.HasOne(e => e.Student)
                    .WithMany(u => u.StudentCourses)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("course_enrollment_user_fk");

                entity.HasOne(e => e.Course)
                    .WithMany()
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("course_enrollment_course_fk");
            });

            modelBuilder.Entity<CourseResource>(entity =>
            {
                entity.ToTable("CourseResource");

                entity.HasKey(e => e.ResourceId)
                .HasName("courseresource_pk");

                entity.Property(e => e.ResourceId)
                .HasColumnName("ResourceId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.CourseId)
                .HasColumnName("CourseId")
                .IsRequired();

                entity.Property(e => e.ResourceType)
                .HasColumnName("ResourceType")
                .IsRequired();

                entity.Property(e => e.ContentInfo)
                .HasColumnName("ContentInfo")
                .IsRequired();

                entity.Property(e => e.LessonId)
                .HasColumnName("LessonId")
                .IsRequired();

                entity.HasOne(e => e.Course)
                    .WithMany()
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("course_resource_fk");

                entity.HasOne(e => e.Lesson)
                    .WithMany(l => l.Resources)
                    .HasForeignKey(e => e.LessonId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("resource_lesson_fk");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("Lesson");

                entity.HasKey(e => e.LessonId)
                .HasName("lesson_pk");

                entity.Property(e => e.LessonId)
                .HasColumnName("LessonId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                .HasColumnName("Title")
                .IsRequired();

                entity.Property(e => e.Description)
                .HasColumnName("Description")
                .IsRequired();

                entity.Property(e => e.CourseId)
                .HasColumnName("CourseId")
                .IsRequired();

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Lessons)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("course_lesson_fk");
            });

            modelBuilder.Entity<StudentCourseAssignment>(entity =>
            {
                entity.ToTable("StudentCourseAssignment");

                entity.HasKey(e => e.StudentCourseAssignmentId)
                .HasName("studentcourseassignment_pk");

                entity.Property(e => e.StudentCourseAssignmentId)
                .HasColumnName("StudentCourseAssignmentId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                .HasColumnName("UserId")
                .IsRequired();

                entity.Property(e => e.AssignmentId)
                .HasColumnName("AssignmentId")
                .IsRequired();

                entity.Property(e => e.Score)
                .HasColumnName("Score")
                .IsRequired();

                entity.Property(e => e.FileUrl)
                .HasColumnName("FileUrl");

                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("studentcourseassignment_student_fk");

                entity.HasOne(e => e.Assignment)
                    .WithMany()
                    .HasForeignKey(e => e.AssignmentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("studentcourseassignment_assignment_fk");
            });

            modelBuilder.Entity<CourseAssignments>(entity =>
            {
                entity.ToTable("CourseAssignments");

                entity.HasKey(e => e.AssignmentId)
                .HasName("courseassignments_pk");

                entity.Property(e => e.AssignmentId)
                .HasColumnName("AssignmentId")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.CourseId)
                .HasColumnName("CourseId")
                .IsRequired();

                entity.Property(e => e.Title)
                .HasColumnName("Title")
                .IsRequired();

                entity.Property(e => e.Description)
                .HasColumnName("Description")
                .IsRequired();

                entity.Property(e => e.OverallScore)
                .HasColumnName("OverallScore")
                .IsRequired();

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Assignments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("assignment_course_fk");

                
            });
        }
    }
}
