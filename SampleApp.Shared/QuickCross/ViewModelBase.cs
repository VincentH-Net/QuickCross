using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace QuickCross
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #if __ANDROID__ || __IOS__
        private List<string> propertyNames;
        private List<string> commandNames;

        public void RaisePropertiesChanged()
        {
            if (propertyNames == null)
            {
                propertyNames = new List<string>();
                foreach (var fieldInfo in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                {
                    if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.Name.StartsWith("PROPERTYNAME_"))
                    {
                        propertyNames.Add((string)fieldInfo.GetValue(null));
                    }
                }
            }
            foreach (string propertyName in propertyNames) RaisePropertyChanged(propertyName);
        }

        public List<string> CommandNames
        {
            get
            {
                if (commandNames == null) {
                    commandNames = new List<string>();
                    foreach (var propertyInfo in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (propertyInfo.PropertyType == typeof(RelayCommand))
                        {
                            commandNames.Add(propertyInfo.Name);
                        }
                    }
                }
                return commandNames;
            }
        }

        public T GetPropertyValue<T>(string propertyName)
        {
			var pi = GetType().GetProperty(propertyName);
			return (pi == null) ? default(T) : (T)pi.GetValue(this);
        }

		public void SetPropertyValue(string propertyName, object value)
		{
			var pi = GetType().GetProperty(propertyName);
			if (pi != null) pi.SetValue(this, value);
		}

		public bool ExecuteCommand(string commandName, object parameter = null)
		{
			bool isExecuted = false;
			if (!string.IsNullOrEmpty(commandName))
			{
				var pi = this.GetType().GetProperty(commandName);
				if (pi != null)
				{
					var command = pi.GetValue(this) as RelayCommand;
					if (command != null)
					{
						command.Execute(parameter);
						isExecuted = true;
					}	
				}
			}
			return isExecuted;
		}

        #endif

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) ApplicationBase.RunOnUIThread(() => handler(this, new PropertyChangedEventArgs(propertyName)));
        }
    }
}
