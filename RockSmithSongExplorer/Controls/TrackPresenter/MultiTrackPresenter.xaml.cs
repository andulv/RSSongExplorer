using GalaSoft.MvvmLight.Messaging;
using RockSmithSongExplorer.Controls.TrackPresenter;
using RockSmithSongExplorer.Models;
using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RockSmithSongExplorer.Controls
{

    /// <summary>
    /// Interaction logic for TrackPresenter.xaml
    /// </summary>
    public partial class MultiTrackPresenter : UserControl
    {
        readonly int _barWidth = 600;
        readonly int _barHeight = 160;  //Height of one track (all strings + tracktext/barmarkers)
        readonly int _barSpacingY = 30;

        readonly Dictionary<float, int> _barTimePixelMapping = new Dictionary<float, int>();

        public static readonly DependencyProperty MultiTrackProperty = DependencyProperty.Register("MultiTrack", typeof(SimplifiedSong), typeof(MultiTrackPresenter), new PropertyMetadata(null, OnMultiTrackPropertyChanged));

        private static void OnMultiTrackPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trackPresenter=(MultiTrackPresenter)d;
            trackPresenter.RenderTrack();
        }

        public SimplifiedSong MultiTrack
        {
            get { return (SimplifiedSong)GetValue(MultiTrackProperty); }
            set { SetValue(MultiTrackProperty, value); }
        }

        public MultiTrackPresenter()
        {
            InitializeComponent();
            Messenger.Default.Register<NavigateToTimeMessage>(this, x =>
            {
                var barIndex = MultiTrack.MainTrack.GetBarIndex(x.Time);
                var bar = MultiTrack.MainTrack.Bars[barIndex];
                double offset = barIndex * _barWidth;

                var transform = (canvas.LayoutTransform as ScaleTransform);
                offset = offset * transform.ScaleX;

                scrollViewer.ScrollToHorizontalOffset(offset);
            });
        }


        SimplifiedSong _renderedTrack = null;

        void RenderTrack()
        {
            if (MultiTrack == _renderedTrack)
                return;
            _renderedTrack = MultiTrack;

            _barTimePixelMapping.Clear();
            canvas.Children.Clear();

            if (MultiTrack == null)
                return;

            var mainTrack = MultiTrack.MainTrack;
            if (mainTrack == null || mainTrack.Bars == null)
            {
                canvas.Width = 0;
                canvas.Height = 0;
                return;
            }
            var trackCount = MultiTrack.AdditionalTracks.Count + 1;
            var heightAllTracks = trackCount * (_barHeight + (_barSpacingY*2));
            var heightSectionInfo = 60;
            canvas.Height = heightSectionInfo + heightAllTracks;
            canvas.Width = mainTrack.Bars.Count * _barWidth;


            var yOffset = 0;
            RenderSectionNames(MultiTrack.MainTrack, yOffset);
            yOffset += 20;
            RenderVocalTrack(yOffset);
            yOffset += 40;
            RenderInstrumentTrack(mainTrack, yOffset);
            yOffset += _barHeight + _barSpacingY;
            foreach (var track in MultiTrack.AdditionalTracks)
            {
                RenderInstrumentTrack(track, yOffset);
                yOffset += _barHeight + _barSpacingY;
            }          
        }

        private void RenderInstrumentTrack(SimplifiedTrack track, int yOffset)
        {
            new InstrumentTrackRenderer(this.canvas, track, yOffset, _barHeight, _barWidth)
                .DoRender();
        }

        private void RenderVocalTrack(int yOffset)
        {
            if (MultiTrack.VocalTrack!=null)
            {

                foreach(var vocal in MultiTrack.VocalTrack.Vocal)
                {
                    var startBar = MultiTrack.MainTrack.GetBar(vocal.Time);
                    var endBar = MultiTrack.MainTrack.GetBar(vocal.Time+vocal.Length);
                }
            }
            
        }

        private void RenderSectionNames(SimplifiedTrack referenceTrack, int yOffset)
        {
            var xOffset = 0;
            int barId = 1;
            SongSection prevSection = null;
            foreach (var bar in referenceTrack.Bars)
            {
                var currentSection = MultiTrack.Sections.LastOrDefault(x => x.StartTime <= bar.StartTime);
                if (currentSection != null)
                {
                    var tb = new TextBlock() { Text = currentSection.Name + " " + currentSection.Number, Foreground = Brushes.Cyan };
                    Canvas.SetLeft(tb, xOffset);
                    Canvas.SetTop(tb, yOffset);
                    canvas.Children.Add(tb);

                    if (ReferenceEquals(prevSection, currentSection) && prevSection!=null)
                        tb.Text = "(" + tb.Text + ")";

                    prevSection = currentSection;
                }


                xOffset += _barWidth;
                barId++;
            }

        }


        private int _zoomPercent = 100;
        private void UpdateZoomPercent(int newValue)
        {
            if (newValue < 10)
                _zoomPercent = 10;
            else if(newValue>999)
                _zoomPercent = 999;
            else
                _zoomPercent = newValue;

            txtZoom.Text = _zoomPercent + "%";
            var scaleValue = (float)_zoomPercent / 100f;

            var transform = (canvas.LayoutTransform as ScaleTransform);
            var offsetAdjusted = scrollViewer.HorizontalOffset / transform.ScaleX;
            transform.ScaleX = scaleValue;
            var newOffsetAdjusted = offsetAdjusted * transform.ScaleX;
            scrollViewer.ScrollToHorizontalOffset(newOffsetAdjusted);
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoomPercent(_zoomPercent + 10);
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            UpdateZoomPercent(_zoomPercent - 10);
        }

        private void txtZoom_LostFocus(object sender, RoutedEventArgs e)
        {
            var txt = txtZoom.Text.Replace("%", "");
            int newValue = _zoomPercent;
            int.TryParse(txt, out newValue);
            UpdateZoomPercent(newValue);
        }

        private void txtZoom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                var traversalRequest = new TraversalRequest(FocusNavigationDirection.Next) { Wrapped = true };
                (sender as TextBox).MoveFocus(traversalRequest);
            }
        }
    }
}
