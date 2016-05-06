using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RockSmithSongExplorer
{
    public static class RocksmithRenderHelper
    {
        public static Border CreateNoteElement(double pixelsPerStringHalf, byte stringIndex, string noteText)
        {
            var border = new Border();
            border.Background = RocksmithRenderHelper.GetStringColor(stringIndex);

            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(2);
            border.Opacity = 0.8;
            border.Height = pixelsPerStringHalf * 2.0d;
            border.Child = new TextBlock()
            {
                Text = noteText,
                Padding = new Thickness(4, 0, 4, 0),
                FontWeight = FontWeights.Bold,
                Foreground = RocksmithRenderHelper.GetFontColorForString(stringIndex),
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center
            };
            return border;
        }

        public static Border CreateUnpitchedSlideNotes(double pixelsPerStringHalf, byte stringIndex, int fretStart, int fretEnd)
        {
            var fontBrush = RocksmithRenderHelper.GetFontColorForString(stringIndex);

            var border = new Border()
            {
                Background = RocksmithRenderHelper.GetStringColor(stringIndex),
                Padding = new Thickness(0, 0, 0, 0),
                Margin = new Thickness(0, 0, 0, 0),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Opacity = 0.8,
                Height = pixelsPerStringHalf * 2.0d
        };


            var grid = new Grid();
            grid.Children.Add( new TextBlock()
            {
                Text = fretStart.ToString(),
                Padding = new Thickness(4, 0, 4, 0),
                FontWeight = FontWeights.Bold,
                Foreground = fontBrush,
                Background = Brushes.Transparent,
                HorizontalAlignment=HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            });

            grid.Children.Add(new TextBlock()
            {
                Text = "(" + fretEnd + ")",
                Padding = new Thickness(4, 0, 4, 0),
                FontWeight = FontWeights.Bold,
                Foreground = fontBrush,
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            });

            grid.Children.Add(new Path()
            {
                Stroke = fontBrush,
                StrokeThickness = 2,
                Stretch = Stretch.Fill,
                Margin = new Thickness(6, 5, 5, 10),
                Data = new LineGeometry()
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                }
            });

            border.Child = grid;
            return border;
        }

        public static Brush GetStringColor(int stringNo)
        {
            switch (stringNo)
            {
                case 0:
                    return Brushes.Red;
                case 1:
                    return Brushes.Yellow;
                case 2:
                    return Brushes.Blue;
                case 3:
                    return Brushes.Orange;
                case 4:
                    return Brushes.Green;
                case 5:
                    return Brushes.Purple;
                default:
                    return Brushes.White;
            }
        }

        public static Brush GetFontColorForString(int stringNo)
        {
            switch (stringNo)
            {
                case 2:
                    return Brushes.White;
                default:
                    return Brushes.Black;
            }
        }
    }
}
