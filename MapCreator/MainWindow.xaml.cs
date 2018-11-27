using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace MapCreator
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Actions { dragging, rotating, none }
        private Actions currentAction = Actions.none;

        private Image SelectedImage;
        private Point initialClickPosition;
        private Image baseImage;
        private Line rotateLine;
        private double oldAngle = 0;

        private FixtureCollection Fixtures;


        public MainWindow()
        {
            InitializeComponent();
            Fixtures = FixtureCollection.FromFile();
            
            rotateLine = new Line();
            rotateLine.StrokeThickness = 1;
            rotateLine.Stroke = Brushes.Red;
            MainCanvas.Children.Add(rotateLine);
            rotateLine.X1 = 0; rotateLine.X2 = 0;
            rotateLine.Y1 = rotateLine.Y2 = 0;
            Panel.SetZIndex(rotateLine, 3);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var files = System.IO.Directory.GetFiles("BaseImages");

            foreach (var file in files)
            {
                var fix = Fixtures.FindByName(System.IO.Path.GetFileNameWithoutExtension(file));
                fix.LoadImage(file);

                if (file == @"BaseImages\120mm.png")
                {
                    baseImage = fix.img;
                    Panel.SetZIndex(fix.img, 0);
                    Bigwindow.Height = fix.img.Height * 2;
                    Bigwindow.ResizeMode = ResizeMode.NoResize;
                }
                else
                {
                    Panel.SetZIndex(fix.img, 1);

                }
                Canvas.SetLeft(fix.img, fix.Position.X);
                Canvas.SetTop(fix.img, fix.Position.Y);
                RotateTransform rotateTransform = new RotateTransform(fix.Rotation);
                fix.img.RenderTransform = rotateTransform;
                MainCanvas.Children.Add(fix.img);
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Image;
            if (image == baseImage)
                return;

            if (image != null && MainCanvas.CaptureMouse())
            {
                currentAction = Actions.dragging;
                initialClickPosition = e.GetPosition(MainCanvas);
                SelectedImage = image;
                Panel.SetZIndex(SelectedImage, 2); // in case of multiple images
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectedImage != null)
            {

                Fixtures.FindByImage(SelectedImage).Position = new Point(Canvas.GetLeft(SelectedImage), Canvas.GetTop(SelectedImage));

                MainCanvas.ReleaseMouseCapture();
                Panel.SetZIndex(SelectedImage, 1);
                currentAction = Actions.none;
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentAction == Actions.dragging)
            {
                var position = e.GetPosition(MainCanvas);
                var offset = position - initialClickPosition;
                initialClickPosition = position;
                Canvas.SetLeft(SelectedImage, Canvas.GetLeft(SelectedImage) + offset.X);
                Canvas.SetTop(SelectedImage, Canvas.GetTop(SelectedImage) + offset.Y);
            }
            else if (currentAction == Actions.rotating)
            {
                var position = e.GetPosition(MainCanvas);
                var angle = Math2D.AngleBetween(new Vector(1, 0), new Vector(rotateLine.X2 - rotateLine.X1, rotateLine.Y2 - rotateLine.Y1));
                if (angle != 0)
                {
                    var offset = angle - oldAngle;
                    oldAngle = angle;
                    rotateLine.X2 = position.X;
                    rotateLine.Y2 = position.Y;
                    Fixtures.FindByImage(SelectedImage).Rotate(offset);
                }
            }
        }


        private void MainCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentAction = Actions.rotating;

            if(SelectedImage == null)
            {
                var image = e.Source as Image;
                if (image == baseImage)
                    return;
                SelectedImage = image;
            }

            if (MainCanvas.CaptureMouse())
            {
                initialClickPosition.X = Canvas.GetLeft(SelectedImage) + SelectedImage.ActualWidth / 2; ;
                initialClickPosition.Y = Canvas.GetTop(SelectedImage) + SelectedImage.ActualHeight / 2; ;
                rotateLine.X1 = initialClickPosition.X;
                rotateLine.Y1 = initialClickPosition.Y;
                rotateLine.X2 = e.GetPosition(MainCanvas).X;
                rotateLine.Y2 = e.GetPosition(MainCanvas).Y;
                oldAngle = Math2D.AngleBetween(new Vector(1, 0), new Vector(rotateLine.X2 - rotateLine.X1, rotateLine.Y2 - rotateLine.Y1));
                Panel.SetZIndex(SelectedImage, 2);
            }
        }

        private void MainCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectedImage != null)
            {
                MainCanvas.ReleaseMouseCapture();
                Panel.SetZIndex(SelectedImage, 1);
                rotateLine.X1 = rotateLine.X2 = rotateLine.Y2 = rotateLine.Y1 = 0;
                currentAction = Actions.none;
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Fixtures.SaveToFile();
        }

        private void LoadImages()
        {
            foreach (var fix in Fixtures.Fixtures)
            {
                if (fix.Name == @"120mm")
                {
                    baseImage = fix.img;
                    Panel.SetZIndex(fix.img, 0);
                    Bigwindow.Height = fix.img.Height * 2;
                    Bigwindow.ResizeMode = ResizeMode.NoResize;
                }
                else
                {
                    Panel.SetZIndex(fix.img, 1);

                }
                Canvas.SetLeft(fix.img, 0);
                Canvas.SetTop(fix.img, 0);
                MainCanvas.Children.Add(fix.img);
            }

        }

        private void Discover_Click(object sender, RoutedEventArgs e)
        {
            var files = System.IO.Directory.GetFiles("BaseImages");

            foreach (var file in files)
            {
                var fix = Fixture.FromPNG(file);
                Fixtures.Add(fix);

                if (file == @"BaseImages\120mm.png")
                {
                    baseImage = fix.img;
                    Panel.SetZIndex(fix.img, 0);
                    Bigwindow.Height = fix.img.Height * 2;
                    Bigwindow.ResizeMode = ResizeMode.NoResize;
                }
                else
                {
                    Panel.SetZIndex(fix.img, 1);

                }
                Canvas.SetLeft(fix.img, 0);
                Canvas.SetTop(fix.img, 0);
               // MainCanvas.Children.Add(fix.img);
            }

            Settings settings = new Settings();
            settings.Fixtures = this.Fixtures;
            settings.Show();
        }

        private void Compress_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)MainCanvas.RenderSize.Width, (int)MainCanvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(MainCanvas);
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(rtb));

            var bitmapImage = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            
            WriteableBitmap final = new WriteableBitmap(bitmapImage);

            

            int width = (int)final.Width;
            int height = (int)final.Height;
            int red;
            int green;
            int blue;
            int alpha;
            byte sourceRed;
            byte sourceGreen;
            byte sourceBlue;
            byte sourceAlpha;
            uint[] pixels = new uint[width * height];
            byte[] pixelData = new byte[4];

            int stride = final.PixelWidth * 4;
            int size = final.PixelHeight * stride;
            byte[] pixelsSource = new byte[size];
            final.CopyPixels(pixelsSource, stride, 0);

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int sourceIdx = y * stride + 4 * x;
                    sourceRed = pixelsSource[sourceIdx];
                    sourceGreen = pixelsSource[sourceIdx + 1];
                    sourceBlue = pixelsSource[sourceIdx + 2];
                    sourceAlpha = pixelsSource[sourceIdx + 3];

                    int i = (int)final.Width * y + x;

                    if (sourceRed != 255)
                    {
                        red = 0;
                        green = 0;
                        alpha = 255;
                        blue = 0;
                    }
                    else
                    {
                        red = 255;
                        green = 255;
                        blue = 255;
                        alpha = 255;
                    }


                    try
                    {
                        pixels[i] = (uint)((alpha << 24) + (green << 16) + (red << 8) + blue);
                    }
                    catch
                    {
                        ;
                    }
                }
            }
            final.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

            CompressedImage.Source = final;

            /*
            int stride = bitmapImage.PixelWidth * 4;
            int size = bitmapImage.PixelHeight * stride;
            byte[] pixels = new byte[size];
            bitmapImage.CopyPixels(pixels, stride, 0);

            

            int h = final.PixelHeight;
            int w = final.PixelWidth;
            int[] pixelData = new int[w * h];
            int widthInByte = 4 * w;

            final.CopyPixels(pixelData, widthInByte, 0);
            
            for (int y = 0; y < bitmapImage.PixelHeight; ++y)
            {
                for (int x = 0; x < bitmapImage.PixelWidth; ++x)
                {
                    int index = y * stride + 4 * x;
                    byte red = pixels[index];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    if (red < 255)
                    {
                        try
                        {
                            pixelData[index] = 0;
                        }
                        catch
                        { }
                    }
                }
            }
            final.WritePixels(new Int32Rect(0, 0, w, h), pixelData, widthInByte, 0);

            CompressedImage.Source = final;
            */

        }
    }
}
