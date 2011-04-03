namespace Controller.Interfaces
{
	public interface IRepositoryScannerWindow
	{
		void SetCurrentRepository(string currentRepository);
		void SetCurrentPath(string currentPath);
		void SetProgress(int progressPercentage);
		void Close();
	}
}