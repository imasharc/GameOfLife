using GameOfLife.Models;

namespace GameOfLife.Tests.Models;

public class GridTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidDimensions_ShouldCreateCorrectGrid()
    {
        // ARRANGE & ACT
        var grid = new Grid(10, 5);

        // ASSERT
        Assert.Equal(10, grid.Width);
        Assert.Equal(5, grid.Height);
        Assert.Equal(50, grid.TotalCells);
        Assert.Equal(0, grid.AliveCellCount);
    }

    [Fact]
    public void Constructor_WithMinimumDimensions_ShouldCreateCorrectGrid()
    {
        // ARRANGE & ACT
        var grid = new Grid(1, 1);

        // ASSERT
        Assert.Equal(1, grid.Width);
        Assert.Equal(1, grid.Height);
        Assert.Equal(1, grid.TotalCells);
        Assert.False(grid.GetCell(0, 0).IsAlive);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(-1, 5)]
    [InlineData(5, 0)]
    [InlineData(5, -1)]
    [InlineData(0, 0)]
    public void Constructor_WithInvalidDimensions_ShouldThrowArgumentException(int width, int height)
    {
        // ACT & ASSERT
        Assert.Throws<ArgumentException>(() => new Grid(width, height));
    }

    [Fact]
    public void Constructor_ShouldInitializeAllCellsAsDead()
    {
        // ARRANGE & ACT
        var grid = new Grid(3, 3);

        // ASSERT
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Assert.False(grid.GetCell(row, col).IsAlive);
            }
        }
    }

    #endregion

    #region Cell Access Tests

    [Fact]
    public void GetCell_WithValidCoordinates_ShouldReturnCorrectCell()
    {
        // ARRANGE
        var grid = new Grid(5, 5);

        // ACT
        var cell = grid.GetCell(2, 3);

        // ASSERT
        Assert.NotNull(cell);
        Assert.False(cell.IsAlive);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(5, 0)]
    [InlineData(0, 5)]
    [InlineData(5, 5)]
    public void GetCell_WithInvalidCoordinates_ShouldThrowIndexOutOfRangeException(int row, int col)
    {
        // ARRANGE
        var grid = new Grid(5, 5);

        // ACT & ASSERT
        var exception = Assert.Throws<IndexOutOfRangeException>(() => grid.GetCell(row, col));
        Assert.Contains($"({row}, {col})", exception.Message);
        Assert.Contains("5 rows × 5 columns", exception.Message);
    }

    [Fact]
    public void SetCell_WithValidCoordinates_ShouldUpdateCellState()
    {
        // ARRANGE
        var grid = new Grid(3, 3);

        // ACT
        grid.SetCell(1, 1, true);

        // ASSERT
        Assert.True(grid.GetCell(1, 1).IsAlive);
        Assert.Equal(1, grid.AliveCellCount);
    }

    [Fact]
    public void SetCell_WithInvalidCoordinates_ShouldThrowIndexOutOfRangeException()
    {
        // ARRANGE
        var grid = new Grid(3, 3);

        // ACT & ASSERT
        Assert.Throws<IndexOutOfRangeException>(() => grid.SetCell(3, 0, true));
    }

    [Fact]
    public void ToggleCell_ShouldChangeAliveState()
    {
        // ARRANGE
        var grid = new Grid(3, 3);
        Assert.False(grid.GetCell(1, 1).IsAlive);

        // ACT - Toggle from dead to alive
        grid.ToggleCell(1, 1);

        // ASSERT
        Assert.True(grid.GetCell(1, 1).IsAlive);

        // ACT - Toggle from alive to dead
        grid.ToggleCell(1, 1);

        // ASSERT
        Assert.False(grid.GetCell(1, 1).IsAlive);
    }

    #endregion

    #region Neighbor Counting Tests

    [Fact]
    public void CountAliveNeighbors_WithNoAliveNeighbors_ShouldReturnZero()
    {
        // ARRANGE - All cells dead by default
        var grid = new Grid(3, 3);

        // ACT
        var neighborCount = grid.CountAliveNeighbors(1, 1);

        // ASSERT
        Assert.Equal(0, neighborCount);
    }

    [Fact]
    public void CountAliveNeighbors_WithAllAliveNeighbors_ShouldReturnEight()
    {
        // ARRANGE - Create 3x3 grid with center cell surrounded by alive cells
        var grid = new Grid(3, 3);

        // Set all neighbors alive (but not center cell)
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (row != 1 || col != 1) // Skip center cell
                {
                    grid.SetCell(row, col, true);
                }
            }
        }

        // ACT
        var neighborCount = grid.CountAliveNeighbors(1, 1);

        // ASSERT
        Assert.Equal(8, neighborCount);
    }

    [Theory]
    [InlineData(0, 0)] // Top-left corner
    [InlineData(0, 2)] // Top-right corner
    [InlineData(2, 0)] // Bottom-left corner
    [InlineData(2, 2)] // Bottom-right corner
    public void CountAliveNeighbors_AtCorners_ShouldCountOnlyValidNeighbors(int row, int col)
    {
        // ARRANGE - 3x3 grid with all cells alive
        var grid = new Grid(3, 3);
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                grid.SetCell(r, c, true);
            }
        }

        // ACT
        var neighborCount = grid.CountAliveNeighbors(row, col);

        // ASSERT - Corner cells should have exactly 3 neighbors
        Assert.Equal(3, neighborCount);
    }

    [Theory]
    [InlineData(0, 1)] // Top edge
    [InlineData(1, 0)] // Left edge
    [InlineData(1, 2)] // Right edge
    [InlineData(2, 1)] // Bottom edge
    public void CountAliveNeighbors_AtEdges_ShouldCountOnlyValidNeighbors(int row, int col)
    {
        // ARRANGE - 3x3 grid with all cells alive
        var grid = new Grid(3, 3);
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                grid.SetCell(r, c, true);
            }
        }

        // ACT
        var neighborCount = grid.CountAliveNeighbors(row, col);

        // ASSERT - Edge cells should have exactly 5 neighbors
        Assert.Equal(5, neighborCount);
    }

    [Fact]
    public void CountAliveNeighbors_WithSpecificPattern_ShouldCountCorrectly()
    {
        // ARRANGE - Create L-shaped pattern
        var grid = new Grid(4, 4);
        grid.SetCell(0, 1, true); // Above
        grid.SetCell(1, 0, true); // Left
        grid.SetCell(1, 2, true); // Right
        grid.SetCell(2, 1, true); // Below
        grid.SetCell(2, 2, true); // Bottom-right diagonal

        // ACT
        var neighborCount = grid.CountAliveNeighbors(1, 1);

        // ASSERT
        Assert.Equal(5, neighborCount);
    }

    [Fact]
    public void CountAliveNeighbors_WithSingleCell_ShouldReturnZero()
    {
        // ARRANGE
        var grid = new Grid(1, 1);

        // ACT
        var neighborCount = grid.CountAliveNeighbors(0, 0);

        // ASSERT
        Assert.Equal(0, neighborCount);
    }

    #endregion

    #region Evolution Tests

    [Fact]
    public void CalculateNextGeneration_WithConwayRules_ShouldCalculateCorrectly()
    {
        // ARRANGE - Create blinker pattern (oscillates)
        var grid = new Grid(5, 5);
        grid.SetCell(2, 1, true); // Horizontal line
        grid.SetCell(2, 2, true);
        grid.SetCell(2, 3, true);
        var rules = new GameRules(); // Conway's rules

        // ACT
        grid.CalculateNextGeneration(rules);

        // ASSERT - Check NextState values (before applying)
        Assert.True(grid.GetCell(1, 2).NextState); // Should become alive
        Assert.True(grid.GetCell(2, 2).NextState); // Should stay alive
        Assert.True(grid.GetCell(3, 2).NextState); // Should become alive
        Assert.False(grid.GetCell(2, 1).NextState); // Should die
        Assert.False(grid.GetCell(2, 3).NextState); // Should die
    }

    [Fact]
    public void CalculateNextGeneration_WithNullRules_ShouldThrowArgumentNullException()
    {
        // ARRANGE
        var grid = new Grid(3, 3);

        // ACT & ASSERT
        Assert.Throws<ArgumentNullException>(() => grid.CalculateNextGeneration(null!));
    }

    [Fact]
    public void ApplyNextGeneration_ShouldApplyCalculatedStates()
    {
        // ARRANGE - Create blinker pattern
        var grid = new Grid(5, 5);
        grid.SetCell(2, 1, true);
        grid.SetCell(2, 2, true);
        grid.SetCell(2, 3, true);
        var rules = new GameRules();

        // ACT
        grid.CalculateNextGeneration(rules);
        grid.ApplyNextGeneration();

        // ASSERT - Pattern should have rotated 90 degrees
        Assert.False(grid.GetCell(2, 1).IsAlive);
        Assert.True(grid.GetCell(1, 2).IsAlive);  // Vertical line
        Assert.True(grid.GetCell(2, 2).IsAlive);
        Assert.True(grid.GetCell(3, 2).IsAlive);
        Assert.False(grid.GetCell(2, 3).IsAlive);
    }

    [Fact]
    public void EvolutionCycle_BlinkerPattern_ShouldOscillate()
    {
        // ARRANGE - Create horizontal blinker
        var grid = new Grid(5, 5);
        grid.SetCell(2, 1, true);
        grid.SetCell(2, 2, true);
        grid.SetCell(2, 3, true);
        var rules = new GameRules();

        // Store original state
        var originalState = grid.Clone();

        // ACT - Evolve one generation (should become vertical)
        grid.CalculateNextGeneration(rules);
        grid.ApplyNextGeneration();

        // ASSERT - Should be vertical now
        Assert.True(grid.GetCell(1, 2).IsAlive);
        Assert.True(grid.GetCell(2, 2).IsAlive);
        Assert.True(grid.GetCell(3, 2).IsAlive);
        Assert.Equal(3, grid.AliveCellCount);

        // ACT - Evolve second generation (should return to horizontal)
        grid.CalculateNextGeneration(rules);
        grid.ApplyNextGeneration();

        // ASSERT - Should be back to original horizontal state
        Assert.True(grid.Equals(originalState));
    }

    #endregion

    #region Utility Method Tests

    [Fact]
    public void Clear_ShouldSetAllCellsToDead()
    {
        // ARRANGE
        var grid = new Grid(3, 3);
        grid.SetCell(0, 0, true);
        grid.SetCell(1, 1, true);
        grid.SetCell(2, 2, true);
        Assert.Equal(3, grid.AliveCellCount);

        // ACT
        grid.Clear();

        // ASSERT
        Assert.Equal(0, grid.AliveCellCount);
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Assert.False(grid.GetCell(row, col).IsAlive);
                Assert.False(grid.GetCell(row, col).NextState);
            }
        }
    }

    [Fact]
    public void RandomizeGrid_WithSeed_ShouldProduceDeterministicResults()
    {
        // ARRANGE
        var grid1 = new Grid(10, 10);
        var grid2 = new Grid(10, 10);
        const int seed = 12345;

        // ACT
        grid1.RandomizeGrid(0.5, seed);
        grid2.RandomizeGrid(0.5, seed);

        // ASSERT
        Assert.True(grid1.Equals(grid2));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(2.0)]
    public void RandomizeGrid_WithInvalidProbability_ShouldThrowArgumentOutOfRangeException(double invalidProbability)
    {
        // ARRANGE
        var grid = new Grid(3, 3);

        // ACT & ASSERT
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            grid.RandomizeGrid(invalidProbability));
        Assert.Equal("probability", exception.ParamName);
    }

    [Theory]
    [InlineData(0.0)] // Should produce no alive cells
    [InlineData(1.0)] // Should produce all alive cells
    public void RandomizeGrid_WithExtremeProbabilities_ShouldWorkCorrectly(double probability)
    {
        // ARRANGE
        var grid = new Grid(5, 5);

        // ACT
        grid.RandomizeGrid(probability, 12345);

        // ASSERT
        var expectedAliveCells = probability == 0.0 ? 0 : 25;
        Assert.Equal(expectedAliveCells, grid.AliveCellCount);
    }

    [Fact]
    public void Clone_ShouldCreateIdenticalCopy()
    {
        // ARRANGE
        var original = new Grid(3, 3);
        original.SetCell(0, 0, true);
        original.SetCell(1, 1, true);
        original.SetCell(2, 2, true);

        // ACT
        var clone = original.Clone();

        // ASSERT
        Assert.NotSame(original, clone);
        Assert.True(original.Equals(clone));
        Assert.Equal(original.Width, clone.Width);
        Assert.Equal(original.Height, clone.Height);
        Assert.Equal(original.AliveCellCount, clone.AliveCellCount);
    }

    [Fact]
    public void Clone_ModifyingClone_ShouldNotAffectOriginal()
    {
        // ARRANGE
        var original = new Grid(3, 3);
        original.SetCell(1, 1, true);
        var clone = original.Clone();

        // ACT
        clone.SetCell(0, 0, true);
        clone.SetCell(1, 1, false);

        // ASSERT
        Assert.True(original.GetCell(1, 1).IsAlive);
        Assert.False(original.GetCell(0, 0).IsAlive);
        Assert.True(clone.GetCell(0, 0).IsAlive);
        Assert.False(clone.GetCell(1, 1).IsAlive);
    }

    #endregion

    #region Collection Methods Tests

    [Fact]
    public void GetAllCells_ShouldReturnAllCells()
    {
        // ARRANGE - Fixed coordinates: 3×2 grid means 3 width, 2 height
        // Valid coordinates: (0,0), (0,1), (0,2), (1,0), (1,1), (1,2)
        var grid = new Grid(3, 2);
        grid.SetCell(0, 0, true);
        grid.SetCell(1, 2, true);

        // ACT
        var allCells = grid.GetAllCells().ToList();

        // ASSERT
        Assert.Equal(6, allCells.Count);
        Assert.Equal(2, allCells.Count(cell => cell.IsAlive));
        Assert.Equal(4, allCells.Count(cell => !cell.IsAlive));
    }

    [Fact]
    public void GetAllCellsWithCoordinates_ShouldReturnCellsWithCorrectCoordinates()
    {
        // ARRANGE
        var grid = new Grid(2, 2);
        grid.SetCell(1, 0, true);

        // ACT
        var cellsWithCoords = grid.GetAllCellsWithCoordinates().ToList();

        // ASSERT
        Assert.Equal(4, cellsWithCoords.Count);

        var aliveCell = cellsWithCoords.Single(item => item.Cell.IsAlive);
        Assert.Equal(1, aliveCell.Row);
        Assert.Equal(0, aliveCell.Col);

        // Verify all coordinates are present
        var coordinates = cellsWithCoords.Select(item => (item.Row, item.Col)).ToHashSet();
        var expectedCoords = new HashSet<(int, int)> { (0, 0), (0, 1), (1, 0), (1, 1) };
        Assert.True(coordinates.SetEquals(expectedCoords));
    }

    #endregion

    #region Integration Tests - Known Patterns

    [Fact]
    public void BlockPattern_ShouldRemainStable()
    {
        // ARRANGE - Create 2x2 block (still life)
        var grid = new Grid(4, 4);
        grid.SetCell(1, 1, true);
        grid.SetCell(1, 2, true);
        grid.SetCell(2, 1, true);
        grid.SetCell(2, 2, true);
        var rules = new GameRules();
        var originalState = grid.Clone();

        // ACT - Evolve several generations
        for (int i = 0; i < 5; i++)
        {
            grid.CalculateNextGeneration(rules);
            grid.ApplyNextGeneration();
        }

        // ASSERT - Should remain unchanged
        Assert.True(grid.Equals(originalState));
        Assert.Equal(4, grid.AliveCellCount);
    }

    [Fact]
    public void GliderPattern_ShouldMoveCorrectly()
    {
        // ARRANGE - Create glider pattern in large grid
        var grid = new Grid(10, 10);
        // Standard glider pattern
        grid.SetCell(1, 2, true);
        grid.SetCell(2, 3, true);
        grid.SetCell(3, 1, true);
        grid.SetCell(3, 2, true);
        grid.SetCell(3, 3, true);
        var rules = new GameRules();

        // ACT - Evolve 4 generations (glider returns to same shape but moved)
        for (int i = 0; i < 4; i++)
        {
            grid.CalculateNextGeneration(rules);
            grid.ApplyNextGeneration();
        }

        // ASSERT - Glider should have moved down and right by 1
        Assert.Equal(5, grid.AliveCellCount);
        Assert.True(grid.GetCell(2, 3).IsAlive);
        Assert.True(grid.GetCell(3, 4).IsAlive);
        Assert.True(grid.GetCell(4, 2).IsAlive);
        Assert.True(grid.GetCell(4, 3).IsAlive);
        Assert.True(grid.GetCell(4, 4).IsAlive);
    }

    [Fact]
    public void EmptyGrid_ShouldRemainEmpty()
    {
        // ARRANGE
        var grid = new Grid(5, 5);
        var rules = new GameRules();

        // ACT
        grid.CalculateNextGeneration(rules);
        grid.ApplyNextGeneration();

        // ASSERT
        Assert.Equal(0, grid.AliveCellCount);
    }

    #endregion

    #region Edge Cases and Performance

    [Fact]
    public void LargeGrid_ShouldWorkCorrectly()
    {
        // ARRANGE
        var grid = new Grid(100, 100);

        // ACT - This should not throw or timeout
        grid.RandomizeGrid(0.3, 42);
        var rules = new GameRules();
        grid.CalculateNextGeneration(rules);
        grid.ApplyNextGeneration();

        // ASSERT
        Assert.Equal(100, grid.Width);
        Assert.Equal(100, grid.Height);
        Assert.True(grid.AliveCellCount >= 0);
        Assert.True(grid.AliveCellCount <= 10000);
    }

    [Fact]
    public void ToString_ShouldProvideReadableRepresentation()
    {
        // ARRANGE
        var grid = new Grid(3, 2);
        grid.SetCell(0, 1, true);
        grid.SetCell(1, 0, true);

        // ACT
        var stringRep = grid.ToString();

        // ASSERT
        Assert.Contains("Grid 3×2", stringRep);
        Assert.Contains("(2/6 alive)", stringRep);
        Assert.Contains("█", stringRep); // Alive cells
        Assert.Contains("·", stringRep); // Dead cells
    }

    #endregion

    #region Equals and GetHashCode Tests

    [Fact]
    public void Equals_WithIdenticalGrids_ShouldReturnTrue()
    {
        // ARRANGE
        var grid1 = new Grid(3, 3);
        var grid2 = new Grid(3, 3);
        grid1.SetCell(1, 1, true);
        grid2.SetCell(1, 1, true);

        // ACT & ASSERT
        Assert.True(grid1.Equals(grid2));
        Assert.True(grid2.Equals(grid1));
    }

    [Fact]
    public void Equals_WithDifferentDimensions_ShouldReturnFalse()
    {
        // ARRANGE
        var grid1 = new Grid(3, 3);
        var grid2 = new Grid(3, 4);

        // ACT & ASSERT
        Assert.False(grid1.Equals(grid2));
    }

    [Fact]
    public void Equals_WithDifferentCellStates_ShouldReturnFalse()
    {
        // ARRANGE
        var grid1 = new Grid(3, 3);
        var grid2 = new Grid(3, 3);
        grid1.SetCell(1, 1, true);
        // grid2 has all cells dead

        // ACT & ASSERT
        Assert.False(grid1.Equals(grid2));
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // ARRANGE
        var grid = new Grid(3, 3);

        // ACT & ASSERT
        Assert.False(grid.Equals(null));
    }

    [Fact]
    public void GetHashCode_WithSameGridState_ShouldReturnSameHashCode()
    {
        // ARRANGE
        var grid1 = new Grid(3, 3);
        var grid2 = new Grid(3, 3);
        grid1.SetCell(0, 0, true);
        grid2.SetCell(0, 0, true);

        // ACT
        var hash1 = grid1.GetHashCode();
        var hash2 = grid2.GetHashCode();

        // ASSERT
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentGridStates_ShouldReturnDifferentHashCodes()
    {
        // ARRANGE
        var grid1 = new Grid(3, 3);
        var grid2 = new Grid(3, 3);
        grid1.SetCell(0, 0, true);
        grid2.SetCell(1, 1, true);

        // ACT
        var hash1 = grid1.GetHashCode();
        var hash2 = grid2.GetHashCode();

        // ASSERT
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Equals_IgnoresNextState_OnlyComparesCurrentState()
    {
        // ARRANGE
        var grid1 = new Grid(3, 3);
        var grid2 = new Grid(3, 3);

        // Set same current state
        grid1.SetCell(1, 1, true);
        grid2.SetCell(1, 1, true);

        // Set different next states
        grid1.GetCell(1, 1).NextState = true;
        grid2.GetCell(1, 1).NextState = false;

        // ACT & ASSERT - Should still be equal since we only compare IsAlive
        Assert.True(grid1.Equals(grid2));
    }

    #endregion
}