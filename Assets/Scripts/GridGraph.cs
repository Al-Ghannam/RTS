using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGraph : MonoBehaviour {
	
	public int width{get; set;} //Width of the grid(in nodes).
	public int depth{get; set;} //Depth of the grid (in nodes).
	public Vector3 center{get; set;} //Center of graph.
	public Vector3 rotation{get; set;} //Rotation of graph.
	public float nodeSize = 30F;
	
	//Which axis to measure the climbing on. X=0, Y=1, Z=2
	public int maxClimbAxis = 1;
	//The maximum difference between two nodes to place a connection.
	public float maxClimb = 0.4f;
	//Maximum slope for nodes to be walkable.
	public float maxSlope = 90;
	//Check for height of node to determine walkability.
	public bool heightCheck = true;
	//LayerMask for height checking.
	public LayerMask heightMask = -1;
	
	public float numOfNeighbours = 8;
	public int[] neighbourOffsets;
	public int[] neighbourCosts;
	public int[] neighbourXOffsets;
	public int[] neighbourZOffsets;
	
	GridNode[] nodes;
	public Vector2 gridSize;
	public Vector2 unclampedSize;
	public Matrix4x4 boundsMatrix; //Matrix mainly for visualizing the grid.
	public Matrix4x4 matrix; // Matrix for translating, rotating, and scaling the graph.
	
	void Start(){
		//gridSize = new Vector2(10,10);
		//nodeSize = 50F;
	}
	
	/* Sets Grid size based on Width and Depth,
	 * and update the matrix
	 */
	public void setGraphSize(){
		gridSize = new Vector2(width,depth)*nodeSize;
		generateMatrix();
	}
	
	/* Shame to say this is entirely copied from Aron Granberg's.
	 * This generates the matrix by which I know where each node
	 * on the grid actually is in world coordinates.
	 */
	public void generateMatrix(){
		//Reverse sign if negative
		gridSize.x *= Mathf.Sign(gridSize.x);
		gridSize.y *= Mathf.Sign(gridSize.y);
		
		gridSize.x = gridSize.x < nodeSize? nodeSize : gridSize.x;
		gridSize.y = gridSize.y < nodeSize? nodeSize : gridSize.y;
		
		width = Mathf.FloorToInt (gridSize.x / nodeSize);
		depth = Mathf.FloorToInt (gridSize.y / nodeSize);
		if (Mathf.Approximately (gridSize.x / nodeSize,Mathf.CeilToInt (gridSize.x / nodeSize))) {
			width = Mathf.CeilToInt (gridSize.x / nodeSize);
		}
		
		if (Mathf.Approximately (gridSize.y / nodeSize,Mathf.CeilToInt (gridSize.y / nodeSize))) {
			depth = Mathf.CeilToInt (gridSize.y / nodeSize);
		}
		
		boundsMatrix.SetTRS(center,Quaternion.Euler(rotation), new Vector3(1,1,1));
		matrix.SetTRS(boundsMatrix.MultiplyPoint3x4 (-new Vector3 (gridSize.x,0,gridSize.y)*0.5F),
			Quaternion.Euler(rotation),new Vector3(nodeSize,1,nodeSize));
	}
	
	public void createNodes(int n){
		GridNode[] tmp = new GridNode[n];
		for(int i=0; i<n; i++){
			tmp[i] = new GridNode();	
		}
		nodes = tmp;
	}
	
	/** Visualize a node surrounded by 8 other nodes. Four cross, one on each side,
	 * and four diagonally. Neighbours are ranked as follows: First the cross nodes,
	 * starting from the top and going clockwise. So the node below is the third.
	 * Then the diagonal nodes, starting from the top right corner and going clockwise.
	 * So the top left corner node is the last(eighth).*/
	public void setNeighbourOffsets(){
		neighbourXOffsets = new int[8]{
			0,1,0,-1,
			1,1,-1,-1
		};
		neighbourZOffsets = new int[8]{
			-1,0,1,0,
			-1,1,1,-1
		};
		neighbourOffsets = new int[8]{
			-width,1,width,-1,
			-width+1,width+1,width-1,-width-1
		};
	}
	
	public void setNeighbourCosts(){
		//The diagonal cost is experimental. Needs tweaking.
		int crossCost = Mathf.RoundToInt(nodeSize);
		int diagonalCost = Mathf.RoundToInt(nodeSize*Mathf.Sqrt(2.0F));
		neighbourCosts = new int[8]{
			crossCost,crossCost,crossCost,crossCost,
			diagonalCost, diagonalCost, diagonalCost, diagonalCost
		};
	}
	
	public void scan(){
		if(nodeSize<=0)
			return;
		generateMatrix();
		createNodes(width*depth);
		setNeighbourOffsets();
		setNeighbourCosts();
		
		for(int z=0; z<depth; z++){
			for(int x=0; x<width; x++){
				GridNode node = nodes[z*width+x];
				node.setIndex(z*width+x);
				//Debug.Log("index: " + (z*width+x) + 
				//	" node.getIndex: " + node.getIndex());
				SetNodeWalkability(node, x, z);
			}
		}
		
		for(int x=0; x<width; x++){
			for(int z=0; z<depth; z++){
				GridNode node = nodes[z*width+x];
				calculateConnections(nodes, x, z, node);
			}
		}
		Debug.Log("Graph Created! LoLoLoLoooy!!");
	}
	/* This function does the following:
	 * a) In case of height checking, which I guess will always be the
	 * case with this game, since the levels are terrains and we can't
	 * set different layers, we check first if the node is within the
	 * appropriate height limits. We do that by raycasting from 100 points
	 * above the node. 100 is set arbitrarily. I should substitute that for
	 * a variable, but later. If the raycast hits, then the node is of appropriate
	 * height; else we set walkable to false.
	 * b) Next is slope checking, which is obvious from the code.
	 * c) Finally, we need to check if the node is obstructed by something
	 * above it.
	 */
	public void SetNodeWalkability(GridNode node, int x, int z){
		node.position = matrix.MultiplyPoint3x4(new Vector3(x+0.5F,0,z+0.5F));
		RaycastHit hit;
		bool walkable = true;
		
		if(heightCheck){
			if(Physics.Raycast(node.position+Vector3.up*100,
				-Vector3.up,out hit,100.0F+.005F,heightMask)){
				node.position = hit.point;
				if(hit.point.y > 33 && hit.point.y < 40){
					Debug.Log(hit.point);
					Debug.Log(hit.collider);
				}
			}
			else{
				walkable = false;
			}
		}
		//We need to check for the slope.
		if(walkable && heightCheck){
			Physics.Raycast(node.position+Vector3.up*100,
				-Vector3.up,out hit,100.0F+.005F,heightMask);
			if(hit.normal != Vector3.zero){ // If it's not a flat surface:
				float angle = Vector3.Dot(hit.normal.normalized, Vector3.up);
				float cosAngleMaxSlope = Mathf.Cos(maxSlope*Mathf.Deg2Rad);
				if(angle < cosAngleMaxSlope)
					walkable = false;
			}
		}
		//If it's still walkable, check for an obstruction.
		if(walkable){
			//To be done.
		}
		node.walkable = walkable;
	}
	
	/* An enjoyably self-explanatory function :)*/
	public void calculateConnections(GridNode[] nodes, int x, int z,
		GridNode node){
		
		if(!node.walkable){
			//Set the first 8 bits (connection bits) to 0 and return.
			node.flags = node.flags & ~256;
			return;
		}
		
		int index = node.getIndex();
		for(int i=0; i<8; i++){
			int neighbourX = x + neighbourXOffsets[i];
			int neighbourZ = z + neighbourZOffsets[i];
				
			//Check the bounds.
			if(neighbourX >= width || neighbourZ >= depth ||
				neighbourX < 0 || neighbourZ < 0)
				continue;

			GridNode neighbour = nodes[index + neighbourOffsets[i]];
			float climb = Mathf.Abs(node.position[maxClimbAxis] -
				neighbour.position[maxClimbAxis]);
			if(!neighbour.walkable || climb > maxClimb)
				continue;
			
			node.setConnection(i,1);
		}
	}
	
	
	public void OnDrawGizmos(){		
		Gizmos.color = Color.white;
		//Gizmos.DrawLine(new Vector3(110,1,10), new Vector3(10,1,10));
		//Gizmos.matrix = boundsMatrix;
		
		Gizmos.DrawWireCube(new Vector3(0,0,0), new Vector3(gridSize.x,0,gridSize.y));
		
		//Draw nodes.
		for(int i=0; i<nodes.Length; i++){
			if(nodes[i].position.y > 58) Gizmos.color = Color.red;
			else Gizmos.color = Color.green;
			if(nodes[i].walkable)
				Gizmos.DrawSphere(nodes[i].position,2F);
			for(int j=0; j<8; j++){
				Gizmos.color = Color.white;
				if(nodes[i].getConnection(j))
					Gizmos.DrawLine(nodes[i].position,
						nodes[(nodes[i].getIndex()+neighbourOffsets[j])].position);
			}
		}
		
		//Draw connections.
		
		//Gizmos.matrix = Matrix4x4.identity;
	}
	
	public void onDestroy(){
		nodes = null;	
	}
	
}
