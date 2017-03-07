using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Input : MonoBehaviour
{
    private GridManager2 gridManager2;
    public LayerMask Tiles;
    private GameObject activeTile;
    public GameObject indicator;
    private GameObject go;

    void Awake()
    {
        gridManager2 = GetComponent<GridManager2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (activeTile == null)
                SelectTile();
            else
                AttemptMove();
        }
        /*
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            activeTile = null;
            Destroy(go);
        }
        */


    }

    // Tries to select a tile if the players left-clicks and no other tile is selected.
    void SelectTile()
    {
        Ray ray = GameObject.Find("Player2Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50f, Tiles);
        if (hit)
        {
            activeTile = hit.collider.gameObject;
            go = Instantiate(indicator, new Vector2(activeTile.transform.position.x, activeTile.transform.position.y), Quaternion.identity) as GameObject;
        }
    }

    //tries to select and move a tile if the player left-clicks and another tile has already been selected
    void AttemptMove()
    {
        Ray ray = GameObject.Find("Player2Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50f, Tiles);
        if (hit)
        {
            if (Vector2.Distance(activeTile.transform.position, hit.collider.gameObject.transform.position) <= 1.25f)
            {
                TileControl2 activeControl = activeTile.GetComponent<TileControl2>();
                TileControl2 hitControl = hit.collider.gameObject.GetComponent<TileControl2>();

                GridManager2.XY2 activeXY = activeControl.MyXY2;
                GridManager2.XY2 hitXY = hitControl.MyXY2;

                activeControl.Move2(hitXY);
                hitControl.Move2(activeXY);

                gridManager2.SwitchTiles(hitXY, activeXY);

                /*   
                    if (gridManager.SwitchBack())
                    {
                        activeControl = gridManager.GetTileControl(hitXY);
                        hitControl = gridManager.GetTileControl(activeXY);
                        activeXY = activeControl.MyXY;
                        hitXY = hitControl.MyXY;

                        activeControl.Move(hitXY);
                        hitControl.Move(activeXY);

                        gridManager.SwitchTiles(hitXY, activeXY);

                    }
                  */
            }
            activeTile = null;
            Destroy(go);
        }

    }
}
