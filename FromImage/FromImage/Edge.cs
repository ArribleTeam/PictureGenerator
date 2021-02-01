using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FromImage
{
    struct Edge : IEquatable<Edge>
    {
        public int Size { get; private set; }
        public Color[] Pixels { get; private set; }

        public Edge(int size, Color[] pixels)
        {
            Size = size;
            Pixels = pixels;
        }

        public bool Equals(Edge other)
        {
            if (other.Size != this.Size)
            {
                return false;
            }

            for (int i = 0; i < this.Size; i++)
            {
                if (this.Pixels[i] != other.Pixels[i])
                {
                    return false;
                }
            }
            //Console.WriteLine("equals");

            return true;
        }
    }
}
