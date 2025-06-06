using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace MyAnimationApp
{
    public class LizardAnimation : Control
    {
        private readonly List<List<(IBrush Fill, Geometry Shape)>> _frames = new();
        private readonly List<int> _baseSequence = new();
        private readonly List<int> _overlaySequence = new();

        private readonly DispatcherTimer _baseTimer;
        private readonly DispatcherTimer _overlayTimer;

        private int _baseFrameIndex = 0;
        private int _overlayFrameIndex = 0;

        public LizardAnimation()
        {
            LoadFramesAndSequences();

            _baseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            _overlayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(110) };

            _baseTimer.Tick += (_, _) =>
   {
       _baseFrameIndex++;
       if (_baseFrameIndex >= _baseSequence.Count)
       {
           _baseFrameIndex = _baseSequence.Count - 1;
           _baseTimer.Stop();
       }
       InvalidateVisual();
   };

            _overlayTimer.Tick += (_, _) =>
            {
                _overlayFrameIndex++;
                if (_overlayFrameIndex >= _overlaySequence.Count)
                    _overlayTimer.Stop();
                InvalidateVisual();
            };

            _baseTimer.Start();
            _overlayTimer.Start();
        }

        private void LoadFramesAndSequences()
        {
            string assetsPath = Path.Combine(AppContext.BaseDirectory, "Assets");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var framesJson = File.ReadAllText(Path.Combine(assetsPath, "frames.json"));
            var sequenceJson = File.ReadAllText(Path.Combine(assetsPath, "sequence.json"));

            var rawFrames = JsonSerializer.Deserialize<List<List<FrameLayer>>>(framesJson, options)!;
            foreach (var frame in rawFrames)
            {
                var parsedFrame = new List<(IBrush, Geometry)>();
                foreach (var layer in frame)
                {
                    if (string.IsNullOrWhiteSpace(layer.Color) || string.IsNullOrWhiteSpace(layer.Geometry))
                        continue;

                    parsedFrame.Add((
                        new SolidColorBrush(Color.Parse(layer.Color)),
                        Geometry.Parse(layer.Geometry)
                    ));
                }
                _frames.Add(parsedFrame);
            }

            var sequenceGroups = JsonSerializer.Deserialize<List<List<int>>>(sequenceJson, options)!;
            _baseSequence.AddRange(sequenceGroups.ElementAtOrDefault(0) ?? []);
            _overlaySequence.AddRange(sequenceGroups.ElementAtOrDefault(1) ?? []);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_frames.Count == 0)
                return;

            double scale = 0.6;
            double offsetX = (Bounds.Width * scale) / 2;
            double offsetY = (Bounds.Height * scale) / 4;

            var matrix = Matrix.CreateScale(scale, scale) *
                         Matrix.CreateTranslation(offsetX, offsetY);

            using (context.PushTransform(matrix))
            {
                
                if (_baseFrameIndex < _baseSequence.Count)
                {
                    var frame = _frames[_baseSequence[_baseFrameIndex]];
                    foreach (var (fill, shape) in frame)
                        context.DrawGeometry(fill, null, shape);
                }

                
                if (_overlayFrameIndex < _overlaySequence.Count)
                {
                    var overlayFrame = _frames[_overlaySequence[_overlayFrameIndex]];
                    foreach (var (fill, shape) in overlayFrame)
                        context.DrawGeometry(fill, null, shape);
                }
            }
        }

        private class FrameLayer
        {
            public string Color { get; set; } = default!;
            public string Geometry { get; set; } = default!;
        }
    }
}