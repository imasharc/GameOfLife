using GameOfLife.Models;
using GameOfLife.Services;

namespace GameOfLife.Tests.Services;

public class GameEngineTests : IDisposable
{
    private GameEngine _gameEngine;

    public GameEngineTests()
    {
        _gameEngine = new GameEngine(10, 10, 100); // Small, fast for testing
    }

    public void Dispose()
    {
        _gameEngine?.Dispose();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateCorrectly()
    {
        // ARRANGE & ACT
        var engine = new GameEngine(20, 15, 200);

        // ASSERT
        Assert.Equal(20, engine.Grid.Width);
        Assert.Equal(15, engine.Grid.Height);
        Assert.Equal(200, engine.EvolutionSpeed);
        Assert.Equal(0, engine.Generation);
        Assert.False(engine.IsRunning);
        Assert.False(engine.IsPaused);
        Assert.False(engine.IsActive);
        Assert.NotNull(engine.GameRules);
        Assert.NotNull(engine.Grid);

        engine.Dispose();
    }

    [Fact]
    public void Constructor_WithDefaultParameters_ShouldUseDefaults()
    {
        // ARRANGE & ACT
        var engine = new GameEngine();

        // ASSERT
        Assert.Equal(40, engine.Grid.Width);
        Assert.Equal(20, engine.Grid.Height);
        Assert.Equal(500, engine.EvolutionSpeed);

        engine.Dispose();
    }

    [Theory]
    [InlineData(0, 10, 100)]
    [InlineData(-1, 10, 100)]
    [InlineData(10, 0, 100)]
    [InlineData(10, -1, 100)]
    [InlineData(10, 10, 0)]
    [InlineData(10, 10, -1)]
    public void Constructor_WithInvalidParameters_ShouldThrowArgumentException(int width, int height, int speed)
    {
        // ACT & ASSERT
        Assert.Throws<ArgumentException>(() => new GameEngine(width, height, speed));
    }

    #endregion

    #region State Management Tests

    [Fact]
    public async Task StartAsync_WhenStopped_ShouldStartGame()
    {
        // ARRANGE
        // Add alive cells to prevent immediate extinction
        _gameEngine.Grid.SetCell(2, 2, true);
        _gameEngine.Grid.SetCell(2, 3, true);
        _gameEngine.Grid.SetCell(2, 4, true); // Blinker pattern

        var stateChangedFired = false;
        GameState capturedState = default;

        _gameEngine.StateChanged += (sender, args) =>
        {
            if (args.State == GameState.Started)
            {
                stateChangedFired = true;
                capturedState = args.State;
            }
        };

        // ACT
        var startTask = _gameEngine.StartAsync();

        // Give it a moment to start
        await Task.Delay(50);

        // ASSERT
        Assert.True(_gameEngine.IsRunning);
        Assert.False(_gameEngine.IsPaused);
        Assert.True(_gameEngine.IsActive);
        Assert.True(stateChangedFired);
        Assert.Equal(GameState.Started, capturedState);

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public async Task StartAsync_WhenAlreadyRunning_ShouldThrowInvalidOperationException()
    {
        // ARRANGE
        // Add alive cells to prevent extinction
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.Grid.SetCell(1, 2, true);
        _gameEngine.Grid.SetCell(1, 3, true);

        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);

        // ACT & ASSERT
        await Assert.ThrowsAsync<InvalidOperationException>(() => _gameEngine.StartAsync());

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public async Task Pause_WhenRunning_ShouldPauseGame()
    {
        // ARRANGE
        // Add alive cells to prevent extinction
        _gameEngine.Grid.SetCell(2, 2, true);
        _gameEngine.Grid.SetCell(2, 3, true);
        _gameEngine.Grid.SetCell(2, 4, true); // Blinker pattern

        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);

        var stateChangedFired = false;
        GameState capturedState = default;

        _gameEngine.StateChanged += (sender, args) =>
        {
            if (args.State == GameState.Paused)
            {
                stateChangedFired = true;
                capturedState = args.State;
            }
        };

        // ACT
        _gameEngine.Pause();

        // ASSERT
        Assert.True(_gameEngine.IsRunning);
        Assert.True(_gameEngine.IsPaused);
        Assert.False(_gameEngine.IsActive);
        Assert.True(stateChangedFired);
        Assert.Equal(GameState.Paused, capturedState);

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public void Pause_WhenNotRunning_ShouldThrowInvalidOperationException()
    {
        // ACT & ASSERT
        Assert.Throws<InvalidOperationException>(() => _gameEngine.Pause());
    }

    [Fact]
    public async Task Pause_WhenAlreadyPaused_ShouldThrowInvalidOperationException()
    {
        // ARRANGE
        // Add alive cells to prevent extinction
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.Grid.SetCell(1, 2, true);
        _gameEngine.Grid.SetCell(1, 3, true);

        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);
        _gameEngine.Pause(); // Manually pause first

        // ACT & ASSERT
        Assert.Throws<InvalidOperationException>(() => _gameEngine.Pause());

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public async Task Resume_WhenPaused_ShouldResumeGame()
    {
        // ARRANGE
        // Add alive cells to prevent extinction
        _gameEngine.Grid.SetCell(2, 2, true);
        _gameEngine.Grid.SetCell(2, 3, true);
        _gameEngine.Grid.SetCell(2, 4, true); // Blinker pattern

        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);
        _gameEngine.Pause(); // Manually pause first

        var stateChangedFired = false;
        GameState capturedState = default;

        _gameEngine.StateChanged += (sender, args) =>
        {
            if (args.State == GameState.Resumed)
            {
                stateChangedFired = true;
                capturedState = args.State;
            }
        };

        // ACT
        _gameEngine.Resume();

        // ASSERT
        Assert.True(_gameEngine.IsRunning);
        Assert.False(_gameEngine.IsPaused);
        Assert.True(_gameEngine.IsActive);
        Assert.True(stateChangedFired);
        Assert.Equal(GameState.Resumed, capturedState);

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public void Resume_WhenNotRunning_ShouldThrowInvalidOperationException()
    {
        // ACT & ASSERT
        Assert.Throws<InvalidOperationException>(() => _gameEngine.Resume());
    }

    [Fact]
    public async Task Resume_WhenNotPaused_ShouldThrowInvalidOperationException()
    {
        // ARRANGE
        // Add alive cells to prevent extinction
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.Grid.SetCell(1, 2, true);
        _gameEngine.Grid.SetCell(1, 3, true);

        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);

        // Ensure game is running and not paused
        Assert.True(_gameEngine.IsRunning);
        Assert.False(_gameEngine.IsPaused);

        // ACT & ASSERT
        Assert.Throws<InvalidOperationException>(() => _gameEngine.Resume());

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public async Task StopAsync_WhenRunning_ShouldStopGame()
    {
        // ARRANGE
        // Add alive cells to prevent extinction
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.Grid.SetCell(1, 2, true);
        _gameEngine.Grid.SetCell(1, 3, true);

        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);

        var stateChangedFired = false;
        GameState capturedState = default;

        _gameEngine.StateChanged += (sender, args) =>
        {
            if (args.State == GameState.Stopped)
            {
                stateChangedFired = true;
                capturedState = args.State;
            }
        };

        // ACT
        await _gameEngine.StopAsync();

        // ASSERT
        Assert.False(_gameEngine.IsRunning);
        Assert.False(_gameEngine.IsPaused);
        Assert.False(_gameEngine.IsActive);
        Assert.True(stateChangedFired);
        Assert.Equal(GameState.Stopped, capturedState);
    }

    [Fact]
    public async Task StopAsync_WhenAlreadyStopped_ShouldNotThrow()
    {
        // ACT & ASSERT - Should not throw
        await _gameEngine.StopAsync();
        await _gameEngine.StopAsync(); // Called twice
    }

    #endregion

    #region Evolution Tests

    [Fact]
    public void EvolveOneGeneration_ShouldIncrementGeneration()
    {
        // ARRANGE
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.Grid.SetCell(1, 2, true);
        _gameEngine.Grid.SetCell(1, 3, true);
        var initialGeneration = _gameEngine.Generation;

        // ACT
        _gameEngine.EvolveOneGeneration();

        // ASSERT
        Assert.Equal(initialGeneration + 1, _gameEngine.Generation);
    }

    [Fact]
    public void EvolveOneGeneration_ShouldFireGenerationEvolvedEvent()
    {
        // ARRANGE
        var eventFired = false;
        GenerationEventArgs? capturedArgs = null;

        _gameEngine.GenerationEvolved += (sender, args) =>
        {
            eventFired = true;
            capturedArgs = args;
        };

        _gameEngine.Grid.SetCell(1, 1, true);

        // ACT
        _gameEngine.EvolveOneGeneration();

        // ASSERT
        Assert.True(eventFired);
        Assert.NotNull(capturedArgs);
        Assert.Equal(1, capturedArgs.Generation);
        Assert.NotNull(capturedArgs.Grid);
    }

    [Fact]
    public async Task GameLoop_ShouldEvolveGenerationsAutomatically()
    {
        // ARRANGE
        _gameEngine.Grid.SetCell(2, 1, true);
        _gameEngine.Grid.SetCell(2, 2, true);
        _gameEngine.Grid.SetCell(2, 3, true); // Blinker pattern

        var generationCount = 0;
        _gameEngine.GenerationEvolved += (sender, args) => generationCount++;

        // ACT
        var startTask = _gameEngine.StartAsync();
        await Task.Delay(250); // Wait for a few evolutions
        await _gameEngine.StopAsync();

        // ASSERT
        Assert.True(generationCount >= 1);
        Assert.True(_gameEngine.Generation >= 1);
    }

    [Fact]
    public async Task GameLoop_WithExtinctPopulation_ShouldPauseAutomatically()
    {
        // ARRANGE - Empty grid (extinction scenario)
        var stateChangedFired = false;
        GameState capturedState = default;

        _gameEngine.StateChanged += (sender, args) =>
        {
            if (args.State == GameState.Extinct)
            {
                stateChangedFired = true;
                capturedState = args.State;
            }
        };

        // ACT
        var startTask = _gameEngine.StartAsync();
        await Task.Delay(150); // Wait for extinction detection
        await _gameEngine.StopAsync();

        // ASSERT
        Assert.True(stateChangedFired);
        Assert.Equal(GameState.Extinct, capturedState);
    }

    #endregion

    #region Configuration Tests

    [Fact]
    public void SetGrid_WhenStopped_ShouldUpdateGrid()
    {
        // ARRANGE
        var stateChangedFired = false;
        _gameEngine.StateChanged += (sender, args) => stateChangedFired = true;

        // ACT
        _gameEngine.SetGrid(15, 12);

        // ASSERT
        Assert.Equal(15, _gameEngine.Grid.Width);
        Assert.Equal(12, _gameEngine.Grid.Height);
        Assert.Equal(0, _gameEngine.Generation); // Should reset generation
        Assert.True(stateChangedFired);
    }

    [Fact]
    public async Task SetGrid_WhenRunning_ShouldThrowInvalidOperationException()
    {
        // ARRANGE
        _gameEngine.Grid.SetCell(1, 1, true); // Prevent extinction
        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);

        // ACT & ASSERT
        Assert.Throws<InvalidOperationException>(() => _gameEngine.SetGrid(15, 12));

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public void SetRules_WithValidRules_ShouldUpdateRules()
    {
        // ARRANGE
        var newRules = new GameRules(new HashSet<int> { 3, 6 }, new HashSet<int> { 2, 3 });
        var stateChangedFired = false;
        _gameEngine.StateChanged += (sender, args) => stateChangedFired = true;

        // ACT
        _gameEngine.SetRules(newRules);

        // ASSERT
        Assert.Equal("B36/S23", _gameEngine.GameRules.ToString());
        Assert.True(stateChangedFired);
    }

    [Fact]
    public void SetRules_WithNull_ShouldThrowArgumentNullException()
    {
        // ACT & ASSERT
        Assert.Throws<ArgumentNullException>(() => _gameEngine.SetRules(null!));
    }

    [Fact]
    public void SetSpeed_WithValidSpeed_ShouldUpdateSpeed()
    {
        // ARRANGE
        var stateChangedFired = false;
        _gameEngine.StateChanged += (sender, args) => stateChangedFired = true;

        // ACT
        _gameEngine.SetSpeed(250);

        // ASSERT
        Assert.Equal(250, _gameEngine.EvolutionSpeed);
        Assert.True(stateChangedFired);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void SetSpeed_WithInvalidSpeed_ShouldThrowArgumentException(int invalidSpeed)
    {
        // ACT & ASSERT
        Assert.Throws<ArgumentException>(() => _gameEngine.SetSpeed(invalidSpeed));
    }

    [Fact]
    public void RandomizeGrid_WhenStopped_ShouldRandomizeGrid()
    {
        // ARRANGE
        var stateChangedFired = false;
        _gameEngine.StateChanged += (sender, args) => stateChangedFired = true;

        // ACT
        _gameEngine.RandomizeGrid(0.5, 12345);

        // ASSERT
        Assert.True(_gameEngine.Grid.AliveCellCount > 0);
        Assert.Equal(0, _gameEngine.Generation); // Should reset generation
        Assert.True(stateChangedFired);
    }

    [Fact]
    public async Task RandomizeGrid_WhenRunning_ShouldThrowInvalidOperationException()
    {
        // ARRANGE
        _gameEngine.Grid.SetCell(1, 1, true); // Prevent extinction
        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);

        // ACT & ASSERT
        Assert.Throws<InvalidOperationException>(() => _gameEngine.RandomizeGrid());

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public void Reset_WhenStopped_ShouldResetToInitialState()
    {
        // ARRANGE
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.EvolveOneGeneration(); // Advance generation
        var stateChangedFired = false;
        _gameEngine.StateChanged += (sender, args) => stateChangedFired = true;

        // ACT
        _gameEngine.Reset();

        // ASSERT
        Assert.Equal(0, _gameEngine.Grid.AliveCellCount);
        Assert.Equal(0, _gameEngine.Generation);
        Assert.True(stateChangedFired);
    }

    [Fact]
    public async Task Reset_WhenRunning_ShouldThrowInvalidOperationException()
    {
        // ARRANGE
        _gameEngine.Grid.SetCell(1, 1, true); // Prevent extinction
        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);

        // ACT & ASSERT
        Assert.Throws<InvalidOperationException>(() => _gameEngine.Reset());

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    #endregion

    #region Statistics Tests

    [Fact]
    public void GetStatistics_ShouldReturnCurrentGameState()
    {
        // ARRANGE
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.Grid.SetCell(1, 2, true);
        _gameEngine.SetSpeed(300);

        // ACT
        var stats = _gameEngine.GetStatistics();

        // ASSERT
        Assert.Equal(0, stats.Generation);
        Assert.Equal(2, stats.AliveCells);
        Assert.Equal(100, stats.TotalCells); // 10x10 grid
        Assert.Equal(0.02, stats.Density, 2); // 2/100
        Assert.Equal(10, stats.GridWidth);
        Assert.Equal(10, stats.GridHeight);
        Assert.Equal("B3/S23", stats.Rules);
        Assert.Equal(300, stats.EvolutionSpeed);
        Assert.False(stats.IsRunning);
        Assert.False(stats.IsPaused);
    }

    [Fact]
    public async Task GetStatistics_WhenRunning_ShouldReflectRunningState()
    {
        // ARRANGE
        // Add alive cells to prevent extinction
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.Grid.SetCell(1, 2, true);
        _gameEngine.Grid.SetCell(1, 3, true);

        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);

        // ACT
        var stats = _gameEngine.GetStatistics();

        // ASSERT
        Assert.True(stats.IsRunning);
        Assert.False(stats.IsPaused);

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    [Fact]
    public async Task GetStatistics_WhenPaused_ShouldReflectPausedState()
    {
        // ARRANGE
        // Add alive cells to prevent extinction
        _gameEngine.Grid.SetCell(1, 1, true);
        _gameEngine.Grid.SetCell(1, 2, true);
        _gameEngine.Grid.SetCell(1, 3, true);

        var startTask = _gameEngine.StartAsync();
        await Task.Delay(50);
        _gameEngine.Pause(); // Manually pause

        // ACT
        var stats = _gameEngine.GetStatistics();

        // ASSERT
        Assert.True(stats.IsRunning);
        Assert.True(stats.IsPaused);

        // CLEANUP
        await _gameEngine.StopAsync();
    }

    #endregion

    #region Event Tests

    [Fact]
    public void StateChanged_ShouldFireOnStateTransitions()
    {
        // ARRANGE
        var firedStates = new List<GameState>();
        _gameEngine.StateChanged += (sender, args) => firedStates.Add(args.State);

        // ACT
        _gameEngine.SetSpeed(200);
        _gameEngine.Reset();
        _gameEngine.RandomizeGrid();

        // ASSERT
        Assert.Contains(GameState.SpeedChanged, firedStates);
        Assert.Contains(GameState.Reset, firedStates);
        Assert.Contains(GameState.GridChanged, firedStates);
    }

    [Fact]
    public void GenerationEvolved_ShouldProvideGridSnapshot()
    {
        // ARRANGE
        Grid? capturedGrid = null;
        _gameEngine.GenerationEvolved += (sender, args) => capturedGrid = args.Grid;

        _gameEngine.Grid.SetCell(1, 1, true);

        // ACT
        _gameEngine.EvolveOneGeneration();

        // ASSERT
        Assert.NotNull(capturedGrid);
        Assert.NotSame(_gameEngine.Grid, capturedGrid); // Should be a clone
        Assert.Equal(_gameEngine.Grid.Width, capturedGrid.Width);
        Assert.Equal(_gameEngine.Grid.Height, capturedGrid.Height);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public async Task Dispose_WhenRunning_ShouldStopGameCleanly()
    {
        // ARRANGE
        var engine = new GameEngine(5, 5, 50);
        engine.Grid.SetCell(1, 1, true); // Prevent extinction
        var startTask = engine.StartAsync();
        await Task.Delay(100);

        // ACT
        engine.Dispose();

        // ASSERT
        Assert.False(engine.IsRunning);
        Assert.False(engine.IsPaused);
    }

    [Fact]
    public void Dispose_WhenStopped_ShouldNotThrow()
    {
        // ARRANGE
        var engine = new GameEngine();

        // ACT & ASSERT - Should not throw
        engine.Dispose();
        engine.Dispose(); // Called twice
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullLifecycle_StartPauseResumeStop_ShouldWorkCorrectly()
    {
        // ARRANGE
        var stateChanges = new List<GameState>();
        _gameEngine.StateChanged += (sender, args) => stateChanges.Add(args.State);

        // Add stable pattern to prevent extinction
        _gameEngine.Grid.SetCell(2, 2, true);
        _gameEngine.Grid.SetCell(2, 3, true);
        _gameEngine.Grid.SetCell(2, 4, true); // Blinker pattern

        // ACT
        var startTask = _gameEngine.StartAsync();
        await Task.Delay(100);

        _gameEngine.Pause();
        await Task.Delay(50);

        _gameEngine.Resume();
        await Task.Delay(100);

        await _gameEngine.StopAsync();

        // ASSERT
        Assert.Contains(GameState.Started, stateChanges);
        Assert.Contains(GameState.Paused, stateChanges);
        Assert.Contains(GameState.Resumed, stateChanges);
        Assert.Contains(GameState.Stopped, stateChanges);
        Assert.True(_gameEngine.Generation > 0);
    }

    #endregion
}