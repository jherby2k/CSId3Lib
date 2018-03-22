// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using Id3Lib.Exceptions;
using Id3Lib.Frames;
using JetBrains.Annotations;

namespace Id3Lib
{
    /// <summary>
    /// Manage ID3v1 frames
    /// </summary>
    /// <remarks>
    /// The <b>ID3v1</b> class can read an ID3v1 frame form a mp3 file returning the <see cref="FrameModel"/> and
    /// write an ID3v1 form the FrameModel to a mp3 file, it will ignore any fields not supported in ID3v1 tag format.
    /// </remarks>
    [PublicAPI]
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification = "It is the name of the standard"),
     SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "v")]
    public class ID3v1
    {
        [NotNull] static readonly byte[] _id3 = { 0x54, 0x41, 0x47 }; //"TAG"

        [NotNull] static readonly string[] _genres =
        {
            "Blues", "Classic Rock", "Country", "Dance", "Disco", "Funk", "Grunge", "Hip-Hop", "Jazz", "Metal",
            "New Age", "Oldies", "Other", "Pop", "R&B", "Rap", "Reggae", "Rock", "Techno", "Industrial",
            "Alternative", "Ska", "Death Metal", "Pranks", "Soundtrack", "Euro-Techno", "Ambient", "Trip-Hop",
            "Vocal", "Jazz+Funk", "Fusion", "Trance", "Classical", "Instrumental", "Acid", "House",
            "Game", "Sound Clip", "Gospel", "Noise", "Alternative Rock", "Bass", "Soul", "Punk", "Space",
            "Meditative", "Instrumental Pop", "Instrumental Rock", "Ethnic", "Gothic",
            "Darkwave", "Techno-Industrial", "Electronic", "Pop-Folk", "Eurodance", "Dream",
            "Southern Rock", "Comedy", "Cult", "Gangsta", "Top 40", "Christian Rap", "Pop/Funk", "Jungle",
            "Native American", "Cabaret", "New Wave", "Psychadelic", "Rave", "Showtunes", "Trailer", "Lo-Fi",
            "Tribal", "Acid Punk", "Acid Jazz", "Polka", "Retro", "Musical", "Rock & Roll", "Hard Rock", "Folk",
            "Folk/Rock", "National Folk", "Swing", "Fast-Fusion", "Bebob", "Latin", "Revival", "Celtic", "Bluegrass",
            "Avantgarde", "Gothic Rock", "Progressive Rock", "Psychedelic Rock", "Symphonic Rock", "Slow Rock",
            "Big Band", "Chorus", "Easy Listening", "Acoustic", "Humour", "Speech", "Chanson", "Opera", "Chamber Music",
            "Sonata", "Symphony", "Booty Bass", "Primus", "Porn Groove", "Satire", "Slow Jam", "Club",
            "Tango", "Samba", "Folklore", "Ballad", "Power Ballad", "Rhytmic Soul", "Freestyle", "Duet",
            "Punk Rock", "Drum Solo", "Acapella", "Euro-House", "Dance Hall", "Goa", "Drum & Bass", "Club-House",
            "Hardcore", "Terror", "Indie", "BritPop", "Negerpunk", "Polsk Punk", "Beat", "Christian Gangsta Rap",
            "Heavy Metal", "Black Metal", "Crossover", "Contemporary Christian",
            "Christian Rock", "Merengue", "Salsa", "Trash Metal", "Anime", "JPop", "SynthPop"
        };

        /// <summary>
        /// ID3v1 tag length
        /// </summary>
        public static uint TagLength => 128;

        byte _track;

        /// <summary>
        /// Get the title/song name/content description.
        /// </summary>
        [CanBeNull]
        public string Song { get; private set; }

        /// <summary>
        /// Get the lead performer/soloist.
        /// </summary>
        [CanBeNull]
        public string Artist { get; private set; }

        /// <summary>
        /// Get the production year.
        /// </summary>
        [CanBeNull]
        public string Year { get; private set; }

        /// <summary>
        /// Get the album title.
        /// </summary>
        [CanBeNull]
        public string Album { get; private set; }

        /// <summary>
        /// Get the track/artist comment.
        /// </summary>
        [CanBeNull]
        public string Comment { get; private set; }

        /// <summary>
        /// Get the track number.
        /// </summary>
        public byte Track => _track;

        /// <summary>
        /// Get the track genre.
        /// </summary>
        public byte Genre { get; private set; }

        /// <summary>
        /// Get or set the ID3v2 FrameModel.
        /// </summary>
        [NotNull]
        public TagModel FrameModel
        {
            get => GetFrameModel();
            set => SetFrameModel(value);
        }

        /// <summary>
        /// ID3v1 tag manager
        /// </summary>
        public ID3v1()
        {
            Clear();
        }

        /// <summary>
        /// Load tag from stream
        /// </summary>
        /// <param name="stream">Binary stream to load</param>
        public void Deserialize([NotNull] Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1252); // Should be ASCII

                // check for ID3v1 tag
                reader.BaseStream.Seek(-128, SeekOrigin.End);

                var idTag = new byte[3];

                // Read the tag identifier
                reader.Read(idTag, 0, 3);

                // Compare the read tag
                if (Memory.Compare(_id3, idTag) != true)
                    throw new TagNotFoundException("ID3v1 tag was not found");

                var tag = new byte[30]; // Allocate ID3 tag

                reader.Read(tag, 0, 30);
                Song = GetString(encoding, tag);

                reader.Read(tag, 0, 30);
                Artist = GetString(encoding, tag);

                reader.Read(tag, 0, 30);
                Album = GetString(encoding, tag);

                reader.Read(tag, 0, 4);
                Year = tag[0] != 0 && tag[1] != 0 && tag[2] != 0 && tag[3] != 0
                    ? encoding.GetString(tag, 0, 4)
                    : string.Empty;

                reader.Read(tag, 0, 30);
                if (tag[28] == 0) //Track number was stored at position 29 later hack of the original standard.
                {
                    _track = tag[29];
                    Comment = encoding.GetString(tag, 0, Memory.FindByte(tag, 0x00, 0));
                }
                else
                {
                    _track = 0;
                    Comment = GetString(encoding, tag);
                }

                Genre = reader.ReadByte();
            }
        }

        /// <summary>
        /// Save tag from stream
        /// </summary>
        /// <param name="stream">Binary stream to save</param>
        public void Serialize([NotNull] Stream stream)
        {
            var idTag = new byte[3];

            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                // check for ID3v1 tag
                reader.BaseStream.Seek(-128, SeekOrigin.End);

                // Read the tag identifier
                reader.Read(idTag, 0, 3);

                // Is there a ID3v1 tag already?
                if (Memory.Compare(_id3, idTag))
                {
                    //Found a ID3 tag so we will over write the old tag
                    // (and the 'TAG' label too, so WriteAtStreamPosition is clean)
                    stream.Seek(-128, SeekOrigin.End);

                    Write(stream);
                }
                else
                {
                    //Create a new Tag
                    var position = stream.Position;
                    stream.Seek(0, SeekOrigin.End);
                    //TODO: fix this ugly code, at first make the error visible to the user by some means,
                    // a start can be throwing the exception in an inner exception 
                    try
                    {
                        Write(stream);
                    }
                    catch (Exception)
                    {
                        // There was an error while creating the tag
                        // Restore the file to the original state, I hope.
                        stream.SetLength(position);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// overwrite or append ID3v1 tag at current location in the stream
        /// </summary>
        /// <param name="stream"></param>
        public void Write([NotNull] Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1252); // Should be ASCII

                writer.Write(_id3, 0, 3); // Write the ID3 TAG ID: 'TAG'

                var tag = new byte[30];

                if (Song != null)
                {
                    if (Song.Length > 30)
                        Song = Song.Substring(0, 30);
                    // ReSharper disable once PossibleNullReferenceException
                    encoding.GetBytes(Song, 0, Song.Length, tag, 0);
                }
                writer.Write(tag, 0, 30);

                Memory.Clear(tag, 0, 30);
                if (Artist != null)
                {
                    if (Artist.Length > 30)
                        Artist = Artist.Substring(0, 30);
                    // ReSharper disable once PossibleNullReferenceException
                    encoding.GetBytes(Artist, 0, Artist.Length, tag, 0);
                }
                writer.Write(tag, 0, 30);

                Memory.Clear(tag, 0, 30);
                if (Album != null)
                {
                    if (Album.Length > 30)
                        Album = Album.Substring(0, 30);
                    // ReSharper disable once PossibleNullReferenceException
                    encoding.GetBytes(Album, 0, Album.Length, tag, 0);
                }
                writer.Write(tag, 0, 30);

                Memory.Clear(tag, 0, 30);
                if (!string.IsNullOrEmpty(Year))
                {
                    if (!ushort.TryParse(Year, out var year) || year > 9999)
                        Year = "0";
                    // ReSharper disable once PossibleNullReferenceException
                    encoding.GetBytes(Year, 0, Year.Length, tag, 0);
                }
                writer.Write(tag, 0, 4);

                Memory.Clear(tag, 0, 30);
                if (Comment != null)
                {
                    if (Comment.Length > 28)
                        Comment = Comment.Substring(0, 28);
                    // ReSharper disable once PossibleNullReferenceException
                    encoding.GetBytes(Comment, 0, Comment.Length, tag, 0);
                }
                writer.Write(tag, 0, 28);

                writer.Write((byte) 0);
                writer.Write(_track);
                writer.Write(Genre);
            }
        }

        [Pure, NotNull]
        TagModel GetFrameModel()
        {
            var frameModel = new TagModel
            {
                new FrameText("TIT2")
                {
                    TextCode = TextCode.Ascii,
                    Text = Song
                },
                new FrameText("TPE1")
                {
                    TextCode = TextCode.Ascii,
                    Text = Artist
                },
                new FrameText("TALB")
                {
                    TextCode = TextCode.Ascii,
                    Text = Album
                },
                new FrameText("TYER")
                {
                    TextCode = TextCode.Ascii,
                    Text = Year
                },
                new FrameText("TRCK")
                {
                    TextCode = TextCode.Ascii,
                    Text = _track.ToString(CultureInfo.InvariantCulture)
                },
                new FrameFullText("COMM")
                {
                    TextCode = TextCode.Ascii,
                    Language = "eng",
                    Description = "",
                    Text = Comment
                }
            };

            if (Genre < _genres.Length)
                frameModel.Add(new FrameText("TCON")
                {
                    TextCode = TextCode.Ascii,
                    Text = _genres[Genre]
                });

            //TODO: Fix this code!!!!!!!!
            frameModel.Header.TagSize = 0;    //TODO: Invalid size, not filled in until write
            frameModel.Header.Version = 3;    // ID3v2.[3].[0]
            frameModel.Header.Revision = 0;
            frameModel.Header.Unsync = false;
            frameModel.Header.Experimental = false;
            frameModel.Header.Footer = false;
            frameModel.Header.ExtendedHeader = false;

            return frameModel;
        }

        void SetFrameModel([NotNull] TagModel frameModel)
        {
            Clear();

            foreach (var frame in frameModel)
            {
                switch (frame.FrameId)
                {
                    case "TIT2":
                        Song = GetTagText(frame);
                        break;
                    case "TPE1":
                        Artist = GetTagText(frame);
                        break;
                    case "TALB":
                        Album = GetTagText(frame);
                        break;
                    case "TYER":
                        Year = GetTagText(frame);
                        break;
                    case "TRCK":
                        if (!byte.TryParse(GetTagText(frame), out _track))
                            _track = 0;
                        break;
                    case "TCON":
                        Genre = ParseGenre(GetTagText(frame));
                        break;
                    case "COMM":
                        Comment = GetTagText(frame);
                        break;
                }
            }
        }

        void Clear()
        {
            Song = "";
            Artist = "";
            Album = "";
            Year = "";
            Comment = "";
            _track = 0;
            Genre = 255;
        }

        [Pure, CanBeNull]
        static string GetTagText([NotNull] FrameBase tag)
        {
            switch (tag)
            {
                case FrameText frameText:
                    return frameText.Text;
                case FrameFullText frameFullText:
                    return frameFullText.Text;
                default:
                    return null;
            }
        }

        [Pure]
        static byte ParseGenre([CanBeNull] string sGenre)
        {
            if (string.IsNullOrEmpty(sGenre)) return 255;

            // test for a simple number in the field
            if (byte.TryParse(sGenre, out var nGenre))
                return nGenre;

            // "References to the ID3v1 genres can be made by, as first byte, 
            // enter "(" followed by a number from the genres list (appendix A) 
            // and ended with a ")" character."
            // ReSharper disable once PossibleNullReferenceException
            if (sGenre[0] == '(' && sGenre[1] != '(')
            {
                var close = sGenre.IndexOf(')');
                if (close != -1 && byte.TryParse(sGenre.Substring(1, close - 1), out nGenre))
                    return nGenre;
            }

            // not a number, see if it's one of the predefined genre name strings
            for (byte index = 0; index < _genres.Length; index++)
                if (sGenre.Equals(_genres[index].Trim(), StringComparison.OrdinalIgnoreCase))
                    return index;

            return 12; // "Other"
        }

        [NotNull]
        static string GetString([NotNull] Encoding encoding, [NotNull] byte[] tag)
        {
            // I had to use Memory.FindByte function because the encoding function
            // retrieves the 0x00 values and doesn't stop on the first zero. so makes strings with
            // many trailing zeros at the end when converted to XML these are added in binary text.
            var index = Memory.FindByte(tag, 0x00, 0);
            return index < 0 ? encoding.GetString(tag) : encoding.GetString(tag, 0, index);
        }

    }
}

