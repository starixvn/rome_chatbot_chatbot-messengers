namespace Messenger.Client.Tests.Helpers
{
    using System;
    using Messenger.Client.Helpers;
    using Xunit;

    public class CryptographyHelperTests
    {
        private readonly CryptographyHelper cryptographyHelper;

        public CryptographyHelperTests()
        {
            cryptographyHelper = new CryptographyHelper();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Decrypt_EncryptedMessageIsNullOrEmpty_ThrowsArgumentNullException(string encryptedMessage)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cryptographyHelper.Decrypt(encryptedMessage, string.Empty, string.Empty));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Decrypt_KeyIsNullOrEmpty_ThrowsArgumentNullException(string key)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cryptographyHelper.Decrypt("encryptedMessage", key, string.Empty));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Decrypt_SaltIsNullOrEmpty_ThrowsArgumentNullException(string salt)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cryptographyHelper.Decrypt("encryptedMessage", "key", salt));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Encrypt_MessageIsNullOrEmpty_ThrowsArgumentNullException(string message)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cryptographyHelper.Encrypt(message, string.Empty, string.Empty));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Encrypt_KeyIsNullOrEmpty_ThrowsArgumentNullException(string key)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cryptographyHelper.Encrypt("message", key, string.Empty));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Encrypt_SaltIsNullOrEmpty_ThrowsArgumentNullException(string salt)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cryptographyHelper.Encrypt("message", "key", salt));
        }
    }
}