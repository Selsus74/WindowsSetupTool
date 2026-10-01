using System;
using System.Threading;

//---progress info reports to ui---
public readonly record struct SetupProgressInfo(int Done, int Total, int Failed);

//---class init---
public class SetupProgressHelper
{
    private readonly IProgress<SetupProgressInfo> _progress;
    private readonly SynchronizationContext? _context = SynchronizationContext.Current;
    private readonly object _lock = new();

    private int _total;
    private int _done;
    private int _failed;

    //---fires 1x when every task reported it is done. Parameter: failed tasks---
    //---called in the UI-Thread---
    public event Action<int>? AllTasksCompleted;

    //---helper musst be created in UI-Thread---
    public SetupProgressHelper(IProgress<SetupProgressInfo> progress)
    {
        _progress = progress;
    }

    //---Setter: sets max task count (all selected in UI) and resets progress---
    //---must be set before the tasks are started---
    public int TotalTasks
    {
        set
        {
            lock (_lock)
            {
                _total = value;
                _done = 0;
                _failed = 0;
                _progress.Report(new SetupProgressInfo(0, _total, 0));
            }
        }
    }

    //---task is successful: call EXACTLY 1x per completed task---
    //---(the same task should not be able to call both)---
    public void ReportTaskCompleted() => Report(failed: false);

    //---task is failed: call EXACTLY 1x per failed task---
    public void ReportTaskFailed() => Report(failed: true);

    //---internal report method (used in the wrapper on top)---
    private void Report(bool failed)
    {
        lock (_lock)
        {
            //---calls beyond total limit or if no total was set are discarded---
            if (_total == 0 || _done >= _total)
                return;

            _done++;
            if (failed) _failed++;

            _progress.Report(new SetupProgressInfo(_done, _total, _failed));

            if (_done == _total)
            {
                int failedCount = _failed;

                //---same context as Progress<T> post---
                //---Event after last update---
                if (_context != null)
                    _context.Post(_ => AllTasksCompleted?.Invoke(failedCount), null);
                else
                    AllTasksCompleted?.Invoke(failedCount);
            }
        }
    }
}