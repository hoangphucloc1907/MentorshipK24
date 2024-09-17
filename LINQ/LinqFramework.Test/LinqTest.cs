using LinqFramework;

namespace LinqFramework.Test
{
    [TestClass]
    public class WhereSelectFirstTest
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
    public class FilteringOperatorOfTypeTest
    {
        [TestMethod]
        public void TestOfTypeShouldFilterStringsFromMixedArray()
        {
            // Arrange
            object[] mixedList = { 0, "One", "Two", 3, new Student(1, "Bill") };

            // Act
            var result = mixedList.OfType<string>();
            var expected = new[] { "One", "Two" };
            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void TestOfTypeShouldFilterNumbersFromMixedArray()
        {
            // Arrange
            object[] mixedList = { 0, "One", 2, 3, new Student(1, "Bill") };

            // Act
            var result = mixedList.OfType<int>();

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
        }

        [TestMethod]
        public void TestOfTypeShouldFilterStudentObjectsFromMixedArray()
        {
            // Arrange
            var mixedList = new object[] { 0, "One", 2, new Student(1, "Bill"), new Student(2, "John") };
            var expected = new List<Student> { new Student(1, "Bill"), new Student(2, "John") };

            // Act
            var result = mixedList.OfType<Student>();

            // Assert
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void TestOfTypeShouldFilterNullValuesFromMixedArray()
        {
            // Arrange
            var mixedList = new object[] { null, "One", null, 2 };
            var expected = new object[] { null, null };

            // Act
            var result = mixedList.OfType<object>();

            // Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestOfTypeShouldFilterArraysFromMixedArray()
        {
            // Arrange
            object[] mixedList = { new int[] { 1, 2 }, "One", new int[] { 3, 4 }, 5 };
            // Act
            var result = mixedList.OfType<Array>();

            // Assert
            Assert.AreEqual(2, result.Length);
            CollectionAssert.AreEqual(new int[] { 1, 2 }, (int[])result[0]);
            CollectionAssert.AreEqual(new int[] { 3, 4 }, (int[])result[1]);
        }
    }

    [TestClass]
    public class SortTest
    {
        [TestMethod]
        public void TestOrderByAscendingByName()
        {
            var students = new List<Student>
            {
                new Student { StudentID = 1, StudentName = "John", Age = 18 },
                new Student { StudentID = 2, StudentName = "Chris", Age = 21 },
                new Student { StudentID = 3, StudentName = "Bill", Age = 25 },
                new Student { StudentID = 4, StudentName = "Ram", Age = 42 },
                new Student { StudentID = 5, StudentName = "Alex", Age = 31 },
                new Student { StudentID = 6, StudentName = "Chris", Age = 17 },
                new Student { StudentID = 7, StudentName = "Rob", Age = 19 }
            };

            var sortedStudents = students.OrderBy(e => e.StudentName);

            var result = sortedStudents.First();
            var expected = new Student { StudentID = 5, StudentName = "Alex", Age = 31 };

            Assert.AreEqual(expected.StudentID, result.StudentID);
            Assert.AreEqual(expected.StudentName, result.StudentName);
            Assert.AreEqual(expected.Age, result.Age);
        }

        [TestMethod]
        public void TestOrderByDescendingByAge()
        {
            var students = new List<Student>
            {
                new Student { StudentID = 1, StudentName = "John", Age = 18 },
                new Student { StudentID = 2, StudentName = "Chris", Age = 21 },
                new Student { StudentID = 3, StudentName = "Bill", Age = 25 },
                new Student { StudentID = 4, StudentName = "Ram", Age = 42 },
                new Student { StudentID = 5, StudentName = "Alex", Age = 31 },
                new Student { StudentID = 6, StudentName = "Chris", Age = 17 },
                new Student { StudentID = 7, StudentName = "Rob", Age = 19 }
            };

            var sortedStudents = students.OrderByDescending(e => e.Age);

            var result = sortedStudents.First();
            var expected = new Student { StudentID = 4, StudentName = "Ram", Age = 42 };

            Assert.AreEqual(expected.StudentID, result.StudentID);
            Assert.AreEqual(expected.StudentName, result.StudentName);
            Assert.AreEqual(expected.Age, result.Age);
        }

        [TestMethod]
        public void TestThenBy_AscendingByNameThenByAge()
        {
            // Arrange
            var studentList = new List<Student>
            {
                new Student { StudentName = "Alice", Age = 22 },
                new Student { StudentName = "Bob", Age = 20 },
                new Student { StudentName = "Alice", Age = 19 }
            };

            // Act
            var sortedList = studentList.OrderBy(s => s.StudentName).ThenBy(s => s.Age);

            // Assert
            Assert.AreEqual("Alice", sortedList[0].StudentName);
            Assert.AreEqual(19, sortedList[0].Age);
            Assert.AreEqual("Alice", sortedList[1].StudentName);
            Assert.AreEqual(22, sortedList[1].Age);
            Assert.AreEqual("Bob", sortedList[2].StudentName);
        }

        [TestMethod]
        public void TestThenByDescending_AscendingByNameThenByAgeDescending()
        {
            // Arrange
            var studentList = new List<Student>
            {
                new Student { StudentName = "Alice", Age = 22 },
                new Student { StudentName = "Bob", Age = 20 },
                new Student { StudentName = "Alice", Age = 19 }
            };

            // Act
            var sortedList = studentList.OrderByDescending(s => s.StudentName).ThenByDescending(s => s.Age);

            // Assert
            Assert.AreEqual("Bob", sortedList[0].StudentName);
            Assert.AreEqual(20, sortedList[0].Age);
            Assert.AreEqual("Alice", sortedList[1].StudentName);
            Assert.AreEqual(19, sortedList[1].Age);
            Assert.AreEqual("Alice", sortedList[2].StudentName);
        }
    }

    [TestClass]
    public class GroupByToLookupTest
    {
        private List<Student> data = new List<Student>
        {
            new Student { StudentID = 1, StudentName = "John", Age = 18 },
            new Student { StudentID = 2, StudentName = "Steve", Age = 21 },
            new Student { StudentID = 3, StudentName = "Bill", Age = 18 },
            new Student { StudentID = 4, StudentName = "Ram", Age = 20 },
            new Student { StudentID = 5, StudentName = "Abram", Age = 21 },
        };

        [TestMethod]
        public void TestGroupByShouldGroupStudentsByAge()
        {
            // Act
            var result = data.GroupBy(e => e.Age);

            // Define the expected result
            var expected = new Dictionary<int, List<Student>>
            {
                { 18, new List<Student>
                    {
                        new Student { Age = 18, StudentID = 1, StudentName = "John" },
                        new Student { Age = 18, StudentID = 3, StudentName = "Bill" }
                    }
                },
                { 20, new List<Student>
                    {
                        new Student { Age = 20, StudentID = 4, StudentName = "Ram" }
                    }
                },
                { 21, new List<Student>
                    {
                        new Student { Age = 21, StudentID = 2, StudentName = "Steve" },
                        new Student { Age = 21, StudentID = 5, StudentName = "Abram" }
                    }
                }
            };

            // Assert
            CollectionAssert.AreEqual(expected[18], result[18]);
            CollectionAssert.AreEqual(expected[20], result[20]);
            CollectionAssert.AreEqual(expected[21], result[21]);
        }

        [TestMethod]
        public void TestToLookupShouldLookupStudentsByAge()
        {
            // Act
            var result = data.ToLookup(e => e.Age);

            // Define the expected result
            var expected = new Dictionary<int, List<Student>>
            {
                { 18, new List<Student>
                    {
                        new Student { Age = 18, StudentID = 1, StudentName = "John" },
                        new Student { Age = 18, StudentID = 3, StudentName = "Bill" }
                    }
                },
                { 20, new List<Student>
                    {
                        new Student { Age = 20, StudentID = 4, StudentName = "Ram" }
                    }
                },
                { 21, new List<Student>
                    {
                        new Student { Age = 21, StudentID = 2, StudentName = "Steve" },
                        new Student { Age = 21, StudentID = 5, StudentName = "Abram" }
                    }
                }
            };

            // Assert
            CollectionAssert.AreEqual(expected[18], result[18]);
            CollectionAssert.AreEqual(expected[20], result[20]);
            CollectionAssert.AreEqual(expected[21], result[21]);
        }

        [TestMethod]
        public void TestGroupByEmptyArrayShouldReturnEmptyDictionary()
        {
            // Act
            var result = new List<Student>().GroupBy(e => e.StudentName);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestToLookupEmptyArrayShouldReturnEmptyDictionary()
        {
            // Act
            var result = new List<Student>().ToLookup(e => e.StudentName);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestDistinctByShouldReturnDistinctStudentsByStudentID()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { StudentID = 1, StudentName = "John", Age = 18 },
                new Student { StudentID = 2, StudentName = "Steve", Age = 15 },
                new Student { StudentID = 3, StudentName = "Bill", Age = 25 },
                new Student { StudentID = 3, StudentName = "Bill", Age = 25 },
                new Student { StudentID = 3, StudentName = "Bill", Age = 25 },
                new Student { StudentID = 3, StudentName = "Bill", Age = 25 },
                new Student { StudentID = 5, StudentName = "Ron", Age = 19 }
            };

            var expected = new List<Student>
            {
                new Student { StudentID = 1, StudentName = "John", Age = 18 },
                new Student { StudentID = 2, StudentName = "Steve", Age = 15 },
                new Student { StudentID = 3, StudentName = "Bill", Age = 25 },
                new Student { StudentID = 5, StudentName = "Ron", Age = 19 }
            };

            // Act
            var distinctStudents = students.DistinctBy(s => s.StudentID);

            // Assert
            CollectionAssert.AreEquivalent(expected, distinctStudents);
        }
    }

    [TestClass]
    public class AggregateTest
    {
        private List<Student> data = new List<Student>
        {
            new Student { StudentName = "John", Age = 25 },
            new Student { StudentName = "Jane", Age = 22 },
            new Student { StudentName = "Jack", Age = 30 },
            new Student { StudentName = "Jill", Age = 27 },
        };

        [TestMethod]
        public void TestSum()
        {
            //Act 
            var result = data.Sum(s => s.Age);
            //Assert
            Assert.AreEqual(104, result);
        }

        [TestMethod]
        public void TestSumWithSelector()
        {
            //Act 
            var result = data.Sum(s => s.Age * 2);
            //Assert
            Assert.AreEqual(208, result);
        }

        [TestMethod]
        public void TestMax()
        {
            //Act 
            var result = data.Max(s => s.Age);
            //Assert
            Assert.AreEqual(30, result);
        }
        
        [TestMethod]
        public void TestMin()
        {
            //Act 
            var result = data.Min(s => s.Age);
            //Assert
            Assert.AreEqual(22, result);
        }

        [TestMethod]
        public void TestAverage()
        {
            //Act
            var result = data.Average(s => s.Age);
            //Assert
            Assert.AreEqual(26.0, result);
        }

        [TestMethod]
        public void TestAllTrueCondition()
        {
            var result = data.All(s => s.Age > 20);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestAllFalseCondition()
        {
            var result = data.All(s => s.Age > 25);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestAnyTrueCondition()
        {
            var result = data.Any(s => s.Age > 20);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestAny_FalseCondition()
        {
            var result = data.Any(s => s.Age > 30);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestContainsExistingItem()
        {
            var result = data.contains(data[0]);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestContainsNonExistingItem()
        {
            var student = new Student { StudentName = "Nonexistent", Age = 99 };
            var result = data.contains(student);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestElementAtValidIndex()
        {
            var result = data.ElementAt(2);

            Assert.IsNotNull(result);
            Assert.AreEqual("Jack", result.StudentName);
            Assert.AreEqual(30, result.Age);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestElementAtInvalidIndex()
        {
            var result = data.ElementAt(10); // This should throw an exception
        }

        [TestMethod]
        public void TestElementAtOrDefaultValidIndex()
        {
            var result = data.ElementAtOrDefault(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("Jane", result.StudentName);
        }

        [TestMethod]
        public void TestElementAtOrDefault_InvalidIndex()
        {
            var result = data.ElementAtOrDefault(10);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestFirstOrDefaultWithMatchReturnsFirstMatch()
        {
            var result = data.FirstOrDefault(p => p.Age > 24);
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.StudentName);
        }

        [TestMethod]
        public void TestFirstOrDefaultWithNoMatchReturnsDefault()
        {
            var result = data.FirstOrDefault(p => p.Age > 40);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestLastOrDefaultWithMatchReturnsLastMatch()
        {
            var result = data.LastOrDefault(p => p.Age > 24);
            Assert.IsNotNull(result);
            Assert.AreEqual("Jill", result.StudentName);
        }

        [TestMethod]
        public void TestLastOrDefaultWithNoMatchReturnsDefault()
        {
            var result = data.LastOrDefault(p => p.Age > 40);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestLastWithPredicateReturnsLastMatch()
        {
            var result = data.Last(p => p.Age > 24);
            Assert.IsNotNull(result);
            Assert.AreEqual("Jill", result.StudentName);
        }

        [TestMethod]
        public void TestLastWithNoPredicateReturnsLastItem()
        {
            var result = data.Last();
            Assert.IsNotNull(result);
            Assert.AreEqual("Jill", result.StudentName);
        }

        [TestMethod]
        public void TestLastWithNoMatchReturnsDefault()
        {
            var result = data.Last(p => p.Age > 40);
            Assert.IsNull(result);
        }
    }

    public class Student
    {

        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public int Age { get; set; }
        public Student(int studentID, string studentName, int age)
        {
            StudentID = studentID;
            StudentName = studentName;
            Age = age;
        }

        public Student(int studentID, string studentName)
        {
            StudentID = studentID;
            StudentName = studentName;
        }

        public Student()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is Student student &&
                   StudentID == student.StudentID &&
                   StudentName == student.StudentName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StudentID, StudentName);
        }
    }
}