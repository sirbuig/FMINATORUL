using FMInatorul.Controllers;
using FMInatorul.Data;
using FMInatorul.Models;
using FMInatorul.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class HomeControllerSecurityTests
{
    private readonly Mock<ApplicationDbContext> _mockDbContext;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<ILogger<HomeController>> _mockLogger;

    public HomeControllerSecurityTests()
    {
        _mockDbContext = new Mock<ApplicationDbContext>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>();
        _mockLogger = new Mock<ILogger<HomeController>>();
    }

    // Helper function to simulate authenticated user
    private void SetupAuthenticatedUser(string userId)
    {
        var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
        mockClaimsPrincipal.Setup(p => p.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, userId));
        _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
    }

    // Helper function to simulate user being an Admin
    private void SetupAdminUser(string userId)
    {
        SetupAuthenticatedUser(userId);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var mockClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
    }

    // Test if the Index action returns the view correctly
    [Fact]
    public void Index_ReturnsViewResult_WhenCalled()
    {
        // Arrange
        var controller = new HomeController(_mockDbContext.Object, _mockLogger.Object, _mockUserManager.Object);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName); // Assuming the default view is named "Index"
    }

    // Test if IndexNew returns the view correctly
    [Fact]
    public void IndexNew_ReturnsViewResult_WhenCalled()
    {
        // Arrange
        var controller = new HomeController(_mockDbContext.Object, _mockLogger.Object, _mockUserManager.Object);

        // Act
        var result = controller.IndexNew();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("IndexNew", viewResult.ViewName); // Assuming the default view is named "IndexNew"
    }

    // Test if Privacy returns the view correctly
    [Fact]
    public void Privacy_ReturnsViewResult_WhenCalled()
    {
        // Arrange
        var controller = new HomeController(_mockDbContext.Object, _mockLogger.Object, _mockUserManager.Object);

        // Act
        var result = controller.Privacy();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Privacy", viewResult.ViewName); // Assuming the default view is named "Privacy"
    }

    // Test if error handling works correctly
    [Fact]
    public void Error_ReturnsViewResult_WithErrorDetails()
    {
        // Arrange
        var controller = new HomeController(_mockDbContext.Object, _mockLogger.Object, _mockUserManager.Object);

        // Act
        var result = controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model);
        var errorViewModel = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(errorViewModel.RequestId); // Check that RequestId is set
    }

    // Test if non-admin users are redirected when accessing the Admin page
    [Fact]
    public void Admin_ReturnsRedirect_WhenUserIsNotAdmin()
    {
        // Arrange
        var userId = "user123";
        SetupAuthenticatedUser(userId);
        var controller = new HomeController(_mockDbContext.Object, _mockLogger.Object, _mockUserManager.Object);

        // Act
        var result = controller.Admin();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName); // Should redirect to "Index"
    }

    // Test if admin users can access the Admin page
    [Fact]
    public void Admin_ReturnsView_WhenUserIsAdmin()
    {
        // Arrange
        var userId = "admin123";
        SetupAdminUser(userId);
        var controller = new HomeController(_mockDbContext.Object, _mockLogger.Object, _mockUserManager.Object);

        // Mock Materii data
        var materii = new List<Materie> { new Materie { Id = 1, nume = "Math", anStudiu = 1, descriere = "We like math.", FacultateID = 1, semestru = 2 } }.AsQueryable();
        var mockDbSet = new Mock<DbSet<Materie>>();
        mockDbSet.As<IQueryable<Materie>>().Setup(m => m.Provider).Returns(materii.Provider);
        mockDbSet.As<IQueryable<Materie>>().Setup(m => m.Expression).Returns(materii.Expression);
        mockDbSet.As<IQueryable<Materie>>().Setup(m => m.ElementType).Returns(materii.ElementType);
        mockDbSet.As<IQueryable<Materie>>().Setup(m => m.GetEnumerator()).Returns(materii.GetEnumerator());

        _mockDbContext.Setup(c => c.Materii).Returns(mockDbSet.Object);

        // Act
        var result = controller.Admin();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewBagMaterii = viewResult.ViewData["materii"];
        Assert.NotNull(viewBagMaterii);
        Assert.IsType<List<Materie>>(viewBagMaterii);
    }

    // Test if file upload is handled correctly by validating PDF file
    [Fact]
    public async Task Add_Questions_FileUpload_ReturnsBadRequest_WhenFileIsInvalid()
    {
        // Arrange
        var controller = new HomeController(_mockDbContext.Object, _mockLogger.Object, _mockUserManager.Object);
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.ContentType).Returns("application/zip"); // Invalid content type
        var file = fileMock.Object;

        // Act
        var result = await controller.Add_Questions(file);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid file format. Only PDF files allowed!", badRequestResult.Value);
    }

    // Test if file upload is handled correctly with PDF
    [Fact]
    public async Task Add_Questions_FileUpload_ReturnsSuccess_WhenFileIsPdf()
    {
        // Arrange
        var controller = new HomeController(_mockDbContext.Object, _mockLogger.Object, _mockUserManager.Object);
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.ContentType).Returns("application/pdf"); // Valid content type
        var file = fileMock.Object;

        // Simulate a valid API response 
        var mockResponse = new Mock<HttpResponseMessage>();
        mockResponse.Setup(m => m.IsSuccessStatusCode).Returns(true);
        mockResponse.Setup(m => m.Content).Returns(new StringContent(JsonConvert.SerializeObject(new { Questions = new List<object>() })));

        // Act
        var result = await controller.Add_Questions(file);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Admin", redirectResult.ActionName);
    }
}
