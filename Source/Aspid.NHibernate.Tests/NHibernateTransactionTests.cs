#region License
#endregion

using System;
using Moq;
using NHibernate;
using NUnit.Framework;

namespace Aspid.NHibernate.Tests
{
    [TestFixture]
    public class NHibernateTransactionTests
    {
        Mock<ITransaction> transactionMock;

        [SetUp]
        public void SetUp()
        {
            transactionMock = new Mock<ITransaction>();
            transactionMock.SetupGet(x => x.IsActive).Returns(true);
        }

        [Test]
        public void Created_WithNullTransaction_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new NHibernateTransaction(null));
        }

        [Test]
        public void WhenCreated_WithActiveTransaction_IsActive()
        {
            using (var sut = new NHibernateTransaction(transactionMock.Object))
            {
                Assert.IsTrue(sut.IsActive);
            }
        }

        [Test]
        public void WhenCreated_WithInactiveTransaction_IsNotActive()
        {
            transactionMock.SetupGet(x => x.IsActive).Returns(false);

            using (var sut = new NHibernateTransaction(transactionMock.Object))
            {
                Assert.IsFalse(sut.IsActive);
            }
        }

        [Test]
        public void Commit_CallsUnderlyingTransactionCommitMethod()
        {
            using (var sut = new NHibernateTransaction(transactionMock.Object))
            {
                sut.Commit();

                Assert.IsFalse(sut.IsActive);
            }

            transactionMock.Verify(x => x.Commit());
        }

        [Test]
        public void Rollback_CallsUnderlyingTransactionRollbackMethod()
        {
            using (var sut = new NHibernateTransaction(transactionMock.Object))
            {
                sut.Rollback();

                Assert.IsFalse(sut.IsActive);
            }

            transactionMock.Verify(x => x.Rollback());
        }

        [Test]
        public void Commit_GivenAnInactiveTransaction_ThrowsInvalidOperationException()
        {
            transactionMock.SetupGet(x => x.IsActive).Returns(false);

            using (var sut = new NHibernateTransaction(transactionMock.Object))
            {
                Assert.Throws<InvalidOperationException>(() => sut.Commit());
            }
        }

        [Test]
        public void Rollback_GivenAnInactiveTransaction_ThrowsInvalidOperationException()
        {
            transactionMock.SetupGet(x => x.IsActive).Returns(false);

            using (var sut = new NHibernateTransaction(transactionMock.Object))
            {
                Assert.Throws<InvalidOperationException>(() => sut.Rollback());
            }
        }

        [Test]
        public void Dispose_DisposesUnderlyingTransaction()
        {
            var sut = new NHibernateTransaction(transactionMock.Object);
            sut.Dispose();

            Assert.IsFalse(sut.IsActive);
            transactionMock.Verify(x => x.Dispose());
        }
    }
}
