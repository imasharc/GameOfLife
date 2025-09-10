namespace GameOfLife.Services;

// Event arguments for game state change events in the Game of Life engine.
// Provides information about state transitions and current game statistics.
public class GameStateEventArgs : EventArgs
{
    // Gets the new game state that was transitioned to.
    public GameState State { get; init; }

    // Gets the current game statistics at the time of the state change.
    public GameStatistics Statistics { get; init; } = null!;

    // Gets a timestamp when this state change occurred.
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    // Gets a human-readable description of the state change.
    public string Description => State switch
    {
        GameState.Started => "Game simulation started",
        GameState.Paused => "Game simulation paused",
        GameState.Resumed => "Game simulation resumed",
        GameState.Stopped => "Game simulation stopped",
        GameState.Reset => "Game state reset to initial conditions",
        GameState.GridChanged => "Grid configuration changed",
        GameState.RulesChanged => "Game rules modified",
        GameState.SpeedChanged => "Evolution speed adjusted",
        GameState.Extinct => "Population extinct - all cells dead",
        _ => $"Unknown state: {State}"
    };

    // Returns a string representation of this state change event.
    public override string ToString()
    {
        return $"{Timestamp:HH:mm:ss} - {Description}";
    }
}