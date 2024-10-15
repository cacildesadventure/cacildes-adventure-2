using NUnit.Framework;
using UnityEngine;

namespace AF.Tests
{
    public class FormulasTests
    {
        [TestCase(100, 1, 200, 129)]
        [TestCase(100, 5, 200, 167)]
        [TestCase(100, 10, 200, 198)]
        [TestCase(100, 15, 200, 223)]
        [TestCase(100, 25, 200, 265)]
        [TestCase(100, 50, 200, 350)]
        [TestCase(100, 100, 200, 482)]
        public void CalculateStatForLevel_ValidInputs_Multiplier200_ReturnsExpectedResult(int baseValue, int level, int multiplier, int expectedResult)
        {
            // Act
            int result = Formulas.CalculateStatForLevel(baseValue, level, multiplier);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(100, 1, 2.25f, 102)]
        [TestCase(100, 5, 2.25f, 111)]
        [TestCase(100, 10, 2.25f, 122)]
        [TestCase(100, 15, 2.25f, 133)]
        [TestCase(100, 25, 2.25f, 156)]
        [TestCase(100, 50, 2.25f, 212)]
        [TestCase(100, 100, 2.25f, 325)]
        public void CalculateStatForLevel_ForOldFormula_ReturnsExpectedResult(int baseValue, int level, float multiplier, int expectedResult)
        {
            // Act
            int result = baseValue + (int)(level * multiplier);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
