using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stundenplan.Data;

namespace Stundenplan.ViewModels.Abstract
{
    public abstract class DataViewModel<T> : NotifyPropertyChangedObject where T : NotifyPropertyChangedObject
    {
        protected T data;

        protected DataViewModel(T data)
        {
            this.data = data;
            data.PropertyChanged += data_PropertyChanged;
        }

        private void data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Einfach weitergeben, spart schon mal arbeit bei gleichen Property Namen :D
            this.NotifyPropertyChanged(e.PropertyName);
        }

        public override bool Equals(object obj)
        {
            var dataViewModel = obj as DataViewModel<T>;
            if (obj != null)
                return this.data.Equals(dataViewModel.data);
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }
    }
}
