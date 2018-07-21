using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace APKViewer
{
	public abstract class BindableModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		private Dictionary<string, string> _errors = new Dictionary<string, string>();

		public bool HasErrors => _errors.Count > 0;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (!string.IsNullOrEmpty(propertyName))
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public IEnumerable GetErrors(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
				return null;
			return new string[] { _errors[propertyName] };
		}

		protected void SetError(string errorMsg, [CallerMemberName] string propertyName = "")
		{
			if (string.IsNullOrEmpty(propertyName))
				return;

			if (_errors.ContainsKey(propertyName) && string.IsNullOrEmpty(errorMsg))
			{
				_errors.Remove(propertyName);
			}
			else
			{
				_errors[propertyName] = errorMsg;
			}

			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}
	}
}
