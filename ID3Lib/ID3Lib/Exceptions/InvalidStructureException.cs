// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Runtime.Serialization;

namespace Id3Lib.Exceptions
{
    /// <summary>
    /// The exception is thrown when some component of an mp3 file is permanently corrupt.
    /// Re-reading the file will always give you the same error, 
    /// as opposed to I/O or permission errors that can be usefully retried.
    /// </summary>
    /// <remarks>
    /// "Exceptions should be marked [Serializable]"
    /// http://www.codeproject.com/KB/architecture/exceptionbestpractices.aspx#Exceptionsshouldbemarked[Serializable]22
    /// </remarks>
    [Serializable]
    public class InvalidStructureException : Exception
    {
        protected InvalidStructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public InvalidStructureException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InvalidStructureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public InvalidStructureException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}