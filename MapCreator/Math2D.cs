using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MapCreator
{
    public static class Math2D
    {
        public static int stride;

        public static groundColor GetKindOfTerrain(byte[] pixelsSource,  Point index)
        {
            try
            {
                int sourceIdx = (int)index.Y * stride + 4 * (int)index.X;
                if (pixelsSource[sourceIdx] == 255 && pixelsSource[sourceIdx + 1] == 255 && pixelsSource[sourceIdx + 2] == 255)
                    return groundColor.open;
                else if(pixelsSource[sourceIdx] == 0 && pixelsSource[sourceIdx + 1] == 0 && pixelsSource[sourceIdx + 2] == 0)
                    return groundColor.fixture;
                else
                    return groundColor.nonLOSBlockingFixture;                    
            }catch
            {
                return groundColor.fixture;
            }
        }

        public static bool IsPixelDifferntThanWhie(byte[] pixelsSource, Point index)
        {
            try
            {
                int sourceIdx = (int)index.Y * stride + 4 * (int)index.X;
                if (pixelsSource[sourceIdx] == 255 && pixelsSource[sourceIdx + 1] == 255 && pixelsSource[sourceIdx + 2] == 255)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SetPixelRGB(byte[] pixelsSource, Point index, byte r, byte g, byte b, byte alpha = 255)
        {
            int sourceIdx = (int)index.Y * stride + 4 * (int)index.X;
            if (sourceIdx < pixelsSource.Length - 3)
            {
                pixelsSource[sourceIdx + 0] = b;    // blue
                pixelsSource[sourceIdx + 1] = g;    // green
                pixelsSource[sourceIdx + 2] = r;  // red
                pixelsSource[sourceIdx + 3] = alpha;  // alpha
            }
        }

        public static void SetPixelRed(byte[] pixelsSource,  Point index)
        {
            SetPixelRGB(pixelsSource,  index, 255, 0, 0);
        }
        public static void SetPixeDarkRed(byte[] pixelsSource, Point index)
        {
            SetPixelRGB(pixelsSource, index, 100, 0, 0);
        }
        public static void SetPixelGreen(byte[] pixelsSource, Point index)
        {
            SetPixelRGB(pixelsSource,  index, 0, 255, 0);
        }

        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }


        public static List<Point> line(Point start, Point end) => line((int)start.X, (int)start.Y, (int)end.X, (int)end.Y);

        public static List<Point> line(int x, int y, int x2, int y2)
        {
            var result = new List<Point>();

            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                result.Add(new Point(x, y));
                
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }

            return result;
        }
        

        internal static void DrawSquareRGB(byte[] pixelsSourceToWrite, Point clickedPoint, byte r, byte g, byte b)
        {
            var x = clickedPoint.X;
            var y = clickedPoint.Y;

            SetPixelRGB(pixelsSourceToWrite,  new Point(x - 1, y - 1), r, g, b);
            SetPixelRGB(pixelsSourceToWrite,  new Point(x + 0, y - 1), r, g, b);
            SetPixelRGB(pixelsSourceToWrite,  new Point(x + 1, y - 1), r, g, b);

            SetPixelRGB(pixelsSourceToWrite,  new Point(x - 1, y), r, g, b);
            SetPixelRGB(pixelsSourceToWrite,  new Point(x + 0, y), r, g, b);
            SetPixelRGB(pixelsSourceToWrite,  new Point(x + 1, y), r, g, b);

            SetPixelRGB(pixelsSourceToWrite,  new Point(x - 1, y + 1), r, g, b);
            SetPixelRGB(pixelsSourceToWrite,  new Point(x + 0, y + 1), r, g, b);
            SetPixelRGB(pixelsSourceToWrite,  new Point(x + 1, y + 1), r, g, b);
        }
    }
}
