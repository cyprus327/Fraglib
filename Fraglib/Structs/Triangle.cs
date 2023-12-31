using System.Numerics;

namespace Fraglib;

public static unsafe partial class FL {
    /// <region>Triangle</region>
    public struct Triangle {
        /// <name>ctor</name>
        /// <returns>Triangle</returns>
        /// <summary>Initializes an empty Triangle.</summary>
        public Triangle() { 
            Verts = new Vector3[3];
        }

        /// <name>ctor</name>
        /// <returns>Triangle</returns>
        /// <summary>Initializes a new Triangle with the specified vertices</summary>
        /// <param name="v1">The first of the triangle's vertices.</param>
        /// <param name="v2">The second of the triangle's vertices.</param>
        /// <param name="v3">The third of the triangle's vertices.</param>
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) {
            Verts = new Vector3[3] {
                v1, v2, v3
            };
        }

        /// <name>ctor</name>
        /// <returns>Triangle</returns>
        /// <summary>Initializes a new Triangle with the specified vertices</summary>
        /// <param name="v1">The first of the triangle's vertices. The Z will be ignored.</param>
        /// <param name="v2">The second of the triangle's vertices. The Z will be ignored.</param>
        /// <param name="v3">The third of the triangle's vertices. The Z will be ignored.</param>
        public Triangle(Vector2 v1, Vector2 v2, Vector2 v3) {
            Verts = new Vector3[3] {
                new(v1, 0f), new(v2, 0f), new(v3, 0f)
            };
        }

        /// <name>Color</name>
        /// <returns>uint</returns>
        /// <summary>Gets or sets the color of the triangle.</summary>
        public uint Color { get; set; } = White;

        /// <name>Verts</name>
        /// <returns>Vector3[]</returns>
        /// <summary>Gets the vertices of the triangle.</summary>
        public Vector3[] Verts { get; }

        /// <name>GetNormal</name>
        /// <returns>Vector3</returns>
        /// <summary>Calculates the normal of the triangle.</summary>
        public readonly Vector3 GetNormal() {
            return Vector3.Normalize(Vector3.Cross(Verts[1] - Verts[0], Verts[2] - Verts[1]));
        }

        /// <name>GetCenter</name>
        /// <returns>Vector3</returns>
        /// <summary>Calculates the center of the triangle.</summary>
        public readonly Vector3 GetCenter() {
            return (Verts[0] + Verts[1] + Verts[2]) / 3f;
        }
    }
}