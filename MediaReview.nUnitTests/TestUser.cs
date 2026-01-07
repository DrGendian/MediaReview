using MediaReview.Model;

namespace MediaReview.nUnitTests
{
    public class TestUser
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PasswordHashIsDeterministic()
        {
            string pass1 = User._HashPassword("testuser", "password123");
            string pass2 = User._HashPassword("testuser", "password123");
            Assert.That(pass1 == pass2);

        }

        [Test]
        public void PasswordHashIsNullOnEmptyString()
        {
            string pass1 = User._HashPassword("testuser", "");
            Assert.That(pass1, Is.EqualTo(null));
        }

        [Test]
        public void CheckCorrectPassword()
        {

            User user = new User();
            string username = "testuser";
            string password = "password123";
            user._UserName = username;
            user.SetPassword(password);
            user.Save();
            user.checkPassword(username, password);
            Assert.That(user.checkPassword(username, password), Is.True);
        }

        [Test]
        public void CheckIncorrectPassword()
        {
            User user = new User();
            string username = "testuser";
            string password = "password";
            user.checkPassword(username, password);
            Assert.That(user.checkPassword(username, password), Is.False);
        }

        [Test]
        public void UserExistsAfterCreation()
        {
            User user = new User();
            string username = "newuser";
            string password = "newpassword";
            user._UserName = username;
            user.SetPassword(password);
            user.Save();
            Assert.That(User.Exists(username), Is.True);
        }

        [Test]
        public void UserExistsUserNotFound()
        {
            Assert.That(User.Exists("nonexistentuser"), Is.False);
        }

        [Test]
        public void GetUserReturnsCorrectUser()
        {
            User user = User.Get("testuser");
            Assert.That(user._UserName, Is.EqualTo("testuser"));
        }

        [Test]
        public void GetUserReturnsNullForNonexistentUser()
        {
            User user = User.Get("nonexistentuser");
            Assert.That(user, Is.Null);
        }
    }
}
