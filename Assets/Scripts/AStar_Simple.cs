using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class A_Star : MonoBehaviour
{
    

    private bool EndSearch = false;//search have finished

    private GridNode CurrentGridNode;//is the curretn node that i am working on each iteration
    private GridNode GoalGridNode;
	private GridGraph grid;
    //constants
    private const int current_open = 0;
    private const int current_closed = 1;
    private const int current_neither = 2;//neither
    private List<NodeStruct> Openlist= new List<NodeStruct>();//currently a normal list but will be soon updated to priority queue
    private Dictionary<Vector3, NodeStruct> lookup_node=new Dictionary<Vector3,NodeStruct>();
    struct NodeStruct
    {
        private int current_list;
        private Vector3 previous_position;//holding previous point for backtracking to generate the path
        private float gcost;
        private float hcost;
        private float fcost;
        private GridNode current_node;
        public void setnode(GridNode node)
        {
            current_node = node;
        }
        public void set_prev_position(Vector3 position)
        {
            previous_position = position;
        }
        public Vector3 get_prev_position()
        {
            return previous_position;
        }
        public GridNode getnode()
        {
            return current_node;
        }
        public void set_list(int list)
        {
            current_list = list;
        }
        public int get_list() {
            return current_list;
        }
        public float getfcost()
        {
            return fcost;
        }
        public float get_gcost()
        {
            return gcost;
        }
        public void setgcost(float prev_cost)
        {
            gcost = prev_cost + 1;
            fcost = hcost + gcost;
        }
        public void sethcost(float cost)
        {
            hcost = cost;
            fcost = hcost + gcost;
        }
        public Vector3 get_position()
        {
            return current_node.position;
        }

    }
    //aray of struct grid node  holding grid nodes and all its associated data
    private NodeStruct nodeStruct;
	private GridNode start_node;
    //Constructor
    public A_Star()
    {
		
    }

    public void A_StarSetGrid(GridGraph gridGraph){
        grid = gridGraph;
        nodeStruct = new NodeStruct();
        nodeStruct.set_list(current_neither);
        nodeStruct.setgcost(-1);
        nodeStruct.sethcost(0);
    }

    //Methods
    public List<Vector3> FindVectorPath(GridNode currentGridNode, GridNode targetGridNode)
    {
		start_node=currentGridNode;
        Vector3 origPosition = currentGridNode.position;
        CurrentGridNode = currentGridNode;
		nodeStruct.setgcost(0);
		nodeStruct.sethcost(0);
		nodeStruct.setnode(CurrentGridNode);
		lookup_node.Add(CurrentGridNode.position,nodeStruct);
        GoalGridNode = targetGridNode;
         //Check it goal GridNode and current GridNode match
        if (CurrentGridNode == GoalGridNode)
        {
            return new List<Vector3>
			{
				origPosition,
			};
        }

        EndSearch = false;
        lookup_node[CurrentGridNode.position].set_list(current_open);
        Openlist.Clear();
        Openlist.Add(lookup_node[CurrentGridNode.position]);
        lookup_node[CurrentGridNode.position].setgcost(0);
       do
      {
			FindCheapestGridNode();//updates the currentgridnode with the cheapest node

            Debug.Log("node " + CurrentGridNode.position);
            //add current node to closed list
            lookup_node[CurrentGridNode.position].set_list(current_closed);
            if (CurrentGridNode.position == GoalGridNode.position)
                return FindRoute();
            List<NodeStruct> adjacent_nodes=adjacents(lookup_node[CurrentGridNode.position]);
            foreach (NodeStruct current_adj in adjacent_nodes)
            {
                if (current_adj.get_list() != current_open) //node is not in openlist
                {
                    current_adj.set_prev_position(CurrentGridNode.position);// set current node as previous for this node
                    current_adj.sethcost(Heuristic_Euclidean(current_adj.get_position().x, current_adj.get_position().z));// set h costs of this node (estimated costs to goal)
                    //current_adj.setgcost(lookup_node[CurrentGridNode.position].get_gcost()); // set g costs of this node (costs from start to this node)
                    current_adj.set_list(current_open);
                    Openlist.Add(current_adj);// add node to openList
                    lookup_node[current_adj.get_position()] = current_adj;
                    Debug.Log("adjacent " + current_adj.get_position()+" option 1");
                }
                else if (current_adj.get_list() == current_open)
                {
                    // node is in openList
                    if (current_adj.get_gcost() > lookup_node[CurrentGridNode.position].get_gcost())
                    { // costs from current node are cheaper than previous costs
                        current_adj.set_prev_position(CurrentGridNode.position); // set current node as previous for this node
                        current_adj.setgcost(lookup_node[CurrentGridNode.position].get_gcost());// set g costs of this node (costs from start to this node)
                        lookup_node[current_adj.get_position()] = current_adj;
                        Debug.Log("adjacent " + current_adj.get_position());
                    }
                }
            }
			if(Openlist.Count==0 && !EndSearch)
			{
				return new List<Vector3>
			{
				origPosition,
			};
			}
        } while (EndSearch == false);
        

        return FindRoute();
    }

    private List<NodeStruct> adjacents(NodeStruct node1)
    {
        GridNode node = node1.getnode();
    	//return the adjacents of a node
        List<NodeStruct> temp=new List<NodeStruct>();
        int[] neighbourOffsets;
		int width = grid.width;
        neighbourOffsets = new int[8]{
            -width,1,width,-1,
            -width+1,width+1,width-1,-width-1
        };
		int count = 0;
        for(int i=0; i<8; i++){
			if (node.getConnection(i))
			{
				count++;
				GridNode tempNode = grid.getNodes()[node.getIndex() + neighbourOffsets[i]];
				try{
				nodeStruct=lookup_node[tempNode.position];
				}
				catch(Exception ee){
					nodeStruct = new NodeStruct();
					nodeStruct.set_list(current_neither);
					
					nodeStruct.setgcost(node1.get_gcost());
					nodeStruct.setnode(tempNode);
					lookup_node.Add(tempNode.position,nodeStruct);
				}
            if (nodeStruct.get_list()!=current_closed)
			{
					nodeStruct = lookup_node[grid.getNodes()[node.getIndex() + neighbourOffsets[i]].position];
                temp.Add(nodeStruct);
			}
			}
        }
		Debug.LogWarning(count);
		if(temp.Count==0)
			Debug.LogWarning(node1.get_position());
        return temp;
    }
    private float Heuristic_Diagonal(float x, float y)
    {
        float dx = Mathf.Abs(GoalGridNode.position.x- x);
        float dy = Mathf.Abs(GoalGridNode.position.y - y);

        float min_d = Mathf.Min(dx, dy);

        return ((min_d) + (((2 * min_d))));
    }
    private float Heuristic_Euclidean(float x, float y)
    {
        float dx = Mathf.Abs(GoalGridNode.position.x - x);
        float dy = Mathf.Abs(GoalGridNode.position.y - y);

        return Mathf.Sqrt((dx * dx) + (dy * dy));
    }

    private float Heuristic_Manhatten(float x, float y)
    {
        float dx = Mathf.Abs(GoalGridNode.position.x - x);
        float dy = Mathf.Abs(GoalGridNode.position.y - y);

        return ((dx + dy));
    }

    private void FindCheapestGridNode()
    {
        float lowcost = 10000.0f;

        foreach (NodeStruct t in Openlist)
        {
            if (t.getfcost() < lowcost)
            {
                lowcost = t.getfcost();
                CurrentGridNode = t.getnode();
            }
        }

        // have we reached the destination ?. If yes then generate the route
        if (CurrentGridNode == GoalGridNode)
        {
            EndSearch = true;
        }

        Openlist.Remove(lookup_node[CurrentGridNode.position]);
      NodeStruct node= lookup_node[CurrentGridNode.position];
        node.set_list(current_closed);
        lookup_node[CurrentGridNode.position] = node;
		Debug.LogWarning("chosen node  index: "+node.getnode().getIndex());
    }



    private List<Vector3> FindRoute()
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(CurrentGridNode.position);

        do
        {
            path.Add(CurrentGridNode.position);
            CurrentGridNode =lookup_node[lookup_node[CurrentGridNode.position].get_prev_position()].getnode();

        } while (CurrentGridNode.position!=start_node.position);
        return path;
    }
	
}

