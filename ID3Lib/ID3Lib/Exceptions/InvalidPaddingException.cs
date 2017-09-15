// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Globalization;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Id3Lib.Exceptions
{
    /// <summary>
    /// The exception is thrown when the amount of padding 
    /// doesn't match the space left over at the end of the ID3V2 tag.
    /// </summary>
    [Serializable]
    public class InvalidPaddingException : InvalidStructureException
    {
        uint _measured;
        uint _specified;

        protected InvalidPaddingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _measured = info.GetUInt32("measured");
            _specified = info.GetUInt32("specified");
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("measured", _measured);
            info.AddValue("specified", _specified);
            base.GetObjectData(info, context);
        }


        /// <summary>
        /// 
        /// </summary>
        public InvalidPaddingException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InvalidPaddingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public InvalidPaddingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// the number of zero bytes actually found between the last frame in the id3v2 tag, and the first non-zero byte of audio.
        /// </summary>
        public uint Measured
        {
            get { return _measured; }
        }

        /// <summary>
        /// the amount of space between the last frame in the id3v2 tag, and the specified end of the tag block.
        /// </summary>
        public uint Specified
        {
            get { return _specified; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measured"></param>
        /// <param name="specified"></param>
        public InvalidPaddingException(uint measured, uint specified)
            : base("Padding is corrupt, must be zeroes to end of id3 tag.")
        {
            Debug.Assert(measured != specified);
            _measured = measured;
            _specified = specified;
        }

        /// <summary>
        /// overrides default message with a specific "Padding is corrupt" one
        /// </summary>
        public override string Message
        {
            get
            {
                if (_measured > _specified)
                    return string.Format(CultureInfo.InvariantCulture, "Padding is corrupt; {0} zero bytes found, but only {1} bytes should be left between last id3v2 frame and the end of the tag",
                                         _measured, _specified);
                else
                    return string.Format(CultureInfo.InvariantCulture, "Padding is corrupt; {1} bytes should be left between last id3v2 frame and the end of the tag, but only {0} zero bytes found.",
                                         _measured, _specified);
            }
        }
    }
}