using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public List<Vector3Int> pos;
    public List<Vector3Int> taken;
    public Move(List<Vector3Int> pos, List<Vector3Int> taken)
	{
        this.pos = (pos == null) ? new List<Vector3Int>() : pos;
        this.taken = (taken == null) ? new List<Vector3Int>() : taken;
	}
    public Move(Move move)
	{
        this.pos = new List<Vector3Int>();
        foreach(Vector3Int v in move.pos)
		{
            this.pos.Add(v);
		}
        this.taken = new List<Vector3Int>();
        foreach(Vector3Int v in move.taken)
		{
            this.taken.Add(v);
		}
	}
    public override string ToString()
	{
        string res = "pos: {";
        foreach (Vector3Int v in pos)
        {
            res += $" {v}, ";
        }
		
        res += "}\ntaken: {";

        foreach (Vector3Int v in taken)
        {
            res += $" {v}, ";
        }

        res += "}";
        return res;
    }
}
