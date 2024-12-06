using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AF.Tutorial.Tests
{
    public class TutorialUtilsTests
    {
        [Test]
        public void FindWord_ShouldCallOnWordAddedForPlainText()
        {
            // Arrange
            string input = "Hello World";
            List<string> wordsAdded = new();
            List<string> buttonsAdded = new();

            // Act
            TutorialUtils.findWord(
                input,
                onWordAdded: word =>
                {
                    wordsAdded.Add(word);
                    return true;
                },
                onButtonAdd: button =>
                {
                    buttonsAdded.Add(button);
                    return true;
                });

            // Assert
            Assert.AreEqual(1, wordsAdded.Count, "There should be one call to onWordAdded.");
            Assert.AreEqual("Hello World", wordsAdded[0], "The plain text should match the input string.");
            Assert.IsEmpty(buttonsAdded, "There should be no calls to onButtonAdd for plain text.");
        }

        [Test]
        public void FindWord_ShouldCallOnButtonAddForButtonPlaceholder()
        {
            // Arrange
            string input = "{0}";
            List<string> wordsAdded = new();
            List<string> buttonsAdded = new();

            // Act
            TutorialUtils.findWord(
                input,
                onWordAdded: word =>
                {
                    wordsAdded.Add(word);
                    return true;
                },
                onButtonAdd: button =>
                {
                    buttonsAdded.Add(button);
                    return true;
                });

            // Assert
            Assert.AreEqual(1, buttonsAdded.Count, "There should be one call to onButtonAdd.");
            Assert.AreEqual("0", buttonsAdded[0], "The button placeholder should match the extracted value.");
            Assert.AreEqual(1, wordsAdded.Count, "There should be one call to onWordAdded.");
            Assert.AreEqual("", wordsAdded[0], "onWordAdded should be called with an empty string before the placeholder.");
        }

        [Test]
        public void FindWord_ShouldHandleMixedTextAndButtonPlaceholders()
        {
            // Arrange
            string input = "Press {0} to jump and {1} to run";
            List<string> wordsAdded = new();
            List<string> buttonsAdded = new();

            // Act
            TutorialUtils.findWord(
                input,
                onWordAdded: word =>
                {
                    wordsAdded.Add(word);
                    return true;
                },
                onButtonAdd: button =>
                {
                    buttonsAdded.Add(button);
                    return true;
                });

            // Assert
            Assert.AreEqual(3, wordsAdded.Count, "There should be three calls to onWordAdded.");
            Assert.AreEqual("Press ", wordsAdded[0], "The first text fragment should match.");
            Assert.AreEqual(" to jump and ", wordsAdded[1], "The second text fragment should match.");
            Assert.AreEqual(" to run", wordsAdded[2], "The third text fragment should match.");

            Assert.AreEqual(2, buttonsAdded.Count, "There should be two calls to onButtonAdd.");
            Assert.AreEqual("0", buttonsAdded[0], "The first button index should match.");
            Assert.AreEqual("1", buttonsAdded[1], "The second button index should match.");
        }

        [Test]
        public void FindWord_ShouldHandleEmptyString()
        {
            // Arrange
            string input = "";
            List<string> wordsAdded = new();
            List<string> buttonsAdded = new();

            // Act
            TutorialUtils.findWord(
                input,
                onWordAdded: word =>
                {
                    wordsAdded.Add(word);
                    return true;
                },
                onButtonAdd: button =>
                {
                    buttonsAdded.Add(button);
                    return true;
                });

            // Assert
            Assert.IsEmpty(wordsAdded, "onWordAdded should not be called for an empty string.");
            Assert.IsEmpty(buttonsAdded, "onButtonAdd should not be called for an empty string.");
        }

        [Test]
        public void FindWord_ShouldHandleInvalidBracketsGracefully()
        {
            // Arrange
            string input = "Press {99";
            List<string> wordsAdded = new();
            List<string> buttonsAdded = new();

            // Act
            TutorialUtils.findWord(
                input,
                onWordAdded: word =>
                {
                    wordsAdded.Add(word);
                    return true;
                },
                onButtonAdd: button =>
                {
                    buttonsAdded.Add(button);
                    return true;
                });

            // Assert
            Assert.AreEqual(2, wordsAdded.Count, "There should be two calls to onWordAdded for the initial text.");
            Assert.AreEqual("Press ", wordsAdded[0], "The text before the incomplete bracket should match.");
            Assert.AreEqual("99", wordsAdded[1], "The text after the incomplete bracket should match.");
            Assert.IsEmpty(buttonsAdded, "onButtonAdd should not be called for incomplete brackets.");
        }
    }
}
