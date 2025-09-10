using GameOfLife.Models;
using GameOfLife.Services;

namespace GameOfLife.Tests.Services;

// Tests for the supporting classes used by GameEngine
// (GameStatistics, GenerationEventArgs, GameStateEventArgs)
public class GameEngineComponentTests
{
    #region GameStatistics Tests

    [Fact]
    public void GameStatistics_Properties_ShouldBeSetCorrectly()
    {
        // ARRANGE & ACT
        var stats = new GameStatistics
        {
            Generation = 42,
            AliveCells = 150,
            TotalCells = 500,
            Density = 0.3,
            GridWidth = 25,
            GridHeight = 20,
            Rules = "B36/S23",
            EvolutionSpeed = 250,
            IsRunning = true,
            IsPaused = false
        };

        // ASSERT
        Assert.Equal(42, stats.Generation);
        Assert.Equal(150, stats.AliveCells);
        Assert.Equal(500, stats.TotalCells);
        Assert.Equal(0.3, stats.Density);
        Assert.Equal(25, stats.GridWidth);
        Assert.Equal(20, stats.GridHeight);
        Assert.Equal("B36/S23", stats.Rules);
        Assert.Equal(250, stats.EvolutionSpeed);
        Assert.True(stats.IsRunning);
        Assert.False(stats.IsPaused);
    }

    [Fact]
    public void GameStatistics_IsActive_WhenRunningAndNotPaused_ShouldReturnTrue()
    {
        // ARRANGE
        var stats = new GameStatistics
        {
            IsRunning = true,
            IsPaused = false
        };

        // ACT & ASSERT
        Assert.True(stats.IsActive);
    }

    [Fact]
    public void GameStatistics_IsActive_WhenPaused_ShouldReturnFalse()
    {
        // ARRANGE
        var stats = new GameStatistics
        {
            IsRunning = true,
            IsPaused = true
        };

        // ACT & ASSERT
        Assert.False(stats.IsActive);
    }

    [Fact]
    public void GameStatistics_IsActive_WhenStopped_ShouldReturnFalse()
    {
        // ARRANGE
        var stats = new GameStatistics
        {
            IsRunning = false,
            IsPaused = false
        };

        // ACT & ASSERT
        Assert.False(stats.IsActive);
    }

    [Fact]
    public void GameStatistics_DensityPercentage_ShouldFormatCorrectly()
    {
        // ARRANGE
        var stats = new GameStatistics
        {
            Density = 0.3567
        };

        // ACT
        var percentage = stats.DensityPercentage;

        // ASSERT
        Assert.Equal("35.67%", percentage);
    }

    [Theory]
    [InlineData(0.0, "0.00%")]
    [InlineData(0.5, "50.00%")]
    [InlineData(1.0, "100.00%")]
    [InlineData(0.123456, "12.35%")]
    public void GameStatistics_DensityPercentage_WithVariousDensities_ShouldFormatCorrectly(
        double density, string expected)
    {
        // ARRANGE
        var stats = new GameStatistics { Density = density };

        // ACT
        var result = stats.DensityPercentage;

        // ASSERT
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GameStatistics_ToString_ShouldProvideReadableFormat()
    {
        // ARRANGE
        var stats = new GameStatistics
        {
            Generation = 10,
            AliveCells = 25,
            TotalCells = 100,
            Density = 0.25,
            Rules = "B3/S23",
            EvolutionSpeed = 500,
            IsRunning = true,
            IsPaused = false
        };

        // ACT
        var result = stats.ToString();

        // ASSERT
        Assert.Contains("Gen: 10", result);
        Assert.Contains("Alive: 25/100", result);
        Assert.Contains("25.00%", result);
        Assert.Contains("Rules: B3/S23", result);
        Assert.Contains("Speed: 500ms", result);
        Assert.Contains("Status: Running", result);
    }

    [Theory]
    [InlineData(true, false, "Running")]
    [InlineData(true, true, "Paused")]
    [InlineData(false, false, "Stopped")]
    [InlineData(false, true, "Stopped")] // Stopped overrides paused
    public void GameStatistics_ToString_WithVariousStates_ShouldShowCorrectStatus(
        bool isRunning, bool isPaused, string expectedStatus)
    {
        // ARRANGE
        var stats = new GameStatistics
        {
            IsRunning = isRunning,
            IsPaused = isPaused
        };

        // ACT
        var result = stats.ToString();

        // ASSERT
        Assert.Contains($"Status: {expectedStatus}", result);
    }

    #endregion

    #region GenerationEventArgs Tests

    [Fact]
    public void GenerationEventArgs_Properties_ShouldBeSetCorrectly()
    {
        // ARRANGE
        var grid = new Grid(5, 5);
        grid.SetCell(1, 1, true);
        grid.SetCell(2, 2, true);

        // ACT
        var args = new GenerationEventArgs
        {
            Generation = 15,
            AliveCells = 2,
            TotalCells = 25,
            Grid = grid
        };

        // ASSERT
        Assert.Equal(15, args.Generation);
        Assert.Equal(2, args.AliveCells);
        Assert.Equal(25, args.TotalCells);
        Assert.Same(grid, args.Grid);
    }

    [Fact]
    public void GenerationEventArgs_Density_ShouldCalculateCorrectly()
    {
        // ARRANGE & ACT
        var args = new GenerationEventArgs
        {
            AliveCells = 30,
            TotalCells = 200
        };

        // ASSERT
        Assert.Equal(0.15, args.Density);
    }

    [Fact]
    public void GenerationEventArgs_Density_WithZeroTotal_ShouldReturnZero()
    {
        // ARRANGE & ACT
        var args = new GenerationEventArgs
        {
            AliveCells = 5,
            TotalCells = 0
        };

        // ASSERT
        Assert.Equal(0.0, args.Density);
    }

    [Fact]
    public void GenerationEventArgs_IsExtinct_WithZeroAliveCells_ShouldReturnTrue()
    {
        // ARRANGE & ACT
        var args = new GenerationEventArgs
        {
            AliveCells = 0,
            TotalCells = 100
        };

        // ASSERT
        Assert.True(args.IsExtinct);
    }

    [Fact]
    public void GenerationEventArgs_IsExtinct_WithAliveCells_ShouldReturnFalse()
    {
        // ARRANGE & ACT
        var args = new GenerationEventArgs
        {
            AliveCells = 5,
            TotalCells = 100
        };

        // ASSERT
        Assert.False(args.IsExtinct);
    }

    [Fact]
    public void GenerationEventArgs_ToString_ShouldProvideReadableFormat()
    {
        // ARRANGE
        var args = new GenerationEventArgs
        {
            Generation = 42,
            AliveCells = 75,
            TotalCells = 200
        };

        // ACT
        var result = args.ToString();

        // ASSERT
        Assert.Contains("Generation 42", result);
        Assert.Contains("75/200 alive", result);
        Assert.Contains("37.50%", result);
    }

    #endregion

    #region GameStateEventArgs Tests

    [Fact]
    public void GameStateEventArgs_Properties_ShouldBeSetCorrectly()
    {
        // ARRANGE
        var stats = new GameStatistics
        {
            Generation = 10,
            AliveCells = 50
        };

        // ACT
        var args = new GameStateEventArgs
        {
            State = GameState.Started,
            Statistics = stats
        };

        // ASSERT
        Assert.Equal(GameState.Started, args.State);
        Assert.Same(stats, args.Statistics);
        Assert.True(args.Timestamp <= DateTime.UtcNow);
        Assert.True(args.Timestamp >= DateTime.UtcNow.AddMinutes(-1));
    }

    [Theory]
    [InlineData(GameState.Started, "Game simulation started")]
    [InlineData(GameState.Paused, "Game simulation paused")]
    [InlineData(GameState.Resumed, "Game simulation resumed")]
    [InlineData(GameState.Stopped, "Game simulation stopped")]
    [InlineData(GameState.Reset, "Game state reset to initial conditions")]
    [InlineData(GameState.GridChanged, "Grid configuration changed")]
    [InlineData(GameState.RulesChanged, "Game rules modified")]
    [InlineData(GameState.SpeedChanged, "Evolution speed adjusted")]
    [InlineData(GameState.Extinct, "Population extinct - all cells dead")]
    public void GameStateEventArgs_Description_WithValidStates_ShouldReturnCorrectDescription(
        GameState state, string expectedDescription)
    {
        // ARRANGE
        var args = new GameStateEventArgs
        {
            State = state,
            Statistics = new GameStatistics()
        };

        // ACT
        var description = args.Description;

        // ASSERT
        Assert.Equal(expectedDescription, description);
    }

    [Fact]
    public void GameStateEventArgs_Description_WithUnknownState_ShouldReturnUnknownMessage()
    {
        // ARRANGE
        var invalidState = (GameState)999;
        var args = new GameStateEventArgs
        {
            State = invalidState,
            Statistics = new GameStatistics()
        };

        // ACT
        var description = args.Description;

        // ASSERT
        Assert.Contains("Unknown state", description);
        Assert.Contains("999", description);
    }

    [Fact]
    public void GameStateEventArgs_ToString_ShouldIncludeTimestampAndDescription()
    {
        // ARRANGE
        var args = new GameStateEventArgs
        {
            State = GameState.Started,
            Statistics = new GameStatistics()
        };

        // ACT
        var result = args.ToString();

        // ASSERT
        Assert.Matches(@"\d{2}:\d{2}:\d{2} - Game simulation started", result);
    }

    #endregion

    #region GameState Enum Tests

    [Fact]
    public void GameState_AllValues_ShouldBeDefined()
    {
        // ARRANGE
        var expectedStates = new[]
        {
            GameState.Started,
            GameState.Paused,
            GameState.Resumed,
            GameState.Stopped,
            GameState.Reset,
            GameState.GridChanged,
            GameState.RulesChanged,
            GameState.SpeedChanged,
            GameState.Extinct
        };

        // ACT & ASSERT
        foreach (var state in expectedStates)
        {
            Assert.True(Enum.IsDefined(typeof(GameState), state));
        }
    }

    [Fact]
    public void GameState_EnumValues_ShouldHaveExpectedCount()
    {
        // ARRANGE & ACT
        var stateCount = Enum.GetValues<GameState>().Length;

        // ASSERT
        Assert.Equal(9, stateCount);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void GameStatistics_WithRealisticData_ShouldCalculateCorrectly()
    {
        // ARRANGE - Realistic Game of Life scenario
        var stats = new GameStatistics
        {
            Generation = 156,
            AliveCells = 89,
            TotalCells = 800, // 40x20 grid
            Density = 89.0 / 800.0,
            GridWidth = 40,
            GridHeight = 20,
            Rules = "B3/S23",
            EvolutionSpeed = 200,
            IsRunning = true,
            IsPaused = false
        };

        // ACT
        var description = stats.ToString();
        var percentage = stats.DensityPercentage;
        var isActive = stats.IsActive;

        // ASSERT
        Assert.Equal("11.13%", percentage);
        Assert.True(isActive);
        Assert.Contains("Gen: 156", description);
        Assert.Contains("Alive: 89/800", description);
        Assert.Contains("11.13%", description);
    }

    [Fact]
    public void EventArgs_WithGridSnapshot_ShouldPreserveGridState()
    {
        // ARRANGE
        var originalGrid = new Grid(3, 3);
        originalGrid.SetCell(1, 1, true);
        originalGrid.SetCell(1, 2, true);

        var snapshotGrid = originalGrid.Clone();

        var args = new GenerationEventArgs
        {
            Generation = 1,
            AliveCells = snapshotGrid.AliveCellCount,
            TotalCells = snapshotGrid.TotalCells,
            Grid = snapshotGrid
        };

        // ACT - Modify original grid
        originalGrid.SetCell(2, 2, true);

        // ASSERT - Snapshot should be unchanged
        Assert.Equal(2, args.AliveCells);
        Assert.Equal(2, args.Grid.AliveCellCount);
        Assert.NotEqual(originalGrid.AliveCellCount, args.Grid.AliveCellCount);
    }

    #endregion
}