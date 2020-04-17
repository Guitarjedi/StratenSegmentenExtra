using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public class Calculator
    {
        public static double getLenthOfStraat(Straat s)
        {
            double length = 0;

            foreach (var segment in s.Graaf.Map)
            {
                for (int i = 0; i < segment.Vertices.Count - 1; i++)
                {
                    length += getLengthBetweenPoints(segment.Vertices[i], segment.Vertices[i + 1]);
                }
            }

            return length;
        }
        private static double getLengthBetweenPoints(Punt een, Punt ander)
        {
            return Math.Sqrt(Math.Pow(een.X - ander.X, 2) + Math.Pow(een.Y - ander.Y, 2));
        }
    }
}
