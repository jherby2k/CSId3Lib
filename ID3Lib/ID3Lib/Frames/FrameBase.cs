// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Text;
using Id3Lib.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Id3Lib.Frames
{

    /// <summary>
    /// Abstract base frame that provides common functionality to all the frames.
    /// </summary>
    public abstract class FrameBase
    {
        #region Fields
        private string _frameId;
        private byte? _group;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the tag alter flag</summary>
        /// <remarks>
        /// This flag tells the tag parser what to do with this frame if it is
        /// unknown and the tag is altered in any way.																												the frames.
        /// </remarks>
        public bool TagAlter { get; set; }

        /// <summary>
        /// Get or set the file alter flag
        /// </summary>
        /// <remarks>
        /// This flag tells the tag parser what to do with this frame if it is
        /// unknown and the file, excluding the tag, is altered.
        /// </remarks>
        public bool FileAlter { get; set; }

        /// <summary>
        /// Get or set the read only flag
        /// </summary>
        /// <remarks>
        /// This flag, if set, tells the software that the contents of this
        /// frame are intended to be read only.
        /// </remarks>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Get or set the compression flag.
        /// </summary>
        /// <remarks>
        /// This flag indicates whether or not the frame is compressed.
        /// </remarks>
        public bool Compression { get; set; }

        /// <summary>
        /// Get or set the encryption flag.
        /// </summary>
        /// <remarks>
        /// This flag indicates whether or not the frame is encrypted.
        /// </remarks>
        public bool Encryption { get; set; }

        /// <summary>
        /// Get or set the un-synchronisation flag.
        /// </summary>
        /// <remarks>
        /// This flag indicates whether or not un-synchronisation was applied to this frame.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Un-synchronisation")]
        public bool Unsynchronisation { get; set; }

        /// <summary>
        /// Get or set the data length.
        /// </summary>
        /// <remarks>
        /// This flag indicates that a data length indicator has been added to the frame.
        /// </remarks>
        public bool DataLength { get; set; }

        /// <summary>
        /// Get or set the group, if undefined there is no grouping enabled. 
        /// </summary> 
        public byte? Group
        {
            get { return _group; }
            set { _group = value; }
        }

        /// <summary>
        /// ID3 Frame Id frame type
        /// </summary>
        public string FrameId
        {
            get { return _frameId; }
        }
        #endregion

        #region Constructor
        internal FrameBase(string frameId)
        {
            if (frameId == null)
                throw new ArgumentNullException("frameId");

            if (frameId.Length != 4)
                throw new InvalidTagException("Invalid frame type: '" + frameId + "', it must be 4 characters long.");

            _frameId = frameId;
        }
        #endregion


        #region Methods

        /// <summary>
        /// Load frame form binary data
        /// </summary>
        /// <param name="frame">binary frame representation</param>
        public abstract void Parse(byte[] frame);
        /// <summary>
        /// Save frame to binary data
        /// </summary>
        /// <returns>binary frame representation</returns>
        public abstract byte[] Make();
        #endregion
    }
}
