using System;
using System.Drawing;

namespace FromImage
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap(@"C:\Users\lensk\Documents\ProcedurelGeneration\FromImage\Input.png");

            Generator generator = new Generator();
            Bitmap result = generator.GetBigPicture(image, 3, 48);

            result.Save(@"C:\Users\lensk\Documents\ProcedurelGeneration\FromImage\Output.bmp");
        }
    }
}
