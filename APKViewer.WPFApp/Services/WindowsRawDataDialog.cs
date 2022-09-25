namespace APKViewer.WPFApp.Services;

public class WindowsRawDataDialog: IOpenRawDialogService
{
	public void OpenViewRawDialog(MainWindowViewModel viewModel)
	{
		RawDataDialog dialog = new(viewModel);
		dialog.ShowDialog();
	}
}