// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using Id3Lib.Frames;
using JetBrains.Annotations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Id3Lib
{
    /// <summary>
    /// Reduce the complexity the tag model to a simple interface
    /// </summary>
    [PublicAPI]
    public class TagHandler
    {
        TextCode _textCode = TextCode.Ascii; // Default text code
        string _language = "eng"; // Default language

        /// <summary>
        /// provide access to wrapped FrameModel
        /// </summary>
        /// <remarks>
        /// it would be nice to remove this one day, and completely encapsulate a private FrameModel object
        /// </remarks>
        [NotNull]
        public TagModel FrameModel { get; set; }

        /// <summary>
        /// Get the title/song name/content description.
        /// Song is a synonym of the Title
        /// </summary>
        [CanBeNull]
        public string Song
        {
            get => Title;
            set => Title = value;
        }

        /// <summary>
        /// Get the title / song name / content description.
        /// </summary>
        [CanBeNull]
        public string Title
        {
            get => GetTextFrame("TIT2");
            set => SetTextFrame("TIT2", value);
        }

        /// <summary>
        /// Get the lead performer/soloist.
        /// </summary>
        [CanBeNull]
        public string Artist
        {
            get => GetTextFrame("TPE1");
            set => SetTextFrame("TPE1", value);
        }

        /// <summary>
        /// Get the album title.
        /// </summary>
        [CanBeNull]
        public string Album
        {
            get => GetTextFrame("TALB");
            set => SetTextFrame("TALB", value);
        }

        /// <summary>
        /// Get the production year.
        /// </summary>
        [CanBeNull]
        public string Year
        {
            get => GetTextFrame("TYER");
            set => SetTextFrame("TYER", value);
        }

        /// <summary>
        /// Get the composer.
        /// </summary>
        [CanBeNull]
        public string Composer
        {
            get => GetTextFrame("TCOM");
            set => SetTextFrame("TCOM", value);
        }

        /// <summary>
        /// Get the track genre.
        /// </summary>
        [CanBeNull]
        public string Genre
        {
            get => GetTextFrame("TCON");
            set => SetTextFrame("TCON", value);
        }

        /// <summary>
        /// Get the track number.
        /// </summary>
        [CanBeNull]
        public string Track
        {
            get => GetTextFrame("TRCK");
            set => SetTextFrame("TRCK", value);
        }

        /// <summary>
        /// Get the disc number.
        /// </summary>
        /// <remarks>
        /// The 'Part of a set' frame is a numeric string that describes which
        /// part of a set the audio came from. This frame is used if the source
        /// described in the "TALB" frame is divided into several mediums, e.g. a
        /// double CD. The value MAY be extended with a "/" character and a
        /// numeric string containing the total number of parts in the set. E.g.
        /// "1/2".
        /// </remarks>
        [CanBeNull]
        public string Disc
        {
            get => GetTextFrame("TPOS");
            set => SetTextFrame("TPOS", value);
        }

        /// <summary>
        /// Get the length.
        /// the length of the audio file in milliseconds, represented as a numeric string.
        /// </summary>
        public TimeSpan? Length
        {
            get
            {
                var strlen = GetTextFrame("TLEN");
                if (string.IsNullOrEmpty(strlen))
                    return null;

                // test for a simple number in the field
                if (int.TryParse(strlen, out var len))
                    return new TimeSpan(0, 0, 0, 0, len);
                return null;
            }
        }

        /// <summary>
        /// Get the original padding size.
        /// </summary>
        public uint PaddingSize => FrameModel.Header.PaddingSize;

        /// <summary>
        /// Get the lyrics.
        /// (technically: Un-synchronised lyrics/text transcription)
        /// </summary>
        [CanBeNull]
        public string Lyrics
        {
            get => GetFullTextFrame("USLT");
            set => SetFullTextFrame("USLT", value);
        }

        /// <summary>
        /// Get the track / artist comment.
        /// </summary>
        [CanBeNull]
        public string Comment
        {
            get => GetFullTextFrame("COMM");
            set => SetFullTextFrame("COMM", value);
        }

        /// <summary>
        /// Get/Set the associated picture, or null reference
        /// </summary>
        [CanBeNull]
        public Image<Rgba32> Picture
        {
            get
            {
                var frame = FindFrame("APIC") as FramePicture;
                return frame?.Picture;
            }
            set
            {
                if (FindFrame("APIC") is FramePicture frame)
                {
                    if (value != null)
                        frame.Picture = value;
                    else
                        FrameModel.Remove(frame);
                }
                else
                {
                    if (value == null) return;

                    if (FrameFactory.Build("APIC") is FramePicture framePic)
                    {
                        framePic.Picture = value;
                        FrameModel.Add(framePic);
                    }
                }
            }
        }

        /// <summary>
        /// Set the frame text
        /// </summary>
        /// <param name="frameId">Frame type</param>
        /// <param name="message">Value set in frame</param>
        void SetTextFrame([NotNull] string frameId, [CanBeNull] string message)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                if (!string.IsNullOrEmpty(message))
                    ((FrameText) frame).Text = message;
                else
                    FrameModel.Remove(frame);
            }
            else
            {
                if (string.IsNullOrEmpty(message)) return;

                var frameText = (FrameText) FrameFactory.Build(frameId);
                frameText.Text = message;
                frameText.TextCode = _textCode;
                FrameModel.Add(frameText);
            }
        }

        /// <summary>
        /// Get the frame text
        /// </summary>
        /// <param name="frameId">Frame type</param>
        /// <returns>Frame text</returns>
        [NotNull]
        string GetTextFrame([NotNull] string frameId)
        {
            var frame = FindFrame(frameId);
            return frame != null ? ((FrameText) frame).Text : string.Empty;
        }

        /// <summary>
        /// Set the frame full text
        /// </summary>
        /// <param name="frameId">Frame type</param>
        /// <param name="message">Value set in frame</param>
        void SetFullTextFrame([NotNull] string frameId, [CanBeNull] string message)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    var framefulltext = (FrameFullText) frame;
                    framefulltext.Text = message;
                    framefulltext.TextCode = _textCode;
                    framefulltext.Description = string.Empty;
                    framefulltext.Language = _language;
                }
                else
                    FrameModel.Remove(frame);
            }
            else
            {
                if (string.IsNullOrEmpty(message)) return;

                var frameLcText = (FrameFullText) FrameFactory.Build(frameId);
                frameLcText.TextCode = _textCode;
                frameLcText.Language = "eng";
                frameLcText.Description = string.Empty;
                frameLcText.Text = message;
                FrameModel.Add(frameLcText);
            }
        }

        /// <summary>
        /// Get a full text frame value
        /// </summary>
        /// <param name="frameId">Frame type</param>
        /// <returns>Frame text</returns>
        [NotNull]
        string GetFullTextFrame([NotNull] string frameId)
        {
            var frame = FindFrame(frameId);
            return frame != null ? ((FrameFullText) frame).Text : string.Empty;
        }

        /// <summary>
        /// Find a frame in the model
        /// </summary>
        /// <param name="frameId">Frame type</param>
        /// <returns>The found frame if found, otherwise null</returns>
        [CanBeNull]
        FrameBase FindFrame([NotNull] string frameId)
        {
            foreach (var frame in FrameModel)
                if (frame.FrameId == frameId)
                    return frame;
            return null;
        }

        /// <summary>
        /// Attach to the TagModel
        /// </summary>
        /// <param name="frameModel">Frame model to handle</param>
        public TagHandler([NotNull] TagModel frameModel)
        {
            FrameModel = frameModel;
        }
    }
}

