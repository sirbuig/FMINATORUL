namespace FMInatorul.Models
{
    public class InMemoryGameState
    {
        // roomCode <-> GameState
        public static Dictionary<string, GameState> RoomGameStates = new();
    }
}
