#region License
#endregion

using System;

using NUnit.Framework;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class OnDisposeActionTests
    {
        [Test]
        public void Created_WithNullAction_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OnDisposeAction(null));
        }

        [Test]
        public void Dispose_BeforeItsCalled_DoesntPerformAction()
        {
            bool testValue = false;
            var disposable = new OnDisposeAction(() => testValue = true);
            Assert.IsFalse(testValue);
        }

        [Test]
        public void Dispose_CalledOnce_PerformsAction()
        {
            bool testValue = false;
            var disposable = new OnDisposeAction(() => testValue = true);
            
            disposable.Dispose();

            Assert.IsTrue(testValue);
        }

        [Test]
        public void Dispose_CalledTwice_ThrowsInvalidOperationException()
        {
            var disposable = new OnDisposeAction(() => { });

            disposable.Dispose();
            Assert.Throws<InvalidOperationException>(() => disposable.Dispose());
        }
    }
}
