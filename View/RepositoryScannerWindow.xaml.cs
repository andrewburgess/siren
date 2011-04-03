using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Controller;
using Controller.Interfaces;

namespace View
{
	/// <summary>
	/// Interaction logic for RepositoryScannerWindow.xaml
	/// </summary>
	public partial class RepositoryScannerWindow : IRepositoryScannerWindow
	{
		private RepositoryScannerController Controller { get; set; }

		public RepositoryScannerWindow()
		{
			InitializeComponent();
			Controller = new RepositoryScannerController(this);
		}

		#region Implementation of IRepositoryScannerWindow

		public void SetCurrentRepository(string currentRepository)
		{
			CurrentRepository.Content = currentRepository;
		}

		public void SetCurrentPath(string currentPath)
		{
			CurrentPath.Content = currentPath;
		}

		public void SetProgress(int progressPercentage)
		{
			Progress.Value = progressPercentage;
		}

		#endregion
	}
}