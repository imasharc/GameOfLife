using GameOfLife.Models;

namespace GameOfLife.Services;

// Core game engine for Conway's Game of Life.
// Orchestrates game logic, evolution, timing, and state management.
// Follows Single Responsibility Principle by focusing only on game orchestration.
public class GameEngine : IDisposable
{
    private Grid _grid;
    private GameRules _gameRules;
    private int _generation;
    private int _evolutionSpeed;
    private bool _isRunning;
    private bool _isPaused;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Task? _gameLoopTask;

    // Event fired when a new generation is calculated
    public event EventHandler<GenerationEventArgs>? GenerationEvolved;

    // Event fired when game state changes (started, paused, stopped, etc.)
    public event EventHandler<GameStateEventArgs>? StateChanged;

    // Gets the current generation number.
    public int Generation => _generation;

    // Gets whether the game is currently running.
    public bool IsRunning => _isRunning;

    // Gets whether the game is currently paused.
    public bool IsPaused => _isPaused;

    // Gets the current evolution speed in milliseconds.
    public int EvolutionSpeed => _evolutionSpeed;

    // Gets the current game rules.
    public GameRules GameRules => _gameRules;

    // Gets the current grid.
    public Grid Grid => _grid;

    // Gets whether the game loop is active (running and not paused).
    public bool IsActive => _isRunning && !_isPaused;

    // Initializes a new instance of the GameEngine class.
    public GameEngine(int initialWidth = 40, int initialHeight = 20, int initialSpeed = 500)
    {
        if (initialWidth < 1) throw new ArgumentException("Width must be at least 1.", nameof(initialWidth));
        if (initialHeight < 1) throw new ArgumentException("Height must be at least 1.", nameof(initialHeight));
        if (initialSpeed < 1) throw new ArgumentException("Speed must be at least 1 millisecond.", nameof(initialSpeed));

        _grid = new Grid(initialWidth, initialHeight);
        _gameRules = new GameRules();
        _generation = 0;
        _evolutionSpeed = initialSpeed;
        _isRunning = false;
        _isPaused = false;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    // Starts the game evolution loop asynchronously.
    public async Task StartAsync()
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("Game is already running. Use Resume() to resume from pause.");
        }

        _isRunning = true;
        _isPaused = false;

        OnStateChanged(GameState.Started);

        _gameLoopTask = RunGameLoopAsync(_cancellationTokenSource.Token);

        try
        {
            await _gameLoopTask;
        }
        catch (OperationCanceledException)
        {
            // Expected when stopping the game
        }
    }

    // Pauses the game evolution.
    public void Pause()
    {
        if (!_isRunning)
        {
            throw new InvalidOperationException("Game is not running.");
        }

        if (_isPaused)
        {
            throw new InvalidOperationException("Game is already paused.");
        }

        _isPaused = true;
        OnStateChanged(GameState.Paused);
    }

    // Resumes the game evolution from pause.
    public void Resume()
    {
        if (!_isRunning)
        {
            throw new InvalidOperationException("Game is not running.");
        }

        if (!_isPaused)
        {
            throw new InvalidOperationException("Game is not paused.");
        }

        _isPaused = false;
        OnStateChanged(GameState.Resumed);
    }

    // Stops the game evolution loop.
    public async Task StopAsync()
    {
        if (!_isRunning)
        {
            return; // Already stopped
        }

        _cancellationTokenSource.Cancel();

        if (_gameLoopTask != null)
        {
            try
            {
                await _gameLoopTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        _isRunning = false;
        _isPaused = false;
        OnStateChanged(GameState.Stopped);
    }

    // Evolves the grid by one generation manually (useful for step-by-step mode).
    public void EvolveOneGeneration()
    {
        _grid.CalculateNextGeneration(_gameRules);
        _grid.ApplyNextGeneration();
        _generation++;

        OnGenerationEvolved();
    }

    // Resets the game to initial state.
    public void Reset()
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("Cannot reset while game is running. Stop the game first.");
        }

        _grid.Clear();
        _generation = 0;
        OnStateChanged(GameState.Reset);
    }

    // Sets a new grid with the specified dimensions.
    public void SetGrid(int width, int height)
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("Cannot change grid while game is running. Stop the game first.");
        }

        _grid = new Grid(width, height);
        _generation = 0;
        OnStateChanged(GameState.GridChanged);
    }

    // Sets new game rules.
    public void SetRules(GameRules newRules)
    {
        ArgumentNullException.ThrowIfNull(newRules);

        _gameRules = newRules;
        OnStateChanged(GameState.RulesChanged);
    }

    // Sets the evolution speed in milliseconds.
    public void SetSpeed(int speedMs)
    {
        if (speedMs < 1)
        {
            throw new ArgumentException("Speed must be at least 1 millisecond.", nameof(speedMs));
        }

        _evolutionSpeed = speedMs;
        OnStateChanged(GameState.SpeedChanged);
    }

    // Randomizes the grid with the specified probability.
    public void RandomizeGrid(double probability = 0.3, int? seed = null)
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("Cannot randomize grid while game is running. Stop the game first.");
        }

        _grid.RandomizeGrid(probability, seed);
        _generation = 0;
        OnStateChanged(GameState.GridChanged);
    }

    // Gets current game statistics.
    public GameStatistics GetStatistics()
    {
        return new GameStatistics
        {
            Generation = _generation,
            AliveCells = _grid.AliveCellCount,
            TotalCells = _grid.TotalCells,
            Density = (double)_grid.AliveCellCount / _grid.TotalCells,
            GridWidth = _grid.Width,
            GridHeight = _grid.Height,
            Rules = _gameRules.ToString(),
            EvolutionSpeed = _evolutionSpeed,
            IsRunning = _isRunning,
            IsPaused = _isPaused
        };
    }

    // Main game loop that runs evolution cycles.
    private async Task RunGameLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // Wait if paused
            while (_isPaused && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
                break;

            // Evolve one generation
            await Task.Run(() =>
            {
                _grid.CalculateNextGeneration(_gameRules);
                _grid.ApplyNextGeneration();
                _generation++;
            }, cancellationToken);

            OnGenerationEvolved();

            // Check for extinction
            if (_grid.AliveCellCount == 0)
            {
                Pause();
                OnStateChanged(GameState.Extinct);
                continue;
            }

            // Wait for the specified evolution speed
            await Task.Delay(_evolutionSpeed, cancellationToken);
        }
    }

    // Fires the GenerationEvolved event.
    private void OnGenerationEvolved()
    {
        GenerationEvolved?.Invoke(this, new GenerationEventArgs
        {
            Generation = _generation,
            AliveCells = _grid.AliveCellCount,
            TotalCells = _grid.TotalCells,
            Grid = _grid.Clone() // Provide a snapshot
        });
    }

    // Fires the StateChanged event.
    private void OnStateChanged(GameState newState)
    {
        StateChanged?.Invoke(this, new GameStateEventArgs
        {
            State = newState,
            Statistics = GetStatistics()
        });
    }

    // Disposes of the game engine resources.
    public void Dispose()
    {
        if (_isRunning)
        {
            StopAsync().Wait(TimeSpan.FromSeconds(1));
        }

        _cancellationTokenSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}