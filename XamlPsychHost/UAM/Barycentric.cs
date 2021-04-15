//
// Copyright © 2016-2020 miloush.net. All rights reserved.
//

namespace UAM.Windows
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Runtime.CompilerServices;
    using System.Diagnostics;

    // TODO:
    //
    // Triangle
    // 
    //     _a: {0.0737447291612625,0.536729633808136}
    //     _b: {0.0768564343452454,0.540282249450684}
    //     _c: {0.065443180501461,0.518944919109344}
    // 
    // clips 
    // 
    //     _x: 0.0660906583070755
    //     _y: 0.53412693738937378
    // 
    // to
    // 
    //     _x: 0.071377850436924586
    //     _y: 0.53165898198592576
    // 
    // which has barycentric of
    // 
    //     _a: 0.71488708657471367
    //     _b: -1.23801828116108E-14
    // 
    // E-14 is outside of tolerance for zero (which is 2.2E-15)
    // (the tolerance has been taken from .NET source, but might be not reasonable approach as per http://realtimecollisiondetection.net/)

    public struct Triangle
    {
        internal const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
        internal static bool IsZeroOrMore(double value)
        {
            return value > -10.0 * DBL_EPSILON;
        }
        internal static bool IsOneOrLess(double value)
        {
            return value - 1.0 < 10.0 * DBL_EPSILON;
        }
        internal static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }
        internal static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2) return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        private Point _a;
        private Point _b;
        private Point _c;

        public Triangle(Point a, Point b, Point c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public Point A { get { return _a; } set { _a = value; } }
        public Point B { get { return _b; } set { _b = value; } }
        public Point C { get { return _c; } set { _c = value; } }

        public Edge SideAB { get { return new Edge(A, B); } }
        public Edge SideBC { get { return new Edge(B, C); } }
        public Edge SideCA { get { return new Edge(C, A); } }

        public Triangle MidsegmentTriangle { get { return new Triangle(SideAB.MidPoint, SideBC.MidPoint, SideCA.MidPoint); } }

        public double Perimeter
        {
            get { return SideAB.Length + SideCA.Length + SideBC.Length; }
        }
        public double Area
        {
            get
            {
                // http://www.cs.berkeley.edu/~wkahan/Triangle.pdf for numerical stability

                double a = SideAB.Length;
                double b = SideBC.Length;
                double c = SideCA.Length;

                // ensure a > b > c
                double x;
                if (a > b)
                {
                    if (c > a) { x = a; a = b; b = x; }
                }
                else /* b > a */
                {
                    if (c > b) { x = a; a = c; c = x; }
                    else { x = a; a = b; b = x; }
                }
                // max set, [bc] unclear
                if (c > b) { x = b; b = c; c = x; }


                return Math.Sqrt((a + (b + c)) * (c - (a - b)) * (c + (a - b)) * (a + (b - c))) / 4.0;
            }
        }
        public Point Centroid
        {
            get { return new Point((A.X + B.X + C.X) / 3.0, (A.Y + B.Y + C.Y) / 3.0); }
        }

        public double AltitudeFromA { get { return 2 * Area / SideBC.Length; } }
        public double AltitudeFromB { get { return 2 * Area / SideCA.Length; } }
        public double AltitudeFromC { get { return 2 * Area / SideAB.Length; } }

        public Circle CircumCircle { get { return Circle.FromPoints(_a, _b, _c); } }

        public bool Contains(Point p)
        {
            return Contains(BarycentricPoint.FromOrthogonal(p, this));
        }
        public static bool Contains(BarycentricPoint p)
        {
            return IsZeroOrMore(p.A) && IsOneOrLess(p.A) &&
                   IsZeroOrMore(p.B) && IsOneOrLess(p.B) &&
                   IsZeroOrMore(p.C) && IsOneOrLess(p.C);
        }

        public bool Has(Edge e)
        {
            // exact, consider epsilon

            if (e.A == this.A)
                return e.B == this.B || e.B == this.C;

            else if (e.A == this.B)
                return e.B == this.A || e.B == this.C;

            else if (e.A == this.C)
                return e.B == this.A || e.B == this.B;

            return false;
        }

        public Point Clip(Point p)
        {
            BarycentricPoint bp = BarycentricPoint.FromOrthogonal(p, this);

            // we want to clip DBL_EPSILON too
            if (bp.A < 0) return Clip(p, B, C);
            if (bp.B < 0) return Clip(p, A, C);
            if (bp.C < 0) return Clip(p, A, B);

            return p;
        }
        public Point Clip(Point p, out double distance)
        {
            Point clipped = Clip(p);
            distance = (clipped - p).Length;
            return clipped;
        }
        private static Point Clip(Point p, Point lineA, Point lineB)
        {
            // orthogonal clip

            // ax + by + c = 0 side
            // dx + ey + f = 0 normal for P, d = b, e = -a

            double a = lineA.Y - lineB.Y;
            double b = lineB.X - lineA.X;

            if (a == 0) // AB horizontal
            {
                if (lineA.X > lineB.X) { Point l = lineA; lineA = lineB; lineB = l; }

                // clip to AB segment
                if (p.X < lineA.X)
                    return lineA;
                else if (p.X > lineB.X)
                    return lineB;

                return new Point(p.X, lineA.Y);
            }

            if (b == 0) // AB vertical
            {
                if (lineA.Y > lineB.Y) { Point l = lineA; lineA = lineB; lineB = l; }

                // clip to AB segment
                if (p.Y < lineA.Y)
                    return lineA;
                else if (p.Y > lineB.Y)
                    return lineB;

                return new Point(lineA.X, p.Y);
            }

            double c = -(a * lineA.X + b * lineA.Y);
            double f = a * p.Y - b * p.X;

            double y = (a * f - b * c) / (a * a + b * b);
            double x = -(b * y + c) / a;

            p = new Point(x, y);

            // clip to AB segment, we need normals for A and B
            // gx + hy + i = 0, g = d = b, h = e = -a

            double i = a * lineA.Y - b * lineA.X;
            if (f > i)
                return lineA;

            i = a * lineB.Y - b * lineB.X;
            if (f < i)
                return lineB;

            return p;
        }
        public double DistanceFrom(Point p)
        {
            double distance;
            Clip(p, out distance);
            return distance;
        }

        public RegularTriangleSampling SampleRegularly(int steps)
        {
            return new RegularTriangleSampling(this, steps);
        }
        public IEnumerable<Point> SampleFractally()
        {
            Queue<Triangle> triangles = new Queue<Triangle>();
            triangles.Enqueue(this);

            while (triangles.Count > 0)
            {
                Triangle t = triangles.Dequeue();

                yield return t.A;
                yield return t.B;
                yield return t.C;

                Point ab = t.SideAB.MidPoint;
                Point bc = t.SideBC.MidPoint;
                Point ca = t.SideCA.MidPoint;

                triangles.Enqueue(new Triangle(ab, bc, ca)); // covered by others but we want it to go first
                triangles.Enqueue(new Triangle(t.A, ab, ca));
                triangles.Enqueue(new Triangle(t.B, ab, bc));
                triangles.Enqueue(new Triangle(t.C, bc, ca));
            }
        }

        public override string ToString() => $"A={A} B={B} C={C}";
    }

    public struct Edge
    {
        private Point _a;
        private Point _b;

        public Edge(Point a, Point b)
        {
            _a = a;
            _b = b;
        }

        public Point A { get { return _a; } set { _a = value; } }
        public Point B { get { return _b; } set { _b = value; } }
        public Point MidPoint { get { return new Point((_a.X + _b.X) / 2, (_a.Y + _b.Y) / 2); } }

        public double Length { get { return Math.Sqrt(LengthSquared); } }
        public double LengthSquared
        {
            get
            {
                double xDistance = _a.X - _b.X;
                double yDistance = _a.Y - _b.Y;
                return xDistance * xDistance + yDistance * yDistance;
            }
        }

        public void Reverse()
        {
            Point p = _a;
            _a = _b;
            _b = p;
        }

        public Edge Scale(double factor)
        {
            double x = B.X * factor + A.X * (1 - factor);
            double y = B.Y * factor + A.Y * (1 - factor);

            return new Edge(A, new Point(x, y));
        }

        public static explicit operator Vector(Edge e) => e._b - e._a;

        public override string ToString() => $"A={A} B={B}";

        public Point Intersect(Edge e)
        {
            return Intersect(this, e);
        }
        public static Point Intersect(Edge a, Edge b)
        {
            return Intersect(a.A, a.B, b.A, b.B);
        }
        public static Point Intersect(Point a1, Point a2, Point b1, Point b2)
        {
            // naive solution, not checked for stability

            double x1 = a1.X, y1 = a1.Y;
            double x2 = a2.X, y2 = a2.Y;
            double x3 = b1.X, y3 = b1.Y;
            double x4 = b2.X, y4 = b2.Y;

            double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            double x = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
            double y = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);

            return new Point(x / d, y / d);
        }
    }

    public struct Circle
    {
        private Point _center;
        private double _radius;

        public Circle(Point center, double radius)
        {
            _center = center;
            _radius = EnsureNonNegative(radius);
        }

        public Point Center { get { return _center; } set { _center = value; } }
        public double Radius { get { return _radius; } set { _radius = EnsureNonNegative(value); } }
        public double Diameter { get { return 2 * _radius; } set { _radius = EnsureNonNegative(value) / 2; } }
        public double Area
        {
            get { return Math.PI * _radius * _radius; }
            set { _radius = Math.Sqrt(EnsureNonNegative(value) / Math.PI); }
        }

        public bool Contains(Point p)
        {
            double xDistance = p.X - _center.X;
            double yDistance = p.Y - _center.Y;
            return xDistance * xDistance + yDistance * yDistance <= _radius * _radius;
        }

        private static double EnsureNonNegative(double value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Value must be non-negative.");

            return value;
        }

        public static Circle FromPoints(Point p1, Point p2, Point p3)
        {
            double a = Determinant
            (
                p1.X, p1.Y,
                p2.X, p2.Y,
                p3.X, p3.Y
            );

            double xy1 = p1.X * p1.X + p1.Y * p1.Y;
            double xy2 = p2.X * p2.X + p2.Y * p2.Y;
            double xy3 = p3.X * p3.X + p3.Y * p3.Y;

            double bx = -Determinant
            (
                xy1, p1.Y,
                xy2, p2.Y,
                xy3, p3.Y
            );

            double by = Determinant
            (
                xy1, p1.X,
                xy2, p2.X,
                xy3, p3.X
            );

            Point center = new Point(-bx / a / 2, -by / a / 2);
            double radius = (center - p1).Length;

            return new Circle(center, radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Determinant(
            double a, double b, double c,
            double d, double e, double f,
            double g, double h, double i)
        {
            return a * (e * i - f * h) - b * (d * i - f * g) + c * (d * h - e * g);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Determinant(
            double a, double b,
            double d, double e,
            double g, double h)
        {
            return a * (e - h) - b * (d - g) + (d * h - e * g);
        }

    }

    public class RegularTriangleSampling : IReadOnlyList<Point>
    {
        private Triangle _triangle;
        private int _steps;

        public int Count
        {
            get { return GetSamplesCount(_steps); }
        }
        public int StepsPerAxis { get { return _steps; } }
        public Point this[int index]
        {
            get { return GetSample(_steps, index).ToOrthogonal(_triangle); }
        }

        public RegularTriangleSampling(Triangle triangle, int steps)
        {
            if (steps < 1)
                throw new ArgumentOutOfRangeException(nameof(steps), "Number of steps must be a positive number.");

            _triangle = triangle;
            _steps = steps;
        }

        public Triangle GetEnclosingTriangle(Point p)
        {
            Triangle triangle;
            if (TryGetEnclosingTriangle(p, out triangle))
                return triangle;

            throw new ArgumentOutOfRangeException(nameof(p), "The point is outside of the triangle.");
        }
        public bool TryGetEnclosingTriangle(Point p, out Triangle triangle)
        {
            triangle = default(Triangle);

            BarycentricPoint a, b, c;
            if (TryGetEnclosingTriangle(_steps, BarycentricPoint.FromOrthogonal(p, _triangle), out a, out b, out c))
            {
                triangle = new Triangle
                (
                    a.ToOrthogonal(_triangle),
                    b.ToOrthogonal(_triangle),
                    c.ToOrthogonal(_triangle)
                );

                return true;
            }

            return false;
        }
        public bool TryGetEnclosingTriangle(Point p, out IndexedTriangle indices)
        {
            return TryGetEnclosingTriangle(_steps, BarycentricPoint.FromOrthogonal(p, _triangle), out indices);
        }
        public static BarycentricPoint[] GetEnclosingTriangle(int steps, BarycentricPoint p)
        {
            BarycentricPoint[] triangle = new BarycentricPoint[3];
            if (TryGetEnclosingTriangle(steps, p, out triangle[0], out triangle[1], out triangle[2]))
                return triangle;

            throw new ArgumentOutOfRangeException(nameof(p), "The point is outside of the triangle.");
        }
        public static bool TryGetEnclosingTriangle(int steps, BarycentricPoint p, out IndexedTriangle indices)
        {
            int aStep1, aStep2, aStep3;
            int bStep1, bStep2, bStep3;
            indices = default(IndexedTriangle);

            if (TryGetEnclosingTriangle(steps, p, out aStep1, out aStep2, out aStep3, out bStep1, out bStep2, out bStep3))
            {
                indices.IndexOfA = GetIndex(steps, aStep1, bStep1);
                indices.IndexOfB = GetIndex(steps, aStep2, bStep2);
                indices.IndexOfC = GetIndex(steps, aStep3, bStep3);

                return true;
            }

            return false;
        }
        public static bool TryGetEnclosingTriangle(int steps, BarycentricPoint p, out BarycentricPoint a, out BarycentricPoint b, out BarycentricPoint c)
        {
            int aStep1, aStep2, aStep3;
            int bStep1, bStep2, bStep3;
            a = b = c = default(BarycentricPoint);

            if (TryGetEnclosingTriangle(steps, p, out aStep1, out aStep2, out aStep3, out bStep1, out bStep2, out bStep3))
            {
                double dSteps = (double)steps;
                a = new BarycentricPoint(aStep1 / dSteps, bStep1 / dSteps);
                b = new BarycentricPoint(aStep2 / dSteps, bStep2 / dSteps);
                c = new BarycentricPoint(aStep3 / dSteps, bStep3 / dSteps);
                return true;
            }

            return false;
        }
        private static bool TryGetEnclosingTriangle(int steps, BarycentricPoint p, out int aStep1, out int aStep2, out int aStep3, out int bStep1, out int bStep2, out int bStep3)
        {
            if (!Triangle.Contains(p))
            {
                aStep1 = aStep2 = aStep3 = default(int);
                bStep1 = bStep2 = bStep3 = default(int);
                return false;
            }

            double aMultiply = p.A * steps;
            double bMultiply = p.B * steps;
            double cMultiply = p.C * steps;

            int aStep = (int)aMultiply;
            int bStep = (int)bMultiply;
            int cStep = (int)cMultiply;

            // this gives us c in rhombus that contais it
            //
            //
            //    a /.¨¨¨¨/ d     - aStep
            //     /  .  /        / bStep
            //  c /____./ b       \ cStep

            bool Δ = (cStep ^ bStep ^ aStep ^ steps) % 2 != 0;

            if (Triangle.AreClose(aStep, aMultiply) && Triangle.AreClose(bStep, bMultiply))
            {
                //  special case - sampling vertex, always use abc
                Δ = true;

                if (Triangle.IsZero(cMultiply)) // except when we are on the outer edge
                {
                    if (aStep > 0) aStep--; // use left one unless this is the very top vertex where there is none
                    else bStep--;
                }
            }

            if (Δ)
            {
                aStep1 = aStep + 1; bStep1 = bStep;     // a
                aStep2 = aStep; bStep2 = bStep + 1;     // b
                aStep3 = aStep; bStep3 = bStep;         // c
            }
            else
            {
                aStep1 = aStep + 1; bStep1 = bStep;     // a
                aStep2 = aStep + 1; bStep2 = bStep + 1; // d
                aStep3 = aStep; bStep3 = bStep + 1;     // b
            }

            return true;
        }

        public static BarycentricPoint GetSample(int steps, int index)
        {
            int count = GetSamplesCount(steps);

            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index));

            int increasingIndex = count - index;
            double increasingN = (Math.Sqrt(1 + 8 * increasingIndex) - 1) / 2;  // finding N for series sum Sn = N(N+1)/2
            int increasingCount = (int)Math.Ceiling(increasingN);

            int aStepMax = steps + 1;

            int aStep = aStepMax - increasingCount;
            int bStep = index - aStep * (aStepMax + aStepMax - aStep + 1) / 2;

            return new BarycentricPoint(aStep / (double)steps, bStep / (double)steps);
        }
        public static int GetSamplesCount(int steps)
        {
            return (steps + 1) * (steps + 2) / 2;
        }
        public static bool TryGetStepsCount(int samplesCount, out int steps)
        {
            double solution = (Math.Sqrt(1 + 8 * samplesCount) - 1) / 2;  // finding N for series sum Sn = N(N+1)/2
            steps = (int)Math.Ceiling(solution) - 1;

            return steps > 0 && samplesCount == GetSamplesCount(steps);
        }

        public static int GetIndexOfA(int steps)
        {
            return GetSamplesCount(steps) - 1;
        }
        public static int GetIndexOfB(int steps)
        {
            return steps;
        }
        public static int GetIndexOfC(int steps)
        {
            return 0;
        }
        private static int GetIndex(int steps, int aStep, int bStep)
        {
            int aStepMax = steps + 1;
            return bStep + aStep * (aStepMax + aStepMax - aStep + 1) / 2;
        }

        public IEnumerator<Point> GetEnumerator()
        {
            for (int aStep = 0; aStep <= _steps; aStep++)
                for (int bStep = 0; bStep <= _steps; bStep++)
                {
                    double a = aStep / (double)_steps;
                    double b = bStep / (double)_steps;
                    double c = 1 - a - b;

                    if (Triangle.IsZeroOrMore(c) && Triangle.IsOneOrLess(c)) // (c >= 0 && c <= 1)
                        yield return BarycentricPoint.ToOrthogonal(_triangle.A, _triangle.B, _triangle.C, a, b, c);
                }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static IEnumerable<IndexedTriangle> EnumerateInnerTrianglesIndices(int steps)
        {
            //    a /.¨¨¨¨/ d     - aStep
            //     /  .  /        / bStep
            //  c /____./ b       \ cStep

            // for each bStep, return first abc triangles, then adb triangles

            int index1 = 1;
            int index2 = steps + 1;
            int index3 = index1 - 1;

            int remainingSteps = steps;
            while (remainingSteps > 0)
            {
                int initialIndex1 = index1;
                int initialIndex2 = index2;

                for (int i = 0; i < remainingSteps; i++)
                    yield return new IndexedTriangle(index1++, index2++, index3++);

                remainingSteps--;

                index1 = initialIndex1;
                index2 = initialIndex2 + 1;
                index3 += 1;

                for (int i = 0; i < remainingSteps; i++)
                    yield return new IndexedTriangle(index1++, index2++, index3++);

                index1 += 2;
                index2 += 0;
                index3 = index1 - 1;
            }
        }
        public IEnumerable<Triangle> EnumerateInnerTriangles()
        {
            foreach (IndexedTriangle indices in EnumerateInnerTrianglesIndices(_steps))
                yield return indices.ToTriangle(this);
        }
    }

    [Serializable]
    [DebuggerDisplay("a={A} b={B} c={C}")]
    public struct BarycentricPoint : IFormattable, IEquatable<BarycentricPoint>
    {
        private double _a;
        private double _b;

        public double A { get { return _a; } }
        public double B { get { return _b; } }
        public double C { get { return 1 - _a - _b; } }

        public BarycentricPoint(double a, double b)
        {
            _a = a;
            _b = b;
        }

        public void Offset(double offsetA, double offsetB)
        {
            _a += offsetA;
            _b += offsetB;
        }

        public static explicit operator Point(BarycentricPoint p) => new Point(p.A, p.B);
        public static explicit operator BarycentricPoint(Point p) => new BarycentricPoint(p.X, p.Y);

        public Point ToOrthogonal(Triangle t)
        {
            return ToOrthogonal(t.A, t.B, t.C, A, B, C);
        }
        public Point ToOrthogonal(Point a, Point b, Point c)
        {
            return ToOrthogonal(a, b, c, A, B, C);
        }

        public static Point ToOrthogonal(Point a, Point b, Point c, double aBarycentric, double bBarycentric)
        {
            return ToOrthogonal(a, b, c, aBarycentric, bBarycentric, 1 - aBarycentric - bBarycentric);
        }
        public static Point ToOrthogonal(Point a, Point b, Point c, double aBarycentric, double bBarycentric, double cBarycentric)
        {
            return new Point(a.X * aBarycentric + b.X * bBarycentric + c.X * cBarycentric, a.Y * aBarycentric + b.Y * bBarycentric + c.Y * cBarycentric);
        }
        public static BarycentricPoint FromOrthogonal(Point p, Triangle t)
        {
            // http://realtimecollisiondetection.net/

            Vector ba = t.B - t.A;
            Vector ca = t.C - t.A;
            Vector pa = p - t.A;

            double d00 = ba * ba;
            double d01 = ba * ca;
            double d11 = ca * ca;
            double d20 = pa * ba;
            double d21 = pa * ca;
            double denom = d00 * d11 - d01 * d01;

            double b = (d11 * d20 - d01 * d21) / denom;
            double c = (d00 * d21 - d01 * d20) / denom;
            double a = 1.0 - b - c;

            return new BarycentricPoint(a, b);
        }
        public static BarycentricPoint Average(BarycentricPoint p, BarycentricPoint q)
        {
            return new BarycentricPoint((p.A + q.A) / 2, (p.B + q.B) / 2);
        }

        public static BarycentricPoint operator *(BarycentricPoint p, double k)
        {
            return new BarycentricPoint(p.A * k, p.B * k);
        }
        public static BarycentricPoint operator /(BarycentricPoint p, double k)
        {
            return new BarycentricPoint(p.A / k, p.B / k);
        }
        public static Vector operator -(BarycentricPoint p, BarycentricPoint q)
        {
            return new Vector(p.A - q.A, p.B - q.B);
        }

        public static BarycentricPoint Parse(string source)
        {
            return (BarycentricPoint)Point.Parse(source);
        }

        public override string ToString() => ((Point)this).ToString();
        public string ToString(IFormatProvider formatProvider) => ((Point)this).ToString(formatProvider);
        string IFormattable.ToString(string format, IFormatProvider formatProvider) => ((IFormattable)(Point)this).ToString(format, formatProvider);

        #region Equality

        public bool Equals(BarycentricPoint other)
        {
            return BarycentricPoint.Equals(this, other);
        }
        public static bool Equals(BarycentricPoint p, BarycentricPoint q)
        {
            return p.A == q.A && p.B == q.B;
        }
        public override bool Equals(object obj)
        {
            if (null == obj || !(obj is BarycentricPoint))
                return false;

            BarycentricPoint value = (BarycentricPoint)obj;
            return BarycentricPoint.Equals(this, value);
        }
        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode();
        }

        public static bool operator ==(BarycentricPoint p, BarycentricPoint q)
        {
            return p.A == q.A && p.B == q.B;
        }
        public static bool operator !=(BarycentricPoint p, BarycentricPoint q)
        {
            return !(p == q);
        }

        #endregion
    }


    public struct IndexedTriangle
    {
        private int _indexOfA;
        private int _indexOfB;
        private int _indexOfC;

        public int IndexOfA { get { return _indexOfA; } set { _indexOfA = value; } }
        public int IndexOfB { get { return _indexOfB; } set { _indexOfB = value; } }
        public int IndexOfC { get { return _indexOfC; } set { _indexOfC = value; } }

        public IndexedEdge SideAB { get { return new IndexedEdge(_indexOfA, _indexOfB); } }
        public IndexedEdge SideBC { get { return new IndexedEdge(_indexOfB, _indexOfC); } }
        public IndexedEdge SideCA { get { return new IndexedEdge(_indexOfC, _indexOfA); } }

        public bool IsEmpty { get { return _indexOfA == _indexOfB && _indexOfB == _indexOfC; } }
        public bool IsDegenerate { get { return _indexOfA == _indexOfB || _indexOfA == _indexOfC || _indexOfB == _indexOfC; } }

        public IndexedTriangle(int indexOfA, int indexOfB, int indexOfC)
        {
            _indexOfA = indexOfA;
            _indexOfB = indexOfB;
            _indexOfC = indexOfC;
        }

        public Triangle ToTriangle(IReadOnlyList<Point> points)
        {
            return new Triangle(points[_indexOfA], points[_indexOfB], points[_indexOfC]);
        }
        public Triangle ToTriangle<T>(IList<T> points, Func<T, Point> selector)
        {
            return new Triangle(selector(points[_indexOfA]), selector(points[_indexOfB]), selector(points[_indexOfC]));
        }

        public bool Has(IndexedEdge e)
        {
            if (e.IndexOfA == _indexOfA)
                return e.IndexOfB == _indexOfB || e.IndexOfB == _indexOfC;

            else if (e.IndexOfA == _indexOfB)
                return e.IndexOfB == _indexOfA || e.IndexOfB == _indexOfC;

            else if (e.IndexOfA == _indexOfC)
                return e.IndexOfB == _indexOfA || e.IndexOfB == _indexOfB;

            return false;
        }

        public override string ToString() => $"{IndexOfA}-{IndexOfB}-{IndexOfC}";
    }

    public struct IndexedEdge
    {
        private int _indexOfA;
        private int _indexOfB;

        public int IndexOfA { get { return _indexOfA; } set { _indexOfA = value; } }
        public int IndexOfB { get { return _indexOfB; } set { _indexOfB = value; } }

        public IndexedEdge(int indexOfA, int indexOfB)
        {
            _indexOfA = indexOfA;
            _indexOfB = indexOfB;
        }

        public Edge ToEdge(IReadOnlyList<Point> points)
        {
            return new Edge(points[_indexOfA], points[_indexOfB]);
        }

        public bool Equals(IndexedEdge e, bool ignoreDirection)
        {
            return
                (e.IndexOfA == _indexOfA && e.IndexOfB == _indexOfB) ||
                (ignoreDirection && (e.IndexOfA == _indexOfB && e.IndexOfB == _indexOfA));
        }

        public override string ToString() => $"{IndexOfA}-{IndexOfB}";
    }
}
