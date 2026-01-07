using MediaReview.Model;
using MediaReview.System;

namespace MediaReview.nUnitTests
{
    internal class TestRating
    {
        [Test]
        public void TestDefaultValues()
        {
            Rating rating = new Rating();
            Assert.That(rating.stars, Is.EqualTo(0));
            Assert.That(rating.comment, Is.EqualTo(""));
            Assert.That(rating.comment_confirmed, Is.False);

        }

        [Test]
        public void TestConstructor()
        {
            Rating rating = new Rating(5, "Top", 1, 10);
            Assert.That(rating.stars, Is.EqualTo(5));
            Assert.That(rating.comment, Is.EqualTo("Top"));
            Assert.That(rating.user_id, Is.EqualTo(1));
            Assert.That(rating.media_id, Is.EqualTo(10));
            Assert.That(rating.comment_confirmed, Is.False);

        }
        [Test]

        public void GetNotExistentRating()
        {
            Session session = Session.Create("testuser", "password123");
            Assert.Throws(Is.TypeOf<Exception>().And.Message.EqualTo("Rating not found"), () => Rating.Get(-1, session));
        }

        [Test]
        public void Save_WhenUserIsNotAdminAndNotOwner()
        {
            var session = Session.CreateTestSession(userId: 2, userName: "testuser",isAdmin: false, valid: true);

            var rating = new Rating
            {
                user_id = 1,
                media_id = 10,
                stars = 5,
                comment = "Nice"
            };

            rating.BeginEdit(session);

            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                rating.Save();
            });
        }

        [Test]

        public void BeginEditWithInvalidSession()
        {
            var session = Session.CreateInvalidSession(2, "testuser", false, false);

            var rating = new Rating
            {
                user_id = 1,
                media_id = 10,
                stars = 5,
                comment = "Nice"
            };
            Assert.Throws(Is.TypeOf<UnauthorizedAccessException>().And.Message.EqualTo("Invalid session."), () =>
            {
                rating.BeginEdit(session);
            });
        }

    }
}
