using DataStructure;

public class QuadTreeNode
{
    private const int Capacity = 4;
    public Rectangle Boundary { get; private set; }
    public List<Point> Points { get; private set; }
    public bool IsDivided { get; private set; }
    public QuadTreeNode NE { get; private set; }
    public QuadTreeNode NW { get; private set; }
    public QuadTreeNode SE { get; private set; }
    public QuadTreeNode SW { get; private set; }

    public QuadTreeNode(Rectangle boundary)
    {
        Boundary = boundary;
        Points = new List<Point>();
        IsDivided = false;
    }

    public bool Insert(Point point)
    {
        if (!Boundary.Contains(point))
            return false;

        if (Points.Count < Capacity)
        {
            Points.Add(point);
            return true;
        }
        else
        {
            if (!IsDivided)
                Subdivide();

            if (NE.Insert(point)) return true;
            if (NW.Insert(point)) return true;
            if (SE.Insert(point)) return true;
            if (SW.Insert(point)) return true;
        }

        return false;
    }

    private void Subdivide()
    {
        double x = Boundary.X;
        double y = Boundary.Y;
        double w = Boundary.Width / 2;
        double h = Boundary.Height / 2;

        Rectangle ne = new Rectangle(x + w, y - h, w, h);
        NE = new QuadTreeNode(ne);

        Rectangle nw = new Rectangle(x - w, y - h, w, h);
        NW = new QuadTreeNode(nw);

        Rectangle se = new Rectangle(x + w, y + h, w, h);
        SE = new QuadTreeNode(se);

        Rectangle sw = new Rectangle(x - w, y + h, w, h);
        SW = new QuadTreeNode(sw);

        IsDivided = true;
    }

    public void Query(Rectangle range, List<Point> found)
    {
        if (!Boundary.Intersects(range))
            return;

        foreach (var point in Points)
        {
            if (range.Contains(point))
                found.Add(point);
        }

        if (IsDivided)
        {
            NE.Query(range, found);
            NW.Query(range, found);
            SE.Query(range, found);
            SW.Query(range, found);
        }
    }
}
