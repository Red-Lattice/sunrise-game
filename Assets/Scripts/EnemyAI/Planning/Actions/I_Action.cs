public interface I_Action
{
    public abstract bool CanExecute();
    public abstract void HaltAction();
    public abstract void ExecuteAction();
    public abstract bool IsExecuted();
    public abstract bool IsExecuting();
}
