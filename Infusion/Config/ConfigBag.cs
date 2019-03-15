using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Infusion.Config
{
    public sealed class ConfigBag
    {
        private readonly Dictionary<string, PropertyInfo> expressions
            = new Dictionary<string, PropertyInfo>();
        private readonly IConfigBagRepository repository;

        public ConfigBag(IConfigBagRepository repository)
        {
            this.repository = repository;
        }

        public void Register<T>(Expression<Func<T>> expression) where T : new()
            => Register(expression, () => new T());

        public void Register(Expression<Func<int>> expression)
            => Register(expression, () => 0);

        public void Register(Expression<Func<string>> expression)
            => Register(expression, () => null);

        public void Register<T>(Expression<Func<T>> expression, Func<T> getDefaultValue)
        {
            if (expression.Body is MemberExpression memberExpression && memberExpression.Member is PropertyInfo propertyInfo)
            {
                if (memberExpression.Expression == null)
                {
                    var name = expression.Body.ToString();
                    expressions[name] = propertyInfo;
                    var value = repository.Get<T>(name);
                    if (value == null)
                        value = getDefaultValue();
                    propertyInfo.SetValue(null, value);
                }
                else
                    throw new NotSupportedException($"Instance level property configuration not supported: {expression.Body.ToString()}");
            }
            else
                throw new NotSupportedException($"Unsupported configuration expression {expression.Body.ToString()}");
        }

        public void Save()
        {
            foreach (var pair in expressions)
            {
                repository.Update(pair.Key, pair.Value.GetValue(null));
            }
        }
    }
}
