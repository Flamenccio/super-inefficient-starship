using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamenccio.Utility
{
    /// <summary>
    /// A struct that takes one angle format (i.e. degree, radian, vector) and converts it to the rest.
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
        public enum CardinalValues
        {
            North,
            East,
            South,
            West
        }
        public struct Cardinals
        {
            CardinalValues direction;
            Vector2 vector;
            public Vector2 Vector
            {
                readonly get { return vector; }
                set
                {
                    if (!Instance.IsCardinal(value))
                    {
                        vector = Vector2.up;
                        direction = CardinalValues.North;
                        return;
                    }
                    vector = value;
                    vector.Normalize();
                    direction = Instance.VectorToDirection(vector);
                }
            }
            public CardinalValues Direction
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
        public Vector2 DirectionsToVector2(CardinalValues dir)
        {
            return dir switch
            {
                CardinalValues.North => new Vector2(0, 1),
                CardinalValues.East => new Vector2(1, 0),
                CardinalValues.South => new Vector2(0, -1),
                CardinalValues.West => new Vector2(-1, 0),
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
                (int)CardinalValues.North => new Vector2(0, 1),
                (int)CardinalValues.East => new Vector2(1, 0),
                (int)CardinalValues.South => new Vector2(0, -1),
                (int)CardinalValues.West => new Vector2(-1, 0),
                _ => Vector2.zero,
            };
        }
        public CardinalValues OppositeOf(int dir)
        {
            return (CardinalValues)((dir + 2) % 4);
        }
        public CardinalValues OppositeOf(CardinalValues dir)
        {
            return (CardinalValues)(((int)dir + 2) % 4);
        }
        public CardinalValues IntToDirection(int dir)
        {
            return (CardinalValues)(dir % 4);
        }
        public int DirectionToInt(CardinalValues dir)
        {
            return (int)dir;
        }
        /// <summary>
        /// if the given vector is not a cardinal, returns north
        /// </summary>
        public CardinalValues VectorToDirection(Vector2 dir)
        {
            dir.Normalize();
            for (int i = 0; i < (int)CardinalValues.West + 1; i++)
            {
                if (dir.Equals(CardinalVectors[i]))
                {
                    return IntToDirection(i);
                }
            }
            return CardinalValues.North;
        }
        public CardinalValues RandomDirection()
        {
            return (CardinalValues)Random.Range(0, System.Enum.GetNames(typeof(Directions.CardinalValues)).Length);
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
