using UnityEngine;
using System;
using System.Collections.Generic;

public class GridNode{
    /* This is a summation of how this class is used, and why:
    Any node in the grid can only connect to any of the eight nodes surrounding it,
    which I call the neighbouring nodes. Four on each side, and four diagonally.
    Therefore, we can represent those connections by boolean values only.
    I chose to represent them with the first 8 bits of the integer called Flags, as Aron
    Granberg did ingeniously.
    Each node can be walkable or unwalkable. Another boolean value. The Eighth bit in Flags.
    Each node has two indecies. One is the index of the node in its graph, and the other,
    is the index of the graph itself since there are multiple graphs. They are both stored
    in the indicies variable. Also inspired by Aron Granberg's implementation.
    Most of this class is very similar to Aron's, because his is brilliant, and it's all
    I can think of now.
    */

    public int flags;
    //The first 24 bits, of indices, carry the index of this node in the graph specified by the last 8 bits.
    public int indices;
    public Vector3 position;
    public int curent_list=2;
    public float cost=0;
    public float h_cost=0;
    public float g_cost=0;
    public GridNode parent_node = null;
    public bool walkable{
        get{
            return ((flags >> 8) & 1) != 0;
        }
        set{
            flags = (flags & ~0x100) | (value ? 0x100 : 0);
        }
    }
    
    public bool getConnection(int neighbourBit){
        return ((flags >> neighbourBit) & 1) == 1;
    }
    //Set (or remove) a connection between two nodes, by modifying the corresponding flags bit.
    public void setConnection(int neighbourBit, int newValue){
        flags &= ~(1<<neighbourBit);
        flags |= newValue << neighbourBit;
    }

    public int getGridIndex(){
        return (indices >> 24);
    }

    public int getIndex(){
        return (indices & 0xFFFFFF);
    }

    public void setGridIndex(int gridIndex){
        indices &= 0xFFFFFF;
        indices |= gridIndex << 24;
    }

    public void setIndex(int index){
        indices &= ~0xFFFFFF;
        indices |= index;
    }
}