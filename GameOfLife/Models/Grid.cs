namespace GameOfLife.Models;

// Represents a 2D grid of cells for the Game of Life.
// Manages cell placement, neighbor counting, and grid operations.
public class Grid
{
    private readonly Cell[,] _cells;

    // Gets the width (number of columns) of the grid.
    public int Width { get; }

    // Gets the height (number of rows) of the grid.
    public int Height { get; }

    // Gets the total number of cells in the grid.
    public int TotalCells => Width * Height;

    // Gets the number of currently alive cells in the grid.
    public int AliveCellCount => GetAllCells().Count(cell => cell.IsAlive);

    // Initializes a new instance of the Grid class with all cells dead.
    public Grid(int width, int height)
    {
        if (width < 1) throw new ArgumentException("Width must be at least 1.", nameof(width));
        if (height < 1) throw new ArgumentException("Height must be at least 1.", nameof(height));

        Width = width;
        Height = height;
        _cells = new Cell[height, width];

        InitializeGrid();
    }

    // Gets the cell at the specified coordinates.
    public Cell GetCell(int row, int col)
    {
        ValidateCoordinates(row, col);
        return _cells[row, col];
    }

    // Sets the state of the cell at the specified coordinates.
    public void SetCell(int row, int col, bool isAlive)
    {
        ValidateCoordinates(row, col);
        _cells[row, col].IsAlive = isAlive;
    }

    // Toggles the state of the cell at the specified coordinates.
    public void ToggleCell(int row, int col)
    {
        ValidateCoordinates(row, col);
        _cells[row, col].IsAlive = !_cells[row, col].IsAlive;
    }

    // Counts the number of alive neighbors for the cell at the specified coordinates.
    // Uses 8-connectivity (includes diagonal neighbors).
    public int CountAliveNeighbors(int row, int col)
    {
        ValidateCoordinates(row, col);

        int aliveCount = 0;

        // Check all 8 neighboring positions
        for (int deltaRow = -1; deltaRow <= 1; deltaRow++)
        {
            for (int deltaCol = -1; deltaCol <= 1; deltaCol++)
            {
                // Skip the center cell (the cell itself)
                if (deltaRow == 0 && deltaCol == 0) continue;

                int neighborRow = row + deltaRow;
                int neighborCol = col + deltaCol;

                // Check if neighbor is within bounds
                if (IsWithinBounds(neighborRow, neighborCol))
                {
                    if (_cells[neighborRow, neighborCol].IsAlive)
                    {
                        aliveCount++;
                    }
                }
            }
        }

        return aliveCount;
    }

    // Sets the next state for all cells based on the provided game rules.
    // This calculates what each cell's state should be in the next generation.
    public void CalculateNextGeneration(GameRules rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                var cell = _cells[row, col];
                int neighborCount = CountAliveNeighbors(row, col);
                cell.NextState = rules.ShouldCellBeAlive(cell.IsAlive, neighborCount);
            }
        }
    }

    // Applies the calculated next state to all cells.
    // This should be called after CalculateNextGeneration.
    public void ApplyNextGeneration()
    {
        foreach (var cell in _cells)
        {
            cell.Evolve();
        }
    }

    // Clears the grid by setting all cells to dead.
    public void Clear()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                _cells[row, col].IsAlive = false;
                _cells[row, col].NextState = false;
            }
        }
    }

    // Fills the grid with a random pattern.
    public void RandomizeGrid(double probability = 0.3, int? seed = null)
    {
        if (probability < 0.0 || probability > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(probability),
                "Probability must be between 0.0 and 1.0.");
        }

        var random = seed.HasValue ? new Random(seed.Value) : new Random();

        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                _cells[row, col].IsAlive = random.NextDouble() < probability;
            }
        }
    }

    // Gets all cells in the grid as a flat enumerable.
    public IEnumerable<Cell> GetAllCells()
    {
        return _cells.Cast<Cell>();
    }

    // Gets all cells with their coordinates as a collection of tuples.
    public IEnumerable<(int Row, int Col, Cell Cell)> GetAllCellsWithCoordinates()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                yield return (row, col, _cells[row, col]);
            }
        }
    }

    // Creates a deep copy of the current grid.
    public Grid Clone()
    {
        var clonedGrid = new Grid(Width, Height);

        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                clonedGrid._cells[row, col] = _cells[row, col].Clone();
            }
        }

        return clonedGrid;
    }

    // Determines whether the specified coordinates are within the grid bounds.
    private bool IsWithinBounds(int row, int col)
    {
        return row >= 0 && row < Height && col >= 0 && col < Width;
    }

    // Validates that the specified coordinates are within the grid bounds.
    private void ValidateCoordinates(int row, int col)
    {
        if (!IsWithinBounds(row, col))
        {
            throw new IndexOutOfRangeException(
                $"Coordinates ({row}, {col}) are outside grid bounds. " +
                $"Grid size is {Height} rows × {Width} columns.");
        }
    }

    // Initializes all cells in the grid to dead state.
    private void InitializeGrid()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                _cells[row, col] = new Cell(false);
            }
        }
    }

    // Override Equals for proper comparison - only compares IsAlive states
    public override bool Equals(object? obj)
    {
        if (obj is not Grid other) return false;
        if (Width != other.Width || Height != other.Height) return false;

        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                // Only compare IsAlive state, not NextState
                if (_cells[row, col].IsAlive != other._cells[row, col].IsAlive)
                    return false;
            }
        }

        return true;
    }

    // Override GetHashCode for proper hashing - based on IsAlive states only
    public override int GetHashCode()
    {
        var hash = HashCode.Combine(Width, Height);

        // Hash all alive cell positions for reliable comparison
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                if (_cells[row, col].IsAlive)
                {
                    hash = HashCode.Combine(hash, row, col);
                }
            }
        }

        return hash;
    }

    // Returns a string representation of the grid for debugging
    public override string ToString()
    {
        var result = new System.Text.StringBuilder();
        result.AppendLine($"Grid {Width}×{Height} ({AliveCellCount}/{TotalCells} alive)");

        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                result.Append(_cells[row, col].IsAlive ? "█" : "·");
            }
            result.AppendLine();
        }

        return result.ToString();
    }
}