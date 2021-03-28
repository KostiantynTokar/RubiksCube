using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace RubiksCube
{
    static class Visualizer
    {
        private static int windowWidth = 500;
        private static int windowHeight = 500;
        public const int Xmin = -250;
        public const int Xmax = 250;
        public const int Ymin = -250;
        public const int Ymax = 250;
        public const double ViewAngle = 45.0;
        public const double ZNear = 0.1;
        public const double ZFar = 100.0;

        public const double StdCubeSize = 1.2;
        public const double DistBetweenCubes = 0.03;

        private static RubiksCube rc = new RubiksCube();
        private static Queue<RubiksCube> rcOld = new Queue<RubiksCube>();
        private static Queue<EdgeRotationEventArgs> erArgs = new Queue<EdgeRotationEventArgs>();
        private static Queue<CubeRotationEventArgs> crArgs = new Queue<CubeRotationEventArgs>();
        private static Queue<bool> isERArgs = new Queue<bool>();
        private static bool isAnimationEnabled = false;

        private static double angle = 0;
        private static double rotationSpeed = StartingSpeed;

        private const double StartingZ = -10.0;
        private const double StartingAngleXY = 45.0;
        private const double StartingSpeed = 0.5;
        public const double MinRotationSpeed = 0.1;
        public const double RotationSpeedChenge = 0.1;

        public static double RotationSpeed
        {
            get { return rotationSpeed; }
            set { if (value > MinRotationSpeed) rotationSpeed = value; }
        }

        static Visualizer()
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_RGB | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(windowWidth, windowHeight);
            Glut.glutInitWindowPosition(700, 100);
            Glut.glutCreateWindow("Rubik's cube");

            Gl.glViewport(0, 0, windowWidth, windowHeight);
            Gl.glClearColor(0.85f, 0.85f, 0.85f, 0.0f);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(ViewAngle, ((double)windowWidth) / windowHeight, ZNear, ZFar);
            //Gl.glOrtho(-250, 250,
            //               -250, 250, ZNear, ZFar);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glTranslated(0, 0, StartingZ);
            Gl.glRotated(StartingAngleXY, 1, -1, 0);
            Gl.glClearDepth(1.0f);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);

            rc.CubeRotationEvent += rc_CubeRotationEvent;
            rc.EdgeRotationEvent += rc_EdgeRotationEvent;
            isAnimationEnabled = true;
        }

        private static void rc_EdgeRotationEvent(object sender, EdgeRotationEventArgs args)
        {
            //throw new NotImplementedException();
            //erArgs = args;
            //rcPrev = new RubiksCube((RubiksCube)sender);
            erArgs.Enqueue(args);
            rcOld.Enqueue(new RubiksCube((RubiksCube)sender));
            isERArgs.Enqueue(true);
            if (rcOld.Count == 1)
            {
                Glut.glutDisplayFunc(DisplayEdgeRotation);
            }
            Glut.glutPostRedisplay();
            //Glut.glutMainLoopEvent();
        }

        private static void rc_CubeRotationEvent(object sender, CubeRotationEventArgs args)
        {
            //throw new NotImplementedException();
            //crArgs = args;
            //rcPrev = new RubiksCube((RubiksCube)sender);
            crArgs.Enqueue(args);
            rcOld.Enqueue(new RubiksCube((RubiksCube)sender));
            isERArgs.Enqueue(false);
            if (rcOld.Count == 1)
            {
                Glut.glutDisplayFunc(DisplayCubeRotation);
            }
            Glut.glutPostRedisplay();
            //Glut.glutMainLoopEvent();
        }

        public static void Start()
        {
            Glut.glutDisplayFunc(Display);
            Glut.glutReshapeFunc(Reshape);
            Glut.glutKeyboardFunc(Keyb);
            Glut.glutMainLoop();
        }

        public static bool EnableAnimation()
        {
            if (isAnimationEnabled) return false;
            isAnimationEnabled = true;
            rc.CubeRotationEvent += rc_CubeRotationEvent;
            rc.EdgeRotationEvent += rc_EdgeRotationEvent;
            return true;
        }
        public static bool DisableAnimation()
        {
            if (!isAnimationEnabled) return false;
            isAnimationEnabled = false;
            rc.CubeRotationEvent -= rc_CubeRotationEvent;
            rc.EdgeRotationEvent -= rc_EdgeRotationEvent;
            return true;
        }

        private static void Display()
        {
            RubiksCube cur = rcOld.Count == 0 ? rc : rcOld.Peek();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    for (int k = -1; k < 2; ++k)
                    {
                        VisCube(cur.GetCube(new Coordinates(i, j, k)));
                    }
                }
            }
            Gl.glPopMatrix();
            Glut.glutSwapBuffers();
        }

        private static void DisplayEdgeRotation()
        {
            RubiksCube cur = rcOld.Count == 0 ? rc : rcOld.Peek();
            EdgeRotationEventArgs curERArgs = erArgs.Peek();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            double blink = StdCubeSize + DistBetweenCubes;
            Cube tmp = null;
            double direction = 1.0;
            if (curERArgs.ER.Anticlockwise) direction = -1.0;
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    for (int k = -1; k < 2; ++k)
                    {
                        if (curERArgs.ER.EN == EdgeNum.U && k == 1)
                        {
                            Gl.glPushMatrix();
                            Gl.glTranslated(0, blink, 0);
                            Gl.glRotated(angle, 0, -direction, 0);
                            tmp = cur.GetCube(new Coordinates(i, j, k));
                            VisCube(new Cube(tmp.Colors, new Coordinates(tmp.Coord.X, tmp.Coord.Y, 0)));
                            Gl.glPopMatrix();
                        }
                        else if (curERArgs.ER.EN == EdgeNum.F && i == 1)
                        {
                            Gl.glPushMatrix();
                            Gl.glTranslated(0, 0, blink);
                            Gl.glRotated(angle, 0, 0, -direction);
                            tmp = cur.GetCube(new Coordinates(i, j, k));
                            VisCube(new Cube(tmp.Colors, new Coordinates(0, tmp.Coord.Y, tmp.Coord.Z)));
                            Gl.glPopMatrix();
                        }
                        else if (curERArgs.ER.EN == EdgeNum.R && j == 1)
                        {
                            Gl.glPushMatrix();
                            Gl.glTranslated(blink, 0, 0);
                            Gl.glRotated(angle, -direction, 0, 0);
                            tmp = cur.GetCube(new Coordinates(i, j, k));
                            VisCube(new Cube(tmp.Colors, new Coordinates(tmp.Coord.X, 0, tmp.Coord.Z)));
                            Gl.glPopMatrix();
                        }
                        else if (curERArgs.ER.EN == EdgeNum.L && j == -1)
                        {
                            Gl.glPushMatrix();
                            Gl.glTranslated(-blink, 0, 0);
                            Gl.glRotated(angle, direction, 0, 0);
                            tmp = cur.GetCube(new Coordinates(i, j, k));
                            VisCube(new Cube(tmp.Colors, new Coordinates(tmp.Coord.X, 0, tmp.Coord.Z)));
                            Gl.glPopMatrix();
                        }
                        else if (curERArgs.ER.EN == EdgeNum.B && i == -1)
                        {
                            Gl.glPushMatrix();
                            Gl.glTranslated(0, 0, -blink);
                            Gl.glRotated(angle, 0, 0, direction);
                            tmp = cur.GetCube(new Coordinates(i, j, k));
                            VisCube(new Cube(tmp.Colors, new Coordinates(0, tmp.Coord.Y, tmp.Coord.Z)));
                            Gl.glPopMatrix();
                        }
                        else if (curERArgs.ER.EN == EdgeNum.D && k == -1)
                        {
                            Gl.glPushMatrix();
                            Gl.glTranslated(0, -blink, 0);
                            Gl.glRotated(angle, 0, direction, 0);
                            tmp = cur.GetCube(new Coordinates(i, j, k));
                            VisCube(new Cube(tmp.Colors, new Coordinates(tmp.Coord.X, tmp.Coord.Y, 0)));
                            Gl.glPopMatrix();
                        }
                        else VisCube(cur.GetCube(new Coordinates(i, j, k)));
                    }
                }
            }
            //switch (erArgs.ER.EN)
            //{
            //    case EdgeNum.U:
            //        Gl.glTranslated(blink, 0, 0);
            //        Gl.glRotated(angle, direction, 0, 0);
            //        Gl.glTranslated(-blink, 0, 0);
            //        break;
            //    case EdgeNum.F:
            //        Gl.glTranslated(0, 0, blink);
            //        Gl.glRotated(angle, 0, 0, direction);
            //        Gl.glTranslated(0, 0, -blink);
            //        break;
            //    case EdgeNum.R:
            //        Gl.glTranslated(0, blink, 0);
            //        Gl.glRotated(angle, 0, -direction, 0);
            //        Gl.glTranslated(0, -blink, 0);
            //        break;
            //    case EdgeNum.L:
            //        Gl.glTranslated(0, -blink, 0);
            //        Gl.glRotated(angle, 0, direction, 0);
            //        Gl.glTranslated(0, blink, 0);
            //        break;
            //    case EdgeNum.B:
            //        Gl.glTranslated(0, 0, -blink);
            //        Gl.glRotated(angle, 0, 0, -direction);
            //        Gl.glTranslated(0, 0, blink);
            //        break;
            //    case EdgeNum.D:
            //        Gl.glTranslated(-blink, 0, 0);
            //        Gl.glRotated(angle, -direction, 0, 0);
            //        Gl.glTranslated(blink, 0, 0);
            //        break;
            //}
            //Display();
            Glut.glutSwapBuffers();
            AngleIncrementation(curERArgs.ER.On180 ? 180.0 : 90.0);
        }

        private static void DisplayCubeRotation()
        {
            CubeRotationEventArgs curCRArgs = crArgs.Peek();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            double direction = 1.0;
            if (curCRArgs.CR.Anticlockwise) direction = -1.0;
            if (curCRArgs.CR.X)
            {
                Gl.glRotated(angle, -direction, 0, 0);
            }
            else if (curCRArgs.CR.Y)
            {
                Gl.glRotated(angle, 0, -direction, 0);
            }
            else
            {
                Gl.glRotated(angle, 0, 0, -direction);
            }
            Display();
            Gl.glPopMatrix();
            AngleIncrementation(90.0);
        }

        private static void AngleIncrementation(double maxAngle)
        {
            angle += rotationSpeed;
            if (angle > maxAngle)
            {
                angle = 0;
                bool isER = isERArgs.Dequeue();
                if (isER) erArgs.Dequeue();
                else crArgs.Dequeue();
                if (isERArgs.Count == 0) Glut.glutDisplayFunc(Display);
                else
                {
                    bool isERNext = isERArgs.Peek();
                    if (isERNext) Glut.glutDisplayFunc(DisplayEdgeRotation);
                    else Glut.glutDisplayFunc(DisplayCubeRotation);
                }
                //crArgs = null;
                //erArgs = null;
                //rcPrev = null;
                rcOld.Dequeue();
            }
            //System.Threading.Thread.Sleep(1000);
            Glut.glutPostRedisplay();
            //Glut.glutMainLoopEvent();
        }

        private static void Reshape(int w, int h)
        {
            windowWidth = w;
            windowHeight = h;
            Gl.glViewport(0, 0, windowWidth, windowHeight);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(ViewAngle, ((double)windowWidth) / windowHeight, ZNear, ZFar);
        }

        private static void Keyb(byte key, int x, int y)
        {
            //if (angle != 0) return;
            switch (key)
            {
                #region Cube Rotation
                case (byte)'v':
                    rc.RotateCube(new CubeRotation(true, false, false, false));
                    break;
                case (byte)'V':
                    rc.RotateCube(new CubeRotation(true, false, false, true));
                    break;
                case (byte)'b':
                    rc.RotateCube(new CubeRotation(false, true, false, false));
                    break;
                case (byte)'B':
                    rc.RotateCube(new CubeRotation(false, true, false, true));
                    break;
                case (byte)'n':
                    rc.RotateCube(new CubeRotation(false, false, true, false));
                    break;
                case (byte)'N':
                    rc.RotateCube(new CubeRotation(false, false, true, true));
                    break;
                #endregion

                #region Edge Clockwise Rotation
                case (byte)'w':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.U, false, false));
                    break;
                case (byte)'a':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.F, false, false));
                    break;
                case (byte)'s':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.R, false, false));
                    break;
                case (byte)'d':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.B, false, false));
                    break;
                case (byte)'f':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.L, false, false));
                    break;
                case (byte)'x':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.D, false, false));
                    break;
                #endregion

                #region Edge Anticlockwise Rotation
                case (byte)'W':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.U, false, true));
                    break;
                case (byte)'A':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.F, false, true));
                    break;
                case (byte)'S':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.R, false, true));
                    break;
                case (byte)'D':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.B, false, true));
                    break;
                case (byte)'F':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.L, false, true));
                    break;
                case (byte)'X':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.D, false, true));
                    break;
                #endregion

                #region Double Edge Rotation
                case (byte)'i':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.U, true, false));
                    break;
                case (byte)'j':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.F, true, false));
                    break;
                case (byte)'k':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.R, true, false));
                    break;
                case (byte)'l':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.B, true, false));
                    break;
                case (byte)';':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.L, true, false));
                    break;
                case (byte)',':
                    rc.RotateEdge(new EdgeRotation(EdgeNum.D, true, false));
                    break;
                #endregion

                case (byte)'`':
                    if (isAnimationEnabled) DisableAnimation();
                    else EnableAnimation();
                    break;
                case (byte)'=':
                    rotationSpeed += RotationSpeedChenge;
                    break;
                case (byte)'-':
                    rotationSpeed = Math.Max(MinRotationSpeed, rotationSpeed - RotationSpeedChenge);
                    break;

                case (byte)'1':
                    LayerMethod.Solve(rc);
                    break;
            }
            Glut.glutPostRedisplay();
        }

        private static void VisCube(Cube c)
        {
            Gl.glPushMatrix();
            double d = StdCubeSize / 2;
            Gl.glTranslated(c.Coord.Y * (StdCubeSize + DistBetweenCubes), c.Coord.Z * (StdCubeSize + DistBetweenCubes),
                            c.Coord.X * (StdCubeSize + DistBetweenCubes));
            OrientedColorCoordEqualityComparer comp = new OrientedColorCoordEqualityComparer();
            RGBColor color = null;
            OrientedColor tmpoc = null;
            #region U
            tmpoc = new OrientedColor(Color.W, new Coordinates(0, 0, 1));
            if (c.Colors.Contains(tmpoc, comp))
            {
                color = RGBColor.Colors[(int)c.Colors[
                    c.Colors.FindIndex(delegate(OrientedColor oc) { return comp.Equals(oc, tmpoc); })].C];
            }
            else color = RGBColor.Black;
            Gl.glColor3ub(color.R, color.G, color.B);
            //Верх-лево, верх-право, низ-право, низ-лево
            //VisQuad(d, -d, d, d, d, d,
            //        d, d, -d, d, -d, -d);
            VisQuad(-d, d, d, d, d, d,
                    d, d, -d, -d, d, -d);
            #endregion
            #region F
            tmpoc = new OrientedColor(Color.W, new Coordinates(1, 0, 0));
            if (c.Colors.Contains(tmpoc, comp))
            {
                color = RGBColor.Colors[(int)c.Colors[
                    c.Colors.FindIndex(delegate(OrientedColor oc) { return comp.Equals(oc, tmpoc); })].C];
            }
            else color = RGBColor.Black;
            Gl.glColor3ub(color.R, color.G, color.B);
            //Верх-лево, верх-право, низ-право, низ-лево
            //VisQuad(-d, d, -d, d, d, -d,
            //        d, -d, -d, -d, -d, -d);
            VisQuad(-d, d, d, d, d, d,
                    d, -d, d, -d, -d, d);
            #endregion
            #region R
            tmpoc = new OrientedColor(Color.W, new Coordinates(0, 1, 0));
            if (c.Colors.Contains(tmpoc, comp))
            {
                color = RGBColor.Colors[(int)c.Colors[
                    c.Colors.FindIndex(delegate(OrientedColor oc) { return comp.Equals(oc, tmpoc); })].C];
            }
            else color = RGBColor.Black;
            Gl.glColor3ub(color.R, color.G, color.B);
            //Верх-лево, верх-право, низ-право, низ-лево
            //VisQuad(-d, d, d, d, d, d,
            //        d, d, -d, -d, d, -d);
            VisQuad(d, -d, d, d, d, d,
                    d, d, -d, d, -d, -d);
            #endregion
            #region L
            tmpoc = new OrientedColor(Color.W, new Coordinates(0, -1, 0));
            if (c.Colors.Contains(tmpoc, comp))
            {
                color = RGBColor.Colors[(int)c.Colors[
                    c.Colors.FindIndex(delegate(OrientedColor oc) { return comp.Equals(oc, tmpoc); })].C];
            }
            else color = RGBColor.Black;
            Gl.glColor3ub(color.R, color.G, color.B);
            //VisQuad(-d, -d, d, d, -d, d,
            //        d, -d, -d, -d, -d, -d);
            VisQuad(-d, -d, d, -d, d, d,
                    -d, d, -d, -d, -d, -d);
            #endregion
            #region B
            tmpoc = new OrientedColor(Color.W, new Coordinates(-1, 0, 0));
            if (c.Colors.Contains(tmpoc, comp))
            {
                color = RGBColor.Colors[(int)c.Colors[
                    c.Colors.FindIndex(delegate(OrientedColor oc) { return comp.Equals(oc, tmpoc); })].C];
            }
            else color = RGBColor.Black;
            Gl.glColor3ub(color.R, color.G, color.B);
            //VisQuad(-d, d, d, d, d, d,
            //        d, -d, d, -d, -d, d);
            VisQuad(-d, d, -d, d, d, -d,
                    d, -d, -d, -d, -d, -d);
            #endregion
            #region D
            tmpoc = new OrientedColor(Color.W, new Coordinates(0, 0, -1));
            if (c.Colors.Contains(tmpoc, comp))
            {
                color = RGBColor.Colors[(int)c.Colors[
                    c.Colors.FindIndex(delegate(OrientedColor oc) { return comp.Equals(oc, tmpoc); })].C];
            }
            else color = RGBColor.Black;
            Gl.glColor3ub(color.R, color.G, color.B);
            //VisQuad(-d, -d, d, -d, d, d,
            //        -d, d, -d, -d, -d, -d);
            VisQuad(-d, -d, d, d, -d, d,
                    d, -d, -d, -d, -d, -d);
            #endregion
            Gl.glPopMatrix();
        }

        private static void VisQuad(double x1, double y1, double z1, double x2, double y2, double z2,
                                    double x3, double y3, double z3, double x4, double y4, double z4)
        {
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(x1, y1, z1);
            Gl.glVertex3d(x2, y2, z2);
            Gl.glVertex3d(x3, y3, z3);
            Gl.glVertex3d(x4, y4, z4);
            Gl.glEnd();
            Gl.glColor3ub(0, 0, 0);
            Gl.glLineWidth(3);
            Gl.glBegin(Gl.GL_LINE_LOOP);
            Gl.glVertex3d(x1, y1, z1);
            Gl.glVertex3d(x2, y2, z2);
            Gl.glVertex3d(x3, y3, z3);
            Gl.glVertex3d(x4, y4, z4);
            Gl.glEnd();
        }
    }
}
