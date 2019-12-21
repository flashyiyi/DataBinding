using System;
using System.Collections.Generic;

namespace DataBinding
{
    struct BindTarget
    {
        public BindAble target;
        public Action<PropertyChangedEvent> action;

        public void Bind()
        {
            target.onPropertyChanged += action;
        }

        public void UnBind()
        {
            target.onPropertyChanged -= action;
        }
    }

    public class BindHandler
    {
        internal static BindHandler recordingBindHandler = null;

        List<BindTarget> bindTargets = new List<BindTarget>();
        public Action<PropertyChangedEvent> action;

        public BindHandler AddTarget(BindAble target, string propertyName = null)
        {
            var bindTarget = new BindTarget()
            {
                target = target,
                action = e =>
                {
                    if (propertyName == null || propertyName == e.propertyName)
                    {
                        action(e);
                    }
                }
            };

            bindTarget.Bind();
            bindTargets.Add(bindTarget);
            return this;
        }

        public BindHandler AddTarget<T>(Func<T> expression)
        {
            AddExpressionListener(expression);
            return this;
        }

        public BindHandler SetBindAction(Action<PropertyChangedEvent> action)
        {
            this.action = action;
            return this;
        }

        public BindHandler SetBindProperty<T>(Action<T> setter, Func<T> getter)
        {
            SetBindAction(e => setter(getter()));
            setter(AddExpressionListener(getter));
            return this;
        }

        T AddExpressionListener<T>(Func<T> expression)
        {
            recordingBindHandler = this;
            var result = expression.Invoke();
            recordingBindHandler = null;
            return result;
        }

        public void UnBind()
        {
            foreach (var item in bindTargets)
            {
                item.UnBind();
            }
            bindTargets.Clear();
        }        
    }
}
