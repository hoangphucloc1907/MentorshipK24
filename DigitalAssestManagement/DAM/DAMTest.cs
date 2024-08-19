




namespace DAM
{
    public class DAMTest
    {
        
        private static User InitUserData()
        {
            var user = new User();
            user.Name = "Dao";
            user.Id = 1;
            user.AddDrive(new Drive { DriveId = 1, DriveName = "GoogleDrive" });
            user.AddDrive(new Drive { DriveId = 2, DriveName = "OneDrive" });
            return user;
        }

        [Fact]
        public void TestInitUser()
        {
            var user = new User();
            user.Id = 1;
            user.Name = "Loc";

            Assert.Equal(1, user.Id);
            Assert.Equal("Loc", user.Name);
        }

        [Fact]
        public void TestUserHasMultipleDrives() 
        { 
            User user = InitUserData();
            
            Assert.Equal(2, user.Drives.Count);
        }

        [Fact]
        public void TestOneDriveHasMultipleFolders()
        {
            var drive = new Drive { DriveId = 1, DriveName = "GoogleDrive" };
            drive.AddFolder(new Folder { FolderId = 1, FolderName = "mentorship2024" });
            drive.AddFolder(new Folder { FolderId = 2, FolderName = "bbv" });

            Assert.Equal(2, drive.Folders.Count);
        }

        [Fact]
        public void TestOneDriveHasMultipleFiles() 
        {
            var drive = new Drive { DriveId = 1, DriveName = "GoogleDrive" };
            drive.AddFile(new File { FileId = 1, FileName = "mentorship.pdf" });
            drive.AddFile(new File { FileId = 1, FileName = "daovo.docx" });

            Assert.Equal(2, drive.Files.Count);
        }

        [Fact]
        public void TestFolderHasMultipleFilesAndSubfolders()
        {
            var user = InitUserData();

            user.Drives[0].AddFolder(new Folder { FolderId = 1, FolderName = "internship" });
            user.Drives[0].AddFolder(new Folder { FolderId = 2, FolderName = "bbv" });
            user.Drives[0].AddFile(new File { FileId = 1, FileName = "mentorship.pdf" });

            Assert.Equal(2, user.Drives[0].Folders.Count);
            Assert.Equal(1, user.Drives[0].Files.Count);
        }

        [Fact]
        public void TestFolderHasMultpleSubFolders()
        {
            var user = InitUserData();

            user.Drives[0].AddFolder(new Folder { FolderId = 1, FolderName = "internship" });
            Folder folderBbv = new Folder { FolderId = 1, FolderName = "bbv" };
            user.Drives[0].AddFolder(folderBbv);
            user.Drives[0].AddFile(new File { FileId = 1, FileName = "mentorship.pdf" });

            Folder folderWorking = new Folder { FolderId = 1, FolderName = "working" };
            folderBbv.AddFolder(folderWorking);
            folderBbv.AddFolder(new Folder { FolderId = 1, FolderName = "projects" });
            folderBbv.AddFolder(new Folder { FolderId = 1, FolderName = "design" });
            folderBbv.AddFolder(new Folder { FolderId = 1, FolderName = "training" });

            Assert.Equal(4, folderBbv.Folders.Count);

            folderWorking.AddFile(new File { FileId = 1, FileName = "sample.sql" });

            Assert.Equal(1, folderWorking.Files.Count);
        }

        [Fact]
        public void TestUserHasMultipleDrivesWithFolders()
        {
            var user = InitUserData();

            user.Drives[0].AddFolder(new Folder() { FolderId = 1, FolderName = "mentorship2024" });
            user.Drives[0].AddFolder(new Folder() { FolderId = 2, FolderName = "bbv" });

            user.Drives[1].AddFolder(new Folder() { FolderId = 1, FolderName = "mentorship2024" });
            user.Drives[1].AddFolder(new Folder() { FolderId = 2, FolderName = "bbv" });

            Assert.Equal(2, user.Drives[0].Folders.Count);
            Assert.Equal(2, user.Drives[1].Folders.Count);
        }

        [Fact]
        public void TestHasOwnerDriveRightPermission()
        {
            User user = InitUserData();

            Assert.True(user.HasOwnerPermission(driveId: 1));
        }

        [Fact]
        public void TestHasNoOwnerDriveRightPermission()
        {
            User user = InitUserData();

            Assert.False(user.HasOwnerPermission(driveId: 3));

        }
    }

    public class Folder
    {
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public List<Folder> Folders { get; private set; } = [];
        public List<File> Files { get; private set; } = [];

        public void AddFile(File file)
        {
            Files.Add(file);
        }

        public void AddFolder(Folder folder)
        {
            Folders.Add(folder);
        }
    }
    public class File
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
    }

    public class Drive
    {
        public int DriveId { get; internal set; }
        public string DriveName { get; internal set; }
        public List<Folder> Folders { get; private set; } = new List<Folder>();
        public List<File> Files { get; private set; } = [];

        public void AddFile(File file)
        {
            Files.Add(file);
        }

        public void AddFolder(Folder folder)
        {
            Folders.Add(folder);
        }
    }

    internal class User
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Drive> Drives { get; set; } = new List<Drive>();
        public User()
        {
        }

        public void AddDrive(Drive driveInfo)
        {
            Drives.Add(driveInfo);
        }

        public bool HasOwnerPermission(int driveId)
        {
            var userId = this.Id;
            return Drives.Any(e => e.DriveId == driveId);
        }
    }
}