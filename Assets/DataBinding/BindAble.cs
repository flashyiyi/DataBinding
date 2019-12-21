using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DataBinding
{
    public struct PropertyChangedEvent
    {
        public object currentTarget;
        public object target;
        public string propertyName;
    }
    public class BindAble
    {
        public Action<PropertyChangedEvent> onPropertyChanged;
        protected T GetProperty<T>(ref T property, [CallerMemberName]string propertyName = null)
        {
            if (BindHandler.recordingBindHandler != null)
            {
                BindHandler.recordingBindHandler.AddTarget(this, propertyName);
            }
            return property;
        }

        protected void SetProperty<T>(ref T property, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property,value))
                return;

            property = value;
            TriggerPropertyChange(this,propertyName);
        }

        public void TriggerPropertyChange(object target,string propertyName = null)
        {
            onPropertyChanged?.Invoke(new PropertyChangedEvent() { currentTarget = this, target = target, propertyName = propertyName });
        }
    }
}
