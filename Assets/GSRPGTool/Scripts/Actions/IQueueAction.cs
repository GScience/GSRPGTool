namespace RPGTool.Actions
{
    public interface IQueueAction
    {
        void OnStart();
        void OnFinished();
        bool IsFinished();
    }
}