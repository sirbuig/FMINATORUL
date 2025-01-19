using FMInatorul.Controllers;
using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using Xunit;

public class ProfessorsControllerSecurityTests
{
    private readonly Mock<ApplicationDbContext> _mockDbContext;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;

    public ProfessorsControllerSecurityTests()
    {
        _mockDbContext = new Mock<ApplicationDbContext>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>();
        _mockRoleManager = new Mock<RoleManager<IdentityRole>>();
    }

    private void SetupAuthenticatedUser(string userId)
    {
        var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
        mockClaimsPrincipal.Setup(p => p.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, userId));
        _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
    }

    private void SetupProfessor(string userId)
    {
        var professor = new Profesor
        {
            Id = 1,
            ApplicationUserId = userId,
            CompletedProfile = true
        };

        var professors = new List<Profesor> { professor }.AsQueryable();
        var mockDbSet = new Mock<DbSet<Profesor>>();
        mockDbSet.As<IQueryable<Profesor>>().Setup(m => m.Provider).Returns(professors.Provider);
        mockDbSet.As<IQueryable<Profesor>>().Setup(m => m.Expression).Returns(professors.Expression);
        mockDbSet.As<IQueryable<Profesor>>().Setup(m => m.ElementType).Returns(professors.ElementType);
        mockDbSet.As<IQueryable<Profesor>>().Setup(m => m.GetEnumerator()).Returns(professors.GetEnumerator());

        _mockDbContext.Setup(c => c.Professors).Returns(mockDbSet.Object);
    }

    // Test if professor is redirected if profile is not completed
    [Fact]
    public async Task Index_ShouldRedirect_WhenProfileNotCompleted()
    {
        // Arrange
        var userId = "user123";
        SetupAuthenticatedUser(userId);
        SetupProfessor(userId);

        var controller = new ProfessorsController(_mockDbContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // Act
        var result = await controller.Index();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(101, objectResult.StatusCode);
    }

    // Test if professor can successfully edit subjects
    [Fact]
    public async Task EditMaterie_ShouldReturnView_WhenUserIsAuthorized()
    {
        // Arrange
        var userId = "user123";
        SetupAuthenticatedUser(userId);
        SetupProfessor(userId);

        var controller = new ProfessorsController(_mockDbContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        // Act
        var result = await controller.EditMaterie();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(101, viewResult.StatusCode);
    }

    // Test if the professor is able to save changes after editing subject
    [Fact]
    public async Task EditMaterie_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        var userId = "user123";
        SetupAuthenticatedUser(userId);
        SetupProfessor(userId);

        var controller = new ProfessorsController(_mockDbContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        var professor = new Profesor { ApplicationUserId = userId, CompletedProfile = true, SelectedMateriiIds = new List<int> { 1 } };

        // Act
        var result = await controller.EditMaterie(professor);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(101, objectResult.StatusCode);
    }

    // Test if the professor can validate a question
    [Fact]
    public async Task Valideaza_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        var userId = "user123";
        SetupAuthenticatedUser(userId);
        SetupProfessor(userId);

        var controller = new ProfessorsController(_mockDbContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        var question = new IntrebariRasp { Id = 1, intrebare = "What is 2 + 2?" };
        _mockDbContext.Setup(c => c.IntrebariRasps.FindAsync(It.IsAny<int>())).ReturnsAsync(question);

        // Act
        var result = await controller.Valideaza(1);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(101, objectResult.StatusCode);
    }

    // Test if the professor can delete a question
    [Fact]
    public async Task NuValideaza_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange
        var userId = "user123";
        SetupAuthenticatedUser(userId);
        SetupProfessor(userId);

        var controller = new ProfessorsController(_mockDbContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        var question = new IntrebariRasp { Id = 1, intrebare = "What is 2 + 2?" };
        _mockDbContext.Setup(c => c.IntrebariRasps.FindAsync(It.IsAny<int>())).ReturnsAsync(question);

        // Act
        var result = await controller.NuValideaza(1);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.NotNull(objectResult);
        Assert.Equal(101, objectResult.StatusCode);
    }
}
