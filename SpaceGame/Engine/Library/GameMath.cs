using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
namespace Isotope
{
    public sealed class GameMath
    {
        //Normalizes the Vector2
        public static Vector2 NormalizeVector2(Vector2 vector)
        {
            float x = Convert.ToSingle(Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y)));
            if ((x == 0.0f))
            {
                return new Vector2(0, 0);
            }
            else
            {
                float val = 1.0f / x;
                return new Vector2(vector.X * val, vector.Y * val);
            }
        }

        //Single Precision Clamp
        public static float ClampFloat(float value, float min, float max)
        {
            //Create a brand new decimal/integer to avoid cross memory referencing
            float i = new float();
            //Assign the decimal/integer
            i = value;
            //Clamp the deimal/integer based on min and max variables
            if ((i > max))
            {
                i = max;
            }
            if ((i < min))
            {
                i = min;
            }
            //Return the new integer ready for direct use in code
            return i;
        }

        //Single precision clamp function for code optimisation
        public static Vector2 ClampVector(Vector2 value, Vector2 min, Vector2 max)
        {
            return new Vector2(ClampFloat(value.X, min.X, max.X), ClampFloat(value.Y, min.Y, max.Y));
        }

        //Single precision clamp function for code optimisation taking some single values instead of Vector2's
        public static Vector2 ClampVectorSingle(Vector2 value, float minX, float minY, float maxX, float maxY)
        {
            return new Vector2(ClampFloat(value.X, minX, maxX), ClampFloat(value.Y, minY, maxY));
        }

        //Integer based clamp function for code optimisation
        public static int ClampInteger(int value, int min, int max)
        {
            //Create a brand new decimal/integer to avoid cross memory referencing
            int i = new int();
            //Assign the decimal/integer
            i = value;
            //Clamp the deimal/integer based on min and max variables
            if ((i > max))
            {
                i = max;
            }
            if ((i < min))
            {
                i = min;
            }
            //Return the new integer ready for direct use in code
            return i;
        }

        //Generic double precision clamp useful for any instance in code. Will be automatically casted.
        public static double ClampDouble(double value, double min, double max)
        {
            //Create a brand new decimal/integer to avoid cross memory referencing
            double i = new double();
            //Assign the decimal/integer
            i = value;
            //Clamp the deimal/integer based on min and max variables
            if ((i > max))
            {
                i = max;
            }
            if ((i < min))
            {
                i = min;
            }
            //Return the new integer ready for direct use in code
            return i;
        }

        //Linear Interpolates between the two numbers
        public static float Lerp(float A, float B, float amount)
        {
            return A + (B - A) * amount;
        }

        //Linear Interpolates between two Vector2
        public static Vector2 Lerp(Vector2 A, Vector2 B, float amount)
        {
            return A + (B - A) * amount;
        }


        public static float Vector2Distance(Vector2 a, Vector2 b)
        {
            //c = sqrt(a^2 + b^2)
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static float Vector2DistanceSquared(Vector2 a, Vector2 b)
        {
            //c^2 = a^2 + b^2
            float fa = a.X - b.X;
            float fb = a.Y - b.Y;
            return fa * fa + fb * fb;
        }

    }
}
