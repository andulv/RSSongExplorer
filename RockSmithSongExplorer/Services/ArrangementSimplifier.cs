using RockSmithSongExplorer.Models;
using RocksmithToolkitLib.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockSmithSongExplorer.Services
{
    public class ArrangementSimplifier
    {
        public static SimplifiedTrack GetSimplifiedTrack(Song2014 songArrangement)
        {
            var allNotes = new List<SongNote2014>();
            var allChords = new List<SongChord2014>();

            // Rocksmith keeps its notes separated by the difficulty levels. Higher difficulty
            // levels only contain notes for phrases where the notes differ from lower levels.
            // This makes collection a little awkward, as we have to go phrase by phrase, 
            // to extract all the right notes.
            int maxDifficulty = 0;
            int difficultyLevel = int.MaxValue;
            for (int pit = 0; pit < songArrangement.PhraseIterations.Length; ++pit)
            {
                var phraseIteration = songArrangement.PhraseIterations[pit];
                var phrase = songArrangement.Phrases[phraseIteration.PhraseId];
                int difficulty = Math.Min(difficultyLevel, phrase.MaxDifficulty);
                var level = songArrangement.Levels.FirstOrDefault(x => x.Difficulty == difficulty);
                maxDifficulty = Math.Max(difficulty, maxDifficulty);
                float startTime = phraseIteration.Time;
                float endTime = float.MaxValue;
                if (pit < songArrangement.PhraseIterations.Length - 1)
                    endTime = songArrangement.PhraseIterations[pit + 1].Time;

                // gather single notes and chords inside this phrase iteration
                var notes_temp = from n in level.Notes
                                 where n.Time >= startTime && n.Time < endTime
                                 select n;
                var chords_temp = from c in level.Chords
                                  where c.Time >= startTime && c.Time < endTime
                                  select c;

                allNotes.AddRange(notes_temp);
                allChords.AddRange(chords_temp);
            }

            SimplifiedBar currentBar = null;
            List<SimplifiedBar> bars = new List<SimplifiedBar>();
            foreach (var beat in songArrangement.Ebeats)
            {
                if (currentBar == null || (beat.Measure > 0 && currentBar.MesureId != beat.Measure))
                {
                    if (currentBar != null)
                    {
                        currentBar.EndTime = beat.Time;
                        currentBar.Notes = allNotes.Where(x => x.Time >= currentBar.StartTime && x.Time < currentBar.EndTime).ToList();
                        currentBar.Chords = allChords.Where(x => x.Time >= currentBar.StartTime && x.Time < currentBar.EndTime).ToList();
                        bars.Add(currentBar);
                    }

                    currentBar = new SimplifiedBar();
                    currentBar.MesureId = beat.Measure;
                    currentBar.StartTime = beat.Time;
                }
                currentBar.EBeats.Add(beat);
            }
            SimplifiedTrack track = new SimplifiedTrack()
            {
                Bars = bars,
                ArrangementName = songArrangement.Arrangement,
                NumberOfStrings = GetNumberOfStrings(songArrangement),
                ChordTemplates = songArrangement.ChordTemplates.ToList()
            };
            return track;
        }

        private static int GetNumberOfStrings(Song2014 songArrangement)
        {
            bool isBass = songArrangement.Arrangement.ToLower() == "bass";
            return isBass ? 4 : 6;
        }
    }
}
