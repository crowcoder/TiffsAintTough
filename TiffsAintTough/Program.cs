using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace TiffsAintTough
{
    class Program
    {
        private static Random rnd = new Random();

        static void Main(string[] args)
        {
            MakeATiff();
            SplitATiff();        
        }

        /// <summary>
        /// Combines four images into one multi-page .tif
        /// </summary>
        public static void MakeATiff()
        {
            TiffBitmapEncoder tiffEncoder = new TiffBitmapEncoder();

            //The new .tif file that will be the combination of several image sources
            FileStream endResult = new FileStream(@"./images/NewTif.tif", FileMode.OpenOrCreate);
        
            //add three existing image files as pages (frames) to the new .tif file
            Stream imageStream = new FileStream(@"./images/codereview.png", FileMode.Open);
            BitmapFrame frame = BitmapFrame.Create(imageStream);
            tiffEncoder.Frames.Add(frame);

            imageStream = new FileStream(@"./images/happydog.jpg", FileMode.Open);
            frame = BitmapFrame.Create(imageStream);
            tiffEncoder.Frames.Add(frame);

            imageStream = new FileStream(@"./images/lobester.jpg", FileMode.Open);
            frame = BitmapFrame.Create(imageStream);
            tiffEncoder.Frames.Add(frame);

            //Demonstrate that a Frame can be created from any image source, not just existing file
            tiffEncoder.Frames.Add(MakeRandomImage());

            //saves the changes to the encoder
            tiffEncoder.Save(endResult);

            imageStream.Dispose();
            endResult.Dispose();
        }

        /// <summary>
        /// Creates a separate image from from each page of a .tif
        /// </summary>
        public static void SplitATiff()
        {
            using (FileStream fs = File.OpenRead(@"./images/NewTif.tif"))
            {
                TiffBitmapDecoder decoder = new TiffBitmapDecoder(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                for (int i = 0; i < decoder.Frames.Count; i++)
                {
                    TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                    encoder.Frames.Add(decoder.Frames[i]);
                    using (FileStream stream = new FileStream("./images/frame" + i.ToString() + ".tif", FileMode.Create))
                    {
                        encoder.Save(stream);
                    }
                }
            }
        }

        /// <summary>
        /// Helper function that creates a random image 
        /// </summary>
        /// <returns></returns>
        private static BitmapFrame MakeRandomImage()
        {
            System.Drawing.Image img = new System.Drawing.Bitmap(400, 400, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
            {
                g.Clear(Color.White);
                for (int i = 0; i < 42; i++)
                {
                    Pen p = GetRandomPen();
                    Point start = GetRandomPoint();
                    Point end = GetRandomPoint();
                    g.DrawLine(p, start, end);
                    g.Save();
                }                
            }
            var imgStream = new MemoryStream();
            img.Save(imgStream, System.Drawing.Imaging.ImageFormat.Png);
            imgStream.Position = 0;
            return BitmapFrame.Create(imgStream);
        }

        /// <summary>
        /// Generates a randlom Pen
        /// </summary>
        /// <returns></returns>
        private static System.Drawing.Pen GetRandomPen()
        {
            Int32 LineWidth = rnd.Next(3, 25);
            Color rndColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            return new Pen(rndColor, LineWidth);
        }

        /// <summary>
        /// Gets a random point
        /// </summary>
        /// <returns></returns>
        private static Point GetRandomPoint()
        {
            int start = rnd.Next(0, 390);
            int end = rnd.Next(0, 390);
            return new Point(start, end);
        }
    }
}
