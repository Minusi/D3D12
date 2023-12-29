using GameEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.GameProject
{
	[DataContract]
	public class ProjectTemplate
	{
		[DataMember]
		public string ProjectType { get; set; }
		[DataMember]
		public string ProjectFile { get; set; }
		[DataMember]
		public List<string> Folders { get; set; }

		public byte[] Icon { get; set; }
		public byte[] ScreenShot { get; set; }
		public string IconFilePath { get; set; }
		public string ScreenShotFilePath { get; set; }
		public string ProjectFilePath { get; set; }
	}

	internal class CreateProject : ViewModelBase
	{
		// TODO: get the path from the installation location
		private readonly string _templatePath = @"..\..\..\PrimalEditor\ProjectTemplates\";
		private string _projectName = "NewProject";
		public string ProjectName
		{
			get { return _projectName; }
			set
			{
				if (_projectName != value)
				{
					_projectName = value;
					OnPropertyChanged(nameof(ProjectName));
				}
			}
		}

		private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProject\";
		public string ProjectPath
		{
			get { return _projectPath; }
			set
			{
				if(_projectPath != value) 
				{
					_projectPath = value;
					OnPropertyChanged(nameof(ProjectPath));
				}
			}
		}

		private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
		public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; } 

	
		public CreateProject()
		{
			ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
			try
			{
				var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
				Debug.Assert(templatesFiles.Any());
				foreach (var file in  templatesFiles)
				{
					var template = Serializer.FromFile<ProjectTemplate>(file);
					template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
					template.Icon = File.ReadAllBytes(template.IconFilePath);
					template.ScreenShotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "ScreenShot.png"));
					template.ScreenShot = File.ReadAllBytes(template.ScreenShotFilePath);
					template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

					_projectTemplates.Add(template);
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				// TODO: log error
			}
		}
	}

}
