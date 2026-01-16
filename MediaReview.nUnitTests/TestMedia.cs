using MediaReview.System;
using MediaReview.Model;

namespace MediaReview.nUnitTests
{
    internal class TestMedia
    {
        private Session OwnerSession;
        private Session AdminSession;
        private Session ForeignSession;
        private Session InvalidSession;

        [SetUp]
        public void Setup()
        {
            OwnerSession = Session.CreateTestSession(
                userId: 4,
                userName: "owner",
                isAdmin: false,
                valid: true
            );

            AdminSession = Session.CreateTestSession(
                userId: 99,
                userName: "admin",
                isAdmin: true,
                valid: true
            );

            ForeignSession = Session.CreateTestSession(
                userId: 5,
                userName: "other",
                isAdmin: false,
                valid: true
            );

            InvalidSession = Session.CreateInvalidSession(
                userId: 3,
                userName: "owner",
                isAdmin: false,
                valid: false
            );
        }

        [Test]
        public void Save_AsOwner_IsAllowed()
        {
            var media = new Media(OwnerSession)
            { 
                ownerName = "owner",
                title = "Test",
                
                mediaType = Media.MediaType.Movie
            };

            Assert.That(OwnerSession.Valid, Is.EqualTo(true));

            Assert.DoesNotThrow(() => media.Save());
        }

        [Test]
        public void Save_AsAdmin_IsAllowed()
        {
            var media = new Media(AdminSession)
            {
                ownerName = "owner",
                title = "Test2",
                mediaType = Media.MediaType.Movie
            };

            Assert.DoesNotThrow(() => media.Save());
        }

        [Test]
        public void Save_AsForeignUser_ThrowsUnauthorized()
        {
            var media = new Media(ForeignSession)
            {
                ownerName = "owner",
                title = "Test"
            };

            Assert.Throws<UnauthorizedAccessException>(() => media.Save());
        }

        [Test]
        public void Delete_AsOwner_IsAllowed()
        {
            var media = new Media(OwnerSession)
            {
                ownerName = "owner"
            };

            Assert.DoesNotThrow(() => media.Delete());
        }

        [Test]
        public void Delete_AsAdmin_IsAllowed()
        {
            var media = new Media(AdminSession)
            {
                ownerName = "owner"
            };

            Assert.DoesNotThrow(() => media.Delete());
        }

        [Test]
        public void Delete_AsForeignUser_ThrowsUnauthorized()
        {
            var media = new Media(ForeignSession)
            {
                ownerName = "owner"
            };

            Assert.Throws<UnauthorizedAccessException>(() => media.Delete());
        }

        [Test]
        public void GetMedia_WithValidSession_ReturnsMedia()
        {
            var media = Media.GetMedia(14, OwnerSession);
            Assert.That(media, Is.Not.Null);
        }

        [Test]
        public void GetMedia_WithInvalidSession_ReturnsNull()
        {
            var media = Media.GetMedia(99, InvalidSession);
            Assert.That(media, Is.Null);
        }
    }
}
