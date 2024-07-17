using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// Utility class that normalizes directions.
    /// </summary>
    public class Directions
    {
        public static Dictionary<int, Vector2> DirectionDictionary { get; } = new() // TODO Remove
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

        public static Vector2[] CardinalVectors { get; } = new Vector2[4]
        {
            new(0,1),
            new(1,0),
            new(0,-1),
            new(-1,0)
        };

        public enum CardinalValues
        {
            North = 0,
            East = 1,
            South = 2,
            West = 3
        }

        public struct Cardinals // TODO Remove
        {
            private CardinalValues direction;
            private Vector2 vector;

            public Vector2 Vector
            {
                readonly get { return vector; }
                set
                {
                    if (!IsCardinal(value))
                    {
                        vector = Vector2.up;
                        direction = CardinalValues.North;
                        return;
                    }
                    vector = value;
                    vector.Normalize();
                    direction = VectorToDirection(vector);
                }
            }

            public CardinalValues Direction
            {
                readonly get { return direction; }
                set
                {
                    direction = value;
                    vector = DirectionsToVector2(direction);
                }
            }
        }

        /// <summary>
        /// Converts a Directions enum value to a normalized Vector2. Returns (0, 0) by default.
        /// </summary>
        /// <param name="dir">Direction to convert.</param>
        /// <returns>Returns a normalized Vector2 value.</returns>
        public static Vector2 DirectionsToVector2(CardinalValues dir)
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
        public static Vector2 IntToVector2(int dir)
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

        public static CardinalValues OppositeOf(int dir)
        {
            return (CardinalValues)((dir + 2) % 4);
        }

        public static CardinalValues OppositeOf(CardinalValues dir)
        {
            return (CardinalValues)(((int)dir + 2) % 4);
        }

        public static CardinalValues IntToDirection(int dir)
        {
            return (CardinalValues)(dir % 4);
        }

        public static int DirectionToInt(CardinalValues dir)
        {
            return (int)dir;
        }

        /// <summary>
        /// if the given vector is not a cardinal, returns north
        /// </summary>
        public static CardinalValues VectorToDirection(Vector2 dir)
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

        public static CardinalValues RandomCardinal()
        {
            return (CardinalValues)Random.Range(0, System.Enum.GetNames(typeof(CardinalValues)).Length);
        }

        public static Vector2 RandomVector2()
        {
            float rad = Random.Range(0f, 2 * Mathf.PI);

            return new(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        public static bool IsCardinal(Vector2 vector)
        {
            vector.Normalize();

            return CardinalVectors
                .ToList()
                .Exists(x => x.Equals(vector));
        }
    }

    public static class DirectionsExtensions
    {
        /// <summary>
        /// Turns this direction left.
        /// </summary>
        public static Directions.CardinalValues TurnLeft(ref this Directions.CardinalValues dir)
        {
            return TurnLeft(ref dir, 1);
        }

        /// <summary>
        /// Turns this direction right.
        /// </summary>
        public static Directions.CardinalValues TurnRight(ref this Directions.CardinalValues dir)
        {
            return TurnRight(ref dir, 1);
        }

        /// <summary>
        /// Turns this direction left.
        /// </summary>
        /// <param name="turns">How many times to turn left.</param>
        public static Directions.CardinalValues TurnLeft(ref this Directions.CardinalValues dir, int turns)
        {
            int i = (int)dir;
            i = (i - turns) % 4;
            dir = (Directions.CardinalValues)i;

            return dir;
        }

        /// <summary>
        /// Turns this direction right.
        /// </summary>
        /// <param name="turns">How many times to turn right.</param>
        public static Directions.CardinalValues TurnRight(ref this Directions.CardinalValues dir, int turns)
        {
            int i = (int)dir;
            i = (i + turns) % 4;
            dir = (Directions.CardinalValues)i;

            return dir;
        }

        /// <summary>
        /// Return the opposite direction of this direction.
        /// </summary>
        /// <param name="apply">Apply the opposite direction to this direction?</param>
        public static Directions.CardinalValues OppositeOf(ref this Directions.CardinalValues dir, bool apply)
        {
            Directions.CardinalValues opposite = Directions.OppositeOf(dir);
            if (apply) dir = opposite;
            return opposite;
        }
    }
}