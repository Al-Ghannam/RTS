using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphGeneration : MonoBehaviour {
	float gridMaxX = 100;
	float gridMaxY = 100;
	float currentX = 0;
	float currentY = 0;
	float deltaX = 1;
	float deltaY = 1;
	
	
}

public class Connection{
	Node fromNode;
	Node toNode;
	float cost;
	
	public float getCost(){
		return cost;
	}
	public Node getToNode(){
		return toNode;
	}
	public Node getFromNode(){
		return fromNode;
	}
}

public class Node{
	public List<Connection> connections{get; set;}
	public Vector3 position{get; set;}
	public bool invalid{get; set;}
	
	public void addConnection(Connection connection){
		//We create our own add connection function to
		//make sure the connection doesn't already exist.
		bool connectionExists = connections.Exists(
			delegate(Connection connectionItr){
				return (connectionItr.getFromNode() == connection.getFromNode()) &&
					(connectionItr.getToNode() == connection.getToNode()) &&
					(connectionItr.getCost() == connection.getCost());
			}
		);
		if(!connectionExists){
			connections.Add(connection);
		}
	}
}

public class Graph{
	List<Node> nodes;
	
	public void addNode(Node node){
		nodes.Add(node);
	}
	
	public void addConnection(Node node, Connection connection){
		nodes.Find(
			delegate(Node nodeItr){
				return nodeItr.position == node.position;
			}
		).addConnection(connection);
	}
	
	public List<Connection> getConnections(Node fromNode){
		return nodes.Find(
			delegate(Node node){
				return node.position == fromNode.position;
			}
		).connections;
	}
}