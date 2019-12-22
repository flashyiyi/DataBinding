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

        //添加一个绑定目标
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

        //根据表达式添加绑定目标，如 () => obj1.a + obj2.b
        public BindHandler AddTarget<T>(Func<T> expression)
        {
            AddExpressionListener(expression);
            return this;
        }

        //指定绑定后执行的回调，必须配合AddTarget使用
        public BindHandler BindAction(Action<PropertyChangedEvent> action)
        {
            this.action = action;
            return this;
        }

        //属性绑定，setter设置值，getter获得绑定源
        //例子：BindProperty(v => obj2.a = v, () => obj1.a)，表示obj2.a永远等于obj1.a
        public BindHandler BindProperty<T>(Action<T> setter, Func<T> getter)
        {
            BindAction(e => setter(getter()));
            setter(AddExpressionListener(getter));
            return this;
        }

        //属性绑定,setter是从绑定源复制数据到目标的代码
        //例子：BindProperty(() => obj2.a = obj1.a)，表示obj2.a永远等于obj1.a
        //必须保证obj2是一个局部变量而不是属性
        public BindHandler BindProperty(Action setter)
        {
            BindAction(e => setter());
            recordingBindHandler = this;
            setter.Invoke();
            recordingBindHandler = null;
            return this;
        }

        T AddExpressionListener<T>(Func<T> expression)
        {
            recordingBindHandler = this;
            var result = expression.Invoke();
            recordingBindHandler = null;
            return result;
        }

        //移除绑定
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
