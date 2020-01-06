using System;
using UnityEngine;

namespace Raele.Util
{
    /// <summary>
    /// SpacialExpandableMatrix is basically a 2-dimensional array indexed at any spacial point; in other words, it can
    /// contain objects in any integer coordinates, including negative. The array automatically grows when needed.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class SpacialExpandableMatrix<TValue>
    {
        public const int EXTRA_BOUNDARY = 10;

        public int GreaterX { get { return this.Width + this.LowerX - 1; } }
        public int GreaterY { get { return this.Height + this.LowerY - 1; } }
        public int LowerX { get; private set; }
        public int LowerY { get; private set; }
        public int Width { get { return this.m_cells.GetLength(0); } }
        public int Height { get { return this.m_cells.GetLength(1); } }

        private TValue[,] m_cells;

        public SpacialExpandableMatrix()
        {
            this.m_cells = new TValue[EXTRA_BOUNDARY * 2, EXTRA_BOUNDARY * 2];
            this.LowerX = this.LowerY = EXTRA_BOUNDARY * -1;
        }

        /// <summary>
        /// Creates the matrix with the given boundaries. (inclusive, inclusive)
        /// </summary>
        /// <param name="lowerX"></param>
        /// <param name="greaterX"></param>
        /// <param name="lowerY"></param>
        /// <param name="greaterY"></param>
        public SpacialExpandableMatrix(int lowerX, int greaterX, int lowerY, int greaterY)
        {
            (greaterX >= lowerX).AssertNotDefault();
            (greaterY >= lowerY).AssertNotDefault();
            this.m_cells = new TValue[greaterX - lowerX + 1, greaterY - lowerY + 1];
            this.LowerX = lowerX;
            this.LowerY = lowerY;
        }

        public void Clear()
        {
            this.m_cells = new TValue[this.Width, this.Height];
        }

        public TValue Get(int x, int y)
        {
            this.ConvertCoordinates(ref x, ref y);
            return this.m_cells[x, y];
        }

        public TValue GetOrDefault(int x, int y, TValue defaultValue = default(TValue))
        {
            return this.WithinBounds(x, y) ? this.Get(x, y) : defaultValue;
        }

        public void Set(int x, int y, TValue value)
        {
            this.AssertCoordinates(ref x, ref y);
            this.m_cells[x, y] = value;
        }

        public bool WithinBounds(int x, int y)
        {
            return x >= this.LowerX && x <= this.GreaterX && y >= this.LowerY && y <= this.GreaterY;
        }

        private void AssertCoordinates(ref int x, ref int y)
        {
            if (!this.WithinBounds(x, y))
            {
                int diffX = Mathf.Max(0, x - this.GreaterX) + Mathf.Min(0, x - this.LowerX);
                int diffY = Mathf.Max(0, y - this.GreaterY) + Mathf.Min(0, y - this.LowerY);

                this.Resize(this.Width + Mathf.Abs(diffX), this.Height + Mathf.Abs(diffY));
                this.Shift(Mathf.Max(0, diffX * -1), Mathf.Max(0, diffY * -1));

                (diffX < 0).Then(() => this.LowerX += diffX);
                (diffY < 0).Then(() => this.LowerY += diffY);
            }

            this.ConvertCoordinates(ref x, ref y);
        }

        private void ConvertCoordinates(ref int x, ref int y)
        {
            x -= this.LowerX;
            y -= this.LowerY;
        }

        private void Resize(int newWidth, int newHeight)
        {
            TValue[,] newArray = new TValue[newWidth, newHeight];
            int lowerWidth = Math.Min(this.Width, newWidth);
            int lowerHeight = Math.Min(this.Height, newHeight);

            for (int x = 0; x < lowerWidth; x++)
            {
                for (int y = 0; y < lowerHeight; y++)
                {
                    newArray[x, y] = this.m_cells[x, y];
                }
            }

            this.m_cells = newArray;
        }

        private void Shift(int horizontalShift, int verticalShift)
        {
            for (int x = this.Width - 1; x >= 0; x--)
            {
                for (int y = this.Height - 1; y >= 0; y--)
                {
                    this.m_cells[x, y] = (x - horizontalShift < 0 || y - verticalShift < 0)
                            ? default(TValue)
                            : this.m_cells[x - horizontalShift, y - verticalShift];
                }
            }
        }
    }
}
