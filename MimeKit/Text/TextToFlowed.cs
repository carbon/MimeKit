﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Text;

using MimeKit.Utils;

namespace MimeKit.Text {
	/// <summary>
	/// A text to flowed text converter.
	/// </summary>
	/// <remarks>
	/// <para>Wraps text to conform with the flowed text format described in rfc3676.</para>
	/// <para>The Content-Type header for the wrapped output text should be set to
	/// <c>text/plain; format=flowed; delsp=yes</c>.</para>
	/// </remarks>
	public class TextToFlowed : TextConverter
	{
		const int MaxLineLength = 78;

		/// <summary>
		/// Initialize a new instance of the <see cref="TextToFlowed"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new text to flowed text converter.
		/// </remarks>
		public TextToFlowed ()
		{
		}

		/// <summary>
		/// Get the input format.
		/// </summary>
		/// <remarks>
		/// Gets the input format.
		/// </remarks>
		/// <value>The input format.</value>
		public override TextFormat InputFormat {
			get { return TextFormat.Plain; }
		}

		/// <summary>
		/// Get the output format.
		/// </summary>
		/// <remarks>
		/// Gets the output format.
		/// </remarks>
		/// <value>The output format.</value>
		public override TextFormat OutputFormat {
			get { return TextFormat.Flowed; }
		}

		static ReadOnlySpan<char> Unquote (ReadOnlySpan<char> line, out int quoteDepth)
		{
			int index = 0;

			quoteDepth = 0;

			if (line.Length == 0 || line[0] != '>')
				return line;

			do {
				quoteDepth++;
				index++;

				if (index < line.Length && line[index] == ' ')
					index++;
			} while (index < line.Length && line[index] == '>');

			return line.Slice (index);
		}

		static bool StartsWith (string text, int startIndex, string value)
		{
			if (startIndex + value.Length > text.Length)
				return false;

			for (int i = 0; i < value.Length; i++) {
				if (text[startIndex + i] != value[i])
					return false;
			}

			return true;
		}

		static string GetFlowedLine (StringBuilder flowed, ReadOnlySpan<char> line, ref int index, int quoteDepth)
		{
			flowed.Length = 0;

			if (quoteDepth > 0)
				flowed.Append ('>', quoteDepth);

			// Space-stuffed lines which start with a space, "From ", or ">".
			if (quoteDepth > 0 || (line.Length > index && line[index] == ' ') || line.Slice (index).StartsWith ("From ".AsSpan (), StringComparison.Ordinal))
				flowed.Append (' ');

			if (flowed.Length + (line.Length - index) <= MaxLineLength) {
				flowed.Append (line.Slice (index));
				index = line.Length;

				return flowed.ToString ();
			}

			do {
				int nextSpace = line.Slice (index).IndexOf (' ');
				int wordEnd = nextSpace == -1 ? line.Length : nextSpace + index;
				int softBreak = nextSpace == -1 ? 0 : 2; // 2 = space + soft-break space
				int wordLength = wordEnd - index;

				if (flowed.Length + wordLength + softBreak <= MaxLineLength) {
					// The entire word will fit on the remainder of the line.
					flowed.Append (line.Slice (index, wordLength));
					index = wordEnd;
				} else if (wordLength > MaxLineLength - (quoteDepth + 1)) {
					// Even if we insert a soft-break here, the word is longer than what will fit on its own line.
					// No matter what we do, we will need to break the word apart.
					wordLength = MaxLineLength - (flowed.Length + 1);
					flowed.Append (line.Slice (index, wordLength));
					index += wordLength;
					break;
				} else {
					// Only part of the word will fit on the remainder of this line, but it will easily fit
					// on its own line. Insert a soft-break so that we don't break apart this word.
					break;
				}

				while (flowed.Length + 1 < MaxLineLength && index < line.Length && line[index] == ' ')
					flowed.Append (line[index++]);
			} while (index < line.Length && flowed.Length + 1 < MaxLineLength);

			if (index < line.Length)
				flowed.Append (' ');

			return flowed.ToString ();
		}

		/// <summary>
		/// Convert the contents of <paramref name="reader"/> from the <see cref="InputFormat"/> to the
		/// <see cref="OutputFormat"/> and uses the <paramref name="writer"/> to write the resulting text.
		/// </summary>
		/// <remarks>
		/// Converts the contents of <paramref name="reader"/> from the <see cref="InputFormat"/> to the
		/// <see cref="OutputFormat"/> and uses the <paramref name="writer"/> to write the resulting text.
		/// </remarks>
		/// <param name="reader">The text reader.</param>
		/// <param name="writer">The text writer.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="reader"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="writer"/> is <c>null</c>.</para>
		/// </exception>
		public override void Convert (TextReader reader, TextWriter writer)
		{
			StringBuilder flowed;
			string line;

			if (reader is null)
				throw new ArgumentNullException (nameof (reader));

			if (writer is null)
				throw new ArgumentNullException (nameof (writer));

			if (!string.IsNullOrEmpty (Header))
				writer.Write (Header);

			flowed = new StringBuilder (MaxLineLength);

			while ((line = reader.ReadLine ()) != null) {
				// Trim spaces before user-inserted hard line breaks.
				var unquoted = Unquote (line.AsSpan ().TrimEnd (' '), out var quoteDepth);

				// Ensure all lines (fixed and flowed) are 78 characters or fewer in
				// length, counting any trailing space as well as a space added as
				// stuffing, but not counting the CRLF, unless a word by itself
				// exceeds 78 characters.
				int index = 0;

				do {
					var flowedLine = GetFlowedLine (flowed, unquoted, ref index, quoteDepth);
					writer.WriteLine (flowedLine);
				} while (index < unquoted.Length);
			}

			if (!string.IsNullOrEmpty (Footer))
				writer.Write (Footer);
		}
	}
}
