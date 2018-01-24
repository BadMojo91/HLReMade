
/* PLAYER RULES:
 * -Can only move 1 unit at a time
 * -No diagonal moves
 * -1 hit kill
 * -no gravity for player
 * -No RigidBody
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public Transform hootModel;
    public static Transform player;
    const float POS_FIX_TIMER = 2f;
    static float pFixTick =0;
    private CharacterController controller;
    private Vector3 nextPos; //position to move to
    public float nextPosOffset;
    private Vector3 nextMove; //direction to move in
    private Vector3 origin; //position before nextMove
    private bool isMoving, inPos;
    [Range(1f, 20f)]
    public float moveSpeed;
    private static bool respawn;
    private static float respawnTimer;
    public static float SetRespawnTimer {
        set { respawnTimer = value; respawn = true; }
    }
    public static void Respawn(Vector3 pos) {
        respawn = false;
        pos.z = 0;
        player.position = pos;
        pFixTick = POS_FIX_TIMER;
    }

    private void OnTileMove() {
        int x, y;
        x = Mathf.RoundToInt(origin.x);
        y = Mathf.RoundToInt(origin.y);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach(Collider c in colliders) {
            if(c.tag == "Heart") {
                Destroy(c.gameObject);
            }
        }
        if(x < LevelGrid.ld.tileData.GetLength(0) && x >= 0 && y < LevelGrid.ld.tileData.GetLength(1) && y >= 0) {
            if(LevelGrid.ld.tileData[x, y].tileType == 2) {
                LevelGrid.RemoveTile(x, y);
            }
        }
    }

    private void Awake() {
        player = this.transform;
        player.name = "Player";
        controller = GetComponent<CharacterController>();
    }
    private void Update() {

        if(respawn) {
            respawnTimer -= Time.deltaTime;
            if(respawnTimer <= 0) {
                GameController.ReloadLevel = 3;
                Destroy(player.gameObject);
            }
        }

        if(isMoving) {
            pFixTick -= Time.deltaTime;
            if(pFixTick < 0) {
                isMoving = false;
                CenterPosition();
            }
            //if(Physics.CheckSphere(nextPos, 1f, 8)) {
            //    isMoving = false;
            //}
            MoveToSpace(nextMove);
        }
        else {
            if(Input.GetButtonDown("Up")) {
                pFixTick = POS_FIX_TIMER;
                nextMove = Vector3.up;
                CenterOriginFromPosition();
                nextPos = origin + nextMove;
                // if(!Physics.CheckSphere(nextPos, 0.5f, 8))
                isMoving = CheckSpace();

            }
            if(Input.GetButtonDown("Down")) {
                pFixTick = POS_FIX_TIMER;
                nextMove = Vector3.down;
                CenterOriginFromPosition();
                nextPos = origin + nextMove;
                //  if(!Physics.CheckSphere(nextPos, 0.5f, 8))
                isMoving = CheckSpace();
            }
            if(Input.GetButtonDown("Left")) {
                pFixTick = POS_FIX_TIMER;
                nextMove = Vector3.left;
                CenterOriginFromPosition();
                nextPos = origin + nextMove;
                // if(!Physics.CheckSphere(nextPos, 0.5f, 8))
                isMoving = CheckSpace();
                hootModel.localRotation = Quaternion.Euler(0, 45, 0);

            }
            if(Input.GetButtonDown("Right")) {
                pFixTick = POS_FIX_TIMER;
                nextMove = Vector3.right;
                CenterOriginFromPosition();
                nextPos = origin + nextMove;
                //  if(!Physics.CheckSphere(nextPos, 0.5f, 8))
                isMoving = CheckSpace();
                hootModel.localRotation = Quaternion.Euler(0, -45, 0);
            }

        }
    }
    private void CenterOriginFromPosition() {
        origin.x = Mathf.Round(transform.position.x);
        origin.y = Mathf.Round(transform.position.y);
        origin.z = Mathf.Round(transform.position.z);
        
    }
    private void CenterPosition() {
        Vector3 pos;
        pos.x = Mathf.Round(transform.position.x);
        pos.y = Mathf.Round(transform.position.y);
        pos.z = Mathf.Round(transform.position.z);
        transform.position = origin;
        hootModel.localRotation = Quaternion.Euler(0, 0, 0);
    }
    private void MoveToSpace(Vector3 dir) {
       

        if(Vector3.Distance(transform.position, nextPos) < nextPosOffset || Vector3.Distance(transform.position, nextPos) > 1f) {
            isMoving = false;
            origin = nextPos;
            CenterPosition();
            
            nextMove = Vector3.zero;
            OnTileMove();
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, Time.deltaTime * moveSpeed);
            //transform.Translate(nextMove * moveSpeed * Time.deltaTime, Space.World); //FIXME
        }
    }

    private bool CheckSpace() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(nextMove), out hit, 1f)) {
            if(hit.collider.tag == "Stone" || hit.collider.tag == "Bomb" || hit.collider.tag == "Balloon") {
                if(hit.collider.GetComponent<ObjectPhysics>().MoveInDirection(Mathf.RoundToInt(nextMove.x))) {
                    CenterPosition();
                    return false;
                }
                else {
                    return true;
                }
                
            }
            if(hit.collider.tag == "Heart") {
                Destroy(hit.collider.gameObject);
                GameController.heartCounter--;
                return true;
            }
        }
        return true;
    }

    public static void DestroyPlayer(float timer) {
        player.GetComponentInChildren<MeshRenderer>().enabled = false;
        GameObject exp = Instantiate(GameController._prefabs[3]);
        exp.transform.position = player.transform.position;
        SetRespawnTimer = timer;
        
    }

    public static Vector3 GetPlayerPosition() {
        return new Vector3(player.position.x, player.position.y, 0);
    }
}
