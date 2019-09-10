using System;
using System.Reflection;
using System.Text;

namespace CodeInjection.Tests
{
    public interface IInjectionTest
    {
        void TestVoidMethod();

        void TestVoidWithArgs(string stArg, int intArg, SimpleEnum enumArg);

        StringBuilder TestReturnsStringBuilder();

        string TestReturnsString();

        int TestReturnsInt();

        SimpleEnum TestReturnsEnum();

        bool TestReturnsBool();

        void BuggyMethod();
    }
    
    public class InjectedLogic : IInjectedLogic
    {
        public IInjectedLogic BeforeExecute(object target, MethodInfo invokedMethod, object[] parameters, IInjectedLogic next)
        {
            return next;
        }

        public IInjectedLogic ExecutionException(object target, MethodInfo invokedMethod, object[] parameters, Exception e,
            IInjectedLogic previous)
        {
            return previous;
        }

        public IInjectedLogic AfterExecute(object target, MethodInfo invokedMethod, object[] parameters, IInjectedLogic previous)
        {
            return previous;
        }
    }

    public class TestClass : IInjectionTest
    {
        public void TestVoidMethod()
        {
            
        }

        public void TestVoidWithArgs(string stArg, int intArg, SimpleEnum enumArg)
        {
            
        }

        public StringBuilder TestReturnsStringBuilder()
        {
            return new StringBuilder();
        }

        public string TestReturnsString()
        {
            return string.Empty;
        }

        public int TestReturnsInt()
        {
            return 0;
        }

        public SimpleEnum TestReturnsEnum()
        {
            return SimpleEnum.One;
        }

        public bool TestReturnsBool()
        {
            return false;
        }

        public void BuggyMethod()
        {
            throw new Exception();
        }
    }
}