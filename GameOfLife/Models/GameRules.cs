namespace GameOfLife.Models;

// Encapsulates the rules for Conway's Game of Life.
// Provides configurable rules for cell birth and survival conditions.
public class GameRules
{
    // Gets the set of neighbor counts that will cause a dead cell to become alive.
    // Default: {3} (standard Conway's Game of Life rule)
    public HashSet<int> BirthRules { get; private set; }

    // Gets the set of neighbor counts that will keep an alive cell alive.
    // Default: {2, 3} (standard Conway's Game of Life rule)
    public HashSet<int> SurvivalRules { get; private set; }

    // Default constructor - Conway's classic rules (B3/S23)
    public GameRules()
    {
        BirthRules = new HashSet<int> { 3 };        // B3
        SurvivalRules = new HashSet<int> { 2, 3 };  // S23
    }

    // Custom constructor with validation
    public GameRules(HashSet<int> birthRules, HashSet<int> survivalRules)
    {
        // Null checks with proper exception messages
        BirthRules = birthRules ?? throw new ArgumentNullException(nameof(birthRules));
        SurvivalRules = survivalRules ?? throw new ArgumentNullException(nameof(survivalRules));

        // Validate that rules contain only valid neighbor counts (0-8)
        ValidateRules();
    }

    // Determines if a cell should be alive in the next generation based on current state and neighbors
    public bool ShouldCellBeAlive(bool currentlyAlive, int neighborCount)
    {
        // Validate neighbor count is in valid range
        if (neighborCount < 0 || neighborCount > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(neighborCount),
                "Neighbor count must be between 0 and 8 inclusive.");
        }

        if (currentlyAlive)
        {
            // Cell is alive: check survival rules
            return SurvivalRules.Contains(neighborCount);
        }
        else
        {
            // Cell is dead: check birth rules
            return BirthRules.Contains(neighborCount);
        }
    }

    // Updates the birth rules with validation
    public void SetBirthRules(HashSet<int> newBirthRules)
    {
        BirthRules = newBirthRules ?? throw new ArgumentNullException(nameof(newBirthRules));
        ValidateRules();
    }

    // Updates the survival rules with validation
    public void SetSurvivalRules(HashSet<int> newSurvivalRules)
    {
        SurvivalRules = newSurvivalRules ?? throw new ArgumentNullException(nameof(newSurvivalRules));
        ValidateRules();
    }

    // Creates a deep copy of the current rules
    public GameRules Clone()
    {
        return new GameRules(
            new HashSet<int>(BirthRules),
            new HashSet<int>(SurvivalRules)
        );
    }

    // Returns string representation in B/S notation (e.g., "B3/S23")
    public override string ToString()
    {
        var birthString = string.Join("", BirthRules.OrderBy(x => x));
        var survivalString = string.Join("", SurvivalRules.OrderBy(x => x));
        return $"B{birthString}/S{survivalString}";
    }

    // Validates that all rules contain only valid neighbor counts (0-8)
    private void ValidateRules()
    {
        // Find any birth rules outside valid range (0-8)
        var invalidBirthRules = BirthRules.Where(count => count < 0 || count > 8).ToList();
        var invalidSurvivalRules = SurvivalRules.Where(count => count < 0 || count > 8).ToList();

        if (invalidBirthRules.Any())
        {
            throw new ArgumentException($"Invalid birth rules: {string.Join(", ", invalidBirthRules)}. " +
                                      "Neighbor counts must be between 0 and 8.");
        }

        if (invalidSurvivalRules.Any())
        {
            throw new ArgumentException($"Invalid survival rules: {string.Join(", ", invalidSurvivalRules)}. " +
                                      "Neighbor counts must be between 0 and 8.");
        }
    }

    // Override Equals for proper comparison
    public override bool Equals(object? obj)
    {
        if (obj is not GameRules other) return false;

        return BirthRules.SetEquals(other.BirthRules) &&
               SurvivalRules.SetEquals(other.SurvivalRules);
    }

    // Override GetHashCode for proper hashing
    public override int GetHashCode()
    {
        // Combine hash codes of all birth and survival rules
        int birthHash = BirthRules.Aggregate(0, (hash, rule) => hash ^ rule.GetHashCode());
        int survivalHash = SurvivalRules.Aggregate(0, (hash, rule) => hash ^ rule.GetHashCode());
        return HashCode.Combine(birthHash, survivalHash);
    }
}