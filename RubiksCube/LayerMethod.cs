using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube
{
    class StartAlgorithmEventArgs : EventArgs
    {

    }
    class EndAlgorithmEventArgs : EventArgs
    {
        public uint NumOfSteps;
    }

    delegate void StartAlgorithmHandler(object sender, StartAlgorithmEventArgs args);
    delegate void EndAlgorithmHandler(object sender, EndAlgorithmEventArgs args);

    //abstract class Algorithm
    //{
    //    public static event StartAlgorithmHandler StartAlgorithmEvent;
    //    public static event EndAlgorithmHandler EndAlgorithmEvent;

    //    public static List<Rotation> Solve(RubiksCube rc);
    //}


    static class LayerMethod
    {
        public static event StartAlgorithmHandler StartAlgorithmEvent;
        public static event EndAlgorithmHandler EndAlgorithmEvent;

        public static List<Rotation> Solve(RubiksCube rc)
        {
            if (StartAlgorithmEvent != null)
            {
                StartAlgorithmEvent(null, new StartAlgorithmEventArgs());
            }
            List<Rotation> res = new List<Rotation>();
            if (rc.IsSolved()) return res;
            Norm(res, rc);
            int numOfCubeRotates = res.Count;
            BuildWhiteCross(res, rc);
            PutWhiteAngles(res, rc);
            ArrangeBottomAngles(res, rc);
            BuildMidAndYellowCross(res, rc);
            PutYellowAngles(res, rc);
            BuildCorrectYellowCross(res, rc);
            if(EndAlgorithmEvent!=null)
            {
                EndAlgorithmEvent(null, new EndAlgorithmEventArgs { NumOfSteps = (uint)(res.Count - numOfCubeRotates) });
            }
            return res;
        }

        //White to Up, Green to Front, Red to Right
        private static void Norm(List<Rotation> solving, RubiksCube rc)
        {
            Cube w = rc.FindCube(Color.W);
            Cube g = null;//rc.FindCube(Color.G);
            CubeRotation rot = null;
            bool anticlockwise = false;
            if (w.Coord.Z != 1)
            {
                if (w.Coord.Z == -1)
                {
                    rot = new CubeRotation(true, false, false, false);
                    solving.Add(rot);
                    rc.RotateCube(rot);
                    w = rc.FindCube(Color.W);
                }
                bool RotX = w.Coord.X == 1 || w.Coord.X == -1;
                bool RotZ = !RotX;
                anticlockwise = !((RotX && w.Coord.X == 1) || (RotZ && w.Coord.Y == -1));
                rot = new CubeRotation(RotX, false, RotZ, anticlockwise);
                solving.Add(rot);
                rc.RotateCube(rot);
            }
            g = rc.FindCube(Color.G);
            if (g.Coord.X != 1)
            {
                if (g.Coord.X == -1)
                {
                    rot = new CubeRotation(false, true, false, false);
                    solving.Add(rot);
                    rc.RotateCube(rot);
                    g = rc.FindCube(Color.G);
                }
                anticlockwise = g.Coord.Y == -1;
                rot = new CubeRotation(false, true, false, anticlockwise);
                solving.Add(rot);
                rc.RotateCube(rot);
            }
        }
        //private static void BuildWhiteCross(List<Rotation> solving, RubiksCube rc)
        //{
        //    Cube curCube = null;
        //    int x = 0, y = 0;
        //    for (int i = 1; i < 5; ++i)
        //    {
        //        x = 0; y = 0;
        //        if (i == 1) x = 1;
        //        else if (i == 4) x = -1;
        //        else if (i == 2) y = 1;
        //        else if (i == 3) y = -1;
        //        curCube = rc.FindCube(Color.W, (Color)i);
        //        if (curCube.Coord.Z == 1)
        //        {
        //            if (curCube.Coord.X == x && curCube.Coord.Y == y)
        //            {
        //                bool inSitu = false;
        //                foreach (OrientedColor oc in curCube.Colors)
        //                {
        //                    inSitu = oc == new OrientedColor(Color.W, new Coordinates(0, 0, 1));
        //                    if (inSitu) break;
        //                }
        //                if (inSitu) continue;
        //            }
        //            RotateSo(curCube.Coord, new Coordinates(curCube.Coord.X, curCube.Coord.Y, -1),
        //                     ChooseEdge(curCube.Coord.X, curCube.Coord.Y, 0), solving, rc);
        //            curCube = rc.FindCube(Color.W, (Color)i);
        //        }
        //        if (curCube.Coord.Z == 0)
        //        {
        //            EdgeNum en1 = ChooseEdge(curCube.Coord.X, 0, 0);
        //            EdgeNum en2 = ChooseEdge(0, curCube.Coord.Y, 0);
        //            Coordinates from = null;
        //            Coordinates to = null;
        //            Coordinates fromForSide = null;
        //            Coordinates toForSide = null;
        //            EdgeNum side = EdgeNum.U;
        //            Coordinates c = GenericCoordinate((EdgeNum)i);
        //            if ((int)en1 < i && (int)en2 < i)
        //            {
        //                from = new Coordinates(c.X, c.Y, 1);
        //                to = new Coordinates(curCube.Coord.X, 0, 1);
        //                side = en1;
        //                fromForSide = curCube.Coord;
        //                toForSide = new Coordinates(curCube.Coord.X, 0, -1);
        //            }
        //            else if (i < (int)en1)
        //            {
        //                //from = new Coordinates(c.X, c.Y, 1);
        //                //to = new Coordinates(curCube.Coord.X, 0, 1);
        //                side = en2;
        //                fromForSide = curCube.Coord;
        //                toForSide = new Coordinates(0, curCube.Coord.Y, -1);
        //            }
        //            else if (i < (int)en2)
        //            {
        //                //from = new Coordinates(c.X, c.Y, 1);
        //                //to = new Coordinates(0, curCube.Coord.Y, 1);
        //                side = en1;
        //                fromForSide = curCube.Coord;
        //                toForSide = new Coordinates(curCube.Coord.X, 0, -1);
        //            }

        //            if (from != null)
        //            {
        //                RotateSo(from, to, EdgeNum.U, solving, rc);
        //            }
        //            RotateSo(fromForSide, toForSide, side, solving, rc);
        //            if (from != null)
        //            {
        //                RotateSo(to, from, EdgeNum.U, solving, rc);
        //            }
        //            curCube = rc.FindCube(Color.W, (Color)i);
        //        }
        //        if (curCube.Coord.Z == -1)
        //        {
        //            RotateSo(curCube.Coord, new Coordinates(x, y, -1), EdgeNum.D, solving, rc);
        //            curCube=rc.FindCube(Color.W, (Color)i);
        //            bool whiteToDown = curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.W; }).Coord ==
        //                                new Coordinates(0, 0, -1);
        //            if (whiteToDown)
        //            {
        //                RotateSo(curCube.Coord, new Coordinates(x, y, 1), ChooseEdge(x, y, 0), solving, rc);
        //            }
        //            else
        //            {
        //                Coordinates to = new Coordinates(x == 0 ? y : x, y == 0 ? x : y, 0);
        //                RotateSo(curCube.Coord, to, ChooseEdge(x, y, 0), solving, rc);
        //                RotateSo(new Coordinates(x, y, 1), new Coordinates(x == 0 ? y : 0, y == 0 ? x : 0, 1),
        //                         EdgeNum.U, solving, rc);
        //                RotateSo(to, new Coordinates(x == 0 ? y : 0, y == 0 ? x : 0, 1),
        //                         ChooseEdge(x == 0 ? y : 0, y == 0 ? x : 0, 0), solving, rc);
        //                RotateSo(new Coordinates(x == 0 ? y : 0, y == 0 ? x : 0, 1), new Coordinates(x, y, 1),
        //                         EdgeNum.U, solving, rc);
        //            }
        //        }

        //    }
        //}
        private static void BuildWhiteCross(List<Rotation> solving, RubiksCube rc)
        {
            Cube curCube = null;
            EdgeRotation rot = null;
            int x = 0, y = 0; //(x, y, 1) - координаты, где должен стоять кубик
            EdgeNum en;
            for (int i = 1; i < 5; ++i)
            {
                x = 0; y = 0;
                if (i == 1) x = 1;
                else if (i == 4) x = -1;
                else if (i == 2) y = 1;
                else if (i == 3) y = -1;
                curCube = rc.FindCube(Color.W, (Color)i);
                switch (curCube.Coord.Z)
                {
                    case 1:
                        bool WtoU =
                        curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.W; }).Coord == new Coordinates(0, 0, 1);
                        if (WtoU)
                        {
                            if (i == 1)
                            {
                                RotateSo(curCube.Coord, new Coordinates(x, y, 1), EdgeNum.U, solving, rc);
                                break;
                            }
                            if (curCube.Coord.X == x && curCube.Coord.Y == y)
                            {
                                break;
                            }
                            else
                            {
                                //rot = new EdgeRotation((EdgeNum)i, false, false);
                                rot = new EdgeRotation(ChooseEdge(curCube.Coord.X, curCube.Coord.Y, 0), false, false);
                                solving.Add(rot);
                                rc.RotateEdge(rot);
                                curCube = rc.FindCube(Color.W, (Color)i);
                                goto case 0;
                            }
                        }
                        else
                        {
                            en = ChooseEdge(curCube.Coord.X, curCube.Coord.Y, 0);
                            rot = new EdgeRotation(en, false, false);
                            solving.Add(rot);
                            rc.RotateEdge(rot);
                            curCube = rc.FindCube(Color.W, (Color)i);
                            goto case 0;
                        }
                    case 0:
                        EdgeNum en1 = ChooseEdge(curCube.Coord.X, 0, 0);
                        EdgeNum en2 = ChooseEdge(0, curCube.Coord.Y, 0);
                        Coordinates Wto = curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.W; }).Coord;
                        if (Wto.X != 0) en = en2;
                        else en = en1;
                        Coordinates gcen = GenericCoordinate(en);
                        Coordinates to = new Coordinates(gcen.X, gcen.Y, 1);
                        RotateSo(new Coordinates(x, y, 1), to, EdgeNum.U, solving, rc);
                        RotateSo(curCube.Coord, to, en, solving, rc);
                        RotateSo(to, new Coordinates(x, y, 1), EdgeNum.U, solving, rc);
                        break;
                    case -1:
                        RotateSo(curCube.Coord, new Coordinates(x, y, -1), EdgeNum.D, solving, rc);
                        bool WtoD =
                        curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.W; }).Coord==new Coordinates(0, 0, -1);
                        if (WtoD)
                        {
                            rot = new EdgeRotation((EdgeNum)i, true, false);
                            solving.Add(rot);
                            rc.RotateEdge(rot);
                            curCube = rc.FindCube(Color.W, (Color)i);
                            break;
                        }
                        else
                        {
                            rot = new EdgeRotation((EdgeNum)i, false, false);
                            solving.Add(rot);
                            rc.RotateEdge(rot);
                            curCube = rc.FindCube(Color.W, (Color)i);
                            goto case 0;
                        }
                }
            }
        }
        private static void PutWhiteAngles(List<Rotation> solving, RubiksCube rc)
        {
            Cube curCube = null;
            int x = 0, y = 0; //(x, y, 1) - координаты, где должен стоять кубик
            Color c1, c2; //Цвета, которые имеет кубик, кроме белого
            EdgeNum en1;
            EdgeNum en2;
            EdgeNum en;
            Coordinates Wto = null;
            Coordinates tmp = null;
            Coordinates offset = null; //координаты угла, находящегося не на крутящейся на даной итерации вертикальной стороне
            RubiksCube tmprc = null;
            Cube tmpCube = null;
            for (int i = 1; i < 5; ++i)
            {
                if (i == 1) { x = 1; y = 1; c1 = Color.G; c2 = Color.R; }
                else if (i == 2) { x = 1; y = -1; c1 = Color.G; c2 = Color.O; }
                else if (i == 3) { x = -1; y = -1; c1 = Color.O; c2 = Color.B; }
                else { x = -1; y = 1; c1 = Color.B; c2 = Color.R; }
                curCube = rc.FindCube(Color.W, c1, c2);
                switch (curCube.Coord.Z)
                {
                    case 1:
                        Wto = curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.W; }).Coord;
                        bool WtoU = Wto == new Coordinates(0, 0, 1);
                        if (WtoU && curCube.Coord.X == x && curCube.Coord.Y == y)
                        {
                            break;
                        }
                        else
                        {
                            en1 = ChooseEdge(curCube.Coord.X, 0, 0);
                            en2 = ChooseEdge(0, curCube.Coord.Y, 0);
                            if (Wto.X != 0) { en = en1; tmp = GenericCoordinate(en2); }
                            else { en = en2; tmp = GenericCoordinate(en1); }
                            //offset =
                            //    new Coordinates(tmp.Y == 0 ? curCube.Coord.Y : -curCube.Coord.Y,
                            //                    tmp.X == 0 ? curCube.Coord.X : -curCube.Coord.X, -1);
                            offset =
                                new Coordinates(tmp.X == 0 ? -curCube.Coord.X : tmp.X,
                                                tmp.Y == 0 ? -curCube.Coord.Y : tmp.Y, -1);
                            RotateSo(curCube.Coord, new Coordinates(curCube.Coord.X, curCube.Coord.Y, -1), en, solving, rc);
                            RotateSo(new Coordinates(curCube.Coord.X, curCube.Coord.Y, -1), offset, EdgeNum.D, solving, rc);
                            RotateSo(new Coordinates(curCube.Coord.X, curCube.Coord.Y, -1), curCube.Coord, en, solving, rc);
                            curCube = rc.FindCube(Color.W, c1, c2);
                            goto case -1;
                        }
                    case -1:
                        Wto = curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.W; }).Coord;
                        bool WtoD = Wto == new Coordinates(0, 0, -1);
                        if (WtoD)
                        {
                            //разворачиваем кубик так, чтобы белый цыет больше не был внизу
                            //принцип построения смещения аналогичен случаю при z == 1 для tmp.X != 0 (нет разницы, что выбрать)
                            en = ChooseEdge(x, 0, 0);
                            //offset = new Coordinates(y, -x, -1);
                            offset = new Coordinates(-x, y, -1);
                            RotateSo(curCube.Coord, offset, EdgeNum.D, solving, rc);
                            RotateSo(new Coordinates(x, y, 1), new Coordinates(x, y, -1), en, solving, rc);
                            //RotateSo(offset, new Coordinates(-offset.X, -offset.Y, -1), EdgeNum.D, solving, rc);
                            RotateSo(offset, new Coordinates(x, -y, -1), EdgeNum.D, solving, rc);
                            RotateSo(new Coordinates(x, y, -1), new Coordinates(x, y, 1), en, solving, rc);
                            curCube = rc.FindCube(Color.W, c1, c2);
                            //Wto = curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.W; }).Coord;
                        }
                        //этого поворота можно и не записывать
                        tmprc = new RubiksCube(rc);
                        RotateSo(curCube.Coord, new Coordinates(x, y, -1), EdgeNum.D, new List<Rotation>(), tmprc);
                        //tmpCube = tmprc.FindCube(Color.W, c1, c2);
                        tmpCube = tmprc.GetCube(new Coordinates(x, y, -1));
                        Wto = tmpCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.W; }).Coord;
                        en1 = ChooseEdge(x, 0, 0);
                        en2 = ChooseEdge(0, y, 0);
                        if (Wto.X != 0) { en = en2; tmp = GenericCoordinate(en1); }
                        else { en = en1; tmp = GenericCoordinate(en2); }
                        //offset =
                        //        new Coordinates(tmp.Y == 0 ? curCube.Coord.Y : -curCube.Coord.Y,
                        //                        tmp.X == 0 ? curCube.Coord.X : -curCube.Coord.X, -1);
                        //offset =
                        //        new Coordinates(tmp.X == 0 ? -tmpCube.Coord.X : tmp.X,
                        //                        tmp.Y == 0 ? -tmpCube.Coord.Y : tmp.Y, -1);
                        offset =
                                new Coordinates(tmp.X == 0 ? -x : tmp.X,
                                                tmp.Y == 0 ? -y : tmp.Y, -1);
                        RotateSo(curCube.Coord, offset, EdgeNum.D, solving, rc);
                        RotateSo(new Coordinates(x, y, 1), new Coordinates(x, y, -1), en, solving, rc);
                        RotateSo(offset, new Coordinates(x, y, -1), EdgeNum.D, solving, rc);
                        RotateSo(new Coordinates(x, y, -1), new Coordinates(x, y, 1), en, solving, rc);
                        break;
                }
            }
        }
        private static void ArrangeBottomAngles(List<Rotation> solving, RubiksCube rc)
        {
            Color[] ca = { Color.R, Color.G, Color.G, Color.O, Color.O, Color.B, Color.B, Color.R };
            List<Coordinates> lc = new List<Coordinates>();
            //показывает, с каким количеством углов стоит в правильном порядке даный угол
            List<int> inOrder = new List<int>();
            for (int n = 0; n < 2; ++n)
            {
                lc.Clear();
                inOrder.Clear();
                for (int i = 0; i < 4; ++i)
                {
                    lc.Add(new Coordinates(rc.FindCube(Color.Y, ca[2 * i], ca[2 * i + 1]).Coord));
                    inOrder.Add(0);
                }
                Coordinates left = null, right = null, opposite = null;
                Coordinates tmp = null;
                for (int i = 0; i < 4; ++i)
                {
                    left = new Coordinates(lc[i].X, -lc[i].Y, -1);
                    right = new Coordinates(-lc[i].X, lc[i].Y, -1);
                    opposite = new Coordinates(-lc[i].X, -lc[i].Y, -1);
                    if (lc[i].X != lc[i].Y) { tmp = left; left = right; right = tmp; }
                    //if (left == (i == 0 ? lc[3] : lc[i - 1]) || left == (i == 3 ? lc[0] : lc[i + 1])) ++inOrder[i];
                    //if (right == (i == 0 ? lc[3] : lc[i - 1]) || right == (i == 3 ? lc[0] : lc[i + 1])) ++inOrder[i];
                    if (left == (i == 3 ? lc[0] : lc[i + 1])) ++inOrder[i];
                    if (right == (i == 0 ? lc[3] : lc[i - 1])) ++inOrder[i];
                    if (opposite == lc[(2 + i) % 4]) ++inOrder[i];
                }
                if (inOrder[0] == 3) return;
                Coordinates c1 = null, c2 = null;
                bool diag = true;//для того, чтобы определить, рядом ли находятся кубы или напротив
                for (int i = 0; i < 4; ++i) { if (inOrder[i] != 1) diag = false; }
                EdgeNum en1, en2;
                Coordinates tmp1 = null, tmp2 = null;
                Coordinates offset = null;
                int x, y; //общие координаты для en1, en2
                if (diag)
                {
                    //нет разницы, с какой стороны крутить
                    c1 = new Coordinates(1, 1, -1);
                    c2 = new Coordinates(-1, -1, -1);
                    en1 = EdgeNum.R;
                    en2 = EdgeNum.B;
                    //if (c1.X == 1 && c1.Y == 1) { en1 = EdgeNum.R; en2 = EdgeNum.B; }
                    //else if (c1.X == 1 && c1.Y == -1) { en1 = EdgeNum.F; en2 = EdgeNum.R; }
                    //else if (c1.X == -1 && c1.Y == -1) { en1 = EdgeNum.L; en2 = EdgeNum.F; }
                    //else /*if (c1.X == -1 && c1.Y == 1)*/ { en1 = EdgeNum.B; en2 = EdgeNum.L; }
                }
                else
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        if (inOrder[i] == 1)
                        {
                            if (object.ReferenceEquals(c1, null)) c1 = lc[i];
                            else { c2 = lc[i]; break; }
                        }
                    }
                    if (c1.X == c2.X)
                    {
                        en2 = (EdgeNum)(5 - (int)ChooseEdge(c1.X, 0, 0));
                        if (c1.X == 1) en1 = EdgeNum.R;
                        else en1 = EdgeNum.L;
                    }
                    else /*c1.Y == c2.Y*/
                    {
                        en2 = (EdgeNum)(5 - (int)ChooseEdge(0, c1.Y, 0));
                        if (c1.Y == 1) en1 = EdgeNum.B;
                        else en1 = EdgeNum.F;
                    }
                }
                
                tmp1 = GenericCoordinate(en1);
                tmp2 = GenericCoordinate(en2);
                x = tmp1.X != 0 ? tmp1.X : tmp2.X;
                y = tmp1.Y != 0 ? tmp1.Y : tmp2.Y;
                offset = new Coordinates(tmp1.X == 0 ? -x : tmp1.X, tmp1.Y == 0 ? -y : tmp1.Y, -1);
                RotateSo(new Coordinates(x, y, -1), new Coordinates(x, y, 1), en1, solving, rc);
                RotateSo(new Coordinates(x, y, -1), new Coordinates(x, y, 1), en2, solving, rc);
                RotateSo(new Coordinates(x, y, 1), new Coordinates(x, y, -1), en1, solving, rc);
                RotateSo(new Coordinates(x, y, 1), new Coordinates(x, y, -1), en2, solving, rc);
                RotateSo(new Coordinates(x, y, -1), offset, EdgeNum.D, solving, rc);
                RotateSo(new Coordinates(x, y, 1), new Coordinates(x, y, -1), en2, solving, rc);
                RotateSo(offset, new Coordinates(x, y, -1), EdgeNum.D, solving, rc);
                RotateSo(new Coordinates(x, y, -1), new Coordinates(x, y, 1), en2, solving, rc);
                if (!diag) break;
            }
        }
        private static void BuildMidAndYellowCross(List<Rotation> solving, RubiksCube rc)
        {
            Cube c1 = null;
            //Cube c2 = null;
            //Куда поворачивать en и D
            Coordinates from = null;
            Coordinates toU = null;
            Coordinates toD = null;
            Coordinates coordtmp1 = null;
            Coordinates coordtmp2 = null;
            int x, y; //координаты для from и toU
            EdgeNum en;
            //EdgeNum en2;
            //Для нахождения toU
            EdgeNum enU;
            //Для нахождения toD (противоположная к enU)
            EdgeNum enD;
            //Кандидаты с нижней стороны и с второго слоя на замену
            Cube candidateWOYellowD = null;
            Cube candidateWithYellowD = null;
            Cube candidateWOYellowM = null;
            Cube candidateWithYellowM = null;
            while (true)
            {
                candidateWOYellowD = null;
                candidateWithYellowD = null;
                candidateWOYellowM = null;
                candidateWithYellowM = null;
                #region Поиск кандидатов на перестановку с нижней грани
                for (int i = -1; i < 2; ++i)
                {
                    for (int j = -1; j < 2; ++j)
                    {
                        if ((i != 0 && j == 0) || (j != 0 && i == 0))
                        {
                            c1 = rc.GetCube(new Coordinates(i, j, -1));
                            if (c1.Colors.Contains(new OrientedColor(Color.Y, null), new OrientedColorColorEqualityComparer()))
                            {
                                if (candidateWithYellowD == null)
                                {
                                    if (c1.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.Y; }).Coord.Z != -1)
                                    {
                                        candidateWithYellowD = c1;
                                    }
                                }
                            }
                            else
                            {
                                if (candidateWOYellowD == null) candidateWOYellowD = c1;
                            }
                        }
                    }
                }
                #endregion
                #region Поиск кандидатов на перестановку со второго слоя
                for (int i = -1; i < 2; ++i)
                {
                    for (int j = -1; j < 2; ++j)
                    {
                        if (i != 0 && j != 0)
                        {
                            c1 = rc.GetCube(new Coordinates(i, j, 0));
                            if (c1.Colors.Contains(new OrientedColor(Color.Y, null), new OrientedColorColorEqualityComparer()))
                            {
                                if (candidateWithYellowM == null)
                                {
                                    candidateWithYellowM = c1;
                                }
                            }
                            else
                            {
                                if (candidateWOYellowM == null)
                                {
                                    if ((int)c1.Colors[0].C != (int)ChooseEdge(c1.Colors[0].Coord) ||
                                        (int)c1.Colors[1].C != (int)ChooseEdge(c1.Colors[1].Coord))
                                        candidateWOYellowM = c1;
                                }
                            }
                        }
                    }
                }
                #endregion
                if (candidateWithYellowD == null && candidateWOYellowD == null &&
                    candidateWithYellowM == null && candidateWOYellowM == null) break;
                #region candidateWithYellowD != null
                if (candidateWithYellowD != null)
                {
                    if (candidateWithYellowM == null && candidateWOYellowM == null)
                    {
                        en = ChooseEdge(candidateWithYellowD.Coord.X, candidateWithYellowD.Coord.Y, 0);
                        enU = ChooseEdge(candidateWithYellowD.Coord.X == 0 ? 1 : 0,
                                         candidateWithYellowD.Coord.Y == 0 ? 1 : 0, 0);
                    }
                    else if (candidateWithYellowM != null)
                    {
                        int ind = candidateWithYellowM.Colors.FindIndex(delegate(OrientedColor oc) { return oc.C == Color.Y; });
                        enU = ChooseEdge(candidateWithYellowM.Colors[ind].Coord);
                        en = ChooseEdge(candidateWithYellowM.Colors[(1 + ind) % 2].Coord);
                        coordtmp1 = GenericCoordinate(en);
                        RotateSo(candidateWithYellowD.Coord, new Coordinates(coordtmp1.X, coordtmp1.Y, -1), EdgeNum.D, solving, rc);
                    }
                    else /*if (candidateWOYellowM != null)*/
                    {
                        enU = ChooseEdge(candidateWOYellowM.Colors[0].Coord);
                        en = ChooseEdge(candidateWOYellowM.Colors[1].Coord);
                        coordtmp1 = GenericCoordinate(en);
                        RotateSo(candidateWithYellowD.Coord, new Coordinates(coordtmp1.X, coordtmp1.Y, -1), EdgeNum.D, solving, rc);
                    }
                }
                #endregion
                #region candidateWOYellowD != null
                else if (candidateWOYellowD != null)
                {
                    int ind = candidateWOYellowD.Colors.FindIndex(delegate(OrientedColor oc) { return oc.Coord.Z == 0; });
                    en = (EdgeNum)candidateWOYellowD.Colors[ind].C;
                    enU = (EdgeNum)candidateWOYellowD.Colors[(1 + ind) % 2].C;
                    coordtmp1 = GenericCoordinate(en);
                    RotateSo(candidateWOYellowD.Coord, new Coordinates(coordtmp1.X, coordtmp1.Y, -1), EdgeNum.D, solving, rc);
                }
                #endregion
                #region candidateWithYellowD == null && candidateWOYellowD == null
                else /*candidateWithYellowD == null && candidateWOYellowD == null*/
                {
                    //Такое возможно только если собран жёлтый крест внизу, поэтому кандидат с жёлтым с второго слоя всегда null
                    en = ChooseEdge(candidateWOYellowM.Colors[0].Coord);
                    enU = ChooseEdge(candidateWOYellowM.Colors[1].Coord);
                }
                #endregion
                enD = (EdgeNum)(5 - (int)enU);
                coordtmp1 = GenericCoordinate(en);
                coordtmp2 = GenericCoordinate(enU);
                x = coordtmp1.X != 0 ? coordtmp1.X : coordtmp2.X;
                y = coordtmp1.Y != 0 ? coordtmp1.Y : coordtmp2.Y;
                from = new Coordinates(x, y, -1);
                toU = new Coordinates(x, y, 1);
                coordtmp2 = GenericCoordinate(enD);
                toD = new Coordinates(coordtmp1.X != 0 ? coordtmp1.X : coordtmp2.X, coordtmp1.Y != 0 ? coordtmp1.Y : coordtmp2.Y, -1);

                Nine(from, toU, toD, en, solving, rc);
                //RotateSo(from, toU, en, solving, rc);
                //RotateSo(from, toD, EdgeNum.D, solving, rc);
                //RotateSo(from, toU, en, solving, rc);
                //RotateSo(from, toD, EdgeNum.D, solving, rc);
                //RotateSo(from, toU, en, solving, rc);

                //RotateSo(toD, from, EdgeNum.D, solving, rc);
                //RotateSo(toU, from, en, solving, rc);
                //RotateSo(toD, from, EdgeNum.D, solving, rc);
                //RotateSo(toU, from, en, solving, rc);
            }
        }
        private static void PutYellowAngles(List<Rotation> solving, RubiksCube rc)
        {
            //После X'X' нижние углы с прилегающими цветами выглядит так
            //   4     6
            //5  0     1  7
            //              
            //9  2     3  11
            //   8     10
            //(так можно легко в цикле обработать)
            //Ищем направление желтых сторон, кодируем соответствующим образом и обрабатываем
            int[] yellowOr = new int[4];
            Cube curCube = null;
            int count = 0; //номер кубика, который мы обрабатываем в следующем цикле
            Coordinates Yto = null;
            int numOfTurns = 0;
            bool left;
            Coordinates U = null;
            Coordinates Dfrom = null;
            Coordinates Dto = null;
            Coordinates Dopp = null;
            EdgeNum en = EdgeNum.F;
            while (true)
            {
                count = 0;
                for (int i = 1; i > -2; --i)
                {
                    if (i == 0) continue;
                    for (int j = -1; j < 2; ++j)
                    {
                        if (j == 0) continue;
                        curCube = rc.GetCube(new Coordinates(i, j, -1));
                        Yto = curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C == Color.Y; }).Coord;
                        if (Yto.Z != 0) yellowOr[count] = count;
                        else yellowOr[count] = 3 + count * 2 + (Yto.X != 0 ? 1 : 2);
                        ++count;
                    }
                }
                bool isComplete = true;
                for (int i = 0; i < 3 && isComplete; ++i) if (!yellowOr.Contains(i)) isComplete = false;
                if (isComplete) break;

                numOfTurns = FindBestPosition(yellowOr, out left, out isComplete);

                switch (numOfTurns)
                {
                    case 0:
                        if (left)
                        {
                            U = new Coordinates(1, -1, 1);
                            Dto = new Coordinates(-1, -1, -1);
                            en = EdgeNum.L;
                        }
                        else
                        {
                            U = new Coordinates(1, 1, 1);
                            Dto = new Coordinates(-1, 1, -1);
                            en = EdgeNum.R;
                        }
                        break;
                    case 1:
                        if (left)
                        {
                            U = new Coordinates(-1, -1, 1);
                            Dto = new Coordinates(-1, 1, -1);
                            en = EdgeNum.B;
                        }
                        else
                        {
                            U = new Coordinates(1, -1, 1);
                            Dto = new Coordinates(1, 1, -1);
                            en = EdgeNum.F;
                        }
                        break;
                    case 2:
                        if (left)
                        {
                            U = new Coordinates(-1, 1, 1);
                            Dto = new Coordinates(1, 1, -1);
                            en = EdgeNum.R;
                        }
                        else
                        {
                            U = new Coordinates(-1, -1, 1);
                            Dto = new Coordinates(1, -1, -1);
                            en = EdgeNum.L;
                        }
                        break;
                    case 3:
                        if (left)
                        {
                            U = new Coordinates(1, 1, 1);
                            Dto = new Coordinates(1, -1, -1);
                            en = EdgeNum.F;
                        }
                        else
                        {
                            U = new Coordinates(-1, 1, 1);
                            Dto = new Coordinates(-1, -1, -1);
                            en = EdgeNum.B;
                        }
                        break;
                }
                Dfrom = new Coordinates(U.X, U.Y, -1);
                Dopp = new Coordinates(-Dfrom.X, -Dfrom.Y, -1);

                RotateSo(U, Dfrom, en, solving, rc);
                RotateSo(Dfrom, Dopp, EdgeNum.D, solving, rc);
                RotateSo(Dfrom, U, en, solving, rc);
                RotateSo(Dfrom, Dto, EdgeNum.D, solving, rc);
                RotateSo(U, Dfrom, en, solving, rc);
                RotateSo(Dfrom, Dto, EdgeNum.D, solving, rc);
                RotateSo(Dfrom, U, en, solving, rc);

                if (isComplete) break;
            }
            Cube c = rc.FindCube(Color.G, Color.R, Color.Y);
            RotateSo(c.Coord, new Coordinates(1, 1, -1), EdgeNum.D, solving, rc);
        }
        #region Вспомогательные методы для PutYellowAngles
        private static int[] CombinationSix(int[] yellowOr, bool left)
        {
            //left:     right:
            //0->10     0->11
            //4->11     4->3
            //5->3      5->10
            //
            //1->9      1->8
            //6->2      6->9
            //7->8      7->2
            //
            //2->1      2->6
            //8->6      8->7
            //9->7      9->1
            //
            //3->4      3->0
            //10->5     10->4
            //11->0     11->5

            int[] res = new int[4];
            List<int> change = new List<int>();
            for (int i = 0; i < 4; ++i)
            {
                change.Clear();
                if (i == 0) { change.Add(10); change.Add(11); change.Add(3); }
                else if (i == 1) { change.Add(9); change.Add(2); change.Add(8); }
                else if (i == 2) { change.Add(1); change.Add(6); change.Add(7); }
                else { change.Add(4); change.Add(5); change.Add(0); }
                if (!left)
                {
                    if (i % 2 == 0)
                    {
                        change.Insert(3, change[0]);
                        change.RemoveAt(0);
                    }
                    else
                    {
                        change.Insert(0, change[2]);
                        change.RemoveAt(3);
                    }
                }
                if (yellowOr[i] == i) res[i] = change[0];
                else if (yellowOr[i] == 3 + i * 2 + 1) res[i] = change[1];
                //if (yellowOr.Contains(i)) res[i] = change[0];
                //else if (yellowOr.Contains(3 + i * 2 + 1)) res[i] = change[1];
                else res[i] = change[2];
            }
            return res;
        }
        private static int[] RotateClockwise(int[] yellowOr)
        {
            int[] res = new int[4];
            for (int i = 0; i < 4; ++i)
            {
                switch (yellowOr[i])
                {
                    case 0:
                        res[1] = 1;
                        break;
                    case 4:
                        res[1] = 7;
                        break;
                    case 5:
                        res[1] = 6;
                        break;
                    case 1:
                        res[3] = 3;
                        break;
                    case 6:
                        res[3] = 11;
                        break;
                    case 7:
                        res[3] = 10;
                        break;
                    case 2:
                        res[0] = 0;
                        break;
                    case 8:
                        res[0] = 5;
                        break;
                    case 9:
                        res[0] = 4;
                        break;
                    case 3:
                        res[2] = 2;
                        break;
                    case 10:
                        res[2] = 9;
                        break;
                    case 11:
                        res[2] = 8;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            return res;
        }
        //Возвращает количество поворотов по часовой стрелке, необходимое, чтобы подставить нижнюю грань под правильный поворот,
        //или -1, если нужно крутить другой стороной
        //будет собран, если в результате все жёлтые стороны будут смотреть вниз
        private static int FindBestPositionForOneSide(int[] yellowOr, bool left, out bool willBeComplete)
        {
            int[] complete = { 0, 1, 2, 3 };
            int[] var1 = { 0, 6, 8, 11 };
            int[] var2 = { 0, 7, 9, 10 };
            willBeComplete = false;
            int varsCases = -1;
            int res = -1;
            bool curTry = true;
            //Копии для кручения
            int[] yellowOrCopy = new int[4];
            yellowOr.CopyTo(yellowOrCopy, 0);
            int[] varCopy = new int[4];
            int[] handled = null;
            for (int i = 0; i < 4; ++i)
            {
                handled = CombinationSix(yellowOrCopy, left);
                curTry = true;
                for (int j = 0; j < 4 && curTry; ++j)
                {
                    if (!complete.Contains(handled[j])) curTry = false;
                }
                if (curTry)
                {
                    res = i;
                    willBeComplete = true;
                    break;
                }

                //Уже нашли решение, продолжаем искать лучшее
                if (varsCases != -1) { yellowOrCopy = RotateClockwise(yellowOrCopy); continue; }

                //Вариант 1
                var1.CopyTo(varCopy, 0);
                for (int j = 0; j < 4; ++j)
                {
                    curTry = true;
                    for (int k = 0; k < 4 && curTry; ++k)
                    {
                        if (!varCopy.Contains(handled[k])) curTry = false;
                    }
                    if (curTry) break;
                    varCopy = RotateClockwise(varCopy);
                }
                if (curTry)
                {
                    yellowOrCopy = RotateClockwise(yellowOrCopy);
                    varsCases = i; continue;
                }

                //Вариант 2
                var2.CopyTo(varCopy, 0);
                for (int j = 0; j < 4; ++j)
                {
                    curTry = true;
                    for (int k = 0; k < 4 && curTry; ++k)
                    {
                        if (!varCopy.Contains(handled[k])) curTry = false;
                    }
                    if (curTry) break;
                    varCopy = RotateClockwise(varCopy);
                }
                if (curTry)
                {
                    yellowOrCopy = RotateClockwise(yellowOrCopy);
                    varsCases = i; continue;
                }

                yellowOrCopy = RotateClockwise(yellowOrCopy);
            }
            if (res == -1) res = varsCases;
            return res;
        }
        //Возвращает то же значение, что и FindBestPositionForOneSide,
        //но проверяет положение как в случае если крутить сливе, так и справа
        private static int FindBestPosition(int[] yellowOr, out bool left, out bool willBeComplete)
        {
            left = true;
            willBeComplete = false;
            int res = -1;
            int varLeft = -1;
            int varRight = -1;
            varLeft = FindBestPositionForOneSide(yellowOr, left, out willBeComplete);
            if (willBeComplete)
            {
                res = varLeft;
                return res;
            }
            left = false;
            varRight = FindBestPositionForOneSide(yellowOr, left, out willBeComplete);
            if (willBeComplete)
            {
                res = varRight;
                return res;
            }
            if (varLeft == -1) { res = varRight; left = false; }
            else { res = varLeft; left = true; }
            return res;
        }
        #endregion
        private static void BuildCorrectYellowCross(List<Rotation> solving, RubiksCube rc)
        {
            int countOfWrong = 0;
            Cube curCube = null;
            Coordinates correct = null;
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    if ((i != 0 && j != 0) || (i == 0 && j == 0)) continue;
                    curCube = rc.GetCube(new Coordinates(i, j, -1));
                    if ((int)curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C != Color.Y; }).C !=
                        (int)ChooseEdge(i, j, 0))
                    {
                        ++countOfWrong;
                    }
                    else correct = new Coordinates(i, j, -1);
                }
            }
            //Или все стоят правильно, или 1, или все неправильно
            if (countOfWrong == 0) return;
            if (countOfWrong == 4)
            {
                correct = null;
                //Всё равно с какой стороны крутить
                Nine(new Coordinates(1, 1, -1), new Coordinates(1, 1, 1), new Coordinates(-1, 1, -1), EdgeNum.R, solving, rc);
                RotateSo(new Coordinates(1, 0, -1), new Coordinates(-1, 0, -1), EdgeNum.D, solving, rc);
                Nine(new Coordinates(1, 1, -1), new Coordinates(1, 1, 1), new Coordinates(-1, 1, -1), EdgeNum.R, solving, rc);
                for (int i = -1; i < 2 && correct == null; ++i)
                {
                    for (int j = -1; j < 2; ++j)
                    {
                        if ((i != 0 && j != 0) || (i == 0 && j == 0)) continue;
                        curCube = rc.GetCube(new Coordinates(i, j, -1));
                        if ((int)curCube.Colors.Find(delegate(OrientedColor oc) { return oc.C != Color.Y; }).C ==
                            (int)ChooseEdge(i, j, 0))
                        {
                            correct = new Coordinates(i, j, -1); break;
                        }
                    }
                }
            }
            Coordinates left = new Coordinates(correct.X == 0 ? 1 : 0, correct.Y == 0 ? 1 : 0, -1);
            Coordinates right = new Coordinates(-left.X, -left.Y, -1);
            Cube leftCube = rc.GetCube(left);
            //Cube rightCube = rc.GetCube(right);
            EdgeNum en;
            Coordinates from = null;
            Coordinates toU = null;
            Coordinates toD = null;
            Coordinates toRot = null;
            if ((int)leftCube.Colors.Find(delegate(OrientedColor oc) { return oc.C != Color.Y; }).C == (int)ChooseEdge(right))
            {
                toRot = left;
            }
            else toRot = right;
            en = ChooseEdge(toRot.X, toRot.Y, 0);
            from = new Coordinates(toRot.X != 0 ? toRot.X : correct.X, toRot.Y != 0 ? toRot.Y : correct.Y, -1);
            toU = new Coordinates(from.X, from.Y, 1);
            toD = new Coordinates(toRot.X != 0 ? toRot.X : -correct.X, toRot.Y != 0 ? toRot.Y : -correct.Y, -1);
            Nine(from, toU, toD, en, solving, rc);
            RotateSo(new Coordinates(1, 0, -1), new Coordinates(-1, 0, -1), EdgeNum.D, solving, rc);
            Nine(from, toU, toD, en, solving, rc);
        }

        //Первый ход - из from к toU по en, второй - из from к toD по D и т.д.
        private static void Nine(Coordinates from, Coordinates toU, Coordinates toD, EdgeNum en,
                                 List<Rotation> solving, RubiksCube rc)
        {
            RotateSo(from, toU, en, solving, rc);
            RotateSo(from, toD, EdgeNum.D, solving, rc);
            RotateSo(from, toU, en, solving, rc);
            RotateSo(from, toD, EdgeNum.D, solving, rc);
            RotateSo(from, toU, en, solving, rc);

            RotateSo(toD, from, EdgeNum.D, solving, rc);
            RotateSo(toU, from, en, solving, rc);
            RotateSo(toD, from, EdgeNum.D, solving, rc);
            RotateSo(toU, from, en, solving, rc);
        }

        private static EdgeNum ChooseEdge(int x, int y, int z)
        {
            if (x == -1) return EdgeNum.B;
            else if (x == 1) return EdgeNum.F;
            else if (y == -1) return EdgeNum.L;
            else if (y == 1) return EdgeNum.R;
            else if (z == -1) return EdgeNum.D;
            else return EdgeNum.U;
        }
        private static EdgeNum ChooseEdge(Coordinates c)
        {
            return ChooseEdge(c.X, c.Y, c.Z);
        }
        //Возвращает общую координату для грани. Обратная к ChooseEdge
        private static Coordinates GenericCoordinate(EdgeNum en)
        {
            int x = 0, y = 0, z = 0;
            if (en == EdgeNum.U) z = 1;
            else if (en == EdgeNum.F) x = 1;
            else if (en == EdgeNum.R) y = 1;
            else if (en == EdgeNum.L) y = -1;
            else if (en == EdgeNum.B) x = -1;
            else z = -1;
            return new Coordinates(x, y, z);
        }
        //Одноходовый поворот
        private static void RotateSo(Coordinates from, Coordinates to, EdgeNum en, List<Rotation> solving, RubiksCube rc)
        {
            if (from == to) return;
            #region checking
            int count1 = 0, count2 = 0;
            if (from.X != 0) ++count1;
            if (from.Y != 0) ++count1;
            if (from.Z != 0) ++count1;
            if (to.X != 0) ++count2;
            if (to.Y != 0) ++count2;
            if (to.Z != 0) ++count2;
            if (count1 != count2) throw new ArgumentException();
            #endregion

            EdgeRotation er = null;
            bool anticlockwise = false;
            bool on180 = false;
            #region X
            if ((from.X == 1 && to.X == -1) || (from.X == -1 && to.X == 1))
            {
                if (from.Y != 0 && from.Z != 0)
                {
                    //углы
                    if (from.Y == to.Y && from.Z == to.Z)
                    {
                        if(en==EdgeNum.R || en==EdgeNum.L) anticlockwise = from.X == 1 ? (from.Z == -1) : (from.Z == 1);
                        else if (en == EdgeNum.U || en==EdgeNum.D) anticlockwise = from.X == 1 ? (from.Y == 1) : (from.Y == -1);
                        //else if (en == EdgeNum.D) anticlockwise = from.X == 1 ? (from.Y == -1) : (from.Y == 1);
                        //else if (en == EdgeNum.L) anticlockwise = from.X == 1 ? (from.Y == 1) : (from.Y == -1);
                        if ((int)en > 2) anticlockwise = !anticlockwise;
                    }
                    else
                    {
                        on180 = true;
                    }
                }
                else
                {
                    on180 = true;
                }
            }
            #endregion
            #region Y
            else if ((from.Y == 1 && to.Y == -1) || (from.Y == -1 && to.Y == 1))
            {
                if (from.X != 0 && from.Z != 0)
                {
                    //углы
                    if (from.X == to.X && from.Z == to.Z)
                    {
                        if(en==EdgeNum.F || en==EdgeNum.B) anticlockwise = from.Y == 1 ? (from.Z == 1) : (from.Z == -1);
                        else if (en == EdgeNum.U || en == EdgeNum.D) anticlockwise = from.Y == 1 ? (from.X == -1) : (from.X == 1);
                        //if (from.Z == -1) anticlockwise = !anticlockwise;
                        if ((int)en > 2) anticlockwise = !anticlockwise;
                    }
                    else
                    {
                        on180 = true;
                    }
                }
                else
                {
                    on180 = true;
                }
            }
            #endregion
            #region Z
            else if ((from.Z == 1 && to.Z == -1) || (from.Z == -1 && to.Z == 1))
            {
                if (from.X != 0 && from.Y != 0)
                {
                    //углы
                    if (from.X == to.X && from.Y == to.Y)
                    {
                        if (en == EdgeNum.F || en == EdgeNum.B) anticlockwise = from.Z == 1 ? (from.Y == -1) : (from.Y == 1);
                        else if (en == EdgeNum.R || en == EdgeNum.L) anticlockwise = from.Z == 1 ? (from.X == 1) : (from.X == -1);
                        //if (from.X == -1) anticlockwise = !anticlockwise;
                        if ((int)en > 2) anticlockwise = !anticlockwise;
                    }
                    else
                    {
                        on180 = true;
                    }
                }
                else
                {
                    on180 = true;
                }
            }
            #endregion

            if (count1 == 2 && !on180)
            {
                #region X
                //if (from.Z == -1 || from.Z == 1)
                if (en == EdgeNum.U || en == EdgeNum.D)
                {
                    if (from.X == 0 && to.X == 1)
                    {
                        anticlockwise = from.Y == -1 && to.Y == 0;
                        if (from.Z == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.X == 0 && to.X == -1)
                    {
                        anticlockwise = from.Y == 1 && to.Y == 0;
                        if (from.Z == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.X == 1 && to.X == 0)
                    {
                        anticlockwise = from.Y == 0 && to.Y == 1;
                        if (from.Z == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.X == -1 && to.X == 0)
                    {
                        anticlockwise = from.Y == 0 && to.Y == -1;
                        if (from.Z == -1) anticlockwise = !anticlockwise;
                    }
                }
                #endregion
                #region Y
                //else if (from.X == 1 || from.X == -1)
                else if (en == EdgeNum.F || en == EdgeNum.B)
                {
                    if (from.Y == 0 && to.Y == 1)
                    {
                        anticlockwise = from.Z == -1 && to.Z == 0;
                        if (from.X == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.Y == 0 && to.Y == -1)
                    {
                        anticlockwise = from.Z == 1 && to.Z == 0;
                        if (from.X == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.Y == 1 && to.Y == 0)
                    {
                        anticlockwise = from.Z == 0 && to.Z == 1;
                        if (from.X == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.Y == -1 && to.Y == 0)
                    {
                        anticlockwise = from.Z == 0 && to.Z == -1;
                        if (from.X == -1) anticlockwise = !anticlockwise;
                    }
                }
                #endregion
                #region Z
                //else if (from.Y == 1 || from.Y == -1)
                else
                {
                    if (from.Z == 0 && to.Z == 1)
                    {
                        anticlockwise = from.X == -1 && to.X == 0;
                        if (from.Y == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.Z == 0 && to.Z == -1)
                    {
                        anticlockwise = from.X == 1 && to.X == 0;
                        if (from.Y == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.Z == 1 && to.Z == 0)
                    {
                        anticlockwise = from.X == 0 && to.X == 1;
                        if (from.Y == -1) anticlockwise = !anticlockwise;
                    }
                    else if (from.Z == -1 && to.Z == 0)
                    {
                        anticlockwise = from.X == 0 && to.X == -1;
                        if (from.Y == -1) anticlockwise = !anticlockwise;
                    }
                }
                #endregion
            }
            er = new EdgeRotation(en, on180, anticlockwise);
            solving.Add(er);
            rc.RotateEdge(er);
        }
    }
}
