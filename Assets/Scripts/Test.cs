using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	
	public GameObject plane;
	public int nodeSize = 1;
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
	}
	
	void OnDrawGizmos(){
		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
