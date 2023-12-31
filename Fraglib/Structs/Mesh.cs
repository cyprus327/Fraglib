using System.Numerics;

namespace Fraglib;

public static unsafe partial class FL {
    /// <region>Mesh</region>
    public readonly struct Mesh {
        /// <name>ctor</name>
        /// <returns>Mesh</returns>
        /// <summary>Initializes a new instantce of the Mesh struct created from the speciied OBJ file.</summary>
        /// <param name="objFilePath">The OBJ file to create the mesh from.</param>
        public Mesh(string objFilePath) {
            Tris = new();
            FileStream fs = new(objFilePath, FileMode.Open, FileAccess.Read);
            StreamReader stream = new(fs);
            if (!stream.BaseStream.CanRead) {
                return;
            }

            List<Vector3?> verts = new();

            while (!stream.EndOfStream) {
                string? line = stream.ReadLine();

                if (string.IsNullOrEmpty(line)) {
                    continue;
                }

                if (line[0] == 'v') {
                    if (line[1] != ' ') {
                        continue;
                    }

                    string[] info = line.Split(' ');
                    verts.Add(new(
                        Convert.ToSingle(info[1]),
                        -Convert.ToSingle(info[2]),
                        Convert.ToSingle(info[3])
                    ));
                } else if (line[0] == 'f') {
                    string[] info = line.Split(' ');
                    int[] f;
                    if (info[1].Contains('/')) {
                        f = new int[info.Length];
                        for (int i = 1; i < info.Length; i++) {
                            if (info[i] == "\n") {
                                break;
                            }
                            f[i - 1] = Convert.ToInt32(info[i].Split('/')[0]);
                        }
                    } else {
                        f = new int[3] {
                            Convert.ToInt32(info[1]),
                            Convert.ToInt32(info[2]),
                            Convert.ToInt32(info[3])
                        };
                    }

                    Vector3? v1 = verts[f[0] - 1];
                    Vector3? v2 = verts[f[1] - 1]; 
                    Vector3? v3 = verts[f[2] - 1]; 
                    if (v1 != null && v2 != null && v3 != null) {
                        Tris.Add(new((Vector3)v1, (Vector3)v2, (Vector3)v3));
                    }
                }
            }
        }

        /// <name>ctor</name>
        /// <returns>Mesh</returns>
        /// <summary>Initializes a new instantce of the Mesh struct with triangles specified.</summary>
        /// <param name="tris">The mesh's triangles.</param>
        public Mesh(List<Triangle> tris) {
            Tris = tris;
        }

        /// <name>Tris</name>
        /// <returns>List<Triangle></returns>
        /// <summary>The mesh's current triangles.</summary>
        public readonly List<Triangle> Tris { get; init; }

        /// <name>Cube</name>
        /// <returns>Mesh</returns>
        /// <summary>Creates a Mesh with 12 triangles defining a cube.</summary>
        public static Mesh Cube => new(
            new List<Triangle> {
                //south
                new(new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(1f, 1f, 0f)),
                new(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0f), new Vector3(1f, 0f, 0f)),

                // east                                                                                                   
                new(new Vector3(1f, 0f, 0f), new Vector3(1f, 1f, 0f), new Vector3(1f, 1f, 1f)),
                new(new Vector3(1f, 0f, 0f), new Vector3(1f, 1f, 1f), new Vector3(1f, 0f, 1f)),

                // north                                                                                                                                         
                new(new Vector3(1f, 0f, 1f), new Vector3(1f, 1f, 1f), new Vector3(0f, 1f, 1f)),
                new(new Vector3(1f, 0f, 1f), new Vector3(0f, 1f, 1f), new Vector3(0f, 0f, 1f)),
                                                                                                                        
                // west                                                                                                                           
                new(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 1f), new Vector3(0f, 1f, 0f)),
                new(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 0f)),
                                                                                                                        
                // top                                                                                                                            
                new(new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 1f), new Vector3(1f, 1f, 1f)),
                new(new Vector3(0f, 1f, 0f), new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 0f)),
                                                                                                                        
                // bottom                                                                                                                      
                new(new Vector3(1f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 0f)),
                new(new Vector3(1f, 0f, 1f), new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f))
            }
        );
    }
}