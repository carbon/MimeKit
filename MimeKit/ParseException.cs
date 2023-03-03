﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

#if SERIALIZABLE
using System.Security;
using System.Runtime.Serialization;
#endif

namespace MimeKit {
	/// <summary>
	/// A Parse exception as thrown by the various Parse methods in MimeKit.
	/// </summary>
	/// <remarks>
	/// A <see cref="ParseException"/> can be thrown by any of the Parse() methods
	/// in MimeKit. Each exception instance will have a <see cref="TokenIndex"/>
	/// which marks the byte offset of the token that failed to parse as well
	/// as a <see cref="ErrorIndex"/> which marks the byte offset where the error
	/// occurred.
	/// </remarks>
#if SERIALIZABLE
	[Serializable]
#endif
	public class ParseException : FormatException
	{
#if SERIALIZABLE
		/// <summary>
		/// Initialize a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="ParseException"/>.
		/// </remarks>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The stream context.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="info"/> is <c>null</c>.
		/// </exception>
		protected ParseException (SerializationInfo info, StreamingContext context) : base (info, context)
		{
			TokenIndex = info.GetInt32 ("TokenIndex");
			ErrorIndex = info.GetInt32 ("ErrorIndex");
		}
#endif

		/// <summary>
		/// Initialize a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="ParseException"/>.
		/// </remarks>
		/// <param name="message">The error message.</param>
		/// <param name="tokenIndex">The byte offset of the token.</param>
		/// <param name="errorIndex">The byte offset of the error.</param>
		/// <param name="innerException">The inner exception.</param>
		public ParseException (string message, int tokenIndex, int errorIndex, Exception innerException) : base (message, innerException)
		{
			TokenIndex = tokenIndex;
			ErrorIndex = errorIndex;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="ParseException"/>.
		/// </remarks>
		/// <param name="message">The error message.</param>
		/// <param name="tokenIndex">The byte offset of the token.</param>
		/// <param name="errorIndex">The byte offset of the error.</param>
		public ParseException (string message, int tokenIndex, int errorIndex) : base (message)
		{
			TokenIndex = tokenIndex;
			ErrorIndex = errorIndex;
		}

#if SERIALIZABLE
		/// <summary>
		/// When overridden in a derived class, sets the <see cref="System.Runtime.Serialization.SerializationInfo"/>
		/// with information about the exception.
		/// </summary>
		/// <remarks>
		/// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/>
		/// with information about the exception.
		/// </remarks>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="info"/> is <c>null</c>.
		/// </exception>
		[SecurityCritical]
		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			info.AddValue ("TokenIndex", TokenIndex);
			info.AddValue ("ErrorIndex", ErrorIndex);
		}
#endif

		/// <summary>
		/// Get the byte index of the token that was malformed.
		/// </summary>
		/// <remarks>
		/// The token index is the byte offset at which the token started.
		/// </remarks>
		/// <value>The byte index of the token.</value>
		public int TokenIndex {
			get; private set;
		}

		/// <summary>
		/// Get the index of the byte that caused the error.
		/// </summary>
		/// <remarks>
		/// The error index is the byte offset at which the parser encountered an error.
		/// </remarks>
		/// <value>The index of the byte that caused error.</value>
		public int ErrorIndex {
			get; private set;
		}
	}
}
