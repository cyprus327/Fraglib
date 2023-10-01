using System.Collections.Concurrent;
using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class DrawClearEngine : Engine {
    public DrawClearEngine(int w, int h, string t, Action program) : base(w, h, t) {
        _program = program;
    }

    public bool Multithreaded { get; set; } = false;
    
    private readonly Action _program;

    private Action[] actions = new Action[1024];
    private int actionCount = 0, lastActionCount = 0;
    private CustomThreadPool? threadPool = null;

    public void StartThreadPool() {
        threadPool = new CustomThreadPool(Environment.ProcessorCount);
    }

    public void AddAction(Action a) {
        if (!Multithreaded) {
            a();
            return;
        }

        if (actionCount >= actions.Length) {
            Array.Resize(ref actions, actions.Length + 1024);
        }
        actions[actionCount++] = a;
    }

    public override void Update(FrameEventArgs args) {
        _program();

        if (!Multithreaded || actions.Length == 0) {
            return;
        }

        int count = Interlocked.Exchange(ref actionCount, 0);
        for (int i = 0; i < count; i++) {
            threadPool!.AddAction(actions[i]);
        }

        if (count < lastActionCount) {
            Array.Clear(actions, 0, lastActionCount);
        }

        lastActionCount = count;
    }

    public override void OnWindowClose() {
        if (Multithreaded) {
            threadPool!.Stop();
        }
    }
}

internal sealed class CustomThreadPool {
    public CustomThreadPool(int numThreads) {
        _threads = new Thread[numThreads];
        for (int i = 0; i < numThreads; i++) {
            _threads[i] = new Thread(Worker);
            _threads[i].Start();
        }
    }

    private readonly Thread[] _threads;
    private readonly ManualResetEventSlim _signal = new(false);
    private readonly ConcurrentQueue<Action> _queue = new();
    private readonly object _queueLock = new();
    private readonly object _stoppingLock = new();
    private volatile bool stopping = false;

    public void AddAction(Action a) {
        lock (_queueLock) {
            if (_queue.Count >= 512) {
                Monitor.Wait(_queueLock);
            }
            
            _queue.Enqueue(a);
            _signal.Set();
        }
    }

    public void Stop() {
        lock (_stoppingLock) {
            stopping = true;
            _signal.Set();
        }

        foreach (var thread in _threads) {
            thread.Join();
        }
    }

    private void Worker() {
        while (true) {
            _signal.Wait();

            if (stopping) {
                break;
            }

            while (_queue.TryDequeue(out var action)) {
                action();
            }

            lock (_queueLock) {
                Monitor.Pulse(_queueLock);
            }

            _signal.Reset();
        }
    }
}