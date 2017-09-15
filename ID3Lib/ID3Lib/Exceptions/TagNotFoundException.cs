// Copyright(C) 2002-2012 Hugo Rumayor Montemayor, All rights reserved.
using System;
using System.Runtime.Serialization;

namespace Id3Lib.Exceptions
{
	/// <summary>
	/// The exception is thrown when the tag is missing.
	/// </summary>
    [Serializable]
    public class TagNotFoundException : Exception
	{
        protected TagNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        /// <summary>
        /// 
        /// </summary>
		public TagNotFoundException()
		{
		}
	
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
		public TagNotFoundException(string message): base(message)
		{
		}

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
		public TagNotFoundException(string message, Exception inner): base(message, inner)
		{
		}
	}
}