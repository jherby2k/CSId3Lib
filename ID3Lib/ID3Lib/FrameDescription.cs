// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace Id3Lib
{
    /// <summary>
    /// Helper class to get a description of the frame identifiers.
    /// </summary>
    public static class FrameDescription
    {
        #region Fields
        /// <summary>
        /// Keep a relation between frame Frames and descriptions of them
        /// </summary>
        private static Dictionary<string, string> _descriptions = Intitalize();
        #endregion

        #region Methods
        /// <summary>
        /// Obtain a human description of a frame
        /// </summary>
        /// <param name="frameId">the four character frame id</param>
        /// <returns>description of the tag</returns>
        public static string GetDescription(string frameId)
        {
            string description;
            if (_descriptions.TryGetValue(frameId, out description))
                return description;
            return "Unknown tag";

        }

        /// <summary>
        /// Fill the hash with the frame descriptors
        /// </summary>
        private static Dictionary<string, string> Intitalize()
        {
            var descriptions = new Dictionary<string, string>();

            // ID3v2.3
            descriptions.Add("TYER", "Recording Year");
            // ID3v2.4
            descriptions.Add("AENC", "Audio encryption");
            descriptions.Add("APIC", "Attached picture");
            descriptions.Add("ASPI", "Audio seek point index");

            descriptions.Add("COMM", "Comments");
            descriptions.Add("COMR", "Commercial frame");

            descriptions.Add("ENCR", "Encryption method registration");
            descriptions.Add("EQU2", "Equalisation");
            descriptions.Add("ETCO", "Event timing codes");

            descriptions.Add("GEOB", "General encapsulated object");
            descriptions.Add("GRID", "Group identification registration");

            descriptions.Add("LINK", "Linked information");

            descriptions.Add("MCDI", "Music CD identifier");
            descriptions.Add("MLLT", "MPEG location lookup table");

            descriptions.Add("OWNE", "Ownership frame");

            descriptions.Add("PRIV", "Private frame");
            descriptions.Add("PCNT", "Play counter");
            descriptions.Add("POPM", "Popularimeter");
            descriptions.Add("POSS", "Position synchronisation frame");

            descriptions.Add("RBUF", "Recommended buffer size");
            descriptions.Add("RVA2", "Relative volume adjustment");
            descriptions.Add("RVRB", "Reverb");

            descriptions.Add("SEEK", "Seek frame");
            descriptions.Add("SIGN", "Signature frame");
            descriptions.Add("SYLT", "Synchronised lyric/text");
            descriptions.Add("SYTC", "Synchronised tempo codes");

            descriptions.Add("TALB", "Album/Movie/Show title");
            descriptions.Add("TBPM", "Beats per minute)");
            descriptions.Add("TCOM", "Composer");
            descriptions.Add("TCON", "Content type");
            descriptions.Add("TCOP", "Copyright message");
            descriptions.Add("TDEN", "Encoding time");
            descriptions.Add("TDLY", "Playlist delay");
            descriptions.Add("TDOR", "Original release time");
            descriptions.Add("TDRC", "Recording time");
            descriptions.Add("TDRL", "Release time");
            descriptions.Add("TDTG", "Tagging time");
            descriptions.Add("TENC", "Encoded by");
            descriptions.Add("TEXT", "Lyricist/Text writer");
            descriptions.Add("TFLT", "File type");
            descriptions.Add("TIPL", "Involved people list");
            descriptions.Add("TIT1", "Content group description");
            descriptions.Add("TIT2", "Title/song name/content description");
            descriptions.Add("TIT3", "Subtitle/Description refinement");
            descriptions.Add("TKEY", "Initial key");
            descriptions.Add("TLAN", "Language(s)");
            descriptions.Add("TLEN", "Length");
            descriptions.Add("TMCL", "Musician credits list");
            descriptions.Add("TMED", "Media type");
            descriptions.Add("TMOO", "Mood");
            descriptions.Add("TOAL", "Original album/movie/show title");
            descriptions.Add("TOFN", "Original filename");
            descriptions.Add("TOLY", "Original lyricist(s)/text writer(s)");
            descriptions.Add("TOPE", "Original artist(s)/performer(s)");
            descriptions.Add("TOWN", "File owner/licensee");
            descriptions.Add("TPE1", "Lead performer(s)/Soloist(s)");
            descriptions.Add("TPE2", "Band/orchestra/accompaniment");
            descriptions.Add("TPE3", "Conductor/performer refinement");
            descriptions.Add("TPE4", "Interpreted, remixed, or otherwise modified by");
            descriptions.Add("TPOS", "Part of a set");
            descriptions.Add("TPRO", "Produced notice");
            descriptions.Add("TPUB", "Publisher");
            descriptions.Add("TRCK", "Track number/Position in set");
            descriptions.Add("TRSN", "Internet radio station name");
            descriptions.Add("TRSO", "Internet radio station owner");
            descriptions.Add("TSOA", "Album sort order");
            descriptions.Add("TSOP", "Performer sort order");
            descriptions.Add("TSOT", "Title sort order");
            descriptions.Add("TSRC", "ISRC (international standard recording code)");
            descriptions.Add("TSSE", "Software/Hardware and settings used for encoding");
            descriptions.Add("TSST", "Set subtitle");
            descriptions.Add("TXXX", "User defined text information frame");

            descriptions.Add("UFID", "Unique file identifier");
            descriptions.Add("USER", "Terms of use");
            descriptions.Add("USLT", "Unsynchronised lyric/text transcription");

            descriptions.Add("WCOM", "Commercial information");
            descriptions.Add("WCOP", "Copyright/Legal information");
            descriptions.Add("WOAF", "Official audio file webpage");
            descriptions.Add("WOAR", "Official artist/performer webpage");
            descriptions.Add("WOAS", "Official audio source webpage");
            descriptions.Add("WORS", "Official Internet radio station homepage");
            descriptions.Add("WPAY", "Payment");
            descriptions.Add("WPUB", "Publishers official webpage");
            descriptions.Add("WXXX", "User defined URL link frame");
            return descriptions;
        }
        #endregion
    }
}
