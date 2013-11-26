using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {
	
	public GameObject plane;
	public int nodeSize = 1;
	public List<Vector3> path = new List<Vector3>();
	float width, depth;
	public Vector3 startPosition = new Vector3();
	public Vector3 endPosition = new Vector3();
	public GridGraph grid;
	
	// Use this for initialization
	void Start () {
        //Vector3 center = plane.transform.renderer.bounds.center;
        //width = plane.collider.bounds.size.x/nodeSize;
        //depth = plane.collider.bounds.size.z/nodeSize;
        //Vector3 rotation = plane.transform.eulerAngles;

		GameObject gridCarrier = new GameObject("GridCarrier");
		grid = gridCarrier.AddComponent<GridGraph>();
        
        //grid.center = center;
        //grid.width = (int)width;
        //grid.depth = (int)depth;
        //grid.rotation = rotation;
        //grid.setGraphSize();
        //grid.scan();
        //grid.saveGrid();

        grid.loadGrid("Assets/SavedGrids/grid.xml");
	}
	
	public void Update()
	{
		if(Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
		{
			startPosition = shootRay(Input.mousePosition);
            //Debug.Log("s: " + startPosition);

			startPosition = grid.getNearest(startPosition).position;
		}
		else if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
		{
			endPosition = shootRay(Input.mousePosition);
			Debug.Log("e: " + endPosition);
		}
		if(Input.GetMouseButtonDown(1))
		{
			//Debug.Log("s: " + startPosition);
			//Debug.Log("e: " + endPosition);
			path = new List<Vector3>();
			GridNode startNode = new GridNode();
			startNode = grid.getNearest(startPosition);
			startPosition = startNode.position;
			GridNode endNode = new GridNode();
			endNode = grid.getNearest(endPosition);
			endPosition = endNode.position;
			GameObject aStarCarrier = new GameObject("AStarCarrier");
			A_Star aStar = aStarCarrier.AddComponent<A_Star>();
			aStar.A_StarSetGrid(grid);
			path = aStar.FindVectorPath(startNode,endNode);
		}
	}
	
	private Vector3 shootRay(Vector3 pos)
	{
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit,Mathf.Infinity)){
			return hit.point;
		}
		else
			return Vector3.zero;
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
		
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(startPosition, 4F);
		Gizmos.DrawSphere(endPosition, 4F);
	}
}
