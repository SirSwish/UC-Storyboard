using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UC_Storyboard.Core
{
    //Class is used basically as a look-up for Mission Briefing filenames. The games code is programmed to load a specific audio file based on the Mission ID loaded.
    //These are mapped out so that we can offer the ability to over-write mission briefing audio files with custom ones.
    public class AudioFileLookup
    {
        public Dictionary<int, string> idToFileMap;

        public AudioFileLookup()
        {
            // Initialize the dictionary and populate it with ID-audio file mappings
            idToFileMap = new Dictionary<int, string>
        {
            {6, "policem1.wav"},
            {9, "policem2.wav"},
            {12, "policem3.wav"},
            {14, "policem4.wav"},
            {15, "policem5.wav"},
            {16, "policem6.wav"},
            {17, "policem7.wav"},
            {18, "policem8.wav"},
            {19, "policem9.wav"},
            {20, "policem10.wav"},
            {21, "policem11.wav"},
            {22, "policem12.wav"},
            {23, "policem13.wav"},
            {24, "policem14.wav"},
            {25, "policem15.wav"},
            {26, "policem16.wav"},
            {27, "policem17.wav"},
            {28, "policem18.wav"},
            {29, "roperm19.wav"},
            {30, "roperm20.wav"},
            {31, "roperm21.wav"},
            {32, "roperm23.wav"},
            {33, "roperm24.wav"}
        };
        }

        //Returns a filename based on Mission ID that is passed from a calling function
        public string GetFileNameById(int id)
        {
            if (idToFileMap.TryGetValue(id, out string fileName))
            {
                return fileName;
            }

            // Default audio file name
            return "POLICEM.wav";
        }
    }
}
