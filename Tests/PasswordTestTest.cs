﻿using System;
using System.Runtime.InteropServices;
using Cave;
using Cave.Collections;
using Cave.Security;
using NUnit.Framework;

namespace Test;

[TestFixture]
class PasswordTestTest
{
    #region Public Methods

    [Test]
    public void GuessBruteForceTimeTest()
    {
        var test = PasswordTest.GuessBruteForceTime("ABCDEFGH");
        Assert.IsTrue(test < TimeSpan.FromSeconds(0.5));
        test = PasswordTest.GuessBruteForceTime("ZYXABC123");
        Assert.IsTrue(test < TimeSpan.FromSeconds(0.5));
        test = PasswordTest.GuessBruteForceTime("FAHDGCEB");
        Assert.IsTrue(test < TimeSpan.FromSeconds(0.5));
        test = PasswordTest.GuessBruteForceTime("2ZY3AXBC1");
        Assert.IsTrue(test < TimeSpan.FromSeconds(0.5));

        test = PasswordTest.GuessBruteForceTime("göödPaßwörd!");
        Assert.IsTrue(test > TimeSpan.FromDays(1));
    }

    [Test]
    public void GuessBruteForceTriesTest()
    {
        Assert.AreEqual(208827064576d, PasswordTest.GuessBruteForceTries("ABCDEFGH"));
        Assert.AreEqual(101559956668416d, PasswordTest.GuessBruteForceTries("ZYXABC123"));
        Assert.AreEqual(208827064576d, PasswordTest.GuessBruteForceTries("FAHDGCEB"));
        Assert.AreEqual(101559956668416d, PasswordTest.GuessBruteForceTries("2ZY3AXBC1"));
        Assert.AreEqual(5.9873693923837895E+19, PasswordTest.GuessBruteForceTries("gqTU7x_kP!"));
    }

    [Test]
    public void GuessComplexityTest()
    {
        Assert.AreEqual(8, PasswordTest.GuessComplexity(ASCII.GetBytes("ABCDEFGH")));
        Assert.AreEqual(9, PasswordTest.GuessComplexity(ASCII.GetBytes("ZYXABC123")));
        Assert.AreEqual(22, PasswordTest.GuessComplexity(ASCII.GetBytes("FAHDGCEB")));
        Assert.AreEqual(49, PasswordTest.GuessComplexity(ASCII.GetBytes("2ZY3AXBC1")));
        Assert.AreEqual(37, PasswordTest.GuessComplexity(ASCII.GetBytes("gqTU7x_kP!")));
    }

    #endregion Public Methods
}
