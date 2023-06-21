﻿using System.Linq;
using Cave;
using NUnit.Framework;

namespace Test;

public class UTFxxTests
{
    #region Private Fields

    const string Violin = "\U0001D11E";

    #endregion Private Fields

    #region Public Methods

    [Test]
    public void Utf16Test()
    {
        Assert.AreEqual(Violin, ((UTF16BE)Violin).ToString());
        Assert.AreEqual(0x1d11e, ((UTF16BE)Violin).Codepoints.Single());
        CollectionAssert.AreEqual(new[] { 0xD8, 0x34, 0xDD, 0x1E }, ((UTF16BE)Violin).Data);

        Assert.AreEqual(Violin, ((UTF16LE)Violin).ToString());
        Assert.AreEqual(0x1d11e, ((UTF16LE)Violin).Codepoints.Single());
        CollectionAssert.AreEqual(new[] { 0x34, 0xD8, 0x1E, 0xDD }, ((UTF16LE)Violin).Data);

        for (int codepoint = 1; codepoint < 0x10FFFF; codepoint = codepoint * 3 + 7)
        {
            var character = char.ConvertFromUtf32(codepoint);
            var testBe = (UTF16BE)character;
            Assert.AreEqual(codepoint, testBe.Codepoints.Single());
            Assert.AreEqual(character, testBe.ToString());
            Assert.AreEqual(character.Length, testBe.Length);
            var testLe = (UTF16LE)character;
            Assert.AreEqual(codepoint, testLe.Codepoints.Single());
            Assert.AreEqual(character, testLe.ToString());
            Assert.AreEqual(character.Length, testLe.Length);
            var data = testLe.Data;
            data.SwapEndian16();
            CollectionAssert.AreEqual(testBe.Data, data);
        }
        {
            var concat = ((UTF16BE)"Text").Concat("With").Concat("Multiple").Concat("Parts");
            Assert.AreEqual((UTF16BE)"TextWithMultipleParts", concat);
            Assert.AreEqual("TextWithMultipleParts", concat.ToString());
        }
        {
            var concat = ((UTF16LE)"Text").Concat("With").Concat("Multiple").Concat("Parts");
            Assert.AreEqual((UTF16LE)"TextWithMultipleParts", concat);
            Assert.AreEqual("TextWithMultipleParts", concat.ToString());
        }
    }

    [Test]
    public void Utf32Test()
    {
        Assert.AreEqual(Violin, ((UTF32BE)Violin).ToString());
        Assert.AreEqual(0x1d11e, ((UTF32BE)Violin).Codepoints.Single());
        CollectionAssert.AreEqual(new[] { 0x00, 0x01, 0xD1, 0x1E }, ((UTF32BE)Violin).Data);

        Assert.AreEqual(Violin, ((UTF32LE)Violin).ToString());
        Assert.AreEqual(0x1d11e, ((UTF32LE)Violin).Codepoints.Single());
        CollectionAssert.AreEqual(new byte[] { 0x1E, 0xD1, 0x01, 0x00 }, ((UTF32LE)Violin).Data);

        for (int codepoint = 1; codepoint < 0x10FFFF; codepoint = codepoint * 3 + 7)
        {
            var character = char.ConvertFromUtf32(codepoint);
            var testBe = (UTF32BE)character;
            Assert.AreEqual(codepoint, testBe.Codepoints.Single());
            Assert.AreEqual(character, testBe.ToString());
            Assert.AreEqual(character.Length, testBe.Length);
            var testLe = (UTF32LE)character;
            Assert.AreEqual(codepoint, testLe.Codepoints.Single());
            Assert.AreEqual(character, testLe.ToString());
            Assert.AreEqual(character.Length, testLe.Length);
            var data = testLe.Data;
            data.SwapEndian32();
            CollectionAssert.AreEqual(testBe.Data, data);
        }
        {
            var concat = ((UTF32BE)"Text").Concat("With").Concat("Multiple").Concat("Parts");
            Assert.AreEqual((UTF32BE)"TextWithMultipleParts", concat);
            Assert.AreEqual("TextWithMultipleParts", concat.ToString());
        }
        {
            var concat = ((UTF32LE)"Text").Concat("With").Concat("Multiple").Concat("Parts");
            Assert.AreEqual((UTF32LE)"TextWithMultipleParts", concat);
            Assert.AreEqual("TextWithMultipleParts", concat.ToString());
        }
    }

    [Test]
    public void Utf7Test()
    {
        for (int codepoint = 1; codepoint < 0x10FFFF; codepoint = codepoint * 3 + 7)
        {
            var character = char.ConvertFromUtf32(codepoint);
            var test = (UTF7)character;
            Assert.AreEqual(character, test.ToString());
            Assert.AreEqual(character.Length, test.Length);
        }
        var concat = ((UTF7)"Text").Concat("With").Concat("Multiple").Concat("Parts");
        Assert.AreEqual((UTF7)"TextWithMultipleParts", concat);
        Assert.AreEqual("TextWithMultipleParts", concat.ToString());
    }

    [Test]
    public void Utf8Test()
    {
        for (int codepoint = 1; codepoint < 0x10FFFF; codepoint = codepoint * 3 + 7)
        {
            var character = char.ConvertFromUtf32(codepoint);
            var test = (UTF8)character;
            Assert.AreEqual(codepoint, test.Codepoints.Single());
            Assert.AreEqual(character, test.ToString());
            Assert.AreEqual(character.Length, test.Length);
        }
        var concat = ((UTF8)"Text").Concat("With").Concat("Multiple").Concat("Parts");
        Assert.AreEqual((UTF8)"TextWithMultipleParts", concat);
        Assert.AreEqual("TextWithMultipleParts", concat.ToString());
    }

    #endregion Public Methods
}
