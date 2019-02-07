using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell {

    public bool transitable; //Can the player pass through him
    public bool buildable; //Can be builded
    public bool destroyable; //Can be destroyed
    public bool trap; //Is a trap
    public bool isEmpty; //space OR WATER
    public bool winPoint; //arrived to winpoint
    public GameObject gameObject;
    public enum types {cube, ramp, player, empty, trap};
    public types actualType;

    public Vector3 gridPosition;
    public Vector3 globalPosition;

	public Cell (bool _transitable, bool _buildable, bool _trap, bool _destroyable, bool _winPoint, bool _isEmpty, GameObject _gameObject, types _type, Vector3 _gridPos, Vector3 _globalPos)
    {
        transitable = _transitable;
        buildable = _buildable;
        gameObject = _gameObject;
        actualType = _type;
        gridPosition = _gridPos;
        globalPosition = _globalPos;
        destroyable = _destroyable;
        trap = _trap;
        winPoint = _winPoint;
        isEmpty = _isEmpty;
        
    }


}