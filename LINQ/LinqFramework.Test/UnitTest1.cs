using LinqFramework.Extention;

namespace LinqFramework.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestWhereShouldReturnItemsThatMeetCondition()
        {
            //Arrange
            var data = new[]
            {
                new { Name = "John", Age = 25 },
                new { Name = "Jane", Age = 22 },
                new { Name = "Jack", Age = 30 },
                new { Name = "Jill", Age = 27 },
            };
            //Act
            var result = data.Where(e => e.Age >= 25);

            //Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("John", result[0].Name);
            Assert.AreEqual("Jack", result[1].Name);
            Assert.AreEqual("Jill", result[2].Name);
        }

        private readonly Student[] students = new[]
        {
            new Student { StudentID = 1, StudentName = "John", Age = 18 },
            new Student { StudentID = 2, StudentName = "Steve", Age = 21 },
            new Student { StudentID = 3, StudentName = "Bill", Age = 25 },
            new Student { StudentID = 4, StudentName = "Ram", Age = 20 },
            new Student { StudentID = 5, StudentName = "Ron", Age = 31 },
            new Student { StudentID = 6, StudentName = "Chris", Age = 17 },
            new Student { StudentID = 7, StudentName = "Rob", Age = 19 }
        };

        [TestMethod]
        public void TestWhereShouldFilterItemsBasedOnPredicate()
        {
            //Act
            var result = students.Where(std => std.Age > 12 && std.Age < 20);
            //Assert
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void TestCountShouldReturnNumberOfItemsMatchingPredicate()
        {
            //Act
            var result = students.Count(std => std.Age > 12 && std.Age < 20);
            //Assert
            Assert.AreEqual(3, result);

        }

        [TestMethod]
        public void TestFirstShouldReturnFirstItemMatchingPredicate()
        {
            //Act
            var result = students.First(std => std.Age > 20);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.StudentID);
        }

        [TestMethod]
        public void TestFirstShouldReturnFirstItemWhenNoPredicate()
        {
            //Act
            var result = students.First();
            //Assert
            Assert.AreEqual(1, result.StudentID);
        }

        [TestMethod]
        public void TestFirstShouldReturnDefaultWhenNoMatchFound()
        {
            //Act
            var result = students.First(std => std.Age < 10);
            //Assert
            Assert.AreEqual(default, result);
        }

        [TestMethod]
        public void TestSelectShouldReturnItemsBasedOnSelector()
        {
            var result = students.Select(std => new { Id = std.StudentID, Name = std.StudentName });

            Assert.AreEqual(7, result.Count());

            var expected = new[]
            {
                new { Id = 1, Name = "John" },
                new { Id = 2, Name = "Steve" },
                new { Id = 3, Name = "Bill" },
                new { Id = 4, Name = "Ram" },
                new { Id = 5, Name = "Ron" },
                new { Id = 6, Name = "Chris" },
                new { Id = 7, Name = "Rob" }
            };
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void TestWhereSelectFirstShouldReturnFirstItemMatchingPredicate()
        {
            var result = students
                .Where(std => std.Age > 21)
                .Select(std => std.StudentID)
                .First();

            Assert.AreEqual(3, result);
        }

    }

    [TestClass]
    public class FilteringOperatorOfType
    {
        [TestMethod]
        public void TestOfTypeShouldFilterStringsFromMixedArray()
        {
            // Arrange
            object[] mixedList = { 0, "One", "Two", 3, new Student(1, "Bill") };

            // Act
            var result = mixedList.OfType<string>();

            // Assert

        }
    }

    public class Student
    {
        
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public int Age { get; set; }

        public Student(int studentID, string studentName)
        {
            StudentID = studentID;
            StudentName = studentName;
        }

        public Student()
        {
        }
    }
}