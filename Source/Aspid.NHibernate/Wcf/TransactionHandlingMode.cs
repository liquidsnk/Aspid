#region License
#endregion

namespace Aspid.NHibernate.Wcf
{
    public enum TransactionHandlingMode
    {
        Manual = 0,
        AutomaticallyRollbackOnError = 1,
        AutomaticallyCommitOnSuccess = 2,
        Automatic = AutomaticallyRollbackOnError | AutomaticallyCommitOnSuccess
    }
}
