using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

public class RoomHub : Hub
{
    private readonly ApplicationDbContext _context;
    public RoomHub(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task JoinRoomGroup(string roomCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
    }

    public async Task LeaveRoomGroup(string roomCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
    }

    public async Task SendMessage(string roomCode, string userName, string message)
    {
        await Clients.Group(roomCode).SendAsync("ReceiveMessage", userName, message);
    }

    public async Task SendNextQuestion(string roomCode)
    {
        if (!InMemoryGameState.RoomGameStates.TryGetValue(roomCode, out var gameState))
            return;

        var questions = gameState.Questions;
        int currentIndex = gameState.CurrentQuestionIndex;

        // out of questions => end game => leaderboard
        if (currentIndex >= questions.Count)
        {
            await ShowLeaderboard(roomCode);
            return;
        }

        var currentQuestion = questions[currentIndex];
        var payload = new
        {
            QuestionId = currentQuestion.Id,
            Text = currentQuestion.intrebare,
            Variants = currentQuestion.Variante.Select(v => new
            {
                v.Id,
                v.Choice
            }).ToList()
        };

        gameState.UsersAnswered.Clear();

        // send questions with 5 seconds to answer
        await Clients.Group(roomCode).SendAsync("ReceiveQuestion", payload, 5);
    }

    public async Task SubmitAnswer(string roomCode, int questionId, int chosenVariantId)
    {
        if (!InMemoryGameState.RoomGameStates.TryGetValue(roomCode, out var gameState))
            return;

        if (gameState.UsersAnswered.Contains(Context.ConnectionId))
            return;

        gameState.UsersAnswered.Add(Context.ConnectionId);

        var currentIndex = gameState.CurrentQuestionIndex;
        if (currentIndex >= gameState.Questions.Count)
            return;

        var question = gameState.Questions[currentIndex];

        if (question.Id != questionId)
            return;

        var variant = question.Variante.FirstOrDefault(v => v.Id == chosenVariantId);
        if (variant == null)
            return;

        bool isCorrect = (variant.VariantaCorecta == 1);

        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
            return;

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

        if (student == null) return;

        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Code == roomCode);
        if (room == null) return;

        var participant = await _context.Participants
            .FirstOrDefaultAsync(p => p.RoomId == room.RoomId && p.StudentId == student.Id);

        if (participant == null) return;

        if (isCorrect)
        {
            participant.Score += 10;
            await _context.SaveChangesAsync();
        }
    }

    public async Task NextQuestion(string roomCode)
    {
        if (!InMemoryGameState.RoomGameStates.TryGetValue(roomCode, out var gameState))
            return;

        gameState.CurrentQuestionIndex++;

        if (gameState.CurrentQuestionIndex < gameState.Questions.Count)
        {
            await SendNextQuestion(roomCode);
        }
        else
        {
            await ShowLeaderboard(roomCode);
        }
    }

    private async Task ShowLeaderboard(string roomCode)
    {
        var room = await _context.Rooms
            .Include(r => r.Participants)
                .ThenInclude(p => p.Student)
                .ThenInclude(s => s.ApplicationUser)
            .FirstOrDefaultAsync(r => r.Code == roomCode);

        if (room == null) return;

        var leaderboard = room.Participants
            .Select(p => new {
                FullName = p.Student.ApplicationUser.FirstName + " " + p.Student.ApplicationUser.LastName,
                Score = p.Score
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        await Clients.Group(roomCode).SendAsync("ShowLeaderboard", leaderboard);

        if (InMemoryGameState.RoomGameStates.TryGetValue(roomCode, out var gameState))
        {
            gameState.IsGameActive = false;
        }
    }
}
