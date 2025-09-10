namespace GameOfLife.Services;

// Represents comprehensive statistics about the current state of a Game of Life simulation.
// Provides both current state information and configuration details.
public class GameStatistics
{
    public int Generation { get; init; }
    public int AliveCells { get; init; }
    public int TotalCells { get; init; }
    
    // Gets the population density as a ratio (AliveCells / TotalCells).
    public double Density { get; init; }
    public int GridWidth { get; init; }
    public int GridHeight { get; init; }

    // Gets the current game rules in B/S notation (e.g., "B3/S23").
    public string Rules { get; init; } = string.Empty;

    // Gets the current evolution speed in milliseconds between generations.
    public int EvolutionSpeed { get; init; }

    // Gets whether the game engine is currently running.
    public bool IsRunning { get; init; }

    // Gets whether the game engine is currently paused.
    public bool IsPaused { get; init; }

    // Gets whether the game is actively evolving (running and not paused).
    public bool IsActive => IsRunning && !IsPaused;

    // Gets the percentage of alive cells as a formatted string.
    public string DensityPercentage => $"{Density:P2}";

    // Returns a formatted string representation of the current statistics.
    public override string ToString()
    {
        var status = IsRunning ? (IsPaused ? "Paused" : "Running") : "Stopped";
        return $"Gen: {Generation} | Alive: {AliveCells}/{TotalCells} ({DensityPercentage}) | " +
               $"Rules: {Rules} | Speed: {EvolutionSpeed}ms | Status: {status}";
    }
}