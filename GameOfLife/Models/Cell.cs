namespace GameOfLife.Models;

/*
 * Represents a single cell in the Game of Life grid.
 * Encapsulates the state and behavior of an individual cell.
 */

public class Cell
{
    public bool IsAlive { get; set; }
    public bool NextState { get; set; }
    
    public Cell(bool isAlive = false)
    {
        IsAlive = isAlive;
        NextState = false;
    }

    public void Evolve()
    {
        IsAlive = NextState;
    }

    public Cell Clone()
    {
        return new Cell(IsAlive)
        {
            NextState = NextState
        };
    }

    public override string ToString()
    {
        return IsAlive ? "█" : " ";
    }
}