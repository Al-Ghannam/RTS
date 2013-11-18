using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {
	
	public GameObject plane;
	public int nodeSize = 1;
	public List<Vector3> path = new List<Vector3>();
	float width, depth;
	// Use this for initialization
	void Start () {
		Vector3 center = plane.transform.renderer.bounds.center;
		width = plane.collider.bounds.size.x/nodeSize;
		depth = plane.collider.bounds.size.z/nodeSize;
		Vector3 rotation = plane.transform.eulerAngles;
		
		//GridGraph grid = new GridGraph();
		GameObject gridCarrier = new GameObject("GridCarrier");
		GridGraph grid = gridCarrier.AddComponent<GridGraph>();
		grid.center = center;
		grid.width = (int)width;
		grid.depth = (int)depth;
		grid.rotation = rotation;
		grid.setGraphSize();
		grid.scan();
		GameObject aStarCarrier = new GameObject("AStarCarrier");
		GridNode start = new GridNode();
		start = grid.getNodes()[40];
		GridNode goal = new GridNode();
		goal = grid.getNodes()[50];
		Debug.Log(start.position);
		A_Star aStar = aStarCarrier.AddComponent<A_Star>();
		aStar.A_StarSetGrid(grid);
		path = aStar.FindVectorPath(start, goal);
	}
	
	void OnDrawGizmos(){
		//Gizmos.color = Color.white;
		//Gizmos.DrawLine(new Vector3(110,1,10), new Vector3(10,1,10));
		//Gizmos.matrix = boundsMatrix;
		
		//Gizmos.DrawWireCube(new Vector3(0,0,0), new Vector3(gridSize.x,0,gridSize.y));
		
		//Draw nodes.
		/*foreach(Vector3 pos in path){
			if(pos.y > 58) Gizmos.color = Color.red;
			else Gizmos.color = Color.green;
			Gizmos.DrawSphere(pos,2F);
		}*/
	}
	
	// Update is called once per frame
	void Update () {
	}
}
