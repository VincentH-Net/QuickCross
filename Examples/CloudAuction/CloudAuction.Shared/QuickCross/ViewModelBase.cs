using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace QuickCross
{
	public class InstancePropertyOrField
	{
		private PropertyInfo propertyInfo;
		private FieldInfo fieldInfo;

		public object Instance { get; private set; }
		public string PropertyOrFieldName { get; private set; }

		public object Value
		{
			get { return propertyInfo != null ? propertyInfo.GetValue(Instance) : fieldInfo.GetValue(Instance); }
			set { if (propertyInfo != null) propertyInfo.SetValue(Instance, value); else fieldInfo.SetValue(Instance, value); }
		}

		public InstancePropertyOrField(object instance, string propertyOrFieldName)
		{
			Instance = instance;
			PropertyOrFieldName = propertyOrFieldName;

			var type = Instance.GetType();
			propertyInfo = type.GetProperty(PropertyOrFieldName);
			if (propertyInfo == null)
			{
				fieldInfo = type.GetField(PropertyOrFieldName);
				if (fieldInfo == null) throw new ArgumentException(string.Format("The type {0} does not have a property or field named '{1}'", type.FullName, PropertyOrFieldName), "propertyOrFieldName");
			}
		}
	}
	
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

		public static string GetMemberName(Expression<Func<object>> expression)
		{
			var unaryX = expression.Body as UnaryExpression;
			var memberX = (unaryX != null) ? unaryX.Operand as MemberExpression : expression.Body as MemberExpression;
			if (memberX == null || memberX.Member == null) throw new ArgumentException("Invalid expression for MemberName:\n" + expression.ToString() + "\nValid expressions are e.g.:\n() => obj.AProperty\n() => obj.AField");
			return memberX.Member.Name;
		}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) ApplicationBase.RunOnUIThread(() => handler(this, new PropertyChangedEventArgs(propertyName)));
        }
    }
}
