using Bookly.Application;

namespace Bookly.Tests;

[TestFixture]
    public class AuthorUtilsTests
    {
        [Test]
        public void RetrievePossibleAuthorNames_SingleWord_ReturnsItself()
        {
            var result = AuthorUtils.RetrievePossibleAuthorNamesFromAuthor("Пушкин");
            Assert.That(result, Is.EquivalentTo(new[] { "Пушкин" }));
        }

        [Test]
        public void RetrievePossibleAuthorNames_TwoWords_ReturnsAllCombinations()
        {
            var result = AuthorUtils.RetrievePossibleAuthorNamesFromAuthor("Александр Пушкин");

            Assert.That(result, Does.Contain("Александр Пушкин"));
            Assert.That(result, Does.Contain("Пушкин Александр"));
            Assert.That(result, Does.Contain("Александр П."));
            Assert.That(result, Does.Contain("А. Пушкин"));
            Assert.That(result.Count, Is.EqualTo(4));
        }

        [Test]
        public void RetrievePossibleAuthorNames_ThreeWords_ReturnsExpectedCombinations()
        {
            var result = AuthorUtils.RetrievePossibleAuthorNamesFromAuthor("Александр Сергеевич Пушкин");

            Assert.That(result, Does.Contain("Александр Сергеевич Пушкин"));
            Assert.That(result, Does.Contain("А.С. Пушкин"));
            Assert.That(result, Does.Contain("Александр С.П."));
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void RetrievePossibleAuthorNames_AlreadyShortForm_ReturnsOriginal()
        {
            var result = AuthorUtils.RetrievePossibleAuthorNamesFromAuthor("Пушкин А.С.");
            Assert.That(result, Is.EquivalentTo(new[] { "Пушкин А.С." }));
        }

        [Test]
        public void AuthorNameToBestFormat_TwoWords_ReturnsLastNameInitial()
        {
            var result = AuthorUtils.AuthorNameToBestFormat("Александр Пушкин");
            Assert.That(result, Is.EqualTo("Пушкин А."));
        }

        [Test]
        public void AuthorNameToBestFormat_ThreeWords_ReturnsLastNameInitials()
        {
            var result = AuthorUtils.AuthorNameToBestFormat("Александр Сергеевич Пушкин");
            Assert.That(result, Is.EqualTo("Пушкин А.С."));
        }

        [Test]
        public void AuthorNameToBestFormat_AlreadyShort_ReturnsTrimmed()
        {
            var result = AuthorUtils.AuthorNameToBestFormat("Пушкин А.С.");
            Assert.That(result, Is.EqualTo("Пушкин А.С."));
        }

        [Test]
        public void AuthorNameToBestFormat_WithSpacesBetweenInitials_RemovesExtraSpaces()
        {
            var result = AuthorUtils.AuthorNameToBestFormat("Пушкин А. С.");
            Assert.That(result, Is.EqualTo("Пушкин А.С."));
        }

        [Test]
        public void AuthorNameToBestFormat_EmptyOrNull_ReturnsSame()
        {
            Assert.That(AuthorUtils.AuthorNameToBestFormat(""), Is.EqualTo(""));
            Assert.That(AuthorUtils.AuthorNameToBestFormat(null), Is.Null);
        }

        [Test]
        public void AuthorNameToBestFormat_EnglishName_TwoWords_LeavesAsIs()
        {
            var result = AuthorUtils.AuthorNameToBestFormat("Alexander Pushkin");
            Assert.That(result, Is.EqualTo("Pushkin A."));
        }
    }