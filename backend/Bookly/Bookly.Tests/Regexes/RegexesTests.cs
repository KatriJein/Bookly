namespace Bookly.Tests.Regexes;

[TestFixture]
    public class RegexesTests
    {
        [TestCase("user123", true)]
        [TestCase("alex-smith", true)]
        [TestCase("Jane.Doe", true)]
        [TestCase("abc", true)]
        [TestCase("Super_User-12", true)]
        [TestCase("jo", false)]                         
        [TestCase("user name", false)]                  
        [TestCase("root!", false)]                      
        [TestCase("абвгд", false)]                      
        [TestCase("this_login_is_way_too_long", false)] 
        public void LoginRegex_Tests(string input, bool expected)
        {
            var result = Core.Regexes.LoginRegex().IsMatch(input);
            Assert.That(result, Is.EqualTo(expected), $"Login test failed for: \"{input}\"");
        }
        
        [TestCase("user@example.com", true)]
        [TestCase("alex.smith+dev@my-domain.org", true)]
        [TestCase("ivanov_93@mail.ru", true)]
        [TestCase("x@a.io", true)]
        [TestCase("user@", false)]
        [TestCase("@site.com", false)]
        [TestCase("user@site", false)]
        [TestCase("user@@example.com", false)]
        [TestCase("user@exa_mple.com", false)]
        public void EmailRegex_Tests(string input, bool expected)
        {
            var result = Core.Regexes.EmailRegex().IsMatch(input);
            Assert.That(result, Is.EqualTo(expected), $"Email test failed for: \"{input}\"");
        }
        
        [TestCase("Password1", true)]
        [TestCase("Qwerty7@", true)]
        [TestCase("aBcD1234", true)]
        [TestCase("Secure_2024@", true)]
        [TestCase("R2d2Cool!", true)]
        [TestCase("password", false)]  
        [TestCase("12345678", false)]  
        [TestCase("AAAAAAA1", false)]  
        [TestCase("abc12345", false)]  
        [TestCase("Pa1", false)]       
        [TestCase("P@sswordWith$uperLongLengthBeyond32Symbols!", false)]
        [TestCase("Pass w0rd", false)] 
        public void PasswordRegex_Tests(string input, bool expected)
        {
            var result = Core.Regexes.PasswordRegex().IsMatch(input);
            Assert.That(result, Is.EqualTo(expected), $"Password test failed for: \"{input}\"");
        }
    }