using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TileControl2 : NetworkBehaviour {
	
	public GridManager2 GridManager2;
    public GridManager2.XY2 MyXY2;
    public int distanceFromOtherBoard;

	//[Command]
    public void Move2(GridManager2.XY2 xy)
    {
        StartCoroutine(Moving2(xy));
    }

    //a co-routine which moves the tile into its destination
    IEnumerator Moving2(GridManager2.XY2 xy)
    {
        GridManager2.ReportTileMovement(); //Report to the GridManager that this tile is moving so it doesn't check for matches

        Vector2 destination = new Vector2(xy.X + distanceFromOtherBoard, xy.Y);
        bool moving = true;

        while (moving)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, 5f * Time.deltaTime);

            if (Vector2.Distance(transform.position, destination) <= 0.1f)
            {
                transform.position = destination;
                moving = false;
            }
            yield return null;
        }

        MyXY2 = xy;
        gameObject.name = xy.X + "/" + xy.Y; //Not necessary, just helps with the overview in the Hierarchy
        GridManager2.ReportTileStopped(); //report to the GridManager that this tile is done moving.
    }
}
