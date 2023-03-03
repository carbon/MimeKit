﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace MimeKit.Encodings {
	/// <summary>
	/// A pass-through encoder implementing the <see cref="IMimeDecoder"/> interface.
	/// </summary>
	/// <remarks>
	/// Simply copies data as-is from the input buffer into the output buffer.
	/// </remarks>
	public class PassThroughEncoder : IMimeEncoder
	{
		/// <summary>
		/// Initialize a new instance of the <see cref="PassThroughEncoder"/> class.
		/// </summary>
		/// <param name="encoding">The encoding to return in the <see cref="Encoding"/> property.</param>
		/// <remarks>
		/// Creates a new pass-through encoder.
		/// </remarks>
		public PassThroughEncoder (ContentEncoding encoding)
		{
			Encoding = encoding;
		}

		/// <summary>
		/// Clone the <see cref="PassThroughEncoder"/> with its current state.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="PassThroughEncoder"/> with exactly the same state as the current encoder.
		/// </remarks>
		/// <returns>A new <see cref="PassThroughEncoder"/> with identical state.</returns>
		public IMimeEncoder Clone ()
		{
			return new PassThroughEncoder (Encoding);
		}

		/// <summary>
		/// Get the encoding.
		/// </summary>
		/// <remarks>
		/// Gets the encoding that the encoder supports.
		/// </remarks>
		/// <value>The encoding.</value>
		public ContentEncoding Encoding {
			get; private set;
		}

		/// <summary>
		/// Estimate the length of the output.
		/// </summary>
		/// <remarks>
		/// Estimates the number of bytes needed to encode the specified number of input bytes.
		/// </remarks>
		/// <returns>The estimated output length.</returns>
		/// <param name="inputLength">The input length.</param>
		public int EstimateOutputLength (int inputLength)
		{
			return inputLength;
		}

		void ValidateArguments (byte[] input, int startIndex, int length, byte[] output)
		{
			if (input is null)
				throw new ArgumentNullException (nameof (input));

			if (startIndex < 0 || startIndex > input.Length)
				throw new ArgumentOutOfRangeException (nameof (startIndex));

			if (length < 0 || length > (input.Length - startIndex))
				throw new ArgumentOutOfRangeException (nameof (length));

			if (output is null)
				throw new ArgumentNullException (nameof (output));

			if (output.Length < EstimateOutputLength (length))
				throw new ArgumentException ("The output buffer is not large enough to contain the encoded input.", nameof (output));
		}

		/// <summary>
		/// Encode the specified input into the output buffer.
		/// </summary>
		/// <remarks>
		/// Copies the input buffer into the output buffer, verbatim.
		/// </remarks>
		/// <returns>The number of bytes written to the output buffer.</returns>
		/// <param name="input">The input buffer.</param>
		/// <param name="startIndex">The starting index of the input buffer.</param>
		/// <param name="length">The length of the input buffer.</param>
		/// <param name="output">The output buffer.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="input"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="output"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> and <paramref name="length"/> do not specify
		/// a valid range in the <paramref name="input"/> byte array.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="output"/> is not large enough to contain the encoded content.</para>
		/// <para>Use the <see cref="EstimateOutputLength"/> method to properly determine the 
		/// necessary length of the <paramref name="output"/> byte array.</para>
		/// </exception>
		public int Encode (byte[] input, int startIndex, int length, byte[] output)
		{
			ValidateArguments (input, startIndex, length, output);

			Buffer.BlockCopy (input, startIndex, output, 0, length);

			return length;
		}

		/// <summary>
		/// Encode the specified input into the output buffer, flushing any internal buffer state as well.
		/// </summary>
		/// <remarks>
		/// Copies the input buffer into the output buffer, verbatim.
		/// </remarks>
		/// <returns>The number of bytes written to the output buffer.</returns>
		/// <param name="input">The input buffer.</param>
		/// <param name="startIndex">The starting index of the input buffer.</param>
		/// <param name="length">The length of the input buffer.</param>
		/// <param name="output">The output buffer.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="input"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="output"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="startIndex"/> and <paramref name="length"/> do not specify
		/// a valid range in the <paramref name="input"/> byte array.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="output"/> is not large enough to contain the encoded content.</para>
		/// <para>Use the <see cref="EstimateOutputLength"/> method to properly determine the 
		/// necessary length of the <paramref name="output"/> byte array.</para>
		/// </exception>
		public int Flush (byte[] input, int startIndex, int length, byte[] output)
		{
			return Encode (input, startIndex, length, output);
		}

		/// <summary>
		/// Reset the encoder.
		/// </summary>
		/// <remarks>
		/// Resets the state of the encoder.
		/// </remarks>
		public void Reset ()
		{
		}
	}
}
