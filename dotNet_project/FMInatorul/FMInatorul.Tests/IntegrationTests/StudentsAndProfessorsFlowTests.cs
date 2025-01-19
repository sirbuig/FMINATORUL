using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using FMInatorul.Controllers;
using FMInatorul.Models;
using FMInatorul.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Linq.Expressions;
using System.Security.Claims;

namespace FMInatorul.Tests.IntegrationTests
{
    public class StudentRoleTests
    {
        private StudentsController _controller;
        private ProfessorsController _controllerProfessor;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private ApplicationDbContext _mockDbContext;
        private UserManager<IdentityUser> _userManager;
        private Mock<UserManager<IdentityUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }

        private Mock<SignInManager<IdentityUser>> GetMockSignInManager()
        {
            var userManager = GetMockUserManager();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            return new Mock<SignInManager<IdentityUser>>(
                userManager.Object, httpContextAccessor.Object, userClaimsPrincipalFactory.Object, null, null, null, null
            );
        }

        private Mock<ApplicationDbContext> GetMockDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureDeleted(); // Clear the database before each test
            dbContext.Database.EnsureCreated();

            return new Mock<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task Student_UploadPdf_ShouldSaveFileAndGenerateQuiz()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);

            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            // Initialize controller with mock dependencies and mocked user context
            _controller = new StudentsController(_mockDbContext, _mockUserManager.Object, _mockRoleManager.Object);

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.pdf");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3, 4 }));

            // Act
            var result = await _controller.UploadPdf(mockFile.Object);

            // Assert
            Assert.IsType<OkResult>(result); 
        }

        [Fact]
        public async Task EditCollegeProf_UserIsStudent_ReturnsForbidResult()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var studentUser = new ApplicationUser { Id = "student123", UserName = "testStudent" };

            // Mock user retrieval (pretend that the current user is a Student)
            userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(studentUser);

            _controllerProfessor = new ProfessorsController(_mockDbContext, _mockUserManager.Object, _mockRoleManager.Object);

            var mockStudent = new Profesor
            {
                ApplicationUserId = studentUser.Id,
                FacultateID = 1
            };

            // Act
            var result = _controllerProfessor.EditCollegeProf(1, mockStudent);

            // Assert
            Assert.IsType<ForbidResult>(result); // ForbidResult is returned
        }

    }
}
