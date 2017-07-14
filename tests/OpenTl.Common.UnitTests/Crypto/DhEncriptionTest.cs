namespace OpenTl.Common.UnitTests.Crypto
{
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;

    using Xunit;

    public class DhEncriptionTest
    {
        [Fact]
        public void SimpleTest()
        {
            var generator = new DHParametersGenerator();
            generator.Init(2048, 100, new SecureRandom());

            var dhParameters = generator.GenerateParameters();

            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), dhParameters);
            var keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
            keyGen.Init(kgp);

            var aliceKeyPair = keyGen.GenerateKeyPair();
            var privateKey =aliceKeyPair.Private;
        }
    }
}