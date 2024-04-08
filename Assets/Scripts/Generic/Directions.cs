using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a struct that takes one angle format (degree, radian, vector) and converts it to the rest.
/// </summary>
public struct AllAngle
{
    private float degree;
    private float radian;
    private Vector2 vector;
    public float Degree
    {
        get { return degree; }
        set
        {
            degree = value;
            radian = degree * Mathf.Deg2Rad;
            vector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }
    public float Radian
    {
        get { return radian; }
        set
        {
            radian = value;
            degree = radian * Mathf.Rad2Deg;
            vector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }
    public Vector2 Vector
    {
        get { return vector; }
        set
        {
            vector = value;
            radian = Mathf.Atan2(vector.y, vector.x);
            degree = radian * Mathf.Rad2Deg;
        }
    }
}
public class Directions
{
    private readonly Vector2[] cardinalVectors = new Vector2[4]
    {
        new Vector2(0,1),
        new Vector2(1,0),
        new Vector2(0,-1),
        new Vector2(-1,0)
    };
    /// <summary>
    /// A dictionary mapping cardinal and intercardinal values to a list.
    /// Mostly for choosing a random direction.
    /// Exclude 0 if you don't want (0,0).
    /// All cardinal directions are <b>even.</b>
    /// All intercardinal directions are <b>odd.</b>
    /// </summary>
    private Dictionary<int, Vector2> directionsDictionary = new Dictionary<int, Vector2>()
    {
        [0] = new Vector2(0, 0),
        [1] = new Vector2(1, -1),
        [2] = new Vector2(0, -1),
        [3] = new Vector2(1, 1),
        [4] = new Vector2(0, 1),
        [5] = new Vector2(-1, -1),
        [6] = new Vector2(-1, 0),
        [7] = new Vector2(-1, 1),
        [8] = new Vector2(1, 0),
    };
    public Dictionary<int, Vector2> DirectionDictionary { get { return directionsDictionary; } }
    public Vector2[] CardinalVectors
    {
        get { return cardinalVectors; }
    }
    public enum directions
    {
        North,
        East,
        South,
        West
    }
    public struct Cardinals
    {
        directions direction;
        Vector2 vector;
        public Vector2 Vector
        {
            get { return vector; }
            set
            {
                Directions dirClass = new Directions();
                if (!dirClass.IsCardinal(value))
                {
                    vector = Vector2.up;
                    direction = directions.North;
                    return;
                }
                vector = value;
                vector.Normalize();
                direction = dirClass.VectorToDirection(vector);
            }
        }
        public directions Direction
        {
            get { return direction; }
            set
            {
                Directions dirClass = new Directions();
                direction = value;
                vector = dirClass.DirectionsToVector2(direction);
            }
        }
    }
    /// <summary>
    /// Converts a Directions enum value to a normalized Vector2. Returns (0, 0) by default.
    /// </summary>
    /// <param name="dir">Direction to convert.</param>
    /// <returns>Returns a normalized Vector2 value.</returns>
    public Vector2 DirectionsToVector2(directions dir)
    {
        switch (dir)
        {
            case directions.North:
                return new Vector2(0, 1);
            case directions.East:
                return new Vector2(1, 0);
            case directions.South:
                return new Vector2(0, -1);
            case directions.West:
                return new Vector2(-1, 0);
            default:
                return Vector2.zero;
        }
    }
    /// <summary>
    /// Converts an integer value to a normalized Vector2. Returns (0, 0) by default.
    /// </summary>
    /// <param name="dir">Direction (integer) to convert.</param>
    /// <returns>Returns a normalized Vector2 value.</returns>
    public Vector2 IntToVector2(int dir)
    {
        switch (dir)
        {
            case (int)directions.North:
                return new Vector2(0, 1);
            case (int)directions.East:
                return new Vector2(1, 0);
            case (int)directions.South:
                return new Vector2(0, -1);
            case (int)directions.West:
                return new Vector2(-1, 0);
            default:
                return Vector2.zero;
        }
    }
    public directions OppositeOf(int dir)
    {
        return (directions)((dir + 2) % 4);
    }
    public directions OppositeOf(directions dir)
    {
        return (directions)(((int)dir + 2) % 4);
    }
    public directions IntToDirection(int dir)
    {
        return (directions)(dir % 4);
    }
    public int DirectionToInt(directions dir)
    {
        return (int)dir;
    }
    /// <summary>
    /// if the given vector is not a cardinal, returns north
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public directions VectorToDirection(Vector2 dir)
    {
        dir.Normalize();
        for (int i = 0; i < (int)directions.West + 1; i++)
        {
            if (dir.Equals(cardinalVectors[i]))
            {
                return IntToDirection(i);
            }
        }
        return directions.North;
    }
    public directions RandomDirection()
    {
        return (directions)Random.Range(0, System.Enum.GetNames(typeof(Directions.directions)).Length);
    }
    public bool IsCardinal(Vector2 vector)
    {
        vector.Normalize();
        foreach (Vector2 v in cardinalVectors)
        {
            if (vector.Equals(v))
            {
                return true;
            }
        }
        return false;
    }
}
