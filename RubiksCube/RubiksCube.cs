using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube
{
    class Coordinates
    {
        public Coordinates(int x0, int y0, int z0)
        {
            x = Math.Sign(x0);
            y = Math.Sign(y0);
            z = Math.Sign(z0);
        }
        public Coordinates(Coordinates other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
        }
        public int X
        {
            get { return x; }
        }
        public int Y
        {
            get { return y; }
        }
        public int Z
        {
            get { return z; }
        }

        public static bool operator ==(Coordinates c1, Coordinates c2)
        {
            if (object.ReferenceEquals(c1, null) && object.ReferenceEquals(c2, null)) return true;
            if (object.ReferenceEquals(c1, null) || object.ReferenceEquals(c2, null)) return false;
            return c1.X == c2.X && c1.Y == c2.Y && c1.Z == c2.Z;
        }
        public static bool operator !=(Coordinates c1, Coordinates c2)
        {
            if (object.ReferenceEquals(c1, null) && object.ReferenceEquals(c2, null)) return false;
            if (object.ReferenceEquals(c1, null) || object.ReferenceEquals(c2, null)) return true;
            return c1.X != c2.X || c1.Y != c2.Y || c1.Z == c2.Z;
        }

        public override bool Equals(object obj)
        {
            return this == (Coordinates)obj;
        }

        public static Coordinates BuildVector(EdgeNum en)
        {
            Coordinates res = null;
            switch (en)
            {
                case EdgeNum.U:
                    res = new Coordinates(0, 0, 1);
                    break;
                case EdgeNum.F:
                    res = new Coordinates(1, 0, 0);
                    break;
                case EdgeNum.R:
                    res = new Coordinates(0, 1, 0);
                    break;
                case EdgeNum.L:
                    res = new Coordinates(0, -1, 0);
                    break;
                case EdgeNum.B:
                    res = new Coordinates(-1, 0, 0);
                    break;
                case EdgeNum.D:
                    res = new Coordinates(0, 0, -1);
                    break;
            }
            return res;
        }
        public static Coordinates BuildOpposite(Coordinates c)
        {
            return new Coordinates(-c.X, -c.Y, -c.Z);
        }
        public static Coordinates Turn(Coordinates c, bool x0, bool y0, bool z0, bool clockwise)
        {
            int count = 0;
            if (x0) ++count;
            if (y0) ++count;
            if (z0) ++count;
            if (count != 1) throw new ArgumentException();
            int x = 0, y = 0, z = 0;
            int factor = 1;
            if (!clockwise) factor = -1;
            if (x0)
            {
                x = c.X;
                z = factor * c.Y;
                y = factor * (-c.Z);
            }
            else if (y0)
            {
                y = c.Y;
                x = factor * c.Z;
                z = factor * (-c.X);
            }
            else if (z0)
            {
                z = c.Z;
                x = factor * c.Y;
                y = factor * (-c.X);
            }
            return new Coordinates(x, y, z);
        }

        private int x;
        private int y;
        private int z;
    }
    class RGBColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public RGBColor() { R = 0; G = 0; B = 0; }
        public RGBColor(byte r, byte g, byte b) { R = r; G = g; B = b; }
        public RGBColor(RGBColor other) { R = other.R; G = other.G; B = other.B; }

        public static bool operator ==(RGBColor a, RGBColor b)
        {
            return (a.R == b.R && a.G == b.G && a.B == b.B);
        }
        public static bool operator !=(RGBColor a, RGBColor b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (RGBColor)obj;
        }

        public static readonly RGBColor White = new RGBColor(255, 255, 255);
        public static readonly RGBColor Green = new RGBColor(0, 140, 0);
        public static readonly RGBColor Red = new RGBColor(255, 0, 0);
        public static readonly RGBColor Orange = new RGBColor(255, 165, 0);
        public static readonly RGBColor Blue = new RGBColor(0, 0, 255);
        public static readonly RGBColor Yellow = new RGBColor(255, 255, 0);
        public static readonly RGBColor[] Colors = { White, Green, Red, Orange, Blue, Yellow };
        public static readonly RGBColor Black = new RGBColor(0, 0, 0);
    }

    enum EdgeNum { U, F, R, L, B, D }
    enum Color { W, G, R, O, B, Y }

    class OrientedColor
    {
        public OrientedColor(Color c0, Coordinates coord0)
        {
            c = c0;
            coord = coord0;
        }
        public OrientedColor(OrientedColor other)
        {
            c = other.c;
            coord = new Coordinates(other.coord);
        }

        public Color C { get { return c; } }
        public Coordinates Coord { get { return coord; } }

        public static bool operator ==(OrientedColor oc1, OrientedColor oc2)
        {
            return oc1.C == oc2.C && oc1.Coord == oc2.Coord;
        }
        public static bool operator !=(OrientedColor oc1, OrientedColor oc2)
        {
            return oc1.C != oc2.C || oc1.Coord != oc2.Coord;
        }
        public override bool Equals(object obj)
        {
            return this == (OrientedColor)obj;
        }

        private Color c;
        private Coordinates coord;
    }

    class OrientedColorColorEqualityComparer : IEqualityComparer<OrientedColor>
    {
        public bool Equals(OrientedColor oc1, OrientedColor oc2)
        {
            return oc1.C == oc2.C;
        }
        public int GetHashCode(OrientedColor oc)
        {
            return oc.C.GetHashCode();
        }
    }
    class OrientedColorCoordEqualityComparer : IEqualityComparer<OrientedColor>
    {
        public bool Equals(OrientedColor oc1, OrientedColor oc2)
        {
            return oc1.Coord == oc2.Coord;
        }
        public int GetHashCode(OrientedColor oc)
        {
            return oc.C.GetHashCode();
        }
    }

    class Cube
    {
        public Cube(List<OrientedColor> colors0, Coordinates coord0)
        {
            for (int i = 0; i < colors0.Count; ++i)
            {
                for (int j = 0; j < colors0.Count; ++j)
                {
                    if (i == j) continue;
                    if (colors0[i].Coord == colors0[j].Coord) throw new ArgumentException();
                }
            }
            colors = colors0;
            coord = coord0;
        }
        public Cube(Cube other)
        {
            colors = new List<OrientedColor>(other.colors);
            coord = new Coordinates(other.coord);
        }

        public List<OrientedColor> Colors { get { return colors; } }
        public Coordinates Coord { get { return coord; } }

        public static bool operator ==(Cube c1, Cube c2)
        {
            if (object.ReferenceEquals(c1, null) && object.ReferenceEquals(c2, null)) return true;
            if (object.ReferenceEquals(c1, null) || object.ReferenceEquals(c2, null)) return false;
            bool coloreq = c1.Colors.Count == c2.Colors.Count;
            for (int i = 0; i < c1.Colors.Count && coloreq; ++i)
            {
                if (!c1.Colors.Contains(c2.Colors[i])) coloreq = false;
            }
            return coloreq && c1.Coord == c2.Coord;
        }
        public static bool operator !=(Cube c1, Cube c2)
        {
            return !(c1 == c2);
        }
        public override bool Equals(object obj)
        {
            return this == (Cube)obj;
        }

        private List<OrientedColor> colors;
        private Coordinates coord;
    }
    //class Edge
    //{
    //    public Edge(EdgeNum en0, Cube[,] cubes0)
    //    {
    //        en = en0;
    //        cubes = cubes0;
    //    }
    //    public void Rotate(bool on180, bool anticlockwise)
    //    {
    //        int x = 0, y = 0;
    //        Cube[,] res = new Cube[3, 3];
    //        Cube cur = null;
    //        List<OrientedColor> loc = new List<OrientedColor>();
    //        bool xOr = (en==EdgeNum.F || en==EdgeNum.B);
    //        bool yOr = (en==EdgeNum.R || en==EdgeNum.L);
    //        bool zOr = (en==EdgeNum.U || en==EdgeNum.D);
    //        for (int i = 0; i < 3; ++i)
    //        {
    //            for (int j = 0; j < 3; ++j)
    //            {
    //                if (on180)
    //                {
    //                    x = 2 - i;
    //                    y = 2 - j;
    //                }
    //                else if (!anticlockwise)
    //                {
    //                    x = 2 - j;
    //                    y = i;
    //                }
    //                else
    //                {
    //                    x = j;
    //                    y = 2 - i;
    //                }
    //                cur = cubes[x, y];
    //                loc.Clear();
    //                foreach (OrientedColor oc in cur.Colors)
    //                {
    //                    if (oc.Coord == Coordinates.BuildVector(en)) loc.Add(oc);
    //                    else if(on180) loc.Add(new OrientedColor(oc.C, Coordinates.BuildOpposite(oc.Coord)));
    //                    else loc.Add(new OrientedColor(oc.C, Coordinates.Turn(oc.Coord, xOr, yOr, zOr, !anticlockwise)));
    //                }
    //                res[i, j] = new Cube(loc, cur.Coord);
    //            }
    //        }
    //        cubes = res;
    //    }
    //    //Здесь Ox, Oy, Oz направлены как в OpenGL
    //    public void Refocus(bool x, bool y, bool z, bool anticlockwise)
    //    {
    //        bool xOr = (en == EdgeNum.F || en == EdgeNum.B);
    //        bool yOr = (en == EdgeNum.R || en == EdgeNum.L);
    //        bool zOr = (en == EdgeNum.U || en == EdgeNum.D);
    //        if ((x && yOr) || (y && zOr) || (z && xOr)) Rotate(false, (int)en < 3 ? anticlockwise : !anticlockwise);
    //        Cube[,] res = new Cube[3, 3];

    //        cubes = res;
    //    }
    //    public Cube[,] Cubes{get{return cubes;}}
    //    public EdgeNum EN{get{return en;}}

    //    private Cube[,] cubes;
    //    private EdgeNum en;
    //}

    abstract class Rotation
    {
        public bool Anticlockwise
        {
            get { return anticlockwise; }
        }
        protected bool anticlockwise;
    }
    class EdgeRotation : Rotation
    {
        public EdgeRotation(EdgeNum en0, bool on180_, bool anticlockwise0)
        {
            en = en0;
            on180 = on180_;
            if (on180) anticlockwise = false;
            else anticlockwise = anticlockwise0;
        }

        public EdgeNum EN
        {
            get { return en; }
        }
        public bool On180
        {
            get { return on180; }
        }
        //public bool Anticlockwise
        //{
        //    get { return anticlockwise; }
        //}

        private EdgeNum en;
        private bool on180;
        //private bool anticlockwise;
    }
    class CubeRotation : Rotation
    {
        public CubeRotation(bool x0, bool y0, bool z0, bool anticlockwise0)
        {
            int count = 0;
            if (x0) ++count;
            if (y0) ++count;
            if (z0) ++count;
            if (count != 1) throw new ArgumentException();
            anticlockwise = anticlockwise0;
            x = x0;
            y = y0;
            z = z0;
        }

        public bool X
        {
            get { return x; }
        }
        public bool Y
        {
            get { return y; }
        }
        public bool Z
        {
            get { return z; }
        }
        //public bool Anticlockwise
        //{
        //    get { return anticlockwise; }
        //}

        private bool x;
        private bool y;
        private bool z;
        //private bool anticlockwise;
    }

    class CubeRotationEventArgs : EventArgs
    {
        public CubeRotation CR { get; set; }
    }
    class EdgeRotationEventArgs : EventArgs
    {
        public EdgeRotation ER { get; set; }
    }

    delegate void CubeRotationHandler(object sender, CubeRotationEventArgs args);
    delegate void EdgeRotationHandler(object sender, EdgeRotationEventArgs args);

    class RubiksCube
    {
        public RubiksCube()
        {
            colors = new Color[6, 3, 3];
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        colors[i, j, k] = (Color)i;
                    }
                }
            }
        }

        public RubiksCube(RubiksCube other)
        {
            colors = new Color[6, 3, 3];
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        colors[i, j, k] = other.colors[i, j, k];
                    }
                }
            }
        }

        public event CubeRotationHandler CubeRotationEvent;
        public event EdgeRotationHandler EdgeRotationEvent;

        public Color[, ,] Colors { get { return colors; } }

        public void RotateEdge(EdgeRotation er)
        {
            if(EdgeRotationEvent!=null)
            {
                EdgeRotationEvent(this, new EdgeRotationEventArgs { ER = er });
            }
            int ind = (int)er.EN;
            Color tmp;
            if (er.On180)
            {
                Swap(ref colors[ind, 0, 0], ref colors[ind, 2, 2]);
                Swap(ref colors[ind, 1, 0], ref colors[ind, 1, 2]);
                Swap(ref colors[ind, 2, 0], ref colors[ind, 0, 2]);
                Swap(ref colors[ind, 0, 1], ref colors[ind, 2, 1]);
            }
            else
            {
                if ((ind <= 2 && !er.Anticlockwise) || (ind > 2 && er.Anticlockwise))
                {
                    tmp = colors[ind, 0, 0];
                    colors[ind, 0, 0] = colors[ind, 2, 0];
                    colors[ind, 2, 0] = colors[ind, 2, 2];
                    colors[ind, 2, 2] = colors[ind, 0, 2];
                    colors[ind, 0, 2] = tmp;

                    tmp = colors[ind, 1, 0];
                    colors[ind, 1, 0] = colors[ind, 2, 1];
                    colors[ind, 2, 1] = colors[ind, 1, 2];
                    colors[ind, 1, 2] = colors[ind, 0, 1];
                    colors[ind, 0, 1] = tmp;
                }
                else
                {
                    tmp = colors[ind, 0, 0];
                    colors[ind, 0, 0] = colors[ind, 0, 2];
                    colors[ind, 0, 2] = colors[ind, 2, 2];
                    colors[ind, 2, 2] = colors[ind, 2, 0];
                    colors[ind, 2, 0] = tmp;

                    tmp = colors[ind, 1, 0];
                    colors[ind, 1, 0] = colors[ind, 0, 1];
                    colors[ind, 0, 1] = colors[ind, 1, 2];
                    colors[ind, 1, 2] = colors[ind, 2, 1];
                    colors[ind, 2, 1] = tmp;
                }
            }
            Color c1, c2, c3;
            switch (er.EN)
            {
                #region U
                case EdgeNum.U:
                    if (er.On180)
                    {
                        Swap(ref colors[(int)EdgeNum.F, 0, 0], ref colors[(int)EdgeNum.B, 0, 2]);
                        Swap(ref colors[(int)EdgeNum.F, 0, 1], ref colors[(int)EdgeNum.B, 0, 1]);
                        Swap(ref colors[(int)EdgeNum.F, 0, 2], ref colors[(int)EdgeNum.B, 0, 0]);

                        Swap(ref colors[(int)EdgeNum.R, 0, 0], ref colors[(int)EdgeNum.L, 0, 2]);
                        Swap(ref colors[(int)EdgeNum.R, 0, 1], ref colors[(int)EdgeNum.L, 0, 1]);
                        Swap(ref colors[(int)EdgeNum.R, 0, 2], ref colors[(int)EdgeNum.L, 0, 0]);
                    }
                    else if (!er.Anticlockwise)
                    {
                        c1 = colors[(int)EdgeNum.F, 0, 0];
                        c2 = colors[(int)EdgeNum.F, 0, 1];
                        c3 = colors[(int)EdgeNum.F, 0, 2];

                        colors[(int)EdgeNum.F, 0, 0] = colors[(int)EdgeNum.R, 0, 0];
                        colors[(int)EdgeNum.F, 0, 1] = colors[(int)EdgeNum.R, 0, 1];
                        colors[(int)EdgeNum.F, 0, 2] = colors[(int)EdgeNum.R, 0, 2];

                        colors[(int)EdgeNum.R, 0, 0] = colors[(int)EdgeNum.B, 0, 2];
                        colors[(int)EdgeNum.R, 0, 1] = colors[(int)EdgeNum.B, 0, 1];
                        colors[(int)EdgeNum.R, 0, 2] = colors[(int)EdgeNum.B, 0, 0];

                        colors[(int)EdgeNum.B, 0, 2] = colors[(int)EdgeNum.L, 0, 2];
                        colors[(int)EdgeNum.B, 0, 1] = colors[(int)EdgeNum.L, 0, 1];
                        colors[(int)EdgeNum.B, 0, 0] = colors[(int)EdgeNum.L, 0, 0];

                        colors[(int)EdgeNum.L, 0, 2] = c1;
                        colors[(int)EdgeNum.L, 0, 1] = c2;
                        colors[(int)EdgeNum.L, 0, 0] = c3;
                    }
                    else
                    {
                        c1 = colors[(int)EdgeNum.F, 0, 0];
                        c2 = colors[(int)EdgeNum.F, 0, 1];
                        c3 = colors[(int)EdgeNum.F, 0, 2];

                        colors[(int)EdgeNum.F, 0, 0] = colors[(int)EdgeNum.L, 0, 2];
                        colors[(int)EdgeNum.F, 0, 1] = colors[(int)EdgeNum.L, 0, 1];
                        colors[(int)EdgeNum.F, 0, 2] = colors[(int)EdgeNum.L, 0, 0];

                        colors[(int)EdgeNum.L, 0, 2] = colors[(int)EdgeNum.B, 0, 2];
                        colors[(int)EdgeNum.L, 0, 1] = colors[(int)EdgeNum.B, 0, 1];
                        colors[(int)EdgeNum.L, 0, 0] = colors[(int)EdgeNum.B, 0, 0];

                        colors[(int)EdgeNum.B, 0, 2] = colors[(int)EdgeNum.R, 0, 0];
                        colors[(int)EdgeNum.B, 0, 1] = colors[(int)EdgeNum.R, 0, 1];
                        colors[(int)EdgeNum.B, 0, 0] = colors[(int)EdgeNum.R, 0, 2];

                        colors[(int)EdgeNum.R, 0, 0] = c1;
                        colors[(int)EdgeNum.R, 0, 1] = c2;
                        colors[(int)EdgeNum.R, 0, 2] = c3;
                    }
                    break;
#endregion
                #region F
                case EdgeNum.F:
                    if (er.On180)
                    {
                        Swap(ref colors[(int)EdgeNum.U, 2, 0], ref colors[(int)EdgeNum.D, 2, 2]);
                        Swap(ref colors[(int)EdgeNum.U, 2, 1], ref colors[(int)EdgeNum.D, 2, 1]);
                        Swap(ref colors[(int)EdgeNum.U, 2, 2], ref colors[(int)EdgeNum.D, 2, 0]);

                        Swap(ref colors[(int)EdgeNum.R, 0, 0], ref colors[(int)EdgeNum.L, 2, 0]);
                        Swap(ref colors[(int)EdgeNum.R, 1, 0], ref colors[(int)EdgeNum.L, 1, 0]);
                        Swap(ref colors[(int)EdgeNum.R, 2, 0], ref colors[(int)EdgeNum.L, 0, 0]);
                    }
                    else if (!er.Anticlockwise)
                    {
                        c1 = colors[(int)EdgeNum.U, 2, 0];
                        c2 = colors[(int)EdgeNum.U, 2, 1];
                        c3 = colors[(int)EdgeNum.U, 2, 2];

                        colors[(int)EdgeNum.U, 2, 0] = colors[(int)EdgeNum.L, 2, 0];
                        colors[(int)EdgeNum.U, 2, 1] = colors[(int)EdgeNum.L, 1, 0];
                        colors[(int)EdgeNum.U, 2, 2] = colors[(int)EdgeNum.L, 0, 0];

                        colors[(int)EdgeNum.L, 2, 0] = colors[(int)EdgeNum.D, 2, 2];
                        colors[(int)EdgeNum.L, 1, 0] = colors[(int)EdgeNum.D, 2, 1];
                        colors[(int)EdgeNum.L, 0, 0] = colors[(int)EdgeNum.D, 2, 0];

                        colors[(int)EdgeNum.D, 2, 2] = colors[(int)EdgeNum.R, 0, 0];
                        colors[(int)EdgeNum.D, 2, 1] = colors[(int)EdgeNum.R, 1, 0];
                        colors[(int)EdgeNum.D, 2, 0] = colors[(int)EdgeNum.R, 2, 0];

                        colors[(int)EdgeNum.R, 0, 0] = c1;
                        colors[(int)EdgeNum.R, 1, 0] = c2;
                        colors[(int)EdgeNum.R, 2, 0] = c3;
                    }
                    else
                    {
                        c1 = colors[(int)EdgeNum.U, 2, 0];
                        c2 = colors[(int)EdgeNum.U, 2, 1];
                        c3 = colors[(int)EdgeNum.U, 2, 2];

                        colors[(int)EdgeNum.U, 2, 0] = colors[(int)EdgeNum.R, 0, 0];
                        colors[(int)EdgeNum.U, 2, 1] = colors[(int)EdgeNum.R, 1, 0];
                        colors[(int)EdgeNum.U, 2, 2] = colors[(int)EdgeNum.R, 2, 0];

                        colors[(int)EdgeNum.R, 0, 0] = colors[(int)EdgeNum.D, 2, 2];
                        colors[(int)EdgeNum.R, 1, 0] = colors[(int)EdgeNum.D, 2, 1];
                        colors[(int)EdgeNum.R, 2, 0] = colors[(int)EdgeNum.D, 2, 0];

                        colors[(int)EdgeNum.D, 2, 2] = colors[(int)EdgeNum.L, 2, 0];
                        colors[(int)EdgeNum.D, 2, 1] = colors[(int)EdgeNum.L, 1, 0];
                        colors[(int)EdgeNum.D, 2, 0] = colors[(int)EdgeNum.L, 0, 0];

                        colors[(int)EdgeNum.L, 2, 0] = c1;
                        colors[(int)EdgeNum.L, 1, 0] = c2;
                        colors[(int)EdgeNum.L, 0, 0] = c3;
                    }
                    break;
#endregion
                #region R
                case EdgeNum.R:
                    if (er.On180)
                    {
                        Swap(ref colors[(int)EdgeNum.U, 2, 2], ref colors[(int)EdgeNum.D, 0, 2]);
                        Swap(ref colors[(int)EdgeNum.U, 1, 2], ref colors[(int)EdgeNum.D, 1, 2]);
                        Swap(ref colors[(int)EdgeNum.U, 0, 2], ref colors[(int)EdgeNum.D, 2, 2]);

                        Swap(ref colors[(int)EdgeNum.F, 2, 2], ref colors[(int)EdgeNum.B, 0, 2]);
                        Swap(ref colors[(int)EdgeNum.F, 1, 2], ref colors[(int)EdgeNum.B, 1, 2]);
                        Swap(ref colors[(int)EdgeNum.F, 0, 2], ref colors[(int)EdgeNum.B, 2, 2]);
                    }
                    else if (!er.Anticlockwise)
                    {
                        c1 = colors[(int)EdgeNum.U, 0, 2];
                        c2 = colors[(int)EdgeNum.U, 1, 2];
                        c3 = colors[(int)EdgeNum.U, 2, 2];

                        colors[(int)EdgeNum.U, 0, 2] = colors[(int)EdgeNum.F, 0, 2];
                        colors[(int)EdgeNum.U, 1, 2] = colors[(int)EdgeNum.F, 1, 2];
                        colors[(int)EdgeNum.U, 2, 2] = colors[(int)EdgeNum.F, 2, 2];

                        colors[(int)EdgeNum.F, 0, 2] = colors[(int)EdgeNum.D, 2, 2];
                        colors[(int)EdgeNum.F, 1, 2] = colors[(int)EdgeNum.D, 1, 2];
                        colors[(int)EdgeNum.F, 2, 2] = colors[(int)EdgeNum.D, 0, 2];

                        colors[(int)EdgeNum.D, 2, 2] = colors[(int)EdgeNum.B, 2, 2];
                        colors[(int)EdgeNum.D, 1, 2] = colors[(int)EdgeNum.B, 1, 2];
                        colors[(int)EdgeNum.D, 0, 2] = colors[(int)EdgeNum.B, 0, 2];

                        colors[(int)EdgeNum.B, 2, 2] = c1;
                        colors[(int)EdgeNum.B, 1, 2] = c2;
                        colors[(int)EdgeNum.B, 0, 2] = c3;
                    }
                    else
                    {
                        c1 = colors[(int)EdgeNum.U, 0, 2];
                        c2 = colors[(int)EdgeNum.U, 1, 2];
                        c3 = colors[(int)EdgeNum.U, 2, 2];

                        colors[(int)EdgeNum.U, 0, 2] = colors[(int)EdgeNum.B, 2, 2];
                        colors[(int)EdgeNum.U, 1, 2] = colors[(int)EdgeNum.B, 1, 2];
                        colors[(int)EdgeNum.U, 2, 2] = colors[(int)EdgeNum.B, 0, 2];

                        colors[(int)EdgeNum.B, 2, 2] = colors[(int)EdgeNum.D, 2, 2];
                        colors[(int)EdgeNum.B, 1, 2] = colors[(int)EdgeNum.D, 1, 2];
                        colors[(int)EdgeNum.B, 0, 2] = colors[(int)EdgeNum.D, 0, 2];

                        colors[(int)EdgeNum.D, 2, 2] = colors[(int)EdgeNum.F, 0, 2];
                        colors[(int)EdgeNum.D, 1, 2] = colors[(int)EdgeNum.F, 1, 2];
                        colors[(int)EdgeNum.D, 0, 2] = colors[(int)EdgeNum.F, 2, 2];

                        colors[(int)EdgeNum.F, 0, 2] = c1;
                        colors[(int)EdgeNum.F, 1, 2] = c2;
                        colors[(int)EdgeNum.F, 2, 2] = c3;
                    }
                    break;
#endregion
                #region L
                case EdgeNum.L:
                    if (er.On180)
                    {
                        Swap(ref colors[(int)EdgeNum.U, 0, 0], ref colors[(int)EdgeNum.D, 2, 0]);
                        Swap(ref colors[(int)EdgeNum.U, 1, 0], ref colors[(int)EdgeNum.D, 1, 0]);
                        Swap(ref colors[(int)EdgeNum.U, 2, 0], ref colors[(int)EdgeNum.D, 0, 0]);

                        Swap(ref colors[(int)EdgeNum.F, 0, 0], ref colors[(int)EdgeNum.B, 2, 0]);
                        Swap(ref colors[(int)EdgeNum.F, 1, 0], ref colors[(int)EdgeNum.B, 1, 0]);
                        Swap(ref colors[(int)EdgeNum.F, 2, 0], ref colors[(int)EdgeNum.B, 0, 0]);
                    }
                    else if (!er.Anticlockwise)
                    {
                        c1 = colors[(int)EdgeNum.U, 0, 0];
                        c2 = colors[(int)EdgeNum.U, 1, 0];
                        c3 = colors[(int)EdgeNum.U, 2, 0];

                        colors[(int)EdgeNum.U, 0, 0] = colors[(int)EdgeNum.B, 2, 0];
                        colors[(int)EdgeNum.U, 1, 0] = colors[(int)EdgeNum.B, 1, 0];
                        colors[(int)EdgeNum.U, 2, 0] = colors[(int)EdgeNum.B, 0, 0];

                        colors[(int)EdgeNum.B, 2, 0] = colors[(int)EdgeNum.D, 2, 0];
                        colors[(int)EdgeNum.B, 1, 0] = colors[(int)EdgeNum.D, 1, 0];
                        colors[(int)EdgeNum.B, 0, 0] = colors[(int)EdgeNum.D, 0, 0];

                        colors[(int)EdgeNum.D, 2, 0] = colors[(int)EdgeNum.F, 0, 0];
                        colors[(int)EdgeNum.D, 1, 0] = colors[(int)EdgeNum.F, 1, 0];
                        colors[(int)EdgeNum.D, 0, 0] = colors[(int)EdgeNum.F, 2, 0];

                        colors[(int)EdgeNum.F, 0, 0] = c1;
                        colors[(int)EdgeNum.F, 1, 0] = c2;
                        colors[(int)EdgeNum.F, 2, 0] = c3;
                    }
                    else
                    {
                        c1 = colors[(int)EdgeNum.U, 0, 0];
                        c2 = colors[(int)EdgeNum.U, 1, 0];
                        c3 = colors[(int)EdgeNum.U, 2, 0];

                        colors[(int)EdgeNum.U, 0, 0] = colors[(int)EdgeNum.F, 0, 0];
                        colors[(int)EdgeNum.U, 1, 0] = colors[(int)EdgeNum.F, 1, 0];
                        colors[(int)EdgeNum.U, 2, 0] = colors[(int)EdgeNum.F, 2, 0];

                        colors[(int)EdgeNum.F, 0, 0] = colors[(int)EdgeNum.D, 2, 0];
                        colors[(int)EdgeNum.F, 1, 0] = colors[(int)EdgeNum.D, 1, 0];
                        colors[(int)EdgeNum.F, 2, 0] = colors[(int)EdgeNum.D, 0, 0];

                        colors[(int)EdgeNum.D, 2, 0] = colors[(int)EdgeNum.B, 2, 0];
                        colors[(int)EdgeNum.D, 1, 0] = colors[(int)EdgeNum.B, 1, 0];
                        colors[(int)EdgeNum.D, 0, 0] = colors[(int)EdgeNum.B, 0, 0];

                        colors[(int)EdgeNum.B, 2, 0] = c1;
                        colors[(int)EdgeNum.B, 1, 0] = c2;
                        colors[(int)EdgeNum.B, 0, 0] = c3;
                    }
                    break;
#endregion
                #region B
                case EdgeNum.B:
                    if (er.On180)
                    {
                        Swap(ref colors[(int)EdgeNum.U, 0, 0], ref colors[(int)EdgeNum.D, 0, 2]);
                        Swap(ref colors[(int)EdgeNum.U, 0, 1], ref colors[(int)EdgeNum.D, 0, 1]);
                        Swap(ref colors[(int)EdgeNum.U, 0, 2], ref colors[(int)EdgeNum.D, 0, 0]);

                        Swap(ref colors[(int)EdgeNum.R, 0, 2], ref colors[(int)EdgeNum.L, 2, 2]);
                        Swap(ref colors[(int)EdgeNum.R, 1, 2], ref colors[(int)EdgeNum.L, 1, 2]);
                        Swap(ref colors[(int)EdgeNum.R, 2, 2], ref colors[(int)EdgeNum.L, 0, 2]);
                    }
                    else if (!er.Anticlockwise)
                    {
                        c1 = colors[(int)EdgeNum.U, 0, 0];
                        c2 = colors[(int)EdgeNum.U, 0, 1];
                        c3 = colors[(int)EdgeNum.U, 0, 2];

                        colors[(int)EdgeNum.U, 0, 0] = colors[(int)EdgeNum.R, 0, 2];
                        colors[(int)EdgeNum.U, 0, 1] = colors[(int)EdgeNum.R, 1, 2];
                        colors[(int)EdgeNum.U, 0, 2] = colors[(int)EdgeNum.R, 2, 2];

                        colors[(int)EdgeNum.R, 0, 2] = colors[(int)EdgeNum.D, 0, 2];
                        colors[(int)EdgeNum.R, 1, 2] = colors[(int)EdgeNum.D, 0, 1];
                        colors[(int)EdgeNum.R, 2, 2] = colors[(int)EdgeNum.D, 0, 0];

                        colors[(int)EdgeNum.D, 0, 2] = colors[(int)EdgeNum.L, 2, 2];
                        colors[(int)EdgeNum.D, 0, 1] = colors[(int)EdgeNum.L, 1, 2];
                        colors[(int)EdgeNum.D, 0, 0] = colors[(int)EdgeNum.L, 0, 2];

                        colors[(int)EdgeNum.L, 2, 2] = c1;
                        colors[(int)EdgeNum.L, 1, 2] = c2;
                        colors[(int)EdgeNum.L, 0, 2] = c3;
                    }
                    else
                    {
                        c1 = colors[(int)EdgeNum.U, 0, 0];
                        c2 = colors[(int)EdgeNum.U, 0, 1];
                        c3 = colors[(int)EdgeNum.U, 0, 2];

                        colors[(int)EdgeNum.U, 0, 0] = colors[(int)EdgeNum.L, 2, 2];
                        colors[(int)EdgeNum.U, 0, 1] = colors[(int)EdgeNum.L, 1, 2];
                        colors[(int)EdgeNum.U, 0, 2] = colors[(int)EdgeNum.L, 0, 2];

                        colors[(int)EdgeNum.L, 2, 2] = colors[(int)EdgeNum.D, 0, 2];
                        colors[(int)EdgeNum.L, 1, 2] = colors[(int)EdgeNum.D, 0, 1];
                        colors[(int)EdgeNum.L, 0, 2] = colors[(int)EdgeNum.D, 0, 0];

                        colors[(int)EdgeNum.D, 0, 2] = colors[(int)EdgeNum.R, 0, 2];
                        colors[(int)EdgeNum.D, 0, 1] = colors[(int)EdgeNum.R, 1, 2];
                        colors[(int)EdgeNum.D, 0, 0] = colors[(int)EdgeNum.R, 2, 2];

                        colors[(int)EdgeNum.R, 0, 2] = c1;
                        colors[(int)EdgeNum.R, 1, 2] = c2;
                        colors[(int)EdgeNum.R, 2, 2] = c3;
                    }
                    break;
#endregion
                #region D
                case EdgeNum.D:
                    if (er.On180)
                    {
                        Swap(ref colors[(int)EdgeNum.F, 2, 0], ref colors[(int)EdgeNum.B, 2, 2]);
                        Swap(ref colors[(int)EdgeNum.F, 2, 1], ref colors[(int)EdgeNum.B, 2, 1]);
                        Swap(ref colors[(int)EdgeNum.F, 2, 2], ref colors[(int)EdgeNum.B, 2, 0]);

                        Swap(ref colors[(int)EdgeNum.R, 2, 0], ref colors[(int)EdgeNum.L, 2, 2]);
                        Swap(ref colors[(int)EdgeNum.R, 2, 1], ref colors[(int)EdgeNum.L, 2, 1]);
                        Swap(ref colors[(int)EdgeNum.R, 2, 2], ref colors[(int)EdgeNum.L, 2, 0]);
                    }
                    else if (!er.Anticlockwise)
                    {
                        c1 = colors[(int)EdgeNum.F, 2, 0];
                        c2 = colors[(int)EdgeNum.F, 2, 1];
                        c3 = colors[(int)EdgeNum.F, 2, 2];

                        colors[(int)EdgeNum.F, 2, 0] = colors[(int)EdgeNum.L, 2, 2];
                        colors[(int)EdgeNum.F, 2, 1] = colors[(int)EdgeNum.L, 2, 1];
                        colors[(int)EdgeNum.F, 2, 2] = colors[(int)EdgeNum.L, 2, 0];

                        colors[(int)EdgeNum.L, 2, 2] = colors[(int)EdgeNum.B, 2, 2];
                        colors[(int)EdgeNum.L, 2, 1] = colors[(int)EdgeNum.B, 2, 1];
                        colors[(int)EdgeNum.L, 2, 0] = colors[(int)EdgeNum.B, 2, 0];

                        colors[(int)EdgeNum.B, 2, 2] = colors[(int)EdgeNum.R, 2, 0];
                        colors[(int)EdgeNum.B, 2, 1] = colors[(int)EdgeNum.R, 2, 1];
                        colors[(int)EdgeNum.B, 2, 0] = colors[(int)EdgeNum.R, 2, 2];

                        colors[(int)EdgeNum.R, 2, 0] = c1;
                        colors[(int)EdgeNum.R, 2, 1] = c2;
                        colors[(int)EdgeNum.R, 2, 2] = c3;
                    }
                    else
                    {
                        c1 = colors[(int)EdgeNum.F, 2, 0];
                        c2 = colors[(int)EdgeNum.F, 2, 1];
                        c3 = colors[(int)EdgeNum.F, 2, 2];

                        colors[(int)EdgeNum.F, 2, 0] = colors[(int)EdgeNum.R, 2, 0];
                        colors[(int)EdgeNum.F, 2, 1] = colors[(int)EdgeNum.R, 2, 1];
                        colors[(int)EdgeNum.F, 2, 2] = colors[(int)EdgeNum.R, 2, 2];

                        colors[(int)EdgeNum.R, 2, 0] = colors[(int)EdgeNum.B, 2, 2];
                        colors[(int)EdgeNum.R, 2, 1] = colors[(int)EdgeNum.B, 2, 1];
                        colors[(int)EdgeNum.R, 2, 2] = colors[(int)EdgeNum.B, 2, 0];

                        colors[(int)EdgeNum.B, 2, 2] = colors[(int)EdgeNum.L, 2, 2];
                        colors[(int)EdgeNum.B, 2, 1] = colors[(int)EdgeNum.L, 2, 1];
                        colors[(int)EdgeNum.B, 2, 0] = colors[(int)EdgeNum.L, 2, 0];

                        colors[(int)EdgeNum.L, 2, 2] = c1;
                        colors[(int)EdgeNum.L, 2, 1] = c2;
                        colors[(int)EdgeNum.L, 2, 0] = c3;
                    }
                    break;
                #endregion
            }
        }

        public void RotateCube(CubeRotation cr)
        {
            if (CubeRotationEvent != null)
            {
                CubeRotationEvent(this, new CubeRotationEventArgs { CR = cr });
            }
            Color[,] tmp;
            if (cr.X)
            {
                if (!cr.Anticlockwise)
                {
                    tmp = CopyEdge(EdgeNum.U);
                    Assign(EdgeNum.U, CopyEdge(EdgeNum.F));
                    Assign(EdgeNum.F, Reflect(Rotate180(CopyEdge(EdgeNum.D)), false));
                    Assign(EdgeNum.D, CopyEdge(EdgeNum.B));
                    Assign(EdgeNum.B, Reflect(Rotate180(tmp), false));

                    Assign(EdgeNum.L, Rotate90(CopyEdge(EdgeNum.L), true));
                    Assign(EdgeNum.R, Rotate90(CopyEdge(EdgeNum.R), true));
                }
                else
                {
                    tmp = CopyEdge(EdgeNum.U);
                    Assign(EdgeNum.U, Reflect(Rotate180(CopyEdge(EdgeNum.B)), false));
                    Assign(EdgeNum.B, CopyEdge(EdgeNum.D));
                    Assign(EdgeNum.D, Reflect(Rotate180(CopyEdge(EdgeNum.F)), false));
                    Assign(EdgeNum.F, tmp);

                    Assign(EdgeNum.L, Rotate90(CopyEdge(EdgeNum.L), false));
                    Assign(EdgeNum.R, Rotate90(CopyEdge(EdgeNum.R), false));
                }
            }
            else if (cr.Y)
            {
                if (!cr.Anticlockwise)
                {
                    tmp = CopyEdge(EdgeNum.F);
                    Assign(EdgeNum.F, CopyEdge(EdgeNum.R));
                    Assign(EdgeNum.R, Reflect(Rotate180(CopyEdge(EdgeNum.B)), true));
                    Assign(EdgeNum.B, CopyEdge(EdgeNum.L));
                    Assign(EdgeNum.L, Reflect(Rotate180(tmp), true));

                    Assign(EdgeNum.U, Rotate90(CopyEdge(EdgeNum.U), true));
                    Assign(EdgeNum.D, Rotate90(CopyEdge(EdgeNum.D), true));
                }
                else
                {
                    tmp = CopyEdge(EdgeNum.F);
                    Assign(EdgeNum.F, Reflect(Rotate180(CopyEdge(EdgeNum.L)), true));
                    Assign(EdgeNum.L, CopyEdge(EdgeNum.B));
                    Assign(EdgeNum.B, Reflect(Rotate180(CopyEdge(EdgeNum.R)), true));
                    Assign(EdgeNum.R, tmp);

                    Assign(EdgeNum.U, Rotate90(CopyEdge(EdgeNum.U), false));
                    Assign(EdgeNum.D, Rotate90(CopyEdge(EdgeNum.D), false));
                }
            }
            else
            {
                if (!cr.Anticlockwise)
                {
                    tmp = CopyEdge(EdgeNum.U);
                    Assign(EdgeNum.U, Reflect(Rotate90(CopyEdge(EdgeNum.L), false), false));
                    Assign(EdgeNum.L, Rotate90(CopyEdge(EdgeNum.D), true));
                    Assign(EdgeNum.D, Reflect(Rotate90(CopyEdge(EdgeNum.R), false), false));
                    Assign(EdgeNum.R, Rotate90(tmp, true));

                    Assign(EdgeNum.B, Rotate90(CopyEdge(EdgeNum.B), true));
                    Assign(EdgeNum.F, Rotate90(CopyEdge(EdgeNum.F), true));
                }
                else
                {
                    tmp = CopyEdge(EdgeNum.U);
                    Assign(EdgeNum.U, Rotate90(CopyEdge(EdgeNum.R), false));
                    Assign(EdgeNum.R, Reflect(Rotate90(CopyEdge(EdgeNum.D), true), true));
                    Assign(EdgeNum.D, Rotate90(CopyEdge(EdgeNum.L), false));
                    Assign(EdgeNum.L, Reflect(Rotate90(tmp, false), false));

                    Assign(EdgeNum.B, Rotate90(CopyEdge(EdgeNum.B), false));
                    Assign(EdgeNum.F, Rotate90(CopyEdge(EdgeNum.F), false));
                }
            }
        }

        public Cube FindCube(Color c)
        {
            Cube res = null;
            for (int i = 0; i < 6; ++i)
            {
                if (colors[i, 1, 1] == c)
                {
                    res = GetCube(FindCoord((EdgeNum)i, 1, 1));
                    break;
                }
            }
            return res;
        }

        public Cube FindCube(Color c1, Color c2)
        {
            if (c1 == c2) throw new ArgumentException();
            Cube res = null;
            OrientedColor oc1 = new OrientedColor(c1, null);
            OrientedColor oc2 = new OrientedColor(c2, null);
            OrientedColorColorEqualityComparer comp = new OrientedColorColorEqualityComparer();
            bool exit=false;
            for (int i = -1; i < 2 && !exit; ++i)
            {
                for (int j = -1; j < 2 && !exit; ++j)
                {
                    for (int k = -1; k < 2 && !exit; ++k)
                    {
                        //if ((i == 0 && j == 0) || (i == 0 && k == 0) || (j == 0 && k == 0))
                        //{
                            res = GetCube(new Coordinates(i, j, k));
                            if (res.Colors.Count != 2) continue;
                            if (res.Colors.Contains(oc1, comp) && res.Colors.Contains(oc2, comp))
                            {
                                exit = true;
                            }
                        //}
                    }
                }
            }
            return res;
        }

        public Cube FindCube(Color c1, Color c2, Color c3)
        {
            if (c1 == c2 || c1 == c3 || c2 == c3) throw new ArgumentException();
            Cube res = null;
            OrientedColor oc1 = new OrientedColor(c1, null);
            OrientedColor oc2 = new OrientedColor(c2, null);
            OrientedColor oc3 = new OrientedColor(c3, null);
            OrientedColorColorEqualityComparer comp = new OrientedColorColorEqualityComparer();
            bool exit = false;
            for (int i = -1; i < 2 && !exit; ++i)
            {
                for (int j = -1; j < 2 && !exit; ++j)
                {
                    for (int k = -1; k < 2 && !exit; ++k)
                    {
                        if (i != 0 && j != 0 && k != 0)
                        {
                            res = GetCube(new Coordinates(i, j, k));
                            if (res.Colors.Contains(oc1, comp) && res.Colors.Contains(oc2, comp) && res.Colors.Contains(oc3, comp))
                            {
                                exit = true;
                            }
                        }
                    }
                }
            }
            return res;
        }

        public Cube GetCube(Coordinates c)
        {
            Cube res;
            List<OrientedColor> loc = new List<OrientedColor>();
            if (c.X == -1)
            {
                loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 1 - c.Z, c.Y + 1], new Coordinates(-1, 0, 0)));
            }
            else if (c.X == 1)
            {
                loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 1 - c.Z, c.Y + 1], new Coordinates(1, 0, 0)));
            }
            if (c.Y == -1)
            {
                loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 1 - c.Z, 1 - c.X], new Coordinates(0, -1, 0)));
            }
            else if (c.Y == 1)
            {
                loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 1 - c.Z, 1 - c.X], new Coordinates(0, 1, 0)));
            }
            if (c.Z == -1)
            {
                loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 1 + c.X, 1 + c.Y], new Coordinates(0, 0, -1)));
            }
            else if (c.Z == 1)
            {
                loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 1 + c.X, 1 + c.Y], new Coordinates(0, 0, 1)));
            }
            //switch (c.X)
            //{
            //    case -1:
            //        switch (c.Y)
            //        {
            //            case -1:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 0, 0], BuildVector(EdgeNum.D)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 2, 2], BuildVector(EdgeNum.L)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 2, 0], BuildVector(EdgeNum.B)));
            //                        break;
            //                    case 0:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 1, 2], BuildVector(EdgeNum.L)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 1, 0], BuildVector(EdgeNum.B)));
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 0, 0], BuildVector(EdgeNum.U)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 0, 2], BuildVector(EdgeNum.L)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 0, 0], BuildVector(EdgeNum.B)));
            //                        break;
            //                }
            //                break;
            //            case 0:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 0, 1], BuildVector(EdgeNum.D)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 2, 1], BuildVector(EdgeNum.B)));
            //                        break;
            //                    case 0:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 1, 1], BuildVector(EdgeNum.B)));
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 0, 1], BuildVector(EdgeNum.U)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 0, 1], BuildVector(EdgeNum.B)));
            //                        break;
            //                }
            //                break;
            //            case 1:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 0, 2], BuildVector(EdgeNum.D)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 2, 2], BuildVector(EdgeNum.R)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 2, 2], BuildVector(EdgeNum.B)));
            //                        break;
            //                    case 0:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 1, 2], BuildVector(EdgeNum.R)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 1, 2], BuildVector(EdgeNum.B)));
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 0, 2], BuildVector(EdgeNum.U)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 0, 2], BuildVector(EdgeNum.R)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.B, 0, 2], BuildVector(EdgeNum.B)));
            //                        break;
            //                }
            //                break;
            //        }
            //        break;
            //    case 0:
            //        switch (c.Y)
            //        {
            //            case -1:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 1, 0], BuildVector(EdgeNum.D)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 2, 1], BuildVector(EdgeNum.L)));
            //                        break;
            //                    case 0:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 1, 1], BuildVector(EdgeNum.L)));
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 1, 0], BuildVector(EdgeNum.U)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 0, 1], BuildVector(EdgeNum.L)));
            //                        break;
            //                }
            //                break;
            //            case 0:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 1, 1], BuildVector(EdgeNum.D)));
            //                        break;
            //                    case 0:
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 1, 1], BuildVector(EdgeNum.U)));
            //                        break;
            //                }
            //                break;
            //            case 1:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 1, 2], BuildVector(EdgeNum.D)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 2, 1], BuildVector(EdgeNum.R)));
            //                        break;
            //                    case 0:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 1, 1], BuildVector(EdgeNum.R)));
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 1, 2], BuildVector(EdgeNum.U)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 0, 1], BuildVector(EdgeNum.R)));
            //                        break;
            //                }
            //                break;
            //        }
            //        break;
            //    case 1:
            //        switch (c.Y)
            //        {
            //            case -1:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 2, 0], BuildVector(EdgeNum.D)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 2, 0], BuildVector(EdgeNum.L)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 2, 0], BuildVector(EdgeNum.F)));
            //                        break;
            //                    case 0:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 1, 0], BuildVector(EdgeNum.L)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 1, 0], BuildVector(EdgeNum.F)));
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 2, 0], BuildVector(EdgeNum.U)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.L, 0, 0], BuildVector(EdgeNum.L)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 0, 0], BuildVector(EdgeNum.F)));
            //                        break;
            //                }
            //                break;
            //            case 0:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 2, 1], BuildVector(EdgeNum.D)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 2, 1], BuildVector(EdgeNum.F)));
            //                        break;
            //                    case 0:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 1, 1], BuildVector(EdgeNum.F)));
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 2, 1], BuildVector(EdgeNum.U)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 0, 1], BuildVector(EdgeNum.F)));
            //                        break;
            //                }
            //                break;
            //            case 1:
            //                switch (c.Z)
            //                {
            //                    case -1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.D, 2, 2], BuildVector(EdgeNum.D)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 2, 0], BuildVector(EdgeNum.R)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 2, 2], BuildVector(EdgeNum.F)));
            //                        break;
            //                    case 0:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 1, 0], BuildVector(EdgeNum.R)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 1, 2], BuildVector(EdgeNum.F)));
            //                        break;
            //                    case 1:
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.U, 2, 2], BuildVector(EdgeNum.U)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.R, 0, 0], BuildVector(EdgeNum.R)));
            //                        loc.Add(new OrientedColor(colors[(int)EdgeNum.F, 0, 2], BuildVector(EdgeNum.F)));
            //                        break;
            //                }
            //                break;
            //        }
            //        break;
            //}
            res = new Cube(loc, c);
            return res;
        }

        public bool IsSolved()
        {
            bool res = true;
            for (int i = 0; i < 6 && res; ++i)
            {
                for (int j = 0; j < 3 && res; ++j)
                {
                    for (int k = 0; k < 3 && res; ++k)
                    {
                        res = colors[i, 0, 0] == colors[i, j, k];
                    }
                }
            }
            return res;
        }

        protected static Coordinates BuildVector(EdgeNum en)
        {
            Coordinates res = null;
            switch (en)
            {
                case EdgeNum.U:
                    res = new Coordinates(0, 0, 1);
                    break;
                case EdgeNum.F:
                    res = new Coordinates(1, 0, 0);
                    break;
                case EdgeNum.R:
                    res = new Coordinates(0, 1, 0);
                    break;
                case EdgeNum.L:
                    res = new Coordinates(0, -1, 0);
                    break;
                case EdgeNum.B:
                    res = new Coordinates(-1, 0, 0);
                    break;
                case EdgeNum.D:
                    res = new Coordinates(0, 0, -1);
                    break;
            }
            return res;
        }

        protected static Coordinates FindCoord(EdgeNum en, int i, int j)
        {
            int x = 0, y = 0, z = 0;
            switch (en)
            {
                case EdgeNum.U:
                    z = 1;
                    x = i - 1;
                    y = j - 1;
                    break;
                case EdgeNum.F:
                    x = 1;
                    z = i - 1;
                    y = j - 1;
                    break;
                case EdgeNum.R:
                    y = 1;
                    z = i - 1;
                    x = j - 1;
                    break;
                case EdgeNum.L:
                    y = -1;
                    z = i - 1;
                    x = j - 1;
                    break;
                case EdgeNum.B:
                    x = -1;
                    z = i - 1;
                    y = j - 1;
                    break;
                case EdgeNum.D:
                    z = -1;
                    x = i - 1;
                    y = j - 1;
                    break;
            }
            return new Coordinates(x, y, z);
        }

        private Color[,] CopyEdge(EdgeNum en)
        {
            Color[,] res = new Color[3, 3];
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    res[i, j] = colors[(int)en, i, j];
                }
            }
            return res;
        }

        private void Assign(EdgeNum en, Color[,] edge)
        {
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    colors[(int)en, i, j] = edge[i, j];
                }
            }
        }

        private static Color[,] Rotate180(Color[,] edge)
        {
            Color[,] res = new Color[3, 3];
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    res[i, j] = edge[i == 0 ? 2 : i == 2 ? 0 : 1, j == 0 ? 2 : j == 2 ? 0 : 1];
                }
            }
            return res;
        }
        private static Color[,] Rotate90(Color[,] edge, bool right)
        {
            Color[,] res = new Color[3, 3];
            int x = 0, y = 0;
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (right)
                    {
                        y = i;
                        x = 2 - j;
                    }
                    else
                    {
                        x = j;
                        y = 2 - i;
                    }
                    res[i, j] = edge[x, y];
                }
            }
            return res;
        }
        private static Color[,] Reflect(Color[,] edge, bool horizontal)
        {
            Color[,] res = new Color[3, 3];
            int x = 0, y = 0;
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (!horizontal)
                    {
                        x = i;
                        if (j == 0) y = 2;
                        else if (j == 1) y = 1;
                        else y = 0;
                    }
                    else
                    {
                        y = j;
                        x = 2 - i;
                    }
                    res[i, j] = edge[x, y];
                }
            }
            return res;
        }

        private static void Swap(ref Color c1, ref Color c2)
        {
            Color tmp = c1;
            c1 = c2;
            c2 = tmp;
        }

        protected Color[, ,] colors;
    }

    //class RubiksCube2
    //{
    //    private Cube[,,] cubes;

    //    public RubiksCube2()
    //    {
    //        cubes = new Cube[3, 3, 3];
    //        List<OrientedColor> loc = new List<OrientedColor>();
    //        for (int i = -1; i < 2; ++i)
    //        {
    //            for (int j = -1; j < 2; ++j)
    //            {
    //                for (int k = -1; k < 2; ++k)
    //                {
    //                    loc.Clear();
    //                    if (k == -1) loc.Add(new OrientedColor(Color.Y, BuildVector(EdgeNum.D)));
    //                    else if (k == 1) loc.Add(new OrientedColor(Color.W, BuildVector(EdgeNum.U)));
    //                    if (i == -1) loc.Add(new OrientedColor(Color.B, BuildVector(EdgeNum.B)));
    //                    else if (i == 1) loc.Add(new OrientedColor(Color.G, BuildVector(EdgeNum.F)));
    //                    if (j == -1) loc.Add(new OrientedColor(Color.O, BuildVector(EdgeNum.L)));
    //                    else if (j == 1) loc.Add(new OrientedColor(Color.R, BuildVector(EdgeNum.R)));
    //                    cubes[i + 1, j + 1, k + 1] = new Cube(loc, new Coordinates(i, j, k));
    //                }
    //            }
    //        }
    //    }

    //    public void RotateEdge(EdgeRotation er)
    //    {

    //    }

    //    public Cube FindCube(Color c)
    //    {
    //        Cube res = null;
    //        bool exit=false;
    //        for (int i = 0; i < 3 && !exit; ++i)
    //        {
    //            for (int j = 0; j < 3 && !exit; ++j)
    //            {
    //                for (int k = 0; k < 3 && !exit; ++k)
    //                {
    //                    res = cubes[i, j, k];
    //                    if (res.Colors.Count == 1 && res.Colors[0].C == c) exit = true; 
    //                }
    //            }
    //        }
    //        return res;
    //    }
    //    public Cube FindCube(Color c1, Color c2)
    //    {
    //        Cube res = null;
    //        OrientedColorColorEqualityComparer comp = new OrientedColorColorEqualityComparer();
    //        OrientedColor oc1 = new OrientedColor(c1, null);
    //        OrientedColor oc2 = new OrientedColor(c2, null);
    //        bool exit = false;
    //        for (int i = 0; i < 3 && !exit; ++i)
    //        {
    //            for (int j = 0; j < 3 && !exit; ++j)
    //            {
    //                for (int k = 0; k < 3 && !exit; ++k)
    //                {
    //                    res = cubes[i, j, k];
    //                    if (res.Colors.Count == 2 && res.Colors.Contains(oc1, comp) && res.Colors.Contains(oc2, comp)) exit = true;
    //                }
    //            }
    //        }
    //        return res;
    //    }
    //    public Cube FindCube(Color c1, Color c2, Color c3)
    //    {
    //        Cube res = null;
    //        OrientedColorColorEqualityComparer comp = new OrientedColorColorEqualityComparer();
    //        OrientedColor oc1 = new OrientedColor(c1, null);
    //        OrientedColor oc2 = new OrientedColor(c2, null);
    //        OrientedColor oc3 = new OrientedColor(c2, null);
    //        bool exit = false;
    //        for (int i = 0; i < 3 && !exit; ++i)
    //        {
    //            for (int j = 0; j < 3 && !exit; ++j)
    //            {
    //                for (int k = 0; k < 3 && !exit; ++k)
    //                {
    //                    res = cubes[i, j, k];
    //                    if (res.Colors.Count == 3 && res.Colors.Contains(oc1, comp) && res.Colors.Contains(oc2, comp) &&
    //                        res.Colors.Contains(oc3, comp)) exit = true;
    //                }
    //            }
    //        }
    //        return res;
    //    }

    //    public Cube GetCube(Coordinates c)
    //    {
    //        return new Cube(cubes[c.X + 1, c.Y + 1, c.Z + 1]);
    //    }

    //    protected static Coordinates BuildVector(EdgeNum en)
    //    {
    //        Coordinates res = null;
    //        switch (en)
    //        {
    //            case EdgeNum.U:
    //                res = new Coordinates(0, 0, 1);
    //                break;
    //            case EdgeNum.F:
    //                res = new Coordinates(1, 0, 0);
    //                break;
    //            case EdgeNum.R:
    //                res = new Coordinates(0, 1, 0);
    //                break;
    //            case EdgeNum.L:
    //                res = new Coordinates(0, -1, 0);
    //                break;
    //            case EdgeNum.B:
    //                res = new Coordinates(-1, 0, 0);
    //                break;
    //            case EdgeNum.D:
    //                res = new Coordinates(0, 0, -1);
    //                break;
    //        }
    //        return res;
    //    }
    //}
}
