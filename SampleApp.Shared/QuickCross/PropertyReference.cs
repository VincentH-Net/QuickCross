using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QuickCross
{
	public class PropertyReference
	{
		/// <summary>Gets the name of a property or field in a typesafe manner</summary>
		/// <param name="expression"> A Linq expression that specifies the name of a property or field in a typesafe manner, 
        /// e.g.: <example>() => obj.AProperty</example> or <example>() => obj.AField</example>
		/// </param>
		/// <returns>The name of the property or field specified in the expression</returns>
		public static string GetMemberName(Expression<Func<object>> expression)
		{
            if (expression == null) return null;
			var unaryX = expression.Body as UnaryExpression;
			var memberX = (unaryX != null) ? unaryX.Operand as MemberExpression : expression.Body as MemberExpression;
			if (memberX == null || memberX.Member == null) throw new ArgumentException("Invalid expression for MemberName:\n" + expression.ToString() + "\nValid expressions are e.g.:\n() => obj.AProperty\n() => obj.AField");
			return memberX.Member.Name;
		}

		private PropertyInfo propertyInfo;
		private FieldInfo fieldInfo;
        private Type convertType, convertBackType;
        private bool typesDiffer;

		public object ContainingObject { get; private set; }
		public string PropertyOrFieldName { get; private set; }

		public object Value
		{
			get
			{
                object val;
				if (propertyInfo != null) val = propertyInfo.GetValue(ContainingObject);
				else if (fieldInfo != null) val = fieldInfo.GetValue(ContainingObject);
				else throw new InvalidOperationException("Cannot access the Value because no PropertyOrFieldName was specified");

                return convertBackType != null && typesDiffer ? Convert.ChangeType(val, convertBackType) : val;
			}

			set
			{
                object val = convertType != null && typesDiffer ? Convert.ChangeType(value, convertType) : value;

                if (Value == val) return;
				if (propertyInfo != null) propertyInfo.SetValue(ContainingObject, val);
				else fieldInfo.SetValue(ContainingObject, val);
			}
		}

        public PropertyReference(object containingObject, string propertyOrFieldName, Type convertBackType = null)
		{
			ContainingObject = containingObject;
			PropertyOrFieldName = propertyOrFieldName;
			if (string.IsNullOrEmpty(PropertyOrFieldName)) return;

            this.convertBackType = convertBackType;

			var type = ContainingObject.GetType();
			propertyInfo = type.GetProperty(PropertyOrFieldName);
			if (propertyInfo != null) {
                convertType = propertyInfo.PropertyType;
            } else {
				fieldInfo = type.GetField(PropertyOrFieldName);
				if (fieldInfo == null) throw new ArgumentException(string.Format("The type {0} has no property or field named '{1}'", type.FullName, PropertyOrFieldName), "propertyOrFieldName");
                convertType = fieldInfo.FieldType;
			}

            typesDiffer = convertType != convertBackType;
		}

		/// <summary>Constructor</summary>
		/// <param name="containingObject">The object instance that contains the property or field</param>
		/// <param name="propertyOrField">A Linq expression that specifies the name of the property or field in a typesafe manner,
        /// e.g.: <example>() => obj.AProperty</example> or <example>() => obj.AField</example>
		/// </param>
        public PropertyReference(object containingObject, Expression<Func<object>> propertyOrField, Type convertBackType = null)
			: this(containingObject, GetMemberName(propertyOrField), convertBackType)
		{ }
	}
}