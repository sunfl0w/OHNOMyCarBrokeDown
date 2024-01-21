using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Sadly Unity does not support custom navmeshes so I programmed a workable replacement.
/// The NavMesh component can be used to compute the "hopefully" shortest path between two points using the GetPath() method.
/// Note that each call of GetPath() will parse the supplied navmesh again.
/// </summary>
public class NavMesh : MonoBehaviour {
    class NavNode {
        public Vector3 pos { get; set; }
        public NavNode parent { get; set; }
        public int triangleStartIndex { get; }
        public float g { get; set; }
        public float f { get; set; }
        public List<int> neighbours { get; }

        public NavNode(Vector3 pos, int triangleStartIndex, Mesh navmesh) {
            this.pos = pos;
            parent = null;
            this.triangleStartIndex = triangleStartIndex;
            g = float.PositiveInfinity;
            f = float.PositiveInfinity;
            neighbours = GetTriangleNeighbours(triangleStartIndex, navmesh);
        }
    }

    struct Portal {
        public Vector3 left { get; set; }
        public Vector3 right { get; set; }

        public Portal(Vector3 left, Vector3 right) {
            this.left = left;
            this.right = right;
        }
    }

    /// <summary>
    /// The mesh used for pathfinding. The mesh must only consist of triangles.
    /// Adjacent triangles must share vertices.
    /// </summary>
    public Mesh navmesh;

    /// <summary>
    /// Scaling differences of meshes between Blender and Unity might cause problems.
    /// This scaling factor is used to scale a mash to account for the scaling Unity expects.
    /// </summary>
    public float navmeshScale = 1.0f;

    /// <summary>
    /// Blender uses a different coordinate system compared to Unity.
    /// Enabling this flag will ensure correct conversion.
    /// </summary>
    public bool switchAxisYZ = true;

    private List<NavNode> navgraph;

    void Start() {
        navgraph = ParseNavmesh();
    }

    /// <summary>
    /// Parses the supplied mesh to generate a node graph.
    /// </summary>
    private List<NavNode> ParseNavmesh() {
        List<NavNode> navgraph = new List<NavNode>();
        for (int i = 0; i < navmesh.triangles.Length; i += 3) {
            float x = (navmesh.vertices[navmesh.triangles[i]].x +
                       navmesh.vertices[navmesh.triangles[i + 1]].x +
                       navmesh.vertices[navmesh.triangles[i + 2]].x) / 3.0f * navmeshScale;
            float y = (navmesh.vertices[navmesh.triangles[i]].y +
                       navmesh.vertices[navmesh.triangles[i + 1]].y +
                       navmesh.vertices[navmesh.triangles[i + 2]].y) / 3.0f * navmeshScale;
            float z = (navmesh.vertices[navmesh.triangles[i]].z +
                       navmesh.vertices[navmesh.triangles[i + 1]].z +
                       navmesh.vertices[navmesh.triangles[i + 2]].z) / 3.0f * navmeshScale;
            if(switchAxisYZ) {
                float tmp = y;
                y = z;
                z = tmp;
            }
            Vector3 nodePos = new Vector3(x, y, z);
            NavNode node = new NavNode(nodePos, i, navmesh);
            navgraph.Add(node);
        }
        return navgraph;
    }

    /// <summary>
    /// Iterates over all triangles in the navmesh and finds the neighbours of the specified triangle,
    /// </summary>
    private static List<int> GetTriangleNeighbours(int triangleStartIndex, Mesh navmesh) {
        List<int> triangleNeighbours = new List<int>();
        int a = navmesh.triangles[triangleStartIndex];
        int b = navmesh.triangles[triangleStartIndex + 1];
        int c = navmesh.triangles[triangleStartIndex + 2];

        for (int i = 0; i < navmesh.triangles.Length; i += 3) {
            if(i == triangleStartIndex) {
                continue;
            }

            int x = navmesh.triangles[i];
            int y = navmesh.triangles[i + 1];
            int z = navmesh.triangles[i + 2];

            // A shame that in C# a bool can not be implicitly converted to int :C
            int numMatches = Convert.ToInt32(a == x) + 
                             Convert.ToInt32(a == y) +
                             Convert.ToInt32(a == z) +
                             Convert.ToInt32(b == x) +
                             Convert.ToInt32(b == y) +
                             Convert.ToInt32(b == z) +
                             Convert.ToInt32(c == x) +
                             Convert.ToInt32(c == y) +
                             Convert.ToInt32(c == z);

            if (numMatches == 2) { // Tested triangles are neighbours
                triangleNeighbours.Add(i);
            }
        }
        return triangleNeighbours;
    }

    /// <summary>
    /// Returns the node in the open list with the lowest f score.
    /// </summary>
    private NavNode GetNextCandidateFromOpenList(List<NavNode> openList) {
        int minIndex = 0;
        float minScore = openList[minIndex].f;
        for (int i = 1; i < openList.Count; i++) {
            if(openList[i].f < minScore) {
                minScore = openList[i].f;
                minIndex = i;
            }
        }
        return openList[minIndex];
    }

    private void NavgraphReset() {
        foreach(NavNode node in navgraph) {
            node.parent = null;
            node.g = float.PositiveInfinity;
            node.f = float.PositiveInfinity;
        }
    }

    /// <summary>
    /// A* pathfinding using the supplied navmesh.
    /// </summary>
    public List<Vector3> GetPath(Vector3 startPos, Vector3 targetPos) {
        //List<NavNode> navgraph = ParseNavmesh();
        NavgraphReset();

        NavNode startNode = PosToCorrespondingNavNode(startPos, navgraph);
        if (startNode == null) {
            startNode = PosToClosestNavNode(startPos, navgraph); // Backup if startPos is outside mesh
        }
        NavNode endNode = PosToCorrespondingNavNode(targetPos, navgraph);
        if (endNode == null) {
            endNode = PosToClosestNavNode(targetPos, navgraph); // Backup if targetPos is outside mesh
        }

        if (startNode.triangleStartIndex == endNode.triangleStartIndex) { // Start and end are in the same navmesh triangle
            List<Vector3> simplePath = new List<Vector3>();
            simplePath.Add(targetPos);
            return simplePath;
        }

        startNode.g = 0.0f;
        startNode.f = Vector3.Distance(startNode.pos, endNode.pos);

        List<NavNode> openList = new List<NavNode>();
        openList.Add(startNode);

        while(openList.Count > 0) {
            NavNode current = GetNextCandidateFromOpenList(openList);
            if(current.triangleStartIndex == endNode.triangleStartIndex) { // Path found
                break;
            }
            openList.Remove(current);

            for (int i = 0; i < current.neighbours.Count; i++) {
                NavNode neighbour = TriangleStartIndexToNavNode(current.neighbours[i], navgraph);
                float score = current.g + Vector3.Distance(current.pos, neighbour.pos);
                if (score < neighbour.g) {
                    neighbour.parent = current;
                    neighbour.g = score;
                    neighbour.f = score + Vector3.Distance(neighbour.pos, endNode.pos);

                    if(!openList.Contains(neighbour)) {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        // Compute path based on parent node relation
        List<Vector3> path = new List<Vector3>();
        List<int> rawIndexPath = new List<int>();
        NavNode node = endNode;
        path.Add(targetPos); // As end node pos is close but not exactly target pos
        path.Add(node.pos);
        rawIndexPath.Add(node.triangleStartIndex);
        while(node.parent != null) {
            path.Add(node.parent.pos);
            rawIndexPath.Add(node.parent.triangleStartIndex);
            node = node.parent;
        }
        path.Add(startPos);

        // Reverse path such that the first waypoint is at index 0
        path.Reverse();
        rawIndexPath.Reverse();

        // Simplify path
        List<Vector3> simplifiedPath = SimplifyPath(path, rawIndexPath);
        return simplifiedPath;
    }

    /// <summary>
    /// Based on the code described here: https://digestingduck.blogspot.com/2010/03/simple-stupid-funnel-algorithm.html .
    /// The algorithm is really interesing and the arcticle is a fun read ;D
    /// </summary>
    private List<Vector3> SimplifyPath(List<Vector3> rawPath, List<int> rawIndexPath) {
        List<Portal> portals = new List<Portal>();
        portals.Add(new Portal(rawPath[0], rawPath[0]));

        // rawIndexPath contains all triangle indices of triangles moved across in rawPath.
        // Between two consecutive triangles an edge (so called portal) is defined with an orientation based on movement direction along the navgraph.
        for (int i = 0; i < rawIndexPath.Count - 1; i++) {
            portals.Add(GetPortal(rawIndexPath[i], rawIndexPath[i + 1]));
        }

        portals.Add(new Portal(rawPath[rawPath.Count - 1], rawPath[rawPath.Count - 1]));

        Vector3 pApex = portals[0].left;
        Vector3 pLeft = portals[0].left;
        Vector3 pRight = portals[0].right;

        List<Vector3> simplifiedPath = new List<Vector3>();
        simplifiedPath.Add(pApex);

        for (int i = 1; i < portals.Count; i++) {
            Vector3 left = portals[i].left;
            Vector3 right = portals[i].right;

            // Extend right
            if (TriangleArea(pApex, pRight, right) <= 0.0f) {
                if (Vector3.Distance(pApex, pRight) <= 0.001f || TriangleArea(pApex, pLeft, right) > 0.0f) {
                    pRight = right;
                } else {
                    simplifiedPath.Add(pLeft);
                    pApex = pLeft;
                    pLeft = pApex;
                    pRight = pApex;
                    continue;
                }
            }

            // Extend left
            if (TriangleArea(pApex, pLeft, left) >= 0.0f) {
                if(Vector3.Distance(pApex, pLeft) <= 0.001f || TriangleArea(pApex, pRight, left) < 0.0f) {
                    pLeft = left;
                } else {
                    simplifiedPath.Add(pRight);
                    pApex = pRight;
                    pLeft = pApex;
                    pRight = pApex;
                    continue;
                }
            }
        }
        simplifiedPath.Add(portals[portals.Count - 1].left);
        return simplifiedPath;
    }

    /// <summary>
    /// Get the corresponding portal (edge) of triangles triA and triB.
    /// The portal's left and right correspond are the left and right as seen from triA, when moving to triB.
    /// </summary>
    private Portal GetPortal(int triA, int triB) {
        Portal portal = new Portal();
        List<int> commonIndices = new List<int>();

        // Find the common indices of two adjacent triangles (CW ordering).
        // If for one vertex of triA no match was found an index of -1 is added instead.
        for (int x = 0; x < 3; x++) {
            bool common = false;
            for (int y = 0; y < 3; y++) {
                if (navmesh.triangles[triA + x] == navmesh.triangles[triB + y]) {
                    common = true;
                    break;
                }
            }
            if (common) {
                commonIndices.Add(navmesh.triangles[triA + x]);
            } else {
                commonIndices.Add(-1);
            }
        }

        // Order indices such that the first corresponds to the "left" vertex as seen from triA
        if(commonIndices[0] >= 0 && commonIndices[1] == -1) { // Order is wrong. Reorder indices of edge
            commonIndices.Remove(-1);
            (commonIndices[0], commonIndices[1]) = (commonIndices[1], commonIndices[0]);
        } else {
            commonIndices.Remove(-1);
        }

        Vector3 a = navmesh.vertices[commonIndices[0]] * navmeshScale;
        Vector3 b = navmesh.vertices[commonIndices[1]] * navmeshScale;
        if(switchAxisYZ) {
            (a.y, a.z) = (a.z, a.y);
            (b.y, b.z) = (b.z, b.y);
        }
        portal.left = a;
        portal.right = b;
        return portal;
    }

    /// <summary>
    /// Returns the are of a 3D triangle. Note that the computed are can be negative based on winding order.
    /// </summary>
    private float TriangleArea(Vector3 a, Vector3 b, Vector3 c) {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        return Vector3.Cross(ab, ac).y;
    }

    /// <summary>
    /// Computes the node of the given navgraph that represents the triangle with triangleStartIndex as the first index in the index vector of the navmesh.
    /// </summary>
    private NavNode TriangleStartIndexToNavNode(int triangleStartIndex, List<NavNode> navgraph) {
        for(int i = 0; i < navgraph.Count; i++) {
            if(triangleStartIndex == navgraph[i].triangleStartIndex) {
                return navgraph[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Computes the node in the navgraph closest to the specified position.
    /// </summary>
    private NavNode PosToCorrespondingNavNode(Vector3 pos, List<NavNode> navgraph) {
        for (int i = 0; i < navgraph.Count; i++) {
            if(PosInsideTriangle(pos, navgraph[i].triangleStartIndex)) {
                return navgraph[i];
            }
        }
        return null;
    }

    private NavNode PosToClosestNavNode(Vector3 pos, List<NavNode> navgraph) {
        int minIndex = 0;
        float minDistance = Vector3.Distance(pos, navgraph[minIndex].pos);
        for (int i = 1; i < navgraph.Count; i++) {
            float testDistance = Vector3.Distance(pos, navgraph[i].pos);
            if(testDistance < minDistance) {
                minDistance = testDistance;
                minIndex = i;
            }
        }
        return navgraph[minIndex];
    }

    /// <summary>
    /// Tests if a position is inside a triangle of given index.
    /// This test is performed in two dimensions and ignores the Y-Axis.
    /// </summary>
    private bool PosInsideTriangle(Vector3 pos, int triangleStartIndex) {
        Vector3 v1Pos = navmesh.vertices[navmesh.triangles[triangleStartIndex]] * navmeshScale;
        Vector3 v2Pos = navmesh.vertices[navmesh.triangles[triangleStartIndex + 1]] * navmeshScale;
        Vector3 v3Pos = navmesh.vertices[navmesh.triangles[triangleStartIndex + 2]] * navmeshScale;
        if(switchAxisYZ) {
            (v1Pos.y, v1Pos.z) = (v1Pos.z, v1Pos.y);
            (v2Pos.y, v2Pos.z) = (v2Pos.z, v2Pos.y);
            (v3Pos.y, v3Pos.z) = (v3Pos.z, v3Pos.y);
        }

        float alpha = ((v2Pos.z - v3Pos.z) * (pos.x - v3Pos.x) + (v3Pos.x - v2Pos.x) * (pos.z - v3Pos.z)) /
            ((v2Pos.z - v3Pos.z) * (v1Pos.x - v3Pos.x) + (v3Pos.x - v2Pos.x) * (v1Pos.z - v3Pos.z));
        float beta = ((v3Pos.z - v1Pos.z) * (pos.x - v3Pos.x) + (v1Pos.x - v3Pos.x) * (pos.z - v3Pos.z)) /
            ((v2Pos.z - v3Pos.z) * (v1Pos.x - v3Pos.x) + (v3Pos.x - v2Pos.x) * (v1Pos.z - v3Pos.z));
        float gamma = 1.0f - alpha - beta;
        return alpha > 0.0f && beta > 0.0f && gamma > 0.0f;
    }
}
