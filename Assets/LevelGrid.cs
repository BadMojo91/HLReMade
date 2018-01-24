using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class SubMesh {
    public List<int> triangles;
    public SubMesh(List<int> t) {
        triangles = t;
    }
}
public class LevelGrid {

    public static LevelData ld;
    public static List<Vector3> verts = new List<Vector3>();
    public static List<Vector2> uvs = new List<Vector2>();
    public static List<SubMesh> subMesh = new List<SubMesh>();
    public static int index;

    //Create a default level data for testing
    public static void SetDefaultLevelData(int levelNum) {
        ld = LevelData.defaultLevels[levelNum];
    }
   
    public static void UpdateGrid() {
        verts.Clear();
        uvs.Clear();
        //foreach(SubMesh s in subMesh) {
        //    s.triangles.Clear();
        //}
        subMesh.Clear();
        index = 0;

        int sizeX, sizeY;
        sizeX = ld.gridSizeX;
        sizeY = ld.gridSizeY;
        Debug.Log("Grid size set to: " + sizeX + ", " + sizeY);
        MeshFilter filter = GameController.gridObj.GetComponent<MeshFilter>();
        MeshRenderer renderer = GameController.gridObj.GetComponent<MeshRenderer>();
        MeshCollider collider = GameController.gridObj.GetComponent<MeshCollider>();
        for(int i = 0; i < renderer.materials.Length; i++) {
            subMesh.Add(new SubMesh(new List<int>()));
        }

        for(int y = 0; y < sizeY ; y++) {
            for(int x = 0; x < sizeX; x++) {
                if(LevelGrid.ld.tileData[x, y].obj) {
                    
                }
               // else if(ld.tileData[x, y].tileType == 0) {

              //  }
                else {
                    AddVerts(x, y, GameController.GetImageByString(ld.tileData[x, y].tileImage));
                }
            }
        }
        CreateMesh(filter, collider);
    }
    
    private static void AddVerts(float x, float y, int s) {
        x -= 0.5f;
        y -= 0.5f;

        verts.Add(new Vector3(x, y, 0));
        verts.Add(new Vector3(x, y+1, 0));
        verts.Add(new Vector3(x+1, y+1, 0));
        verts.Add(new Vector3(x+1, y, 0));

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        

        AddTriangle(s);
     }

    private static void AddTriangle(int s) {
        subMesh[s].triangles.Add(index);
        subMesh[s].triangles.Add(index + 1);
        subMesh[s].triangles.Add(index + 2);
        subMesh[s].triangles.Add(index);
        subMesh[s].triangles.Add(index + 2);
        subMesh[s].triangles.Add(index + 3);
        index += 4;
    }

    private static void CreateMesh(MeshFilter filter, MeshCollider collider) {
        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = verts.ToArray();
        mesh.subMeshCount = subMesh.Count;
        for(int i = 0; i < subMesh.Count; i++) {
            mesh.SetTriangles(subMesh[i].triangles.ToArray(), i);
        }
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        filter.mesh = mesh;
        collider.sharedMesh = mesh;
    }
    
    public static void RemoveTile(int x, int y) {
        ld.tileData[x, y].tileImage = "";
        ld.tileData[x, y].tileType = 0;
        
        UpdateGrid();
    }

    public static bool CheckTile(int x, int y) {
        if(x >= 0 && x < ld.gridSizeX && y >= 0 && y < ld.gridSizeY) {
            if(ld.tileData[x,y].tileImage == "") {
                return false;
            }
        }
        return true;
           
    }

    public static Vector3 GetPlayerStart() {
        return new Vector3(ld.pStartX, ld.pStartY);
    }
}
