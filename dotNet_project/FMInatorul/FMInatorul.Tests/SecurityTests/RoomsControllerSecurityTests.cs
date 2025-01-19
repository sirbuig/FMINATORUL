using FMInatorul.Controllers;
using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

public class RoomsControllerSecurityTests
{
    private readonly Mock<ApplicationDbContext> _mockDbContext;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly Mock<IHubContext<RoomHub>> _mockRoomHubContext;

    public RoomsControllerSecurityTests()
    {
        _mockDbContext = new Mock<ApplicationDbContext>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>();
        _mockRoleManager = new Mock<RoleManager<IdentityRole>>();
        _mockRoomHubContext = new Mock<IHubContext<RoomHub>>();
    }

    // Helper function to simulate authenticated user
    private void SetupAuthenticatedUser(string userId)
    {
        var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
        mockClaimsPrincipal.Setup(p => p.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, userId));
        _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
    }

    // Helper function to simulate a student in the database
    private void SetupStudent(string userId)
    {
        var student = new Student
        {
            Id = 1,
            ApplicationUserId = userId,
            ApplicationUser = new ApplicationUser
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe"
            }
        };

        var students = new List<Student> { student }.AsQueryable();
        var mockDbSet = new Mock<DbSet<Student>>();
        mockDbSet.As<IQueryable<Student>>().Setup(m => m.Provider).Returns(students.Provider);
        mockDbSet.As<IQueryable<Student>>().Setup(m => m.Expression).Returns(students.Expression);
        mockDbSet.As<IQueryable<Student>>().Setup(m => m.ElementType).Returns(students.ElementType);
        mockDbSet.As<IQueryable<Student>>().Setup(m => m.GetEnumerator()).Returns(students.GetEnumerator());

        _mockDbContext.Setup(c => c.Students).Returns(mockDbSet.Object);
    }

    // Setup Rooms DbSet
    private void SetupRoomsDbSet()
    {
        var rooms = new List<Room>
        {
            new Room { RoomId = 1, Code = "123456" },
            new Room { RoomId = 2, Code = "654321" }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Room>>();
        mockDbSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
        mockDbSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
        mockDbSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
        mockDbSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());

        _mockDbContext.Setup(c => c.Rooms).Returns(mockDbSet.Object);
    }

    // Setup Participants DbSet
    private void SetupParticipantsDbSet()
    {
        var participants = new List<Participant>
        {
            new Participant { RoomId = 1, StudentId = 1 }
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Participant>>();
        mockDbSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
        mockDbSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
        mockDbSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
        mockDbSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

        _mockDbContext.Setup(c => c.Participants).Returns(mockDbSet.Object);
    }

    // Setup for CreateRoom (valid scenario)
    private void SetupCreateRoomRequest(string userId, string roomCode)
    {
        SetupAuthenticatedUser(userId);
        SetupStudent(userId);
        SetupRoomsDbSet();
    }

    // Test if user is unauthorized when trying to join room
    [Fact]
    public async Task JoinRoom_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        var controller = new RoomsController(_mockDbContext.Object, _mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        _mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null); // Simulate no user logged in

        var request = new RoomsController.JoinRoomRequest { Code = "123456" };

        // Act
        var result = await controller.JoinRoom(request);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal("{\"success\":false,\"message\":\"You must be logged in to join a room\"}", jsonResult.Value.ToString());
    }

    // Test if user can successfully join room
    [Fact]
    public async Task JoinRoom_ShouldReturnSuccess_WhenUserIsAuthorized()
    {
        // Arrange
        var userId = "user123";
        SetupCreateRoomRequest(userId, "123456");
        var controller = new RoomsController(_mockDbContext.Object, _mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        var request = new RoomsController.JoinRoomRequest { Code = "123456" };

        // Act
        var result = await controller.JoinRoom(request);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal("{\"success\":true,\"message\":\"Joined room 123456 successfully.\"}", jsonResult.Value.ToString());
    }

    // Test if user tries to join a room that doesn't exist
    [Fact]
    public async Task JoinRoom_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        // Arrange
        var userId = "user123";
        SetupCreateRoomRequest(userId, "123456");
        var controller = new RoomsController(_mockDbContext.Object, _mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        var request = new RoomsController.JoinRoomRequest { Code = "999999" };

        // Act
        var result = await controller.JoinRoom(request);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal("{\"success\":false,\"message\":\"Room not found\"}", jsonResult.Value.ToString());
    }

    // Test if user can create a room successfully
    [Fact]
    public async Task CreateRoom_ShouldReturnSuccess_WhenUserIsAuthorized()
    {
        // Arrange
        var userId = "user123";
        SetupCreateRoomRequest(userId, "123456");
        var controller = new RoomsController(_mockDbContext.Object, _mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        var request = new RoomsController.JoinRoomRequest { Code = "101" };

        // Act
        var result = await controller.CreateRoom(request);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var code = jsonResult.Value.ToString().Contains("code");
        Assert.True(code);
    }

    // Test if user is unable to create room without login
    [Fact]
    public async Task CreateRoom_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        var controller = new RoomsController(_mockDbContext.Object, _mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        _mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null); // Simulate no user logged in
        var request = new RoomsController.JoinRoomRequest { Code = "101" };

        // Act
        var result = await controller.CreateRoom(request);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal("{\"success\":false,\"message\":\"You must be logged in to create a room\"}", jsonResult.Value.ToString());
    }

    // Test if user can leave the room successfully
    [Fact]
    public async Task LeaveRoom_ShouldReturnSuccess_WhenUserIsInRoom()
    {
        // Arrange
        var userId = "user123";
        SetupCreateRoomRequest(userId, "123456");
        var controller = new RoomsController(_mockDbContext.Object, _mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        var code = "123456";

        // Act
        var result = await controller.LeaveRoom(code);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal("{\"success\":true,\"message\":\"You have left the room.\"}", jsonResult.Value.ToString());
    }

    // Test if user tries to leave a room they are not in
    [Fact]
    public async Task LeaveRoom_ShouldReturnError_WhenUserIsNotInRoom()
    {
        // Arrange
        var userId = "user123";
        SetupCreateRoomRequest(userId, "654321");
        var controller = new RoomsController(_mockDbContext.Object, _mockRoomHubContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        var code = "123456";

        // Act
        var result = await controller.LeaveRoom(code);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal("{\"success\":false,\"message\":\"You are not in this room\"}", jsonResult.Value.ToString());
    }
}
