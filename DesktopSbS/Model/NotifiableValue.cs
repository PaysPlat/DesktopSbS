using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSbS.Model
{
    public class NotifiableValue<T> : INotifyPropertyChanged
    {
        private T val;
        public T Value
        {
            get { return this.val; }
            set
            {
                this.val = value;
                RaisePropertyChanged();
            }

        }

        public static implicit operator NotifiableValue<T>(T value)
        {
            return new NotifiableValue<T> { Value = value };
        }

        public static implicit operator T(NotifiableValue<T> notifiableValue)
        {
            return notifiableValue != null ? notifiableValue.Value : default(T);
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
