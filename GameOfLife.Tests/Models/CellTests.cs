using GameOfLife.Models;

namespace GameOfLife.Tests.Models;

public class CellTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNo_Parameters_ShouldCreateDeadCell()
    {
        // ARRANGE & ACT
        var cell = new Cell();

        // ASSERT
        Assert.False(cell.IsAlive);
        Assert.False(cell.NextState);
    }

    [Fact]
    public void Constructor_WithFalseParameter_ShouldCreaateDealCell()
    {
        // ARRANGE & ACT
        var cell = new Cell();

        // ASSERT
        Assert.False(cell.IsAlive);
        Assert.False(cell.NextState);
    }

    [Fact]
    public void Constructor_WithTrueParameter_ShouldCreateAliveCell()
    {
        // ARRANGE & ACT
        var cell = new Cell(true);

        // ASSERT
        Assert.True(cell.IsAlive);
        Assert.False(cell.NextState); // NextState should always start as false
    }

    #endregion

    #region Property Tests

    [Fact]
    public void IsAlive_WhenSet_ShouldUpdateCorrectly()
    {
        // ARRANGE
        var cell = new Cell();

        // ACT
        cell.IsAlive = true;

        // ASSERT
        Assert.True(cell.IsAlive);
    }

    [Fact]
    public void NextState_WhenSet_ShouldUpdateCorrectly()
    {
        // ARRANGE
        var cell = new Cell();

        // ACT
        cell.NextState = true;

        // ASSERT
        Assert.True(cell.NextState);
        Assert.False(cell.IsAlive); // Should not affect current state
    }

    #endregion

    #region Evolve Method Tests

    [Fact]
    public void Evolve_WhenNextStateIsTrue_ShouldMakeCellAlive()
    {
        // ARRANGE
        var cell = new Cell(false);
        cell.NextState = true;

        // ACT
        cell.Evolve();

        // ASSERT
        Assert.True(cell.IsAlive);
    }

    [Fact]
    public void Evolve_WhenNextStateIsFalse_ShouldMakeCellDead()
    {
        // ARRANGE
        var cell = new Cell(true);
        cell.NextState = false;

        // ACT
        cell.Evolve();

        // ASSERT
        Assert.False(cell.IsAlive);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    public void Evolve_WithVariousStates_ShouldSetIsAliveToNextState(
        bool initialState, bool nextState, bool expectedFinalState)
    {
        // ARRANGE
        var cell = new Cell(initialState);
        cell.NextState = nextState;

        // ACT
        cell.Evolve();

        // ASSERT
        Assert.Equal(expectedFinalState, cell.IsAlive);
    }

    #endregion

    #region Clone Method Tests

    [Fact]
    public void Clone_WithDeadCell_ShouldCreateIdenticalCell()
    {
        // ARRANGE
        var original = new Cell(false);
        original.NextState = true;

        // ACT
        var clone = original.Clone();

        // ASSERT
        Assert.False(clone.IsAlive);
        Assert.True(clone.NextState);
        Assert.NotSame(original, clone); // Different instances
    }

    [Fact]
    public void Clone_WithAliveCell_ShouldCreateIdenticalCell()
    {
        // ARRANGE
        var original = new Cell(true);
        original.NextState = false;

        // ACT
        var clone = original.Clone();

        // ASSERT
        Assert.True(clone.IsAlive);
        Assert.False(clone.NextState);
        Assert.NotSame(original, clone); // Different instances
    }

    [Fact]
    public void Clone_ModifyingClone_ShouldNotAffectOriginal()
    {
        // ARRANGE
        var original = new Cell(true);
        var clone = original.Clone();

        // ACT
        clone.IsAlive = false;
        clone.NextState = true;

        // ASSERT
        Assert.True(original.IsAlive);   // Original unchanged
        Assert.False(original.NextState); // Original unchanged
        Assert.False(clone.IsAlive);     // Clone changed
        Assert.True(clone.NextState);    // Clone changed
    }

    #endregion

    #region ToString Method Tests

    [Fact]
    public void ToString_WithDeadCell_ShouldReturnSpace()
    {
        // ARRANGE
        var cell = new Cell(false);

        // ACT
        var result = cell.ToString();

        // ASSERT
        Assert.Equal(" ", result);
    }

    [Fact]
    public void ToString_WithAliveCell_ShouldReturnBlock()
    {
        // ARRANGE
        var cell = new Cell(true);

        // ACT
        var result = cell.ToString();

        // ASSERT
        Assert.Equal("█", result);
    }

    [Theory]
    [InlineData(false, " ")]
    [InlineData(true, "█")]
    public void ToString_WithVariousStates_ShouldReturnCorrectCharacter(
        bool isAlive, string expectedString)
    {
        // ARRANGE
        var cell = new Cell(isAlive);

        // ACT
        var result = cell.ToString();

        // ASSERT
        Assert.Equal(expectedString, result);
    }

    #endregion

    #region Integration Tests (Testing combinations)

    [Fact]
    public void FullLifecycle_DeadToAliveToDeadCell_ShouldWorkCorrectly()
    {
        // ARRANGE
        var cell = new Cell(false);

        // ACT & ASSERT - Step 1: Dead cell
        Assert.False(cell.IsAlive);
        Assert.Equal(" ", cell.ToString());

        // ACT & ASSERT - Step 2: Set to become alive
        cell.NextState = true;
        cell.Evolve();
        Assert.True(cell.IsAlive);
        Assert.Equal("█", cell.ToString());

        // ACT & ASSERT - Step 3: Set to become dead again
        cell.NextState = false;
        cell.Evolve();
        Assert.False(cell.IsAlive);
        Assert.Equal(" ", cell.ToString());
    }

    #endregion

    #region Edge Cases and Error Conditions

    [Fact]
    public void Evolve_CalledMultipleTimes_ShouldBeIdempotent()
    {
        // ARRANGE
        var cell = new Cell(false);
        cell.NextState = true;

        // ACT
        cell.Evolve();
        var firstResult = cell.IsAlive;
        cell.Evolve(); // Call again
        var secondResult = cell.IsAlive;

        // ASSERT
        Assert.True(firstResult);
        Assert.True(secondResult);
        Assert.Equal(firstResult, secondResult);
    }

    #endregion
}