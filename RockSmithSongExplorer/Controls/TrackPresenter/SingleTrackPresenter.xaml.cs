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

namespace RockSmithSongExplorer.Controls.TrackPresenter
{
    /// <summary>
    /// Interaction logic for TrackPresenter.xaml
    /// </summary>
    public partial class SingleTrackPresenter : UserControl
    {
        public static readonly DependencyProperty MultiTrackProperty = DependencyProperty.Register("MultiTrack", typeof(SimplifiedSong), typeof(SingleTrackPresenter), new PropertyMetadata(null, OnTrackPropertyChanged));

        private static void OnTrackPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trackPresenter=(SingleTrackPresenter)d;
            trackPresenter.RenderTrack();
        }

        public SimplifiedSong MultiTrack
        {
            get { return (SimplifiedSong)GetValue(MultiTrackProperty); }
            set { SetValue(MultiTrackProperty, value); }
        }

        public SingleTrackPresenter()
        {
            InitializeComponent();
        }

        TimeSpan m_prevTime = TimeSpan.Zero;
        Dictionary<int, SongNote2014> m_sustainedNotes = new Dictionary<int, SongNote2014>();
        SimplifiedTrack _renderedTrack = null;

        void RenderTrack()
        {
            var Track = MultiTrack.MainTrack;
            if (Track == _renderedTrack)
                return;
            _renderedTrack = Track;
            m_prevTime = TimeSpan.Zero;
            m_sustainedNotes.Clear();
            wrpCanvases.Children.Clear();

            if(Track==null || Track.Bars==null)
                return;

            foreach (var bar in Track.Bars)
            {
                var border = new Border();
                border.Padding = new Thickness(10);
                var canvas = new Canvas();
                canvas.Background = Brushes.Black;
                canvas.Width = 600;
                canvas.Height = 120;
                border.Child = canvas;
                wrpCanvases.Children.Add(border);
                PopulateCanvas(canvas, bar);
            }
        }

        void PopulateCanvas(Canvas canvas, SimplifiedBar bar)
        {
            var canvasHeight = canvas.Height;
            var canvasWidth = canvas.Width;
            var stringStartOffsetY = 20;
            var stringStartOffsetX = 10;
            var pixelsPerStringHalf = (canvasHeight - stringStartOffsetY) / (_renderedTrack.NumberOfStrings * 2);

            //Draw strings
            var y = canvasHeight - pixelsPerStringHalf;
            for (int i = 0; i < _renderedTrack.NumberOfStrings; i++)
            {
                //var y = ((i * 2) + 1) * pixelsPerStringHalf;
                var line = new Line() { X1 = 0, X2 = canvas.Width, Y1 = y, Y2 = y, Stroke = RocksmithRenderHelper.GetStringColor(i), StrokeThickness = 4 };
                canvas.Children.Add(line);
                y -= (pixelsPerStringHalf * 2);
            }

            var rsDuration = bar.EndTime - bar.StartTime;
            var pixelsPerSec = (canvasWidth - stringStartOffsetX) / rsDuration;

            //Draw sustained notes from previous bar(s)
            var prevSustained = m_sustainedNotes.Values.ToList();
            m_sustainedNotes.Clear();
            foreach (var sustainNote in prevSustained)
            {
                var rsChortOffsetFromBarStart = 0;
                var border = RocksmithRenderHelper.CreateNoteElement(pixelsPerStringHalf, sustainNote.String, "(" + sustainNote.Fret + ")");
                ProcessSustain(bar, pixelsPerSec, sustainNote, border);
                Canvas.SetLeft(border, stringStartOffsetX + (rsChortOffsetFromBarStart * pixelsPerSec));
                Canvas.SetTop(border, stringStartOffsetY + (pixelsPerStringHalf * ((3 - sustainNote.String) * 2)));      //String position
                canvas.Children.Add(border);
            }

            //Draw barmarkers
            foreach (var ebeat in bar.EBeats)
            {
                var rsOffset = ebeat.Time - bar.StartTime;
                var xPosPixels = stringStartOffsetX + (pixelsPerSec * rsOffset);
                var line = new Line() { X1 = xPosPixels, X2 = xPosPixels, Y1 = 0, Y2 = canvas.Height, Stroke = Brushes.Wheat, StrokeThickness = 2 };
                canvas.Children.Add(line);
                var tsStart = TimeSpan.FromSeconds(ebeat.Time);
                var timeText = tsStart.ToString(@"m\:ss\.fff");
                if (m_prevTime != TimeSpan.Zero)
                {
                    var diff = tsStart.Subtract(m_prevTime);
                    timeText += " (+" + diff.ToString(@"m\:ss\.fff") + ")";
                }
                m_prevTime = tsStart;

                var tb = new TextBlock() { Text = timeText, Foreground = Brushes.White };
                Canvas.SetLeft(tb, xPosPixels);
                Canvas.SetTop(tb, 10);
                canvas.Children.Add(tb);
            }

            //Draw notes in bar
            foreach (var note in bar.Notes)
            {
                var rsChortOffsetFromBarStart = note.Time - bar.StartTime;
                var border = RocksmithRenderHelper.CreateNoteElement(pixelsPerStringHalf, note.String, note.Fret.ToString());
                ProcessSustain(bar, pixelsPerSec, note, border);
                Canvas.SetLeft(border, stringStartOffsetX + (rsChortOffsetFromBarStart * pixelsPerSec));
                Canvas.SetTop(border, stringStartOffsetY + (pixelsPerStringHalf * ((3 - note.String) * 2)));      //String position
                canvas.Children.Add(border);
            }

            //Draw chords in bar
            foreach (var chord in bar.Chords)
            {
                if(chord.ChordNotes!=null)
                {
                    foreach (var chordnote in chord.ChordNotes)
                    {
                        var rsChortOffsetFromBarStart = chordnote.Time - bar.StartTime;
                        var border = RocksmithRenderHelper.CreateNoteElement(pixelsPerStringHalf, chordnote.String, chordnote.Fret.ToString());
                        ProcessSustain(bar, pixelsPerSec, chordnote, border);
                        Canvas.SetLeft(border, stringStartOffsetX + (rsChortOffsetFromBarStart * pixelsPerSec));
                        Canvas.SetTop(border, stringStartOffsetY + (pixelsPerStringHalf * ((3 - chordnote.String) * 2)));      //String position
                        canvas.Children.Add(border);
                    }
                }
            }
        }

        void ProcessSustain(SimplifiedBar bar, double pixelsPerSec, SongNote2014 note, Border border)
        {
            if (note.Sustain > 0)
            {
                var startTime = Math.Max(bar.StartTime, note.Time);
                var remainingSustain = note.Time + note.Sustain - startTime;

                //Sustain exceeds this bar. add to dictionary, to let the next bar be aware of it.
                if (remainingSustain > bar.EndTime - note.Time)
                {
                    remainingSustain = bar.EndTime - note.Time;
                    m_sustainedNotes[note.String] = note;
                }
                border.Width = remainingSustain * pixelsPerSec;
            }
        }
    }
}
