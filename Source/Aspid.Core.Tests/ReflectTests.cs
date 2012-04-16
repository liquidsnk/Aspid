#region License
/*
 * Original source code obtained from Clarius labs at: http://clarius.codeplex.com/
 */
#endregion

using System;
using System.Linq.Expressions;
using System.Reflection;

using NUnit.Framework;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class ReflectTests
    {
        [Test]
        public void ReflectMethod_GivenNullMethodLambda_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Reflect<Mock>.Method((Expression<Action<Mock>>)null));
        }

        [Test]
        public void ReflectProperty_GivenNullPropertyLambda_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Reflect<Mock>.Property((Expression<Func<Mock, object>>)null));
        }

        [Test]
        public void ReflectField_GivenNullFieldLambda_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Reflect<Mock>.Field((Expression<Func<Mock, object>>)null));
        }

        [Test]
        public void ReflectMethod_GivenNonMethodLambda_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Reflect<Mock>.Method(x => new object()));
        }

        [Test]
        public void ReflectMethod_GivenNonPropertyLambda_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Reflect<Mock>.Property(x => x.PublicField));
        }

        [Test]
        public void ReflectMethod_GivenNonFieldLambda_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Reflect<Mock>.Field(x => x.PublicProperty));
        }

        [Test]
        public void ReflectProperty_GivenPublicProperty_GetsPropertyInfo()
        {
            PropertyInfo info = Reflect<Mock>.Property(x => x.PublicProperty);
            Assert.AreSame(info, typeof(Mock).GetProperty("PublicProperty"));
        }

        [Test]
        public void ReflectField_GivenPublicField_GetsFieldInfo()
        {
            FieldInfo info = Reflect<Mock>.Field(x => x.PublicField);
            Assert.AreSame(info, typeof(Mock).GetField("PublicField"));
        }

        [Test]
        public void ReflectMethod_GivenPublicVoidMethodLambda_GetsMethodInfo()
        {
            MethodInfo info = Reflect<Mock>.Method(x => x.PublicVoidMethod());
            Assert.AreSame(info, typeof(Mock).GetMethod("PublicVoidMethod"));
        }

        [Test]
        public void ReflectMethod_GivenPublicParameterlessMethodWithReturn_GetsMethodInfo()
        {
            MethodInfo info = Reflect<Mock>.Method(x => x.PublicMethodNoParameters());
            Assert.AreSame(info, typeof(Mock).GetMethod("PublicMethodNoParameters"));
        }

        [Test]
        public void ReflectMethod_GivenPublicMethodWithReturnAndParameters_GetsMethodInfo()
        {
            MethodInfo info = Reflect<Mock>.Method<string, int>(
                (x, y, z) => x.PublicMethodParameters(y, z));
            Assert.AreSame(info, typeof(Mock).GetMethod("PublicMethodParameters", new Type[] { typeof(string), typeof(int) }));
        }

        [Test]
        public void ReflectProperty_GivenNonPublicProperty_GetsPropertyInfo()
        {
            PropertyInfo info = Reflect<ReflectTests>.Property(x => x.NonPublicProperty);
            Assert.AreSame(info, typeof(ReflectTests).GetProperty("NonPublicProperty", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Test]
        public void ReflectField_GivenNonPublicField_GetsFieldInfo()
        {
            FieldInfo info = Reflect<ReflectTests>.Field(x => x.NonPublicField);
            Assert.AreSame(info, typeof(ReflectTests).GetField("NonPublicField", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Test]
        public void ReflectMethod_GivenNonPublicMethod_GetsMethodInfo()
        {
            MethodInfo info = Reflect<ReflectTests>.Method(x => x.NonPublicMethod());
            Assert.AreSame(info, typeof(ReflectTests).GetMethod("NonPublicMethod", BindingFlags.Instance | BindingFlags.NonPublic));
        }

        [Test]
        public void ReflectPropertyPath_GivenPublicPropertyAccess_ReturnsPropertyPath()
        {
            Assert.AreEqual("OuterProperty", Reflect<Mock>.PropertyPath(x => x.OuterProperty));
        }

        [Test]
        public void ReflectPropertyPath_GivenNestedPropertyAccess_ReturnsPropertyPath()
        {
            Assert.AreEqual("OuterProperty.InnerProperty", Reflect<Mock>.PropertyPath(x => x.OuterProperty.InnerProperty));
        }

        private int NonPublicField;
        private int NonPublicProperty
        {
            get { return NonPublicField; }
            set { NonPublicField = value; }
        }

        private object NonPublicMethod()
        {
            throw new NotImplementedException();
        }

        class Mock
        {
            public class InnerClass
            {
                public InnerClass InnerProperty { get; set; }
            }

            public InnerClass OuterProperty { get; set; }

            public bool PublicField = false;
            private int valueProp;
            
            public Mock()
            {
            }        

            public int PublicProperty
            {
                get { return valueProp; }
                set { valueProp = value; }
            }

            public bool PublicMethodNoParameters()
            {
                throw new NotImplementedException();
            }

            public bool PublicMethodParameters(string foo, int bar)
            {
                throw new NotImplementedException();
            }

            public void PublicVoidMethod()
            {
                throw new NotImplementedException();
            }
        }
    }
}
