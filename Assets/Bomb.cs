using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public float impactRange;
    public float timer;
    public float velocity;
    private Vector3 oldPos;
    private float tick;
    private float startTimer = 0.1f;
    private void Update() {
        velocity = Vector3.Distance(transform.position, oldPos);
        tick -= Time.deltaTime;
        if(tick <= 0) {
            oldPos = transform.position;
            tick = timer;
        }

        if(startTimer > 0)
            startTimer -= Time.deltaTime;
        else
            CheckForImpact();
    }

    public void CheckForImpact() {
        ObjectPhysics op = GetComponent<ObjectPhysics>();
        if(velocity > 0f && op.impact) {
            op.impact = false;
            Collider[] col = Physics.OverlapSphere(transform.position + (Vector3.down * impactRange), 0.2f);
            foreach(Collider c in col) {
                if(c.tag == "Player") {
                    
                    PlayerController.DestroyPlayer(1);
                }
                else if(c.gameObject.tag != "Levelgrid" || c.tag != "Bomb") {
                    //Destroy(c.gameObject);
                }

                if(c.tag == "Levelgrid" && op.CheckSpace(op.px, op.py-1).type != 2) {
                    Debug.Log(op.CheckSpace(op.px, op.py - 1).type);
                    LevelGrid.RemoveTile(op.px, op.py - 1);

                    if(op.CheckSpace(op.px + 1, op.py)._col) {
                        LevelGrid.RemoveTile(op.px + 1, op.py);
                    }
                    if(op.CheckSpace(op.px - 1, op.py)._col) {
                        LevelGrid.RemoveTile(op.px - 1, op.py);
                    }
                    GameObject exp = Instantiate(GameController._prefabs[3]);
                    exp.transform.position = transform.position;
                    LevelGrid.UpdateGrid();
                    Destroy(this.gameObject);
                }
               
            }


            
        }
    }

    private void RemoveTileOrObject(int x, int y) {
       
    }
}
