﻿//
// ParameterTests.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2022 .NET Foundation and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Text;

using NUnit.Framework;

using MimeKit;

namespace UnitTests {
	[TestFixture]
	public class ParameterTests
	{
		[Test]
		public void TestArgumentExceptions ()
		{
			const string invalid = "X-测试文本";

			Assert.Throws<ArgumentNullException> (() => new Parameter ((Encoding) null, "name", "value"));
			Assert.Throws<ArgumentNullException> (() => new Parameter (Encoding.UTF8, null, "value"));
			Assert.Throws<ArgumentException> (() => new Parameter (Encoding.UTF8, string.Empty, "value"));
			Assert.Throws<ArgumentException> (() => new Parameter (Encoding.UTF8, invalid, "value"));
			Assert.Throws<ArgumentNullException> (() => new Parameter (Encoding.UTF8, "name", null));
			Assert.Throws<ArgumentNullException> (() => new Parameter ((string) null, "name", "value"));
			Assert.Throws<ArgumentNullException> (() => new Parameter ("utf-8", null, "value"));
			Assert.Throws<ArgumentException> (() => new Parameter ("utf-8", string.Empty, "value"));
			Assert.Throws<ArgumentException> (() => new Parameter ("utf-8", invalid, "value"));
			Assert.Throws<ArgumentNullException> (() => new Parameter ("utf-8", "name", null));
			Assert.Throws<ArgumentNullException> (() => new Parameter (null, "value"));
			Assert.Throws<ArgumentException> (() => new Parameter (string.Empty, "value"));
			Assert.Throws<ArgumentException> (() => new Parameter (invalid, "value"));
			Assert.Throws<ArgumentNullException> (() => new Parameter ("name", null));
		}

		[Test]
		public void TestBasicFunctionality ()
		{
			var param = new Parameter ("name", "value");

			Assert.AreEqual (Encoding.UTF8.HeaderName, param.Encoding.HeaderName);
			Assert.AreEqual (ParameterEncodingMethod.Default, param.EncodingMethod);
			Assert.IsFalse (param.AlwaysQuote);
			Assert.AreEqual ("name", param.Name);
			Assert.AreEqual ("value", param.Value);
			Assert.AreEqual ("name=\"value\"", param.ToString ());
		}

		[Test]
		public void TestEncode ()
		{
			var builder = new StringBuilder ("Content-Disposition: attachment");
			var param = new Parameter ("filename", "tps-report.doc");
			var options = FormatOptions.Default.Clone ();
			int lineLength = builder.Length;

			options.AlwaysQuoteParameterValues = false;
			options.NewLineFormat = NewLineFormat.Dos;

			param.Encode (options, builder, ref lineLength, Encoding.UTF8);

			Assert.AreEqual ("Content-Disposition: attachment; filename=tps-report.doc", builder.ToString ());
		}

		[Test]
		public void TestEncodeAlwaysQuote ()
		{
			var builder = new StringBuilder ("Content-Disposition: attachment");
			var param = new Parameter ("filename", "tps-report.doc");
			var options = FormatOptions.Default.Clone ();
			int lineLength = builder.Length;

			param.AlwaysQuote = true;
			options.NewLineFormat = NewLineFormat.Dos;

			param.Encode (options, builder, ref lineLength, Encoding.UTF8);

			Assert.AreEqual ("Content-Disposition: attachment; filename=\"tps-report.doc\"", builder.ToString ());
		}

		[Test]
		public void TestEncodeFormatOptionsAlwaysQuote ()
		{
			var builder = new StringBuilder ("Content-Disposition: attachment");
			var param = new Parameter ("filename", "tps-report.doc");
			var options = FormatOptions.Default.Clone ();
			int lineLength = builder.Length;

			options.AlwaysQuoteParameterValues = true;
			options.NewLineFormat = NewLineFormat.Dos;

			param.Encode (options, builder, ref lineLength, Encoding.UTF8);

			Assert.AreEqual ("Content-Disposition: attachment; filename=\"tps-report.doc\"", builder.ToString ());
		}

		[Test]
		public void TestEncodeRfc2047 ()
		{
			var builder = new StringBuilder ("Content-Disposition: attachment");
			var param = new Parameter ("filename", "测试文本.doc");
			var options = FormatOptions.Default.Clone ();
			int lineLength = builder.Length;

			param.EncodingMethod = ParameterEncodingMethod.Rfc2047;
			options.NewLineFormat = NewLineFormat.Dos;

			param.Encode (options, builder, ref lineLength, Encoding.UTF8);

			Assert.AreEqual ("Content-Disposition: attachment; filename=\"=?utf-8?b?5rWL6K+V5paH5pysLmRv?=\r\n\t=?utf-8?q?c?=\"", builder.ToString ());
		}

		[Test]
		public void TestEncodeFormatOptionsRfc2047 ()
		{
			var builder = new StringBuilder ("Content-Disposition: attachment");
			var param = new Parameter ("filename", "测试文本.doc");
			var options = FormatOptions.Default.Clone ();
			int lineLength = builder.Length;

			options.ParameterEncodingMethod = ParameterEncodingMethod.Rfc2047;
			options.NewLineFormat = NewLineFormat.Dos;

			param.Encode (options, builder, ref lineLength, Encoding.UTF8);
			
			Assert.AreEqual ("Content-Disposition: attachment; filename=\"=?utf-8?b?5rWL6K+V5paH5pysLmRv?=\r\n\t=?utf-8?q?c?=\"", builder.ToString ());
		}

		[Test]
		public void TestEncodeRfc2231 ()
		{
			var builder = new StringBuilder ("Content-Disposition: attachment");
			var param = new Parameter ("filename", "测试文本.doc");
			var options = FormatOptions.Default.Clone ();
			int lineLength = builder.Length;

			param.EncodingMethod = ParameterEncodingMethod.Rfc2231;
			options.NewLineFormat = NewLineFormat.Dos;

			param.Encode (options, builder, ref lineLength, Encoding.UTF8);

			Assert.AreEqual ("Content-Disposition: attachment;\r\n\tfilename*=utf-8''%E6%B5%8B%E8%AF%95%E6%96%87%E6%9C%AC.doc", builder.ToString ());
		}

		[Test]
		public void TestEncodeFormatOptionsRfc2231 ()
		{
			var builder = new StringBuilder ("Content-Disposition: attachment");
			var param = new Parameter ("filename", "测试文本.doc");
			var options = FormatOptions.Default.Clone ();
			int lineLength = builder.Length;

			options.ParameterEncodingMethod = ParameterEncodingMethod.Rfc2231;
			options.NewLineFormat = NewLineFormat.Dos;

			param.Encode (options, builder, ref lineLength, Encoding.UTF8);

			Assert.AreEqual ("Content-Disposition: attachment;\r\n\tfilename*=utf-8''%E6%B5%8B%E8%AF%95%E6%96%87%E6%9C%AC.doc", builder.ToString ());
		}
	}
}
