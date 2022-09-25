using System;
using System.Windows;

namespace APKViewer.WPFApp.Services;

public class WindowsMessageBox: IMessageBoxService
{
	public bool OpenDialog(string message)
	{
		MessageBoxResult result = MessageBox.Show(message, String.Empty, MessageBoxButton.OK);
		return result == MessageBoxResult.Yes || result == MessageBoxResult.OK;
	}
}