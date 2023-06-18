using Loom.Core;
using Loom.GameDev.Model;
using Loom.GameProject.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Loom.GameDev.View
{
    public partial class NewScriptDialog : Window
    {
        private static readonly string _cppCode = @"#include ""{0}.h""

namespace {1}
{{
	REGISTER_SCRIPT({0});

	void {0}::begin_play()
	{{
		// initialization logic
	}}

	void {0}::update(float dt)
	{{
		// update logic
	}}
}}";

        private static readonly string _hCode = @"#pragma once

#include ""GameEntity.h""

namespace {1}
{{
	class {0} : public fabric::script::entity_script
	{{
	public:
		constexpr {0}(fabric::entity::entity entity)
			: fabric::script::entity_script(entity)
		{{
			// pre initialization logic
		}}

		virtual void begin_play() override;
		virtual void update(float dt) override;
	}};
}}";

        private static readonly string _namespace = GetNamespaceFromProjectName();

        private static string GetNamespaceFromProjectName()
        {
            var projectName = Project.Current.ProjectName;
            projectName = projectName.Replace(' ', '_');

            return projectName;
        }

        public NewScriptDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            pathTextBox.Text = @"Source\";
        }

        private bool Validate()
        {
            bool isValid = false;
            var name = scriptTextbox.Text.Trim();
            var path = pathTextBox.Text.Trim();
            messageTextBlock.Text = string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                messageTextBlock.Text = "Type in a script name.";
            }
            else if ((name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) || name.Any(x => char.IsWhiteSpace(x)))
            {
                messageTextBlock.Text = "Invalid character(s) used in script name.";
            }
            if (string.IsNullOrEmpty(path))
            {
                messageTextBlock.Text = "Select a valid location.";
            }
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                messageTextBlock.Text = "Invalid character(s) used in script path.";
            }
            else if (!Path.GetFullPath(Path.Combine(Project.Current.ProjectPath, path)).Contains(Path.Combine(Project.Current.ProjectPath, @"Source\")))
            {
                messageTextBlock.Text = "Script must be added to (a sub-folder of) Source.";
            }
            else if (Path.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.ProjectPath, path), $"{name}.cpp"))) ||
                Path.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.ProjectPath, path), $"{name}.h"))))
            {
                messageTextBlock.Text = $"Script {name} already exists in this folder.";
            }
            if (string.IsNullOrEmpty(messageTextBlock.Text))
            {
                isValid = true;
            }

            return isValid;
        }

        private void OnName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Validate()) return;

            var name = scriptTextbox.Text.Trim();

            messageTextBlock.Text = $"{name}.h and {name}.cpp will be added to {Project.Current.ProjectName}";
        }

        private void OnBrowseButtonClicked(object sender, RoutedEventArgs e)
        {
            FolderBrowser.Browse(pathTextBox);
        }

        private async void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;
            IsEnabled = false;

            try
            {
                var name = scriptTextbox.Text;
                var path = Path.GetFullPath(Path.Combine(Project.Current.ProjectPath, pathTextBox.Text.Trim()));
                var solution = Project.Current.Solution;
                var projectName = Project.Current.ProjectName;

                await Task.Run(() => CreateScript(name, path, solution, projectName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to create script {scriptTextbox.Text}.");
            }
        }

        private void CreateScript(string name, string path, string solution, string projectName)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var cpp = Path.GetFullPath(Path.Combine(path, $"{name}.cpp"));
            var h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

            using (var sw = File.CreateText(cpp))
            {
                sw.Write(string.Format(_cppCode, name, _namespace));
            }

            using (var sw = File.CreateText(h))
            {
                sw.Write(string.Format(_hCode, name, _namespace));
            }

            string[] files = new string[] { h, cpp };

            for (int i = 0; i < 3; i++)
            {
                if (!VisualStudio.AddFilesToSolution(solution, projectName, files)) System.Threading.Thread.Sleep(1000);
                else break;
            }
        }

        private void OnPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }
    }
}
