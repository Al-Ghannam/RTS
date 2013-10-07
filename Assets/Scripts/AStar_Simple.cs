using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar_Simple{
	
	public struct NodeRecord{
		public Node node;
		public Connection connection;
		public float costSoFar;
		public float heuristic;
		public float estimatedTotalCost;
	}

	//To get whether a node is invalid or not(i.e blocked by a unit, building, etc.)
	public static bool invalid(Node node){
		if(node.invalid || node == null)
			return true;
		return false;
	}
	
	//It gets the distance between two nodes.(Duh!)
	public static float Distance(Node firstNode, Node secondNode){
		if(invalid (firstNode) || invalid(secondNode))
			return float.MaxValue;
		return Vector3.Distance(firstNode.position, secondNode.position);
	}
	
	//A naive heuristic for now(Underestimating).
	static float heuristic(Node node, Node goal){
		return Distance(node,goal);
	}
	
	//Get the node with the least cost so far.
	static NodeRecord smallestElement(List<Node> openNodes, Dictionary<Node,NodeRecord> openNodesRecords){
		float leastCost = float.MaxValue;
		int index = 0;
		for(int i=0; i<openNodesRecords.Count; i++){
			if(openNodesRecords[openNodes[i]].estimatedTotalCost < leastCost){
				leastCost = openNodesRecords[openNodes[i]].estimatedTotalCost;
				index = i;
			}
		}
		return openNodesRecords[openNodes[index]];
	}
	
	static List<Node> CalculatePath(Graph graph, Node start, Node goal){
		List<Node> openNodes = new List<Node>();
		List<Node> closedNodes = new List<Node>();
		Dictionary<Node,NodeRecord> openNodeRecords= new Dictionary<Node, NodeRecord>();
		Dictionary<Node,NodeRecord> closedNodeRecords = new Dictionary<Node, NodeRecord>();
		//Add the start node's nodeRecord.
		NodeRecord startRecord = new NodeRecord();
		startRecord.node = start;
		startRecord.costSoFar = 0;
		startRecord.heuristic = heuristic(start,goal);
		startRecord.estimatedTotalCost = startRecord.heuristic;
		startRecord.connection = null;
		
		openNodeRecords[start] = startRecord;
		openNodes.Add(start);
		
		NodeRecord current = startRecord;
		//As long as there are open nodes:
		while(openNodes.Count > 0){
			//Get the smallest node's nodeRecord.
			current = smallestElement(openNodes, openNodeRecords);
			//If we reached the goal break.
			if(current.node == goal) break;
			
			//Otherwise, get the connections from that node.
			List<Connection> connections = graph.getConnections(current.node);
			
			foreach(Connection connection in connections){
				Node endNode = connection.getToNode();
				float endNodeCost = current.costSoFar + connection.getCost();
				NodeRecord endNodeRecord;
				float endNodeHeuristic;
				//Check if we already passed this node before(i.e. if it's considered closed).
				if(closedNodeRecords.ContainsKey(endNode)){
					endNodeRecord = closedNodeRecords[endNode];
					//If we didn't find a better path, skip this iteration.
					if(endNodeRecord.costSoFar <= endNodeCost)
						continue;
					//If we did, remove the node from the closed list.
					closedNodeRecords.Remove(endNode);
					closedNodes.Remove(endNode);
					endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
				}
				//Else, if the node is in the open list:
				else if(openNodeRecords.ContainsKey(endNode)){
					endNodeRecord = openNodeRecords[endNode];
					//If we didn't find a better path, skip this iteration.
					if(endNodeRecord.costSoFar <= endNodeCost)
						continue;
					endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
				}
				else{
					endNodeRecord = new NodeRecord();
					endNodeRecord.node = endNode;
					endNodeHeuristic = heuristic(endNode,goal);
				}
				//We reach here, if we want to update the node.
				//Update the cost, estimate and connection values.
				endNodeRecord.costSoFar = endNodeCost;
				endNodeRecord.connection = connection;
				endNodeRecord.heuristic = endNodeHeuristic;
				endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;
				//Then add it to the open list:
				openNodeRecords[endNode] = endNodeRecord;
				if(!openNodes.Contains(endNode))
					openNodes.Add(endNode);
				
			}
			//We finished following all connections in the 'current' node
			//Add it to the closed list, and remove it from the open one
			openNodes.Remove(current.node);
			openNodeRecords.Remove(current.node);
			closedNodes.Add(current.node);
			closedNodeRecords[current.node] = current;
		}
		//We reach here if there are no more nodes, or we have reached our goal.
		//Find out which:
		if(current.node != goal)
			return null;
		//Else, get the path(will be reversed):
		List<Node> path = new List<Node>();
		while(current.node != start){
			path.Add(current.node);
			current = closedNodeRecords[current.connection.getFromNode()];
		}
		//Reverse the path:
		path.Reverse();
		return path;
	}
	
}

