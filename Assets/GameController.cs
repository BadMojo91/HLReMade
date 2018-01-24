using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameController : MonoBehaviour {
    public int levelNum;
    public GameObject[] prefabs;
    public static GameObject[] _prefabs;
    public const byte HEART = 6;
    public const byte BOMB = 5;
    public const byte STONE = 7;
    public static GameObject gridObj;
    public static List<Material> materials = new List<Material>();
    public LevelData levelData;
    public static int heartCounter; //Hearts left
    public static bool reloadLevel;
    public static float reloadTimer;
    public static float ReloadLevel {
        set { reloadTimer = value; reloadLevel = true; }
    }
    public void Start() {
        _prefabs = prefabs;
        StartCoroutine(GetMaterials());
        LevelData.ImportHLLevelData();
        CreateNewGameGrid();
    }
    public void Update() {
        if(reloadLevel) {
            if(reloadTimer > 0)
                reloadTimer -= Time.deltaTime;
            else {
                reloadLevel = false;
                CreateNewGameGrid();
            }
        }
    }
    private IEnumerator GetMaterials() {
        materials.Clear();
        foreach(Texture t in Resources.LoadAll("Tiles", typeof(Texture))) {
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = t;
            materials.Add(mat);
        }
        yield return new WaitForEndOfFrame();
    }
    private void CreateNewGameGrid() {
        //StartCoroutine(DestroyGridObjects());
        heartCounter = 0;
        if(gridObj != null)
            Destroy(gridObj);
        gridObj = new GameObject();
        gridObj.name = "LevelGrid";
        gridObj.tag = "Levelgrid";
        gridObj.transform.position = Vector3.zero;
        gridObj.AddComponent<MeshFilter>();
        gridObj.AddComponent<MeshRenderer>();
        gridObj.AddComponent<MeshCollider>();
        

        gridObj.GetComponent<MeshRenderer>().materials = materials.ToArray();

        MeshFilter meshFilter = gridObj.GetComponent<MeshFilter>();
        LevelGrid.SetDefaultLevelData(levelNum);

        if(!GameObject.FindGameObjectWithTag("Player"))
            Instantiate(prefabs[4]);

        PlayerController.Respawn(LevelGrid.GetPlayerStart());
        LevelGrid.UpdateGrid();
        for(int y = 0; y < LevelGrid.ld.gridSizeY; y++) {
            for(int x = 0; x < LevelGrid.ld.gridSizeX; x++) {
                if(LevelGrid.ld.tileData[x, y].obj) {
                    GameController.CreateObjectAt(LevelGrid.ld.tileData[x, y].tileType, x, y);
                    if(LevelGrid.ld.tileData[x,y].tileType == 6) {
                        heartCounter++;
                    }
                }
            }
        }

    }

    public static int GetImageByString(string image) {
        for(int i = 0; i < materials.Count; i++) {
            if(materials[i].mainTexture.name == image) {
                return i;
            }
        }
        return 0;
    }

    public static void CreateObjectAt(byte type, int x, int y) {
        GameObject o = null;
        switch(type) {
            case HEART:
                o = Instantiate(_prefabs[0]);
                o.transform.position = new Vector3(x, y, 0);
                break;
            case BOMB:
                o = Instantiate(_prefabs[1]);
                o.transform.position = new Vector3(x, y, 0);
                break;
            case STONE:
                o = Instantiate(_prefabs[2]);
                o.transform.position = new Vector3(x, y, 0);
                break;
            default:
                return;
        }
        if(o != null) {
            o.transform.parent = gridObj.transform;
        }
    }

    public static IEnumerator DestroyGridObjects() {
        if(gridObj == null || gridObj.transform.childCount <= 0) { }
        else {
            foreach(GameObject o in gridObj.transform) {
                Destroy(o.transform);
            }
        }
        yield return new WaitForEndOfFrame();
    }
}
