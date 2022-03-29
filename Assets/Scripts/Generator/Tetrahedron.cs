using UnityEngine;

namespace Graphs
{

    public class Tetrahedron
    {
        public Vertex A;
        public Vertex B;
        public Vertex C;
        public Vertex D;

        public bool IsBad;

        private Vector3 sphereCenter;
        private float sphereRadiusSqr;

        public Tetrahedron(Vertex a, Vertex b, Vertex c, Vertex d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            CalculateSphere();
        }

        //https://mathworld.wolfram.com/Circumsphere.html
        private void CalculateSphere()
        {
            float a = new Matrix4x4(
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                Vector4.one
            ).determinant;

            float aSqr = A.Position.sqrMagnitude;
            float bSqr = B.Position.sqrMagnitude;
            float cSqr = C.Position.sqrMagnitude;
            float dSqr = D.Position.sqrMagnitude;

            float Dx = new Matrix4x4(
                new Vector4(aSqr, bSqr, cSqr, dSqr),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                Vector4.one
            ).determinant;

            float Dy = -new Matrix4x4(
                new Vector4(aSqr, bSqr, cSqr, dSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                Vector4.one
            ).determinant;

            float Dz = new Matrix4x4(
                new Vector4(aSqr, bSqr, cSqr, dSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                Vector4.one
            ).determinant;

            float c = new Matrix4x4(
                new Vector4(aSqr, bSqr, cSqr, dSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z)
            ).determinant;

            sphereCenter = new Vector3(Dx, Dy, Dz) / (2 * a);

            sphereRadiusSqr = (Dx * Dx + Dy * Dy + Dz * Dz - 4 * a * c) / (4 * a * a);

        }

        public bool Contains(Vertex v)
        {
            return A == v || B == v || C == v || D == v;
        }

        public bool CircumSphereContains(Vector3 v)
        {
            return (v - sphereCenter).sqrMagnitude <= sphereRadiusSqr;
        }

        public static bool operator ==(Tetrahedron a, Tetrahedron b)
        {
            return (a.A == b.A || a.A == b.B || a.A == b.C || a.A == b.D)
                   && (a.B == b.A || a.B == b.B || a.B == b.C || a.B == b.D)
                   && (a.C == b.A || a.C == b.B || a.C == b.C || a.C == b.D)
                   && (a.D == b.A || a.D == b.B || a.D == b.C || a.D == b.D);
        }

        public static bool operator !=(Tetrahedron a, Tetrahedron b)
        {
            return !(a == b);
        }

    }
}