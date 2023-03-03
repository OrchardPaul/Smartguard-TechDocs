using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GadjIT_App.Shared.StaticObjects
{
    public static class LookUps{

        public class ChapterColour
        {
            public string ColourName { get; set; }
            public string ColourCode { get; set; }
        }

        public static List<ChapterColour> ListChapterColours { 
            get
            {
                List<ChapterColour> listChapterColours = new List<ChapterColour>
                {
                    new ChapterColour { ColourName = "", ColourCode = ""},
                    new ChapterColour { ColourName = "Grey", ColourCode = "#3F000000"},
                    new ChapterColour { ColourName = "Blue", ColourCode = "#3F0074FF"},
                    new ChapterColour { ColourName = "Pink", ColourCode = "#3FFD64EF"},
                    new ChapterColour { ColourName = "Peach", ColourCode = "#3FEA9C66"},
                    new ChapterColour { ColourName = "Yellow", ColourCode = "#3FFFFF00"},
                    new ChapterColour { ColourName = "Beige", ColourCode = "#3F957625"},
                    new ChapterColour { ColourName = "Lilac", ColourCode = "#3F6E6FDB"},
                    new ChapterColour { ColourName = "Green", ColourCode = "#3F32EC29"},
                    new ChapterColour { ColourName = "Aqua", ColourCode = "#3F5BDCD0"}
                };


                return listChapterColours;
            }
        }


        public static string getHTMLColourFromAndroid(string colAndroid)
        {
            string colHTML = "#FFFFFFFF";

            try
            {

                if (!string.IsNullOrEmpty(colAndroid) && (Regex.IsMatch(colAndroid, "^#(?:[0-9a-fA-F]{8})$")))
                {
                    colHTML = "#" + colAndroid.Substring(3, 6) + colAndroid.Substring(1, 2);
                }
                else
                {
                    colHTML = "#FFFFFFFF";
                }

            }
            catch(Exception)
            {
                colHTML = "#FFFFFFFF";
            }

            return colHTML;
        }
    }

    public enum FileStorageType
    {
        
        BackupsSystem = 1
        , BackupsCaseTypeGroup = 2
        , BackupsCaseType = 3
        , BackupsSmartflow = 4
        , BackgroundImages = 20
        , TempUploads = 30
        
    }

}