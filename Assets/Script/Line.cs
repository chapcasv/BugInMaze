
public class Line 
{
    private Cell from;
    private Cell to;
    public Cell From { get => from; set => from = value; }
    public Cell To { get => to; set => to = value; }

    public Line(Cell from, Cell to)
    {
        From = from;
        To = to;
    }
}
