using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using FMInatorul.Controllers;
using FMInatorul.Models;
using FMInatorul.Data;

public class StudentsControllerTest
{
    private readonly StudentsController _controller;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly ApplicationDbContext _mockDbContext;

    public StudentsControllerTest()
    {
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

        _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
            _mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);

        _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

        _mockDbContext = Mock.Of<ApplicationDbContext>(); // Mock the ApplicationDbContext
        

        _controller = new StudentsController(_mockDbContext, _mockUserManager.Object, _mockRoleManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "admin"), 
                    }, "mock")),
                },
            },
        };
    }

    [Fact]
    public async Task UploadPdf_QuizNotNullAsync()
    {
        var fileMock = new Mock<IFormFile>();
        // Set up a stream with some content for testing purposes
        var stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write("Sample PDF Content");
        writer.Flush();
        stream.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.FileName).Returns("sample.pdf");

        // Act
        var result = await _controller.UploadPdf(fileMock.Object) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<QuizModel>(result.Model);
        var quiz = result.Model as QuizModel;
        Assert.NotNull(quiz);

    }

    [Fact]
    public async Task UploadPdf_QuizHasQuestionsAsync()
    {
        var fileMock = new Mock<IFormFile>();
        // Set up a stream with some content for testing purposes
        var stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write("Sample PDF Content");
        writer.Flush();
        stream.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.FileName).Returns("sample.pdf");

        // Act
        var result = await _controller.UploadPdf(fileMock.Object) as ViewResult;
        var quiz = result.Model as QuizModel;

        // Assert
        Assert.NotNull(quiz);
        Assert.NotNull(quiz.Questions);
        Assert.NotEmpty(quiz.Questions);
    }

    [Fact]
    public async void UploadPdf_QuestionsHaveText()
    {
        var fileMock = new Mock<IFormFile>();
        // Set up a stream with some content for testing purposes
        var stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write("Sample PDF Content");
        writer.Flush();
        stream.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.FileName).Returns("sample.pdf");

        // Act
        var result = await _controller.UploadPdf(fileMock.Object) as ViewResult;
        var quiz = result.Model as QuizModel;

        // Assert
        Assert.NotNull(quiz);
        foreach (var question in quiz.Questions)
        {
            Assert.False(string.IsNullOrWhiteSpace(question.Question));
        }
    }

    [Fact]
    public async void UploadPdf_QuestionsHaveAnswers()
    {
        var fileMock = new Mock<IFormFile>();
        // Set up a stream with some content for testing purposes
        var stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write("Sample PDF Content");
        writer.Flush();
        stream.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.FileName).Returns("sample.pdf");

        // Act
        var result = await _controller.UploadPdf(fileMock.Object) as ViewResult;
        var quiz = result.Model as QuizModel;

        // Assert
        Assert.NotNull(quiz);
        foreach (var question in quiz.Questions)
        {
            Assert.NotNull(question.Choices);
            Assert.NotEmpty(question.Choices);
        }
    }

    [Fact]
    public async void UploadPdf_QuestionsHaveCorrectAnswers()
    {
        var fileMock = new Mock<IFormFile>();
        // Set up a stream with some content for testing purposes
        var stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write("Sample PDF Content");
        writer.Flush();
        stream.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.FileName).Returns("sample.pdf");

        // Act
        var result = await _controller.UploadPdf(fileMock.Object) as ViewResult;
        var quiz = result.Model as QuizModel;

        // Assert
        Assert.NotNull(quiz);
        foreach (var question in quiz.Questions)
        {
            Assert.NotNull(question.Answer);
        }
    }
}
