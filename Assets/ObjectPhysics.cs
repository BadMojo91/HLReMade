using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class s_collider {
    public bool _col;
    public byte type;
    public Transform _collider;
}
public class ObjectPhysics : MonoBehaviour {
    public float moveSpeed=8;
    private bool isMoving;
    public bool impact;
    public int px,py;
    private Vector3 nextPos;

    public bool MoveInDirection(int x) {
        Debug.Log(x);
        if(!isMoving) {
            if(!CheckSpace(px + x, py)._col) {
                nextPos = new Vector3(px + x, py);
                isMoving = true;
                return false;
            }
        }
        return true;
    }
    public s_collider CheckSpace(int x, int y) {
        s_collider sCol = new s_collider();
        Collider[] collider = Physics.OverlapSphere(new Vector3(x, y), 0.5f);
        foreach(Collider c in collider) {
            if(c.tag == "Stone" || c.tag == "Heart" || c.tag == "Bomb" || c.tag == "Player") {
                sCol._col = true;
                sCol._collider = c.transform;
                if(c.tag != "Player")
                    sCol.type = 5;
                else
                    sCol.type = 8;

                return sCol;
            }
        }
        if(LevelGrid.CheckTile(x, y)) {
            if(x >= 0 && x < LevelGrid.ld.gridSizeX - 1 && y >= 0 && y < LevelGrid.ld.gridSizeY - 1) {
                if(LevelGrid.ld.tileData[x, y].tileType == 2)
                    sCol.type = 2;
                else if(LevelGrid.ld.tileData[x, y].tileType == 1)
                    sCol.type = 1;
            }
            sCol._col = true;
            sCol._collider = null;
            return sCol;
        }
        else {
            sCol._col = false;
            sCol._collider = null;
           
        }
        //Debug.Log("No Collision!");
        return sCol;
    }
    public void MoveUpdate() {
        int _x = px, _y = py;
        px = Mathf.RoundToInt(transform.position.x);
        py = Mathf.RoundToInt(transform.position.y);

       // if(isMoving && _y != py)
         //   isMoving = !CheckSpace(px, py - 1)._col;
       
         if(isMoving) {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, Time.deltaTime * moveSpeed);
            if(Vector3.Distance(transform.position, nextPos) < 0.01f || Vector3.Distance(transform.position, nextPos) > 1) {
                impact = true;
                transform.position = nextPos;
                isMoving = false;
                
            }
        }

        if(!isMoving) {
           // yield return new WaitForFixedUpdate();
            if(py - 1 >= 0 && !CheckSpace(px, py - 1)._col) {
                nextPos = new Vector3(px, py - 1);
                isMoving = !CheckSpace(px, py - 1)._col;
            }
            else if(px - 1 >= 0 && !CheckSpace(px - 1, py)._col && !CheckSpace(px-1,py-1)._col && CheckSpace(px, py - 1).type != 2 && CheckSpace(px, py - 1).type != 8) {
                    nextPos = new Vector3(px - 1, py);
                    isMoving = !CheckSpace(px - 1, py)._col;
            }
            else if(px + 1 < LevelGrid.ld.gridSizeX && !CheckSpace(px + 1, py)._col && !CheckSpace(px + 1, py - 1)._col && CheckSpace(px, py - 1).type != 2 && CheckSpace(px, py - 1).type != 8) {
                nextPos = new Vector3(px + 1, py);
                isMoving = !CheckSpace(px + 1, py)._col;
            }
            
        }
    }

    private void Update() {

     
            MoveUpdate();
           
    }
}
