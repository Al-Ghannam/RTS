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
		start = grid.getNodes()[2];
		GridNode goal = new GridNode();
		goal = grid.getNodes()[120];
		Debug.Log(start.position);
		A_Star aStar = aStarCarrier.AddComponent<A_Star>();
		aStar.A_StarSetGrid(grid);
		path = aStar.FindVectorPath(start, goal);
        Debug.Log(path.Count);
	}
	
	public void OnDrawGizmos(){		
		
		//Draw nodes.
		for(int i=0; i<path.Count; i++){
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(path[i],2F);
			Gizmos.color = Color.green;
			if(i < path.Count-1)
				Gizmos.DrawLine(path[i],path[i+1]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
