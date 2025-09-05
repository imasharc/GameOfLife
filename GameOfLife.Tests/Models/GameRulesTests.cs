using GameOfLife.Models;

namespace GameOfLife.Tests.Models;

public class GameRulesTests
{
    #region Constructor Tests

    [Fact]
    public void DefaultConstructor_ShouldCreateConwayRules()
    {
        // ARRANGE & ACT
        var rules = new GameRules();

        // ASSERT
        Assert.Contains(3, rules.BirthRules);
        Assert.Single(rules.BirthRules);
        Assert.Contains(2, rules.SurvivalRules);
        Assert.Contains(3, rules.SurvivalRules);
        Assert.Equal(2, rules.SurvivalRules.Count);
    }

    [Fact]
    public void CustomConstructor_WithValidRules_ShouldCreateCorrectly()
    {
        // ARRANGE
        var birthRules = new HashSet<int> { 3, 6 };
        var survivalRules = new HashSet<int> { 2, 3 };

        // ACT
        var rules = new GameRules(birthRules, survivalRules);

        // ASSERT
        Assert.True(rules.BirthRules.SetEquals(birthRules));
        Assert.True(rules.SurvivalRules.SetEquals(survivalRules));
    }

    [Fact]
    public void CustomConstructor_WithNullBirthRules_ShouldThrowArgumentNullException()
    {
        // ARRANGE
        var survivalRules = new HashSet<int> { 2, 3 };

        // ACT & ASSERT
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new GameRules(null!, survivalRules));
        Assert.Equal("birthRules", exception.ParamName);
    }

    [Fact]
    public void CustomConstructor_WithNullSurvivalRules_ShouldThrowArgumentNullException()
    {
        // ARRANGE
        var birthRules = new HashSet<int> { 3 };

        // ACT & ASSERT
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new GameRules(birthRules, null!));
        Assert.Equal("survivalRules", exception.ParamName);
    }

    [Fact]
    public void CustomConstructor_WithInvalidBirthRules_ShouldThrowArgumentException()
    {
        // ARRANGE
        var invalidBirthRules = new HashSet<int> { 3, 9, -1 };
        var survivalRules = new HashSet<int> { 2, 3 };

        // ACT & ASSERT
        var exception = Assert.Throws<ArgumentException>(() =>
            new GameRules(invalidBirthRules, survivalRules));
        Assert.Contains("Invalid birth rules", exception.Message);
        Assert.Contains("9", exception.Message);
        Assert.Contains("-1", exception.Message);
    }

    [Fact]
    public void CustomConstructor_WithInvalidSurvivalRules_ShouldThrowArgumentException()
    {
        // ARRANGE
        var birthRules = new HashSet<int> { 3 };
        var invalidSurvivalRules = new HashSet<int> { 2, 10 };

        // ACT & ASSERT
        var exception = Assert.Throws<ArgumentException>(() =>
            new GameRules(birthRules, invalidSurvivalRules));
        Assert.Contains("Invalid survival rules", exception.Message);
        Assert.Contains("10", exception.Message);
    }

    #endregion

    #region ShouldCellBeAlive Tests

    [Theory]
    [InlineData(false, 3, true)]  // Dead cell with 3 neighbors becomes alive
    [InlineData(false, 2, false)] // Dead cell with 2 neighbors stays dead
    [InlineData(false, 4, false)] // Dead cell with 4 neighbors stays dead
    [InlineData(true, 2, true)]   // Live cell with 2 neighbors survives
    [InlineData(true, 3, true)]   // Live cell with 3 neighbors survives
    [InlineData(true, 1, false)]  // Live cell with 1 neighbor dies
    [InlineData(true, 4, false)]  // Live cell with 4 neighbors dies
    public void ShouldCellBeAlive_WithConwayRules_ShouldReturnExpectedResult(
        bool currentlyAlive, int neighborCount, bool expectedResult)
    {
        // ARRANGE
        var rules = new GameRules(); // Default Conway rules

        // ACT
        var result = rules.ShouldCellBeAlive(currentlyAlive, neighborCount);

        // ASSERT
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(false, 3, true)]  // Birth on 3
    [InlineData(false, 6, true)]  // Birth on 6
    [InlineData(false, 2, false)] // No birth on 2
    [InlineData(true, 2, true)]   // Survival on 2
    [InlineData(true, 3, true)]   // Survival on 3
    [InlineData(true, 6, false)]  // Death on 6 (not in survival rules)
    public void ShouldCellBeAlive_WithCustomRules_ShouldReturnExpectedResult(
        bool currentlyAlive, int neighborCount, bool expectedResult)
    {
        // ARRANGE - HighLife rules (B36/S23)
        var birthRules = new HashSet<int> { 3, 6 };
        var survivalRules = new HashSet<int> { 2, 3 };
        var rules = new GameRules(birthRules, survivalRules);

        // ACT
        var result = rules.ShouldCellBeAlive(currentlyAlive, neighborCount);

        // ASSERT
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    [InlineData(10)]
    public void ShouldCellBeAlive_WithInvalidNeighborCount_ShouldThrowArgumentOutOfRangeException(
        int invalidNeighborCount)
    {
        // ARRANGE
        var rules = new GameRules();

        // ACT & ASSERT
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            rules.ShouldCellBeAlive(true, invalidNeighborCount));
        Assert.Equal("neighborCount", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(8)]
    public void ShouldCellBeAlive_WithValidBoundaryNeighborCounts_ShouldNotThrow(
        int validNeighborCount)
    {
        // ARRANGE
        var rules = new GameRules();

        // ACT & ASSERT - Should not throw
        rules.ShouldCellBeAlive(true, validNeighborCount);
        rules.ShouldCellBeAlive(false, validNeighborCount);
    }

    #endregion

    #region SetRules Tests

    [Fact]
    public void SetBirthRules_WithValidRules_ShouldUpdateCorrectly()
    {
        // ARRANGE
        var rules = new GameRules();
        var newBirthRules = new HashSet<int> { 3, 6, 8 };

        // ACT
        rules.SetBirthRules(newBirthRules);

        // ASSERT
        Assert.True(rules.BirthRules.SetEquals(newBirthRules));
    }

    [Fact]
    public void SetBirthRules_WithNull_ShouldThrowArgumentNullException()
    {
        // ARRANGE
        var rules = new GameRules();

        // ACT & ASSERT
        var exception = Assert.Throws<ArgumentNullException>(() =>
            rules.SetBirthRules(null!));
        Assert.Equal("newBirthRules", exception.ParamName);
    }

    [Fact]
    public void SetBirthRules_WithInvalidRules_ShouldThrowArgumentException()
    {
        // ARRANGE
        var rules = new GameRules();
        var invalidRules = new HashSet<int> { 3, 9 };

        // ACT & ASSERT
        Assert.Throws<ArgumentException>(() => rules.SetBirthRules(invalidRules));
    }

    [Fact]
    public void SetSurvivalRules_WithValidRules_ShouldUpdateCorrectly()
    {
        // ARRANGE
        var rules = new GameRules();
        var newSurvivalRules = new HashSet<int> { 1, 2, 3, 4, 5 };

        // ACT
        rules.SetSurvivalRules(newSurvivalRules);

        // ASSERT
        Assert.True(rules.SurvivalRules.SetEquals(newSurvivalRules));
    }

    [Fact]
    public void SetSurvivalRules_WithNull_ShouldThrowArgumentNullException()
    {
        // ARRANGE
        var rules = new GameRules();

        // ACT & ASSERT
        var exception = Assert.Throws<ArgumentNullException>(() =>
            rules.SetSurvivalRules(null!));
        Assert.Equal("newSurvivalRules", exception.ParamName);
    }

    #endregion

    #region Clone Tests

    [Fact]
    public void Clone_ShouldCreateIdenticalCopy()
    {
        // ARRANGE
        var birthRules = new HashSet<int> { 3, 6 };
        var survivalRules = new HashSet<int> { 2, 3, 4 };
        var original = new GameRules(birthRules, survivalRules);

        // ACT
        var clone = original.Clone();

        // ASSERT
        Assert.NotSame(original, clone);
        Assert.True(original.BirthRules.SetEquals(clone.BirthRules));
        Assert.True(original.SurvivalRules.SetEquals(clone.SurvivalRules));
    }

    [Fact]
    public void Clone_ModifyingClone_ShouldNotAffectOriginal()
    {
        // ARRANGE
        var original = new GameRules();
        var clone = original.Clone();
        var newBirthRules = new HashSet<int> { 1, 2, 3 };

        // ACT
        clone.SetBirthRules(newBirthRules);

        // ASSERT
        Assert.Contains(3, original.BirthRules);
        Assert.Single(original.BirthRules);
        Assert.True(clone.BirthRules.SetEquals(newBirthRules));
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithDefaultRules_ShouldReturnConwayNotation()
    {
        // ARRANGE
        var rules = new GameRules();

        // ACT
        var result = rules.ToString();

        // ASSERT
        Assert.Equal("B3/S23", result);
    }

    [Fact]
    public void ToString_WithCustomRules_ShouldReturnCorrectNotation()
    {
        // ARRANGE
        var birthRules = new HashSet<int> { 3, 6 };
        var survivalRules = new HashSet<int> { 2, 3 };
        var rules = new GameRules(birthRules, survivalRules);

        // ACT
        var result = rules.ToString();

        // ASSERT
        Assert.Equal("B36/S23", result);
    }

    [Fact]
    public void ToString_WithUnorderedRules_ShouldReturnSortedNotation()
    {
        // ARRANGE
        var birthRules = new HashSet<int> { 6, 3, 8 };
        var survivalRules = new HashSet<int> { 5, 2, 4 };
        var rules = new GameRules(birthRules, survivalRules);

        // ACT
        var result = rules.ToString();

        // ASSERT
        Assert.Equal("B368/S245", result);
    }

    [Fact]
    public void ToString_WithEmptyRules_ShouldReturnEmptyNotation()
    {
        // ARRANGE
        var birthRules = new HashSet<int>();
        var survivalRules = new HashSet<int>();
        var rules = new GameRules(birthRules, survivalRules);

        // ACT
        var result = rules.ToString();

        // ASSERT
        Assert.Equal("B/S", result);
    }

    #endregion

    #region Equals and GetHashCode Tests

    [Fact]
    public void Equals_WithSameRules_ShouldReturnTrue()
    {
        // ARRANGE
        var rules1 = new GameRules();
        var rules2 = new GameRules();

        // ACT & ASSERT
        Assert.True(rules1.Equals(rules2));
        Assert.True(rules2.Equals(rules1));
        Assert.Equal(rules1, rules2);
    }

    [Fact]
    public void Equals_WithDifferentRules_ShouldReturnFalse()
    {
        // ARRANGE
        var rules1 = new GameRules();
        var rules2 = new GameRules(new HashSet<int> { 3, 6 }, new HashSet<int> { 2, 3 });

        // ACT & ASSERT
        Assert.False(rules1.Equals(rules2));
        Assert.False(rules2.Equals(rules1));
        Assert.NotEqual(rules1, rules2);
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // ARRANGE
        var rules = new GameRules();

        // ACT & ASSERT
        Assert.False(rules.Equals(null));
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // ARRANGE
        var rules = new GameRules();
        var other = "not a GameRules";

        // ACT & ASSERT
        Assert.False(rules.Equals(other));
    }

    [Fact]
    public void GetHashCode_WithSameRules_ShouldReturnSameHashCode()
    {
        // ARRANGE
        var rules1 = new GameRules();
        var rules2 = new GameRules();

        // ACT
        var hash1 = rules1.GetHashCode();
        var hash2 = rules2.GetHashCode();

        // ASSERT
        Assert.Equal(hash1, hash2);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void FullWorkflow_CreateCustomRulesAndValidateLogic_ShouldWorkCorrectly()
    {
        // ARRANGE - Create Morley rules (B368/S245)
        var birthRules = new HashSet<int> { 3, 6, 8 };
        var survivalRules = new HashSet<int> { 2, 4, 5 };
        var rules = new GameRules(birthRules, survivalRules);

        // ACT & ASSERT - Test string representation
        Assert.Equal("B368/S245", rules.ToString());

        // ACT & ASSERT - Test logic
        Assert.True(rules.ShouldCellBeAlive(false, 3)); // Birth on 3
        Assert.True(rules.ShouldCellBeAlive(false, 6)); // Birth on 6
        Assert.True(rules.ShouldCellBeAlive(false, 8)); // Birth on 8
        Assert.False(rules.ShouldCellBeAlive(false, 2)); // No birth on 2

        Assert.True(rules.ShouldCellBeAlive(true, 2)); // Survive on 2
        Assert.True(rules.ShouldCellBeAlive(true, 4)); // Survive on 4
        Assert.True(rules.ShouldCellBeAlive(true, 5)); // Survive on 5
        Assert.False(rules.ShouldCellBeAlive(true, 3)); // Die on 3
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ValidateRules_WithAllValidNumbers_ShouldNotThrow()
    {
        // ARRANGE & ACT - Should not throw
        var allValidBirth = new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        var allValidSurvival = new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        // ASSERT
        Assert.NotNull(new GameRules(allValidBirth, allValidSurvival));
    }

    [Fact]
    public void SetRules_AfterValidation_ShouldMaintainRulesIntegrity()
    {
        // ARRANGE
        var rules = new GameRules();
        var newBirthRules = new HashSet<int> { 2, 3, 4 };
        var newSurvivalRules = new HashSet<int> { 1, 2, 3, 4, 5 };

        // ACT
        rules.SetBirthRules(newBirthRules);
        rules.SetSurvivalRules(newSurvivalRules);

        // ASSERT
        Assert.Equal("B234/S12345", rules.ToString());
        Assert.True(rules.ShouldCellBeAlive(false, 2));
        Assert.True(rules.ShouldCellBeAlive(true, 5));
        Assert.False(rules.ShouldCellBeAlive(false, 1));
        Assert.False(rules.ShouldCellBeAlive(true, 6));
    }

    #endregion
}