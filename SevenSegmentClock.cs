using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace MyAnimationApp
{
    public class SevenSegmentClock : Control
    {
        private readonly DispatcherTimer _timer;
        private bool _showColon = true;

        private const int SegmentWidth = 13;
        private const int SegmentLength = 45;
        private const int TriangleSize = 8;
        private const int DigitSpacing = 5;

        private static readonly SolidColorBrush SegmentBrush = new(Colors.White);

        private static readonly bool[,] DigitSegments =
        {
            { true, true, true, true, true, true, false },
            { false, true, true, false, false, false, false },
            { true, true, false, true, true, false, true },
            { true, true, true, true, false, false, true },
            { false, true, true, false, false, true, true },
            { true, false, true, true, false, true, true },
            { true, false, true, true, true, true, true },
            { true, true, true, false, false, false, false },
            { true, true, true, true, true, true, true },
            { true, true, true, true, false, true, true }
        };

        public SevenSegmentClock()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, _) =>
            {
                _showColon = !_showColon;
                InvalidateVisual();
            };
            _timer.Start();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            string time = DateTime.Now.ToString("HHmm");
            double y = Bounds.Height - 130;

            for (int i = 0; i < time.Length; i++)
            {
                double x = i * (SegmentLength * 2 + DigitSpacing);
                DrawDigit(context, time[i] - '0', x, y);

                if (i == 1 && _showColon)
                {
                    double colonX = x + SegmentLength + DigitSpacing * 5;
                    DrawColon(context, colonX, y + SegmentLength / 2, 10);
                }
            }
        }

        private void DrawDigit(DrawingContext context, int digit, double x, double y)
        {
            bool[] s = new bool[7];
            for (int i = 0; i < 7; i++) s[i] = DigitSegments[digit, i];

            if (s[0]) DrawHorizontal(context, x + SegmentWidth / 2, y, SegmentLength);
            if (s[1]) DrawVertical(context, x + SegmentLength, y + SegmentWidth / 2, SegmentLength);
            if (s[2]) DrawVertical(context, x + SegmentLength, y + SegmentLength + SegmentWidth / 2, SegmentLength);
            if (s[3]) DrawHorizontal(context, x + SegmentWidth / 2, y + SegmentLength * 2, SegmentLength);
            if (s[4]) DrawVertical(context, x, y + SegmentLength + SegmentWidth / 2, SegmentLength);
            if (s[5]) DrawVertical(context, x, y + SegmentWidth / 2, SegmentLength);
            if (s[6]) DrawHorizontal(context, x + SegmentWidth / 2, y + SegmentLength, SegmentLength);
        }

        private void DrawHorizontal(DrawingContext context, double x, double y, double l)
        {
            double w = SegmentWidth, t = TriangleSize;
            var pts = new[]
            {
                new Point(x + t, y),
                new Point(x + l - t, y),
                new Point(x + l, y + w / 2),
                new Point(x + l - t, y + w),
                new Point(x + t, y + w),
                new Point(x, y + w / 2)
            };
            DrawPolygon(context, pts);
        }

        private void DrawVertical(DrawingContext context, double x, double y, double l)
        {
            double w = SegmentWidth, t = TriangleSize;
            var pts = new[]
            {
                new Point(x, y + t),
                new Point(x, y + l - t),
                new Point(x + w / 2, y + l),
                new Point(x + w, y + l - t),
                new Point(x + w, y + t),
                new Point(x + w / 2, y)
            };
            DrawPolygon(context, pts);
        }

        private void DrawColon(DrawingContext context, double x, double y, double size)
        {
            double spacing = size * 4;
            DrawRhomb(context, x, y, size);
            DrawRhomb(context, x, y + spacing, size);
        }

        private void DrawRhomb(DrawingContext context, double x, double y, double size)
        {
            var pts = new[]
            {
                new Point(x + size / 2, y),
                new Point(x + size, y + size / 2),
                new Point(x + size / 2, y + size),
                new Point(x, y + size / 2)
            };
            DrawPolygon(context, pts);
        }

        private void DrawPolygon(DrawingContext context, Point[] points)
        {
            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                ctx.BeginFigure(points[0], true);
                for (int i = 1; i < points.Length; i++)
                    ctx.LineTo(points[i]);
                ctx.EndFigure(true);
            }
            context.DrawGeometry(SegmentBrush, null, geo);
        }
    }
}