using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FromImage
{
    class Generator
    {
        private static Random rand = new Random();

        public Dictionary<Edge, List<Piece>> LeftEdgeStacks { get; private set; }
        public Dictionary<Edge, List<Piece>> RightEdgeStacks { get; private set; }
        public Dictionary<Edge, List<Piece>> TopEdgeStacks { get; private set; }
        public Dictionary<Edge, List<Piece>> BottomEdgeStacks { get; private set; }
        public Dictionary<Piece, int> PieceStacksCount { get; private set; }
        public HashSet<Piece> Pieces { get; private set; }
        private Queue<(int, int)> bfsQueue;
        private int piecesMapSize;
        private Piece[,] piecesMap;
        private bool[,] enqueuedPiecesMap;

        public Generator()
        {
            LeftEdgeStacks = new Dictionary<Edge, List<Piece>>();
            RightEdgeStacks = new Dictionary<Edge, List<Piece>>();
            TopEdgeStacks = new Dictionary<Edge, List<Piece>>();
            BottomEdgeStacks = new Dictionary<Edge, List<Piece>>();
            Pieces = new HashSet<Piece>();
        }

        public Bitmap GetBigPicture(Bitmap image, int onePieceSize, int bigPictureSize)
        {
            DoPrecalculations(image, onePieceSize);
            Piece startPiece = GetStartPiece();
            int startCoord = bigPictureSize / onePieceSize / 2;
            Bitmap result = new Bitmap(image, bigPictureSize, bigPictureSize);

            piecesMapSize = (bigPictureSize - 1) / (onePieceSize - 1);
            piecesMap = new Piece[piecesMapSize, piecesMapSize];
            enqueuedPiecesMap = new bool[piecesMapSize, piecesMapSize];

            while (true)
            {
                for (int i = 0; i < piecesMapSize; i++)
                {
                    for (int j= 0; j < piecesMapSize; j++)
                    {
                        piecesMap[i, j] = null;
                        enqueuedPiecesMap[i, j] = false;
                    }
                }

                piecesMap[startCoord, startCoord] = startPiece;
                AddPieceToBitmap(ref result, startPiece, 
                    GetRealCoord(startCoord, onePieceSize), GetRealCoord(startCoord, onePieceSize), 
                    onePieceSize);
                
                bfsQueue = new Queue<(int, int)>();
                bfsQueue.Enqueue((startCoord - 1, startCoord));
                bfsQueue.Enqueue((startCoord + 1, startCoord));
                bfsQueue.Enqueue((startCoord, startCoord - 1));
                bfsQueue.Enqueue((startCoord, startCoord + 1));

                enqueuedPiecesMap[startCoord - 1, startCoord] = true;
                enqueuedPiecesMap[startCoord + 1, startCoord] = true;
                enqueuedPiecesMap[startCoord, startCoord - 1] = true;
                enqueuedPiecesMap[startCoord, startCoord + 1] = true;

                bool successfulGeneration = true;

                while (bfsQueue.Count > 0)
                {
                    (int, int) coords = bfsQueue.Dequeue();
                    if (!PlacePiece(ref result, coords, onePieceSize))
                    {
                        successfulGeneration = false;
                        break;
                    }
                }

                if (successfulGeneration)
                {
                    break;
                }
            }

            return result;
        }

        private bool PlacePiece(ref Bitmap bitmap, (int, int) coords, int onePieceSize)
        {
            IEnumerable<Piece> possiblePieces = Pieces.ToList();

            if (coords.Item1 > 0)
            {
                if (piecesMap[coords.Item1 - 1, coords.Item2] != null)
                {
                    Edge rightEdge = piecesMap[coords.Item1 - 1, coords.Item2].RightEdge;
                    if (LeftEdgeStacks.ContainsKey(rightEdge))
                    {
                        List<Piece> fittingPieces = LeftEdgeStacks[rightEdge];
                        if (fittingPieces != null)
                        {
                            possiblePieces = fittingPieces;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (!enqueuedPiecesMap[coords.Item1 - 1, coords.Item2])
                {
                    bfsQueue.Enqueue((coords.Item1 - 1, coords.Item2));
                    enqueuedPiecesMap[coords.Item1 - 1, coords.Item2] = true;
                }
            }

            if (coords.Item1 < piecesMapSize - 1)
            {
                if (piecesMap[coords.Item1 + 1, coords.Item2] != null)
                {
                    Edge leftEdge = piecesMap[coords.Item1 + 1, coords.Item2].LeftEdge;
                    if (RightEdgeStacks.ContainsKey(leftEdge))
                    {
                        List<Piece> fittingPieces = RightEdgeStacks[leftEdge];
                        if (fittingPieces != null)
                        {
                            possiblePieces = possiblePieces.Intersect(fittingPieces);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (!enqueuedPiecesMap[coords.Item1 + 1, coords.Item2])
                {
                    bfsQueue.Enqueue((coords.Item1 + 1, coords.Item2));
                    enqueuedPiecesMap[coords.Item1 + 1, coords.Item2] = true;
                }
            }

            if (coords.Item2 > 0)
            {
                if (piecesMap[coords.Item1, coords.Item2 - 1] != null)
                {
                    Edge bottomEdge = piecesMap[coords.Item1, coords.Item2 - 1].BottomEdge;
                    if (TopEdgeStacks.ContainsKey(bottomEdge))
                    {
                        List<Piece> fittingPieces = TopEdgeStacks[bottomEdge];
                        if (fittingPieces != null)
                        {
                            possiblePieces = possiblePieces.Intersect(fittingPieces);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (!enqueuedPiecesMap[coords.Item1, coords.Item2 - 1])
                {
                    bfsQueue.Enqueue((coords.Item1, coords.Item2 - 1));
                    enqueuedPiecesMap[coords.Item1, coords.Item2 - 1] = true;
                }
            }

            if (coords.Item2 < piecesMapSize - 1)
            {
                if (piecesMap[coords.Item1, coords.Item2 + 1] != null)
                {
                    Edge topEdge = piecesMap[coords.Item1, coords.Item2 + 1].TopEdge;
                    if (BottomEdgeStacks.ContainsKey(topEdge))
                    {
                        List<Piece> fittingPieces = BottomEdgeStacks[topEdge];
                        if (fittingPieces != null)
                        {
                            possiblePieces = possiblePieces.Intersect(fittingPieces);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (!enqueuedPiecesMap[coords.Item1, coords.Item2 + 1])
                {
                    bfsQueue.Enqueue((coords.Item1, coords.Item2 + 1));
                    enqueuedPiecesMap[coords.Item1, coords.Item2 + 1] = true;
                }
            }

            List<Piece> possiblePiecesList = possiblePieces.ToList();
            if (possiblePiecesList.Count == 0)
            {
                return false;
            }

            Piece resultPiece = possiblePiecesList[rand.Next(possiblePiecesList.Count)];
            piecesMap[coords.Item1, coords.Item2] = resultPiece;
            AddPieceToBitmap(ref bitmap, resultPiece, 
                GetRealCoord(coords.Item1, onePieceSize), GetRealCoord(coords.Item2, onePieceSize),
                onePieceSize);

            return true;
        }

        private int GetRealCoord(int coord, int onePieceSize)
        {
            return coord * (onePieceSize - 1);
        }

        private void AddPieceToBitmap(ref Bitmap bitmap, Piece piece, int x, int y, int onePieceSize)
        {
            for (int i = 0; i < onePieceSize; i++)
            {
                for (int j = 0; j < onePieceSize; j++)
                {
                    bitmap.SetPixel(x + i, y + j, piece.Pixels[i, j]);
                }
            }

            bitmap.Save(@"C:\Users\lensk\Documents\ProcedurelGeneration\FromImage\PreOutput.bmp");
        }

        private Piece CreatePiece(Bitmap image, int onePieceSize, int startX, int startY)
        {
            Color[,] pixels = new Color[onePieceSize, onePieceSize];

            for (int i = 0; i < onePieceSize; i++)
            {
                for (int j = 0; j < onePieceSize; j++)
                {
                    pixels[i, j] = image.GetPixel(startX + i, startY + j);
                }
            }

            Piece piece = new Piece(onePieceSize, pixels);
            return piece;
        }

        private void DoPrecalculations(Bitmap image, int onePieceSize)
        {
            for (int i = 0; i < image.Width - onePieceSize + 1; i++)
            {
                for (int j = 0; j < image.Height - onePieceSize + 1; j++)
                {
                    Pieces.Add(CreatePiece(image, onePieceSize, i, j));
                }
            }

            foreach (Piece piece in Pieces)
            {
                //left egde
                if (!LeftEdgeStacks.ContainsKey(piece.LeftEdge))
                {
                    LeftEdgeStacks.Add(piece.LeftEdge, new List<Piece>());
                }
                LeftEdgeStacks[piece.LeftEdge].Add(piece);

                //right egde
                if (!RightEdgeStacks.ContainsKey(piece.RightEdge))
                {
                    RightEdgeStacks.Add(piece.RightEdge, new List<Piece>());
                }
                RightEdgeStacks[piece.RightEdge].Add(piece);

                //top egde
                if (!TopEdgeStacks.ContainsKey(piece.TopEdge))
                {
                    TopEdgeStacks.Add(piece.TopEdge, new List<Piece>());
                }
                TopEdgeStacks[piece.TopEdge].Add(piece);

                //bottom egde
                if (!BottomEdgeStacks.ContainsKey(piece.BottomEdge))
                {
                    BottomEdgeStacks.Add(piece.BottomEdge, new List<Piece>());
                }
                BottomEdgeStacks[piece.BottomEdge].Add(piece);
            }
        }

        private Piece GetStartPiece()
        {
            //TODO
            return Pieces.ToList()[rand.Next(Pieces.Count)];
        }
    }
}
