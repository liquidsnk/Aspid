#region License
#endregion

using System;

using NUnit.Framework;

using Aspid.Core.Entities;

namespace Aspid.Core.Tests.Entities
{
    [TestFixture]
    public class BaseEntityTests
    {
        class TestBaseEntity<T> : BaseEntity<T>
        {
            public TestBaseEntity(T id)
            {
                Id = id;
            }

            public int SomeProperty { get; set; }

            public new bool IsTransient
            {
                get
                {
                    return base.IsTransient;
                }
            }

            public void SetId(T id)
            {
                Id = id;
            }
        }
        
        class AnotherTestBaseEntity<T> : BaseEntity<T>
        {
            public AnotherTestBaseEntity(T id)
            {
                Id = id;
            }
        }

        class AnotherTestBaseEntityDerived<T> : AnotherTestBaseEntity<T>
        {
            public AnotherTestBaseEntityDerived(T id)
                : base(id)
            {
            }
        }

        [Test]
        public void IsTransient_WhenEntityIdIsDefaultForType_ReturnsTrue()
        {
            var entity = new TestBaseEntity<int>(0);
            Assert.IsTrue(entity.IsTransient);
        }

        [Test]
        public void IsTransient_WhenEntityIdIsNotDefaultForType_ReturnsFalse()
        {
            var entity = new TestBaseEntity<int?>(0);
            Assert.IsFalse(entity.IsTransient);
        }

        //Watch out, this behavior may seem strage but is somewhat required to support proxies (e.g. NHibernate lazy loading).
        [Test]
        public void Equals_WhenEntitiesAreOfDiffrentClassButHaveSameId_ReturnsTrue()
        {
            var entity1 = new TestBaseEntity<int>(10);
            var entity2 = new AnotherTestBaseEntity<int>(10);

            Assert.IsTrue(entity1.Equals(entity2));
        }

        [Test]
        public void Equals_PassedANullEntity_ReturnsFalse()
        {
            var entity = new TestBaseEntity<int>(10);
            
            Assert.IsFalse(entity.Equals(null));
        }

        [Test]
        public void Equals_WhenEntitiesAreOfSameClassAndHaveSameId_ReturnsTrue()
        {
            //"SomeProperty" filled to make sure that only the Id affects the Equals
            var entity1 = new TestBaseEntity<int>(10) { SomeProperty = 122 };
            var entity2 = new TestBaseEntity<int>(10) { SomeProperty = 123 };

            Assert.IsTrue(entity1.Equals(entity2));
        }

        [Test]
        public void GetHashCode_WhenChangingPropertiesExceptTheId_ReturnsSameValue()
        {
            var entity = new TestBaseEntity<int>(1);
            int hashCode = entity.GetHashCode();

            entity.SomeProperty = 123;
            Assert.AreEqual(hashCode, entity.GetHashCode());
        }

        [Test]
        public void GetHashCode_WhenAssigningAnId_BecomesTheHashCodeOfTheId()
        {
            //This breaks a rule of GetHashCode implementation, but it's a common one to break.
            //This means that it's not entirely safe to make sets (or anything that depends on the HashCode) of transient objects that become persistent, etc.
            var entity = new TestBaseEntity<int?>(null);
            int hashCode = entity.GetHashCode();

            entity.SetId(123);
            Assert.AreNotEqual(hashCode, entity.GetHashCode());
            Assert.AreEqual(entity.GetHashCode(), entity.Id.GetHashCode());
        }
    }
}
