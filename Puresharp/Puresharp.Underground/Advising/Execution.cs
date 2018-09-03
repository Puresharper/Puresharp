using System;

namespace Puresharp.Underground
{
    public class Execution
    {
        private Invocation m_Invocation;
        private object m_Value;
        private Exception m_Exception;

        public Execution(Invocation invocation, object value, Exception exception)
        {
            this.m_Invocation = invocation;
            this.m_Value = value;
            this.m_Exception = exception;
        }

        public Invocation Invocation
        {
            get { return this.m_Invocation; }
        }

        public object Value
        {
            get { return this.m_Value; }
        }

        public Exception Exception
        {
            get { return this.m_Exception; }
        }
    }
}
