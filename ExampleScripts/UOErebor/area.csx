using System;

public struct AreaRect : IEquatable<AreaRect>
{
    public Location2D TopLeft { get; }
    public Location2D BottomRight { get; }

    public bool InAreaRect(Location2D location) =>
        location.X >= TopLeft.X
        && location.Y >= TopLeft.Y
        && location.X <= BottomRight.X
        && location.Y <= BottomRight.Y;

    public AreaRect(int x1, int y1, int x2, int y2)
    {
        TopLeft = new Location2D(x1, y1);
        BottomRight = new Location2D(x2, y2);
    }
    
    public override bool Equals(object obj)
    {
		if (ReferenceEquals(null, obj))
		  return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        return Equals((AreaRect)obj);
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + TopLeft.GetHashCode();
        hash = (hash * 7) + BottomRight.GetHashCode();
        return hash;
    }

    public bool Equals(AreaRect other)
        => TopLeft == other.TopLeft && BottomRight == other.BottomRight;

}

public class Area
{
    public static Area Pole30 = new Area("pole 30", 2695,3235,2712,3253);
    public static Area Dum62 = new Area("dum 62", new AreaRect(851,1428,861,1450), new AreaRect(859,1428,866,1435));
    public static Area Dum84 = new Area("dum 84", 1682,2258,1701,2276);
    public static Area Dum188 = new Area("dum 188", 2715,3240,2725,3255);
    
    public static Area DulOsamely = new Area("Osamely dul", 1189,877,1248,933);
    public static Area DulZeleznehoPasu = new Area("Dul u Zelezneho pasu",
        new AreaRect(2007,2259,2059,2287),
        new AreaRect(2011,2283,2039,2294),
        new AreaRect(2036,2286,2042,2293));
    public static Area DolyKrajske = new Area("Krajske doly", 730,1509,781,1541);
    public static Area DulSamota = new Area("Dul Samota", 4919,2307,4958,2341);
    
    public readonly AreaRect[] rects;
    
    public Area(string name, int x1, int y1, int x2, int y2)
    {
        rects = new[] { new AreaRect(x1, y1, x2, y2) };
    }
    
    public Area(string name, params AreaRect[] rects)
    {
        this.rects = rects;
    }
    
    public bool InArea()
    {
        Location2D location = UO.Me.Location;
    
        foreach (var rect in rects)
        {
            if (rect.InAreaRect(location))
                return true;
        }
        
        return false;
    }
}