using NUnit.Framework;

namespace HelperBitsUT
{
    public class BaseTest
    {
        [OneTimeSetUp]
        public void FullSetup()
        {
        }
        [OneTimeTearDown]
        public void FullTearDown()
        {

        }
    }
}