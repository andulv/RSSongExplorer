using GalaSoft.MvvmLight.Messaging;
using RockSmithSongExplorer.Models;
using RocksmithToolkitLib.Sng2014HSL;
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
    public class InstrumentTrackRenderer
    {
        const int _yOffsetStringFromTrackTop = 40;
        readonly static String[] _notesNames = new String[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        readonly static String[] _notesNamesHi = new String[] { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

        readonly Canvas _canvas;
        readonly int _yOffset;
        readonly SimplifiedTrack _track;
        readonly int _barWidth;
        readonly int _barHeight;
        readonly int _pixelsPerStringHalf;

        TimeSpan _prevBarTime = TimeSpan.Zero;
        float _prevBeatMarkerPosition = 0;
        bool _firstBeatMarkerHasBeenAdded = false;

        private static string GetTuningName(TuningStrings tuning, bool isBass, int capo, bool inBem = true)
        {
            List<Int32> Notes = new List<Int32>();
            List<String> NoteNames = new List<String>();
            for (Byte s = 0; s < (isBass ? 4 : 6); s++)
                Notes.Add(Sng2014FileWriter.GetMidiNote(tuning.ToShortArray(), s, 0, isBass, capo));
            foreach (var mNote in Notes)
                if (inBem) NoteNames.Add(_notesNamesHi[mNote % 12]); //oct = mNote / 12 - 1
                else NoteNames.Add(_notesNames[mNote % 12]); //oct = mNote / 12 - 1

            return String.Join(" ", NoteNames);
        }

        private  string GetNoteName(byte stringNo, sbyte fretNo)
        {
            var midiNote =Sng2014FileWriter.GetMidiNote(_track.Tuning, stringNo, (byte)fretNo, false, _track.Capo);
            var noteName = _notesNames[midiNote % 12];
            return noteName;
        }

        public InstrumentTrackRenderer(Canvas canvas, SimplifiedTrack track, int yOffset, int barHeight, int barWidth)
        {
            _canvas = canvas;
            _yOffset = yOffset;
            _track = track;
            _barHeight = barHeight;
            _barWidth = barWidth;

            _pixelsPerStringHalf = (_barHeight - _yOffsetStringFromTrackTop) / (_track.NumberOfStrings * 2);
        }


        public void DoRender()
        {
            DrawStrings(_yOffset + _yOffsetStringFromTrackTop, _barHeight - _yOffsetStringFromTrackTop);

            var xOffset = 0;
            int barId = 1;
            foreach (var bar in _track.Bars)
            {
                var rect = new Rectangle()
                {
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 2,
                    Width = _barWidth,
                    Height = _barHeight - 25
                };
                Canvas.SetTop(rect, _yOffset + 25);
                Canvas.SetLeft(rect, xOffset);
                _canvas.Children.Add(rect);

                var tb = new TextBlock() { Text = barId.ToString(), Foreground = Brushes.White };
                Canvas.SetLeft(tb, xOffset);
                Canvas.SetTop(tb, _yOffset);
                _canvas.Children.Add(tb);

                DrawBar(bar, xOffset, _yOffsetStringFromTrackTop);
                xOffset += _barWidth;
                barId++;
            }
        }

        void DrawBar(SimplifiedBar bar, int xOffset, int yOffsetStringFromTrackTop)
        {
            var rsDuration = bar.EndTime - bar.StartTime;
            var pixelsPerSec = _barWidth / rsDuration;

            //Draw beatmarkers
            foreach (var ebeat in bar.EBeats)
            {
                var rsOffset = ebeat.Time - bar.StartTime;
                var xPosPixels = xOffset + (pixelsPerSec * rsOffset);

                var line = new Line() { X1 = xPosPixels, X2 = xPosPixels, Y1 = _yOffset, Y2 = _yOffset + _barHeight, Stroke = Brushes.LightGray, StrokeThickness = 2 };
                _canvas.Children.Add(line);
                var tsStart = TimeSpan.FromSeconds(ebeat.Time);
                var timeText = tsStart.ToString(@"m\:ss\.fff");
                if (_prevBarTime != TimeSpan.Zero)
                {
                    var diff = tsStart.Subtract(_prevBarTime);
                    timeText += " (+" + diff.ToString(@"m\:ss\.fff") + ")";
                }
                _prevBarTime = tsStart;

                var tb = new TextBlock() { Text = timeText, Foreground = Brushes.White };
                Canvas.SetLeft(tb, xPosPixels);
                Canvas.SetTop(tb, _yOffset + 10);
                _canvas.Children.Add(tb);
                if (_firstBeatMarkerHasBeenAdded)
                {
                    var xPosPixels2 = _prevBeatMarkerPosition + ((xPosPixels - _prevBeatMarkerPosition) / 2);
                    var line2 = new Line() { X1 = xPosPixels2, X2 = xPosPixels2, Y1 = _yOffset, Y2 = _yOffset + _barHeight, Stroke = Brushes.Gray, StrokeThickness = 1 };
                    _canvas.Children.Add(line2);
                }
                _prevBeatMarkerPosition = xPosPixels;
                _firstBeatMarkerHasBeenAdded = true;
            }

            //Draw notes in bar
            foreach (var note in bar.Notes)
            {
                RenderSingleNote(bar, xOffset, pixelsPerSec, note);

                var rsChordOffsetFromBarStart = note.Time - bar.StartTime;
                var noteName = GetNoteName(note.String, note.Fret);
                var tb = new TextBlock() { Text = noteName, Foreground = Brushes.LightSteelBlue };
                Canvas.SetLeft(tb, xOffset + (rsChordOffsetFromBarStart * pixelsPerSec));
                Canvas.SetTop(tb, _yOffset + _barHeight);
                _canvas.Children.Add(tb);
            }


            //Draw chords in bar
            foreach (var chord in bar.Chords)
            {
                var ct = _track.ChordTemplates[chord.ChordId];

                RenderNoteFromTemplateIfNotInChord(bar, xOffset, pixelsPerSec, chord, 0, ct.Fret0);
                RenderNoteFromTemplateIfNotInChord(bar, xOffset, pixelsPerSec, chord, 1, ct.Fret1);
                RenderNoteFromTemplateIfNotInChord(bar, xOffset, pixelsPerSec, chord, 2, ct.Fret2);
                RenderNoteFromTemplateIfNotInChord(bar, xOffset, pixelsPerSec, chord, 3, ct.Fret3);
                RenderNoteFromTemplateIfNotInChord(bar, xOffset, pixelsPerSec, chord, 4, ct.Fret4);
                RenderNoteFromTemplateIfNotInChord(bar, xOffset, pixelsPerSec, chord, 5, ct.Fret5);

                if (chord.ChordNotes != null)
                {
                    foreach (var chordnote in chord.ChordNotes)
                        RenderSingleNote(bar, xOffset, pixelsPerSec, chordnote);
                }

                //Draw ChordName
                if (!string.IsNullOrEmpty(ct.DisplayName))
                {
                    var rsChordOffsetFromBarStart = chord.Time - bar.StartTime;
                    var tb = new TextBlock() { Text = ct.DisplayName, Foreground = Brushes.LightSteelBlue };
                    Canvas.SetLeft(tb, xOffset + (rsChordOffsetFromBarStart * pixelsPerSec));
                    Canvas.SetTop(tb, _yOffset + _barHeight);
                    _canvas.Children.Add(tb);
                }
            }
        }

        private void RenderNoteFromTemplateIfNotInChord(SimplifiedBar bar, int xOffset, float pixelsPerSec, SongChord2014 chord, byte stringIndex, sbyte fretIndex)
        {
            if (fretIndex > -1)
            {
                var specificNote = chord.ChordNotes == null ? null : chord.ChordNotes.FirstOrDefault(x => x.String == stringIndex);
                if (specificNote == null)
                {
                    var dummyNote = new SongNote2014()
                    {
                        String = stringIndex,
                        Time = chord.Time,
                        SlideUnpitchTo=-1,
                        SlideTo=-1,
                        LeftHand=-1,
                        Pluck=-1,
                        RightHand=-1,
                        Slap=-1,
                        Fret = fretIndex
                    };
                    RenderSingleNote(bar, xOffset, pixelsPerSec, dummyNote);
                }
            }
        }

        readonly Dictionary<int, Line> _slidesInProgress = new Dictionary<int, Line>();

        private void RenderSingleNote(SimplifiedBar bar, int xOffset, float pixelsPerSec, SongNote2014 note)
        {
            var noteText = note.Fret.ToString();

            if (note.HammerOn > 0 || note.PullOff > 0)
            {
                noteText = "(" + noteText + ")";
            }
            var rsNoteOffsetFromBarStart = note.Time - bar.StartTime;
            var noteOffsetXAbsolute = xOffset + (rsNoteOffsetFromBarStart * pixelsPerSec);
            var noteOffsetYAbsolute = _yOffset + _yOffsetStringFromTrackTop + (_pixelsPerStringHalf * ((_track.NumberOfStrings - 1 - note.String) * 2));

            //Slide started from previous note
            Line slideLineToAdd = null;
            if(_slidesInProgress.ContainsKey(note.String))
            {
                slideLineToAdd = _slidesInProgress[note.String];
                slideLineToAdd.X2 = noteOffsetXAbsolute + 8;
                _slidesInProgress.Remove(note.String);
            }

            Border border = null;
            if(note.SlideTo>-1  && note.LinkNext==1)
            {
                border = RocksmithRenderHelper.CreateNoteElement(_pixelsPerStringHalf, note.String, noteText);
                var line = new Line()
                {
                    Stroke= RocksmithRenderHelper.GetFontColorForString(note.String),
                    StrokeThickness=2,
                    X1 = noteOffsetXAbsolute + 8,
                    Y1 = noteOffsetYAbsolute + 4,
                    Y2 = noteOffsetYAbsolute + 16,
                };

                //Slide down instead of up
                if(note.String>note.SlideTo)
                {
                    var lowPoint = line.Y2;
                    line.Y1 = line.Y2;
                    line.Y2 = lowPoint;
                }

                _slidesInProgress.Add(note.String, line);
            }
            else if(note.SlideUnpitchTo>-1)
            {
                border = RocksmithRenderHelper.CreateUnpitchedSlideNotes(_pixelsPerStringHalf, note.String, note.Fret, note.SlideUnpitchTo);
            }
            else
            {
                border = RocksmithRenderHelper.CreateNoteElement(_pixelsPerStringHalf, note.String, noteText);
            }

            if (note.LinkNext!=0)
            {

            }

            if (note.Sustain > 0)
                border.Width = note.Sustain * pixelsPerSec;

            Canvas.SetLeft(border, noteOffsetXAbsolute);
            Canvas.SetTop(border, noteOffsetYAbsolute);      //String position
            _canvas.Children.Add(border);

            //Add this after the notes have been added, to appear in front
            if (slideLineToAdd!=null)
            {
                var adjustMent = border.Height - 20;
                slideLineToAdd.Y1 += (adjustMent / 2);
                slideLineToAdd.Y2 +=( adjustMent / 2); 
                _canvas.Children.Add(slideLineToAdd);
            }

        }

        private void DrawStrings(int trackStartY, int height)
        {
            var y = trackStartY + height - _pixelsPerStringHalf;
            for (int i = 0; i < _track.NumberOfStrings; i++)
            {
                var line = new Line() { X1 = 0, X2 = _canvas.Width, Y1 = y, Y2 = y, Stroke = RocksmithRenderHelper.GetStringColor(i), StrokeThickness = 4 };
                _canvas.Children.Add(line);
                y -= (_pixelsPerStringHalf * 2);
            }
        }

    }
}
