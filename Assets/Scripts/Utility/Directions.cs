using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Utility
{
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
            readonly get { return degree; }
            set
            {
                degree = value;
                radian = degree * Mathf.Deg2Rad;
                vector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            }
        }
        public float Radian
        {
            readonly get { return radian; }
            set
            {
                radian = value;
                degree = radian * Mathf.Rad2Deg;
                vector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            }
        }
        public Vector2 Vector
        {
            readonly get { return vector; }
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
        static Directions() { }
        private Directions() { }
        public static Directions Instance { get; } = new Directions();
        public Dictionary<int, Vector2> DirectionDictionary { get; } = new()
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
        public Vector2[] CardinalVectors { get; } = new Vector2[4]
        {
            new(0,1),
            new(1,0),
            new(0,-1),
            new(-1,0)
        };
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
                readonly get { return vector; }
                set
                {
                    if (!Instance.IsCardinal(value))
                    {
                        vector = Vector2.up;
                        direction = directions.North;
                        return;
                    }
                    vector = value;
                    vector.Normalize();
                    direction = Instance.VectorToDirection(vector);
                }
            }
            public directions Direction
            {
                readonly get { return direction; }
                set
                {
                    direction = value;
                    vector = Instance.DirectionsToVector2(direction);
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
            return dir switch
            {
                directions.North => new Vector2(0, 1),
                directions.East => new Vector2(1, 0),
                directions.South => new Vector2(0, -1),
                directions.West => new Vector2(-1, 0),
                _ => Vector2.zero,
            };
        }
        /// <summary>
        /// Converts an integer value to a normalized Vector2. Returns (0, 0) by default.
        /// </summary>
        /// <param name="dir">Direction (integer) to convert.</param>
        /// <returns>Returns a normalized Vector2 value.</returns>
        public Vector2 IntToVector2(int dir)
        {
            return dir switch
            {
                (int)directions.North => new Vector2(0, 1),
                (int)directions.East => new Vector2(1, 0),
                (int)directions.South => new Vector2(0, -1),
                (int)directions.West => new Vector2(-1, 0),
                _ => Vector2.zero,
            };
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
        public directions VectorToDirection(Vector2 dir)
        {
            dir.Normalize();
            for (int i = 0; i < (int)directions.West + 1; i++)
            {
                if (dir.Equals(CardinalVectors[i]))
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
            foreach (Vector2 v in CardinalVectors)
            {
                if (vector.Equals(v))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
