using System.Drawing;

namespace FromImage
{
    class Piece
    {
        public int Size { get; private set; }

        public Color[,] Pixels { get; private set; }

        public Edge LeftEdge { get; private set; }
        public Edge RightEdge { get; private set; }
        public Edge TopEdge { get; private set; }
        public Edge BottomEdge { get; private set; }

        public Piece(int size, Color[,] pixels)
        {
            Size = size;
            Pixels = pixels;

            //left edge
            Color[] leftEdgePixels = new Color[Size];
            for (int i = 0; i < Size; i++)
            {
                leftEdgePixels[i] = Pixels[i, 0];
            }
            LeftEdge = new Edge(Size, leftEdgePixels);

            //right edge
            Color[] rightEdgePixels = new Color[Size];
            for (int i = 0; i < Size; i++)
            {
                rightEdgePixels[i] = Pixels[i, Size - 1];
            }
            RightEdge = new Edge(Size, rightEdgePixels);

            //top edge
            Color[] topEdgePixels = new Color[Size];
            for (int i = 0; i < Size; i++)
            {
                topEdgePixels[i] = Pixels[0, i];
            }
            TopEdge = new Edge(Size, topEdgePixels);

            //bottom edge
            Color[] bottomEdgePixels = new Color[Size];
            for (int i = 0; i < Size; i++)
            {
                bottomEdgePixels[i] = Pixels[Size - 1, i];
            }
            BottomEdge = new Edge(Size, bottomEdgePixels);
        }
    }
}
