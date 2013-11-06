using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickCross
{
    public abstract class ApplicationBase
    {
        private static object _syncRoot = new Object();
        private static volatile ApplicationBase _instance;
        private TaskScheduler _uiTaskScheduler;

        /// <summary>
        /// Initializes the application singleton and the UI thread scheduler
        /// </summary>
        /// <param name="uiTaskScheduler">Optional. If omitted, ensure that the application is always created on the UI thread.</param>
        protected ApplicationBase(object currentNavigationContext = null, TaskScheduler uiTaskScheduler = null)
        {
            lock (_syncRoot) 
            {
                if (_instance != null) throw new InvalidOperationException("No more than one instance of ApplicationBase may be created.");
                _instance = this;
            }

            CurrentNavigationContext = currentNavigationContext;
            _uiTaskScheduler = (uiTaskScheduler != null) ? uiTaskScheduler : TaskScheduler.FromCurrentSynchronizationContext();
            // We could add a platform-specific check here to ensure that _uiTaskScheduler can access the UI thread - e.g. try something that requires UI acces and catch the exception, rethrow with clear error message.
        }

        public static ApplicationBase Instance { get { return _instance; } }

        public static Task RunOnUIThread(Action action)
        {
            return (Instance == null) ?
                       Task.Factory.StartNew(action) : // This is intended for a runtime context where no Application instance exists and we are already in a UI thread - i.e. when code is run at design-time in Visual Studio
                       Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, Instance._uiTaskScheduler);
        }

        public object CurrentNavigationContext { get; set; }
    }
}
