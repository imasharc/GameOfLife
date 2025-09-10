namespace GameOfLife.Services;

// Enumeration of possible game states for the Game of Life engine.
public enum GameState
{
    Started,
    Paused,
    Resumed,
    Stopped,
    Reset,
    GridChanged,
    RulesChanged,
    SpeedChanged,
    Extinct
}