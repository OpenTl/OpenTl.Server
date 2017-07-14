namespace OpenTl.Common.UnitTests.Crypto
{
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Security;

    using Xunit;

    public class DhEncriptionTest
    {
        [Fact]
        public void SimpleTest()
        {
            var generator = new DHParametersGenerator();
            generator.Init(1024, 7, new SecureRandom());

            var dhParameters = generator.GenerateParameters();

            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), dhParameters);
            var keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
            keyGen.Init(kgp);

            var serverKeyPair = keyGen.GenerateKeyPair();
            var serverKeyAgree = AgreementUtilities.GetBasicAgreement("DH");
            serverKeyAgree.Init(serverKeyPair.Private);
            
            var clientKeyPair = keyGen.GenerateKeyPair();
            var clientKeyAgree = AgreementUtilities.GetBasicAgreement("DH");
            clientKeyAgree.Init(clientKeyPair.Private);

            var serverAgree = serverKeyAgree.CalculateAgreement(clientKeyPair.Public);
            var clientAgree = clientKeyAgree.CalculateAgreement(serverKeyPair.Public);
            
            Assert.Equal(serverAgree, clientAgree);
        }
    }
}