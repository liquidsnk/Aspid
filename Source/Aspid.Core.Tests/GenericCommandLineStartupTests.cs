#region License
#endregion

using System;

using NUnit.Framework;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class GenericCommandLineStartupTests
    {
        class FailingSetupStartupMock : GenericCommandLineStartup
        {
            public bool ApplicationSetupWasCalled;
            public bool ApplicationRunWasCalled;

            protected override int ApplicationSetup(string[] args)
            {
                ApplicationSetupWasCalled = true;
                throw new NotImplementedException();
            }

            protected override int ApplicationRun()
            {
                ApplicationRunWasCalled = true;
                return 0;
            }
        }

        class FailingRunStartupMock : GenericCommandLineStartup
        {
            public bool ApplicationSetupWasCalled;
            public bool ApplicationRunWasCalled;

            protected override int ApplicationSetup(string[] args)
            {
                ApplicationSetupWasCalled = true;
                return 0;
            }

            protected override int ApplicationRun()
            {
                ApplicationRunWasCalled = true;
                throw new NotImplementedException();
            }
        }

        class FailingBothWithReturnDelegatesMock : GenericCommandLineStartup
        {
            public FailingBothWithReturnDelegatesMock()
            {
                GetReturnCodeForSetupOnUncaughtException = (x => 2);
                GetReturnCodeForRunOnUncaughtException = (x => 3);
            }

            protected override int ApplicationSetup(string[] args)
            {
                throw new NotImplementedException();
            }

            protected override int ApplicationRun()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void Setup_WhenTheresAnException_Returns1ByDefault()
        {
            var sut = new FailingSetupStartupMock();
            Assert.AreEqual(1, sut.Setup(null));
        }

        [Test]
        public void Setup_WhenTheresAnException_CallsCorrespondingDelegateToObtainReturnValue()
        {
            var sut = new FailingBothWithReturnDelegatesMock();
            Assert.AreEqual(2, sut.Setup(null));
        }

        [Test]
        public void Run_WhenTheresAnException_Returns1ByDefault()
        {
            var sut = new FailingRunStartupMock();
            sut.Setup(null);
            Assert.AreEqual(1, sut.Run());
        }

        [Test]
        public void Run_WhenTheresAnException_CallsCorrespondingDelegateToObtainReturnValue()
        {
            var sut = new FailingBothWithReturnDelegatesMock();
            Assert.AreEqual(3, sut.Run());
        }

        [Test]
        public void SetupAndRun_WhenTheresAnExceptionAtSetup_Returns1ByDefault()
        {
            var sut = new FailingSetupStartupMock();
            Assert.AreEqual(1, sut.SetupAndRun(null));
        }

        [Test]
        public void SetupAndRun_WhenTheresAnExceptionAtRun_Returns1ByDefault()
        {
            var sut = new FailingRunStartupMock();
            Assert.AreEqual(1, sut.SetupAndRun(null));
        }


        [Test]
        public void Setup_WhenCalled_CallsApplicationSetup()
        {
            FailingRunStartupMock sut = new FailingRunStartupMock();
            sut.Setup(null);
            Assert.IsTrue(sut.ApplicationSetupWasCalled);
        }

        [Test]
        public void Run_WhenCalled_CallsApplicationRun()
        {
            FailingRunStartupMock sut = new FailingRunStartupMock();
            sut.Run();
            Assert.IsTrue(sut.ApplicationRunWasCalled);
        }

        [Test]
        public void SetupAndRun_WhenCalled_CallsApplicationSetup()
        {
            FailingRunStartupMock sut = new FailingRunStartupMock();
            sut.SetupAndRun(null);
            Assert.IsTrue(sut.ApplicationSetupWasCalled);
        }

        [Test]
        public void SetupAndRun_WhenCalledAndSetupSucceding_CallsApplicationRun()
        {
            FailingRunStartupMock sut = new FailingRunStartupMock();
            sut.SetupAndRun(null);
            Assert.IsTrue(sut.ApplicationRunWasCalled);
        }

        [Test]
        public void SetupAndRun_WhenCalledAndSetupFails_DoesNotCallApplicationRun()
        {
            FailingSetupStartupMock sut = new FailingSetupStartupMock();
            sut.SetupAndRun(null);
            Assert.IsTrue(sut.ApplicationSetupWasCalled);
            Assert.IsFalse(sut.ApplicationRunWasCalled);
        }
    }
}
