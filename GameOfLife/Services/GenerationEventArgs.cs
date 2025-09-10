using GameOfLife.Models;

namespace GameOfLife.Services;

// Event arguments for generation evolution events in the Game of Life engine.
// Provides information about a completed generation cycle including grid snapshot.
public class GenerationEventArgs : EventArgs
{
    // Gets the generation number that was just completed.
    public int Generation { get; init; }
    public int AliveCells { get; init; }
    public int TotalCells { get; init; }

    // Gets a snapshot (clone) of the grid state after this generation.
    // This is a deep copy to prevent modification of the original grid.
    public Grid Grid { get; init; } = null!;

    // Gets the population density after this generation.
    public double Density => TotalCells > 0 ? (double)AliveCells / TotalCells : 0.0;

    // Gets whether the population went extinct in this generation.
    public bool IsExtinct => AliveCells == 0;

    // Returns a string representation of this generation's results.
    public override string ToString()
    {
        return $"Generation {Generation}: {AliveCells}/{TotalCells} alive ({Density:P2})";
    }
}