
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P01_StudentSystem.Data;
using P01_StudentSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<StudentSystemContext>(options =>
                    options.UseInMemoryDatabase("StudentSystemDB"))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<StudentSystemContext>())
            {
                DbSeeder.Seed(context);
                Console.WriteLine("Database seeded successfully!");
            }
        }
    }
}



namespace P01_StudentSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [StringLength(10)]
        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<Homework> Homeworks { get; set; } = new HashSet<Homework>();
        public ICollection<StudentCourse> StudentCourses { get; set; } = new HashSet<StudentCourse>();
    }
}



namespace P01_StudentSystem.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [MaxLength(80)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public ICollection<Resource> Resources { get; set; } = new HashSet<Resource>();
        public ICollection<Homework> Homeworks { get; set; } = new HashSet<Homework>();
        public ICollection<StudentCourse> StudentCourses { get; set; } = new HashSet<StudentCourse>();
    }
}


namespace P01_StudentSystem.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public string Url { get; set; }

        public ResourceType ResourceType { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}



namespace P01_StudentSystem.Models
{
    public class Homework
    {
        public int HomeworkId { get; set; }

        public string Content { get; set; }

        public ContentType ContentType { get; set; }

        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}

// File: Models/StudentCourse.cs
namespace P01_StudentSystem.Models
{
    public class StudentCourse
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}

// File: Models/Enums.cs
namespace P01_StudentSystem.Models
{
    public enum ResourceType
    {
        Video,
        Presentation,
        Document,
        Other
    }

    public enum ContentType
    {
        Application,
        Pdf,
        Zip
    }
}



namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext(DbContextOptions options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>().HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<Resource>().Property(r => r.Url).IsUnicode(false);
            modelBuilder.Entity<Homework>().Property(h => h.Content).IsUnicode(false);
            modelBuilder.Entity<Student>().Property(s => s.PhoneNumber).IsUnicode(false);
        }
    }
}



namespace P01_StudentSystem.Data
{
    public static class DbSeeder
    {
        public static void Seed(StudentSystemContext context)
        {
            var student = new Student { Name = "Ahmed", RegisteredOn = DateTime.Now };
            var course = new Course { Name = "C# Basics", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), Price = 100 };
            var resource = new Resource { Name = "Intro Video", Url = "http://video", ResourceType = ResourceType.Video, Course = course };
            var homework = new Homework { Content = "file.zip", ContentType = ContentType.Zip, SubmissionTime = DateTime.Now, Student = student, Course = course };

            context.Students.Add(student);
            context.Courses.Add(course);
            context.Resources.Add(resource);
            context.Homeworks.Add(homework);
            context.StudentCourses.Add(new StudentCourse { Student = student, Course = course });

            context.SaveChanges();
        }
    }
}