using FMInatorul.Controllers;
using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Text.Json;

namespace FMInatorul.Tests.SecurityTests
{
    public class RoomsControllerSecurityTests
    {
        private readonly ApplicationDbContext _mockDbContext;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<IHubContext<RoomHub>> _mockRoomHubContext;
        private RoomsController _controller;

        public RoomsControllerSecurityTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            _mockDbContext = new ApplicationDbContext(options);
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null
            );
            _mockRoomHubContext = new Mock<IHubContext<RoomHub>>();

            _controller = new RoomsController(
                _mockDbContext, _mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        }

        private void SetupHttpContextWithUser(string userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(context => context.User).Returns(mockClaimsPrincipal);
            mockHttpContext.Setup(context => context.TraceIdentifier).Returns("test-trace-id");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        // Test: User is unauthorized when trying to join a room
        [Fact]
        public async Task JoinRoom_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
        {
            _mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null);
            var request = new RoomsController.JoinRoomRequest { Code = "123456" };

            // Act
            var result = await _controller.JoinRoom(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);

            // Expected and actual result comparison
            var expectedResult = new { success = false, message = "You must be logged in to join a room" };
            var actualResult = jsonResult.Value;

            Assert.Equal(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(actualResult));
        }

        // Test: User can successfully join a room when authorized
        [Fact]
        public async Task JoinRoom_ShouldReturnSuccess_WhenUserIsAuthorized()
        {
            // Arrange
            await _mockDbContext.Database.OpenConnectionAsync();
            await _mockDbContext.Database.EnsureCreatedAsync();

            var roomCode = "123456";

            // Seed the database with test data
            var facultate = new Facultate
            {
                Id = 1,
                nume = "Test Facultate"
            };
            await _mockDbContext.Facultati.AddAsync(facultate);

            var studentUser = new ApplicationUser
            {
                Id = "student123",
                FirstName = "testStudent",
                LastName = "1",
                IdStud = 1
            };
            await _mockDbContext.Users.AddAsync(studentUser);

            var professorUser = new ApplicationUser
            {
                Id = "professor123",
                FirstName = "testProfessor",
                LastName = "2",
                IdProf = 1
            };
            await _mockDbContext.Users.AddAsync(professorUser);
            await _mockDbContext.SaveChangesAsync();

            var student = new Student
            {
                Id = 1,
                ApplicationUserId = studentUser.Id,
                Year = 1,
                Semester = 1,
                FacultateID = facultate.Id
            };
            await _mockDbContext.Students.AddAsync(student);

            var professor = new Profesor
            {
                Id = 1,
                ApplicationUserId = professorUser.Id,
                FacultateID = facultate.Id
            };
            await _mockDbContext.Professors.AddAsync(professor);

            var materie = new Materie
            {
                Id = 1,
                nume = "Mathematics",
                anStudiu = 1,
                semestru = 1,
                descriere = "Mathematics 101",
                FacultateID = facultate.Id
            };
            await _mockDbContext.Materii.AddAsync(materie);

            var intrebareRasp = new IntrebariRasp
            {
                Id = 1,
                intrebare = "What is 2+2?",
                raspunsCorect = "4",
                validareProfesor = 1,
                MaterieId = materie.Id
            };
            await _mockDbContext.IntrebariRasps.AddAsync(intrebareRasp);

            var varianta = new Variante
            {
                Id = 1,
                Choice = "4",
                IntrebariRaspId = intrebareRasp.Id,
                VariantaCorecta = 1
            };
            await _mockDbContext.Variantes.AddAsync(varianta);
            await _mockDbContext.SaveChangesAsync();
            await _mockDbContext.SaveChangesAsync();

            var room = new Room
            {
                RoomId = 1,
                Code = roomCode,
                Participants = new List<Participant>(),
                MaterieID = materie.Id,
                Materie = materie
            };
            await _mockDbContext.Rooms.AddAsync(room);
            await _mockDbContext.SaveChangesAsync();

            var participant = new Participant
            {
                ParticipantId = 1,
                RoomId = room.RoomId,
                Room = room,
                StudentId = student.Id,
                Student = student
            };
            await _mockDbContext.Participants.AddAsync(participant);
            await _mockDbContext.SaveChangesAsync();


            // Mock UserManager to return userId correctly
            _mockUserManager
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(studentUser.Id);


            // Mock IHubContext
            var mockRoomHubContext = new Mock<IHubContext<RoomHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockGroup = new Mock<IClientProxy>();
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockGroup.Object);
            mockRoomHubContext.Setup(h => h.Clients).Returns(mockClients.Object);

            var controller = new RoomsController(_mockDbContext, mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);



            // Set up the HttpContext with the correct user
            SetupHttpContextWithUser(studentUser.Id);

            var request = new RoomsController.JoinRoomRequest { Code = roomCode };

            // Act
            var result = await controller.JoinRoom(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedResult = new { success = true, message = $"Joined room {roomCode} successfully." };
            Assert.Equal(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(jsonResult.Value));
        }

        // Test: Joining a non-existent room
        [Fact]
        public async Task JoinRoom_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            await _mockDbContext.Database.OpenConnectionAsync();
            await _mockDbContext.Database.EnsureCreatedAsync();

            var roomCode = "123456";

            // Seed the database with test data
            var facultate = new Facultate
            {
                Id = 1,
                nume = "Test Facultate"
            };
            await _mockDbContext.Facultati.AddAsync(facultate);

            var studentUser = new ApplicationUser
            {
                Id = "student123",
                FirstName = "testStudent",
                LastName = "1",
                IdStud = 1
            };
            await _mockDbContext.Users.AddAsync(studentUser);

            var professorUser = new ApplicationUser
            {
                Id = "professor123",
                FirstName = "testProfessor",
                LastName = "2",
                IdProf = 1
            };
            await _mockDbContext.Users.AddAsync(professorUser);
            await _mockDbContext.SaveChangesAsync();

            var student = new Student
            {
                Id = 1,
                ApplicationUserId = studentUser.Id,
                Year = 1,
                Semester = 1,
                FacultateID = facultate.Id
            };
            await _mockDbContext.Students.AddAsync(student);

            var professor = new Profesor
            {
                Id = 1,
                ApplicationUserId = professorUser.Id,
                FacultateID = facultate.Id
            };
            await _mockDbContext.Professors.AddAsync(professor);

            var materie = new Materie
            {
                Id = 1,
                nume = "Mathematics",
                anStudiu = 1,
                semestru = 1,
                descriere = "Mathematics 101",
                FacultateID = facultate.Id
            };
            await _mockDbContext.Materii.AddAsync(materie);

            var intrebareRasp = new IntrebariRasp
            {
                Id = 1,
                intrebare = "What is 2+2?",
                raspunsCorect = "4",
                validareProfesor = 1,
                MaterieId = materie.Id
            };
            await _mockDbContext.IntrebariRasps.AddAsync(intrebareRasp);

            var varianta = new Variante
            {
                Id = 1,
                Choice = "4",
                IntrebariRaspId = intrebareRasp.Id,
                VariantaCorecta = 1
            };
            await _mockDbContext.Variantes.AddAsync(varianta);
            await _mockDbContext.SaveChangesAsync();
            await _mockDbContext.SaveChangesAsync();

            var room = new Room
            {
                RoomId = 1,
                Code = roomCode,
                Participants = new List<Participant>(),
                MaterieID = materie.Id,
                Materie = materie
            };
            await _mockDbContext.Rooms.AddAsync(room);
            await _mockDbContext.SaveChangesAsync();

            var participant = new Participant
            {
                ParticipantId = 1,
                RoomId = room.RoomId,
                Room = room,
                StudentId = student.Id,
                Student = student
            };
            await _mockDbContext.Participants.AddAsync(participant);
            await _mockDbContext.SaveChangesAsync();


            // Mock UserManager to return userId correctly
            _mockUserManager
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(studentUser.Id);


            // Mock IHubContext
            var mockRoomHubContext = new Mock<IHubContext<RoomHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockGroup = new Mock<IClientProxy>();
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockGroup.Object);
            mockRoomHubContext.Setup(h => h.Clients).Returns(mockClients.Object);

            var controller = new RoomsController(_mockDbContext, mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);



            // Set up the HttpContext with the correct user
            SetupHttpContextWithUser(studentUser.Id);
            var request = new RoomsController.JoinRoomRequest { Code = "999999" };

            // Act
            var result = await controller.JoinRoom(request);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedResult = new { success = false, message = "Room not found" };
            Assert.Equal(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(jsonResult.Value));
        }

        // Test: User leaves a room successfully
        [Fact]
        public async Task LeaveRoom_ShouldReturnSuccess_WhenUserIsInRoom()
        {
            // Arrange
            await _mockDbContext.Database.OpenConnectionAsync();
            await _mockDbContext.Database.EnsureCreatedAsync();

            var roomCode = "123456";

            // Seed the database with test data
            var facultate = new Facultate
            {
                Id = 1,
                nume = "Test Facultate"
            };
            await _mockDbContext.Facultati.AddAsync(facultate);

            var studentUser = new ApplicationUser
            {
                Id = "student123",
                FirstName = "testStudent",
                LastName = "1",
                IdStud = 1
            };
            await _mockDbContext.Users.AddAsync(studentUser);

            var professorUser = new ApplicationUser
            {
                Id = "professor123",
                FirstName = "testProfessor",
                LastName = "2",
                IdProf = 1
            };
            await _mockDbContext.Users.AddAsync(professorUser);
            await _mockDbContext.SaveChangesAsync();

            var student = new Student
            {
                Id = 1,
                ApplicationUserId = studentUser.Id,
                Year = 1,
                Semester = 1,
                FacultateID = facultate.Id
            };
            await _mockDbContext.Students.AddAsync(student);

            var professor = new Profesor
            {
                Id = 1,
                ApplicationUserId = professorUser.Id,
                FacultateID = facultate.Id
            };
            await _mockDbContext.Professors.AddAsync(professor);

            var materie = new Materie
            {
                Id = 1,
                nume = "Mathematics",
                anStudiu = 1,
                semestru = 1,
                descriere = "Mathematics 101",
                FacultateID = facultate.Id
            };
            await _mockDbContext.Materii.AddAsync(materie);

            var intrebareRasp = new IntrebariRasp
            {
                Id = 1,
                intrebare = "What is 2+2?",
                raspunsCorect = "4",
                validareProfesor = 1,
                MaterieId = materie.Id
            };
            await _mockDbContext.IntrebariRasps.AddAsync(intrebareRasp);

            var varianta = new Variante
            {
                Id = 1,
                Choice = "4",
                IntrebariRaspId = intrebareRasp.Id,
                VariantaCorecta = 1
            };
            await _mockDbContext.Variantes.AddAsync(varianta);
            await _mockDbContext.SaveChangesAsync();
            await _mockDbContext.SaveChangesAsync();

            var room = new Room
            {
                RoomId = 1,
                Code = roomCode,
                Participants = new List<Participant>(),
                MaterieID = materie.Id,
                Materie = materie
            };
            await _mockDbContext.Rooms.AddAsync(room);
            await _mockDbContext.SaveChangesAsync();

            var participant = new Participant
            {
                ParticipantId = 1,
                RoomId = room.RoomId,
                Room = room,
                StudentId = student.Id,
                Student = student
            };
            await _mockDbContext.Participants.AddAsync(participant);
            await _mockDbContext.SaveChangesAsync();


            // Mock UserManager to return userId correctly
            _mockUserManager
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(studentUser.Id);


            // Mock IHubContext
            var mockRoomHubContext = new Mock<IHubContext<RoomHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockGroup = new Mock<IClientProxy>();
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockGroup.Object);
            mockRoomHubContext.Setup(h => h.Clients).Returns(mockClients.Object);

            var controller = new RoomsController(_mockDbContext, mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

            // Set up the HttpContext with the correct user
            SetupHttpContextWithUser(studentUser.Id);

            // Act
            var result = await controller.LeaveRoom(roomCode);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedResult = new { success = true, message = "You have left the room." };
            Assert.Equal(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(jsonResult.Value));
        }

        // Test: User tries to leave a room they are not in
        [Fact]
        public async Task LeaveRoom_ShouldReturnError_WhenUserIsNotInRoom()
        {
            await _mockDbContext.Database.OpenConnectionAsync();
            await _mockDbContext.Database.EnsureCreatedAsync();

            var roomCode = "123456";

            // Seed the database with test data
            var facultate = new Facultate
            {
                Id = 1,
                nume = "Test Facultate"
            };
            await _mockDbContext.Facultati.AddAsync(facultate);

            var studentUser = new ApplicationUser
            {
                Id = "student123",
                FirstName = "testStudent",
                LastName = "1",
                IdStud = 1
            };
            await _mockDbContext.Users.AddAsync(studentUser);

            var professorUser = new ApplicationUser
            {
                Id = "professor123",
                FirstName = "testProfessor",
                LastName = "2",
                IdProf = 1
            };
            await _mockDbContext.Users.AddAsync(professorUser);
            await _mockDbContext.SaveChangesAsync();

            var student = new Student
            {
                Id = 1,
                ApplicationUserId = studentUser.Id,
                Year = 1,
                Semester = 1,
                FacultateID = facultate.Id
            };
            await _mockDbContext.Students.AddAsync(student);

            var professor = new Profesor
            {
                Id = 1,
                ApplicationUserId = professorUser.Id,
                FacultateID = facultate.Id
            };
            await _mockDbContext.Professors.AddAsync(professor);

            var materie = new Materie
            {
                Id = 1,
                nume = "Mathematics",
                anStudiu = 1,
                semestru = 1,
                descriere = "Mathematics 101",
                FacultateID = facultate.Id
            };
            await _mockDbContext.Materii.AddAsync(materie);

            var intrebareRasp = new IntrebariRasp
            {
                Id = 1,
                intrebare = "What is 2+2?",
                raspunsCorect = "4",
                validareProfesor = 1,
                MaterieId = materie.Id
            };
            await _mockDbContext.IntrebariRasps.AddAsync(intrebareRasp);

            var varianta = new Variante
            {
                Id = 1,
                Choice = "4",
                IntrebariRaspId = intrebareRasp.Id,
                VariantaCorecta = 1
            };
            await _mockDbContext.Variantes.AddAsync(varianta);
            await _mockDbContext.SaveChangesAsync();
            await _mockDbContext.SaveChangesAsync();

            var room = new Room
            {
                RoomId = 1,
                Code = roomCode,
                Participants = new List<Participant>(),
                MaterieID = materie.Id,
                Materie = materie
            };
            await _mockDbContext.Rooms.AddAsync(room);
            await _mockDbContext.SaveChangesAsync();

            var participant = new Participant
            {
                ParticipantId = 1,
                RoomId = room.RoomId,
                Room = room,
                StudentId = student.Id,
                Student = student
            };
            await _mockDbContext.Participants.AddAsync(participant);
            await _mockDbContext.SaveChangesAsync();


            // Mock UserManager to return userId correctly
            _mockUserManager
                .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns(studentUser.Id);


            // Mock IHubContext
            var mockRoomHubContext = new Mock<IHubContext<RoomHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockGroup = new Mock<IClientProxy>();
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockGroup.Object);
            mockRoomHubContext.Setup(h => h.Clients).Returns(mockClients.Object);

            var controller = new RoomsController(_mockDbContext, mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

            // Act
            var result = await controller.LeaveRoom("999999");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var expectedResult = new { success = false, message = "Room not found" };
            Assert.Equal(JsonSerializer.Serialize(expectedResult), JsonSerializer.Serialize(jsonResult.Value));
        }
    }
}
