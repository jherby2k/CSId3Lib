// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Text;
using Id3Lib.Exceptions;
using Id3Lib.Frames;

namespace Id3Lib
{
    /// <summary>
    /// Handle the loading and saving of ID3 tags.
    /// </summary>
    /// <remarks>
    /// The <c>FrameManager</c> class manages the conversion of a ID3v2 tag from binary form 
    /// to a <see cref="TagManager"/> that can be manipulated and saved later again to
    /// a binary form.
    /// </remarks>
    public static class TagManager
    {
        #region Methods
        /// <summary>
        /// Load the ID3v2 frames to a binary stream
        /// </summary>
        /// <param name="stream">Binary stream holding the ID3 Tag</param>
        /// <returns>Model keeping the ID3 Tag structure</returns>
        public static TagModel Deserialize(Stream stream)
        {
            var frameModel = new TagModel();
            frameModel.Header.Deserialize(stream); // load the ID3v2 header
            if (frameModel.Header.Version != 3 & frameModel.Header.Version != 4)
                throw new NotImplementedException("ID3v2 Version " + frameModel.Header.Version + " is not supported.");

            uint id3TagSize = frameModel.Header.TagSize;

            if (frameModel.Header.Unsync == true)
            {
                var memory = new MemoryStream();
                id3TagSize -= Sync.Unsafe(stream, memory, id3TagSize);
                stream = memory; // This is now the stream
                if (id3TagSize <= 0)
                    throw new InvalidTagException("Data is missing after the header.");
            }
            uint rawSize;
            // load the extended header
            if (frameModel.Header.ExtendedHeader == true)
            {
                frameModel.ExtendedHeader.Deserialize(stream);
                rawSize = id3TagSize - frameModel.ExtendedHeader.Size;
                if (id3TagSize <= 0)
                    throw new InvalidTagException("Data is missing after the extended header.");
            }
            else
            {
                rawSize = id3TagSize;
            }

            // Read the frames
            if (rawSize <= 0)
                throw new InvalidTagException("No frames are present in the Tag, there must be at least one present.");

            // Load the tag frames
            uint index = 0;
            var frameHelper = new FrameHelper(frameModel.Header);
            // repeat while there is at least one complete frame available, 10 is the minimum size of a valid frame
            // but what happens when there's only, say, 5 bytes of padding?
            // we need to read a single byte to inspect for padding, then if not, read a whole tag.
            while (index < rawSize)
            {
                byte[] frameId = new byte[4];
                stream.Read(frameId, 0, 1);
                if (frameId[0] == 0)
                {
                    // We reached the padding area between the frames and the end of the tag, 
                    // signified by a zero byte where the frame name should be.

                    // we could double check we actually know what's going on
                    // and check the padding goes exactly to the end of the id3 tag
                    // but in fact it doesn't give us any benefit.
                    //
                    // one of the following cases must apply:
                    //
                    // 1) if the id3 tag specifies more bytes than the frames use up,
                    //    and that space is exactly filled with zeros to the first audio frame,
                    //    it complies with the standard and everything is happy.
                    //
                    // 2) if the id3 tag specifies more bytes than the frames use up,
                    //    and that space isn't completely filled with zeros,
                    //    we assume the software that generated the tag 
                    //    forgot to zero-fill it properly.
                    //
                    // 3) if the zero padding extends past the start of the id3 tag,
                    //    we assume the audio payload starts with skippable stuff too.
                    //
                    // 4) if the audio payload doesn't start with a valid mpeg audio frame header,
                    //    (VBR headers have valid mpeg audio frame headers)
                    //    we assume there's a tag in a format we don't recognise.
                    //    It still has to comply with the mpeg sync rules, 
                    //    so we will have to use that to find the start of the audio.
                    // 
                    // in all cases, we read the specified length of the id3 tag
                    // and let the higher-level processing inspect the audio payload
                    // to decide what is audio, what is extra padding, 
                    // and what is unrecognised (non-id3) tags.

                    // how much does the tag size say should be left?
                    frameModel.Header.PaddingSize = rawSize - index;

                    //// advance the stream past any zero bytes, 
                    //// and verify the real measured size against that specified in the tag
                    //uint observed = SeekEndOfPadding(src) + 1;
                    //if( frameModel.Header.PaddingSize != observed )
                    //    throw new InvalidPaddingException(observed, frameModel.Header.PaddingSize);

                    // advance the stream to the specified end of the tag
                    // this skips any non-zero rubbish in the padding without looking at it.
                    stream.Seek(frameModel.Header.PaddingSize - 1, SeekOrigin.Current);

                    break;
                }
                if (index + 10 > rawSize)
                {
                    // 10 is the minimum size of a valid frame;
                    // we read one already, if less than 9 chars left it's an error.
                    throw new InvalidTagException("Tag is corrupt, must be formed of complete frames.");
                }
                // read another 3 chars
                stream.Read(frameId, 1, 3);
                index += 4; // have read 4 bytes
                //TODO: Validate key valid ranges
                var reader = new BinaryReader(stream);
                uint frameSize = Swap.UInt32(reader.ReadUInt32());
                index += 4; // have read 4 bytes
                // ID3v2.4 now has sync-safe sizes
                if (frameModel.Header.Version == 4)
                    frameSize = Sync.Unsafe(frameSize);

                // The size of the frame can't be larger than the available space
                if (frameSize > rawSize - index)
                    throw new InvalidFrameException("A frame is corrupt, it can't be larger than the available space remaining.");

                ushort flags = Swap.UInt16(reader.ReadUInt16());
                index += 2; // read 2 bytes
                byte[] frameData = new byte[frameSize];
                reader.Read(frameData, 0, (int)frameSize);
                index += frameSize; // read more bytes
                frameModel.Add(frameHelper.Build(UTF8Encoding.UTF8.GetString(frameId, 0, 4), flags, frameData));
            }
            return frameModel;
        }

        /// <summary>
        /// Save the ID3v2 frames to a binary stream
        /// </summary>
        /// <param name="frameModel">Model keeping the ID3 Tag structure</param>
        /// <param name="stream">Stream keeping the ID3 Tag</param>
        public static void Serialize(TagModel frameModel, Stream stream)
        {
            if (frameModel.Count <= 0)
                throw new InvalidTagException("Can't serialize a ID3v2 tag without any frames, there must be at least one present.");

            var memory = new MemoryStream();
            var writer = new BinaryWriter(memory);

            var frameHelper = new FrameHelper(frameModel.Header);
            // Write the frames in binary format
            foreach (FrameBase frame in frameModel)
            {
                //TODO: Do validations on tag name correctness
                byte[] frameId = new byte[4];
                UTF8Encoding.UTF8.GetBytes(frame.FrameId, 0, 4, frameId, 0);
                writer.Write(frameId); // Write the 4 byte text tag
                ushort flags;
                byte[] buffer = frameHelper.Make(frame, out flags);
                uint frameSize = (uint)buffer.Length;

                if (frameModel.Header.Version == 4)
                    frameSize = Sync.Safe(frameSize);

                writer.Write(Swap.UInt32(frameSize));
                writer.Write(Swap.UInt16(flags));
                writer.Write(buffer);
            }

            uint id3TagSize = (uint)memory.Position;

            // Skip the header 10 bytes for now, we will come back and write the Header
            // with the correct size once have the tag size + padding
            stream.Seek(10, SeekOrigin.Begin);

            // TODO: Add extended header handling
            if (frameModel.Header.Unsync == true)
                id3TagSize += Sync.Safe(memory, stream, id3TagSize);
            else
                memory.WriteTo(stream);

            // update the TagSize stored in the tagModel
            frameModel.Header.TagSize = id3TagSize;

            // next write the padding of zeros, if any
            if (frameModel.Header.Padding)
            {
                for (int i = 0; i < frameModel.Header.PaddingSize; i++)
                    stream.WriteByte(0);
            }

            // next write the footer, if any
            if (frameModel.Header.Footer)
                frameModel.Header.SerializeFooter(stream);

            // Now seek back to the start and write the header
            long position = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            frameModel.Header.Serialize(stream);

            // reset position to the end of the tag
            stream.Position = position;
        }
        #endregion
    }
}
