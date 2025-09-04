# Conway's Game of Life

A modern, interactive implementation of Conway's Game of Life built with C# and .NET 8 LTS. Features a full console-based user interface with configurable rules, real-time interaction, and professional error handling.

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)
![C#](https://img.shields.io/badge/C%23-12.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)
![Platform](https://img.shields.io/badge/Platform-Cross--Platform-lightgrey)

## ✨ Features

- **Interactive Console UI** with colored cells, borders, and real-time statistics
- **Configurable Game Rules** supporting any B/S notation (Birth/Survival rules)
- **Variable Evolution Speed** with adjustable timing controls
- **Dynamic Grid Resizing** to fit your console or preferences  
- **Real-time Controls** - pause/resume during simulation with spacebar
- **Random Pattern Generation** with customizable probability
- **Cross-Platform** compatibility (Windows, macOS, Linux)
- **Professional Architecture** with clean OOP design patterns
- **Comprehensive Error Handling** and input validation
- **Statistics Display** showing generation count, population, and density

## 🎮 Demo

```
┌────────────────────────────────────────┐
│█·█·█·····█·························█··│
│·███······█·························█··│
│·········█····██████················█··│
│··················█········█·····█··█·│
│··················█··········█···█·██·│
│··················█···········███····█│
│····································█·│
│·····································██│
│······································█│
└────────────────────────────────────────┘

Generation: 42 | Rules: B3/S23 | Alive: 89/800
Commands: start | pause | clear | random | speed | size | rules | stats | help | exit
```

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Any terminal/console that supports ANSI colors (most modern terminals)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/GameOfLife.git
   cd GameOfLife
   ```

2. **Build the project**
   ```bash
   dotnet build
   ```

3. **Run the game**
   ```bash
   dotnet run
   ```

## 🎯 Usage

### Basic Commands

| Command | Description | Example |
|---------|-------------|---------|
| `start` | Begin/resume the simulation | `start` |
| `pause` | Pause the simulation | `pause` |
| `clear` | Clear all cells | `clear` |
| `exit` | Quit the game | `exit` |
| `help` | Show help message | `help` |

### Configuration Commands

| Command | Parameters | Description | Example |
|---------|------------|-------------|---------|
| `random` | `[probability]` | Fill grid randomly | `random 0.4` |
| `speed` | `[milliseconds]` | Set evolution speed | `speed 100` |
| `size` | `[width] [height]` | Resize grid | `size 60 30` |
| `rules` | `[birth] [survival]` | Set custom rules | `rules 3 23` |
| `stats` | - | Show statistics | `stats` |

### During Simulation

- **Spacebar** or **P** - Pause/resume
- **Q** or **Escape** - Return to command mode

### Example Session

```bash
> random 0.3          # Fill 30% of cells randomly
> size 50 25          # Set grid to 50×25
> speed 200           # Fast evolution (200ms between generations)  
> rules 36 23         # Use HighLife rules (B36/S23)
> start               # Begin simulation
# Press SPACE to pause, Q to return to commands
```

## 🧬 Game Rules

Conway's Game of Life follows simple rules that create complex, emergent behavior:

### Standard Rules (B3/S23)
- **Birth**: A dead cell with exactly **3** living neighbors becomes alive
- **Survival**: A living cell with **2** or **3** living neighbors stays alive  
- **Death**: All other cells die or remain dead

### Custom Rules
You can experiment with different rule sets using B/S notation:
- `B3/S23` - Classic Conway's Game of Life
- `B36/S23` - HighLife (creates replicators)
- `B368/S245` - Morley (creates interesting patterns)
- `B2/S` - Seeds (explosive growth)

## 🏗️ Architecture

The project demonstrates modern C# and .NET 8 best practices with clean architecture:

```
GameOfLife/
├── Models/
│   ├── Cell.cs           # Individual cell state and behavior
│   ├── Grid.cs           # 2D grid management and neighbor counting
│   └── GameRules.cs      # Configurable game rules (B/S notation)
├── Services/
│   ├── GameEngine.cs     # Core game loop and state management
│   └── DisplayService.cs # Console UI and rendering
├── Utilities/
│   └── InputParser.cs    # Command parsing and validation
└── Program.cs            # Entry point and dependency injection
```

### Key Design Patterns

- **Single Responsibility Principle** - Each class has one clear purpose
- **Dependency Injection** - Services are injected rather than instantiated
- **Async/Await** - Non-blocking game loop with responsive UI
- **Command Pattern** - User input parsing and execution
- **Composition over Inheritance** - Flexible, maintainable object relationships

## 🛠️ Technical Features

### Modern C# Features Used
- **Nullable Reference Types** for better null safety
- **Pattern Matching** with switch expressions  
- **Async/Await** for non-blocking operations
- **LINQ** for collection operations
- **Tuples** for multiple return values
- **Records** with value equality
- **Global Using** statements
- **Top-level programs**

### .NET 8 Features
- **Cross-platform** console applications
- **High-performance** collections (HashSet<T>)
- **CancellationToken** support for cooperative cancellation
- **Task.Run** for CPU-bound operations
- **Modern project file** format with implicit usings

## 🧪 Interesting Patterns to Try

Try these classic Game of Life patterns:

```bash
# Glider Gun (creates infinite gliders)
> clear
> # Set up specific pattern manually or use random with low probability
> random 0.1
> start

# Oscillators and Still Lives
> random 0.3
> start

# Explosive Growth  
> rules 2
> random 0.1
> start
```

## 🤝 Contributing

Contributions are welcome! This project was built as a learning exercise in modern C# and .NET development.

### Areas for Enhancement
- Add pattern loading/saving (RLE format)
- Implement zoom functionality
- Add color themes
- Create web-based version with ASP.NET Core
- Add unit tests
- Implement different grid topologies (torus, infinite)

### Getting Started
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📚 Learning Resources

This project demonstrates concepts from:

- **Object-Oriented Programming** - Encapsulation, inheritance, polymorphism
- **SOLID Principles** - Single responsibility, dependency inversion
- **Async Programming** - Task-based asynchronous pattern
- **Console Applications** - Advanced terminal/console manipulation
- **Game Development** - Game loops, state management, user input
- **Software Architecture** - Clean code, separation of concerns

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [John Conway](https://en.wikipedia.org/wiki/John_Horton_Conway) for creating the Game of Life
- The .NET team for the excellent .NET 8 runtime and tooling
- The C# language design team for modern language features

## 📊 Project Stats

- **Lines of Code**: ~800+
- **Classes**: 6 main classes demonstrating OOP principles
- **Features**: 10+ interactive commands
- **Patterns**: Multiple software design patterns implemented
- **Platform**: Cross-platform (.NET 8)
- **Performance**: Optimized for large grids and fast evolution

---

**Built with ❤️ using C# 12 and .NET 8 LTS**

*This project serves as an excellent example of modern C# development practices, clean architecture, and interactive console application design.*