#region License
#endregion

using System;

using Aspid.Core.Extensions;

namespace Aspid.Core
{
    /// <summary>
    /// Represents an object that performs an action when calling it's Dispose method.
    /// Useful to define blocks, scopes, and other things that become more readable when used in conjunction with C# using() statement.
    /// 
    /// e.g.:
    /// using(tx = I_Return_Disposables.GetTransaction())
    /// {
    ///    tx.Commit();
    /// } //Disposable transaction would rollback here if not comitted.
    /// </summary>
    public class OnDisposeAction : IDisposable
    {
        Action action;
        protected Action Action 
        {
            get { return action ?? (() => {}) ;}
            set { action = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public OnDisposeAction(Action action)
        {
            action.ThrowIfNull("action");

            Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDisposeAction"/> class.
        /// </summary>
        protected OnDisposeAction()
            : this(() => { })
        {
        }

        bool disposed;

        /// <summary>
        /// Performs the task defined for this Disposable when it was created.
        /// </summary>
        public void Dispose()
        {
            if (disposed) throw new InvalidOperationException("Disposable action was already performed, the object is now disposed");
            
            Action();
            disposed = true;
        }
    }
}
