using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Infusion.Config
{
    public sealed class ConfigBag
    {
        private struct ConfigProperty
        {
            public ConfigProperty(PropertyInfo property, object instance)
            {
                Property = property;
                Instance = instance;
            }

            public PropertyInfo Property { get; }
            public object Instance { get; }
        }

        private readonly Dictionary<string, ConfigProperty> expressions
            = new Dictionary<string, ConfigProperty>();
        private readonly IConfigBagRepository repository;

        public ConfigBag(IConfigBagRepository repository)
        {
            this.repository = repository;
        }

        public void Register<T>(Expression<Func<T>> expression) where T : new()
            => Register(expression, () => new T());
        public void Register<T>(string name, Expression<Func<T>> expression) where T : new()
            => Register(expression, () => new T(), name);

        public void Register(Expression<Func<int>> expression)
            => Register(expression, () => 0);
        public void Register(string name, Expression<Func<int>> expression)
            => Register(expression, () => 0, name);

        public void Register(Expression<Func<string>> expression)
            => Register(expression, () => null);
        public void Register(string name, Expression<Func<string>> expression)
            => Register(expression, () => null, name);

        public void Register<T>(Expression<Func<T>> expression, Func<T> getDefaultValue)
        {
            Register(expression, getDefaultValue, expression.Body.ToString());
        }

        public void Register<T>(Expression<Func<T>> expression, Func<T> getDefaultValue, string name)
        {
            if (expression.Body is MemberExpression memberExpression && memberExpression.Member is PropertyInfo propertyInfo)
            {
                object instance = (memberExpression.Expression as ConstantExpression)?.Value;
                name = name ?? expression.Body.ToString();
                expressions[name] = new ConfigProperty(propertyInfo, instance);
                var value = repository.Get<T>(name);
                if (value == null)
                    value = getDefaultValue();
                propertyInfo.SetValue(instance, value);
            }
            else
                throw new NotSupportedException($"Unsupported configuration expression {expression.Body.ToString()}");
        }

        public void Save()
        {
            foreach (var pair in expressions)
            {
                repository.Update(pair.Key, pair.Value.Property.GetValue(pair.Value.Instance));
            }
        }
    }
}
