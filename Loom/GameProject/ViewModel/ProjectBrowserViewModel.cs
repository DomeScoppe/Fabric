using Loom.Core;

namespace Loom.GameProject.ViewModel
{
    class ProjectBrowserViewModel : ViewModelBase
    {
        public RelayCommand<ProjectBrowserView> CreateProjectViewCommand { get; set; }
        public RelayCommand<ProjectBrowserView> OpenProjectViewCommand { get; set; }

        public CreateProjectViewModel CreateProjectVM { get; set; }
        public OpenProjectViewModel OpenProjectVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ProjectBrowserViewModel()
        {
            CreateProjectVM = new CreateProjectViewModel();
            OpenProjectVM = new OpenProjectViewModel();

            CreateProjectViewCommand = new RelayCommand<ProjectBrowserView>(o =>
            {
                CurrentView = CreateProjectVM;
            });

            OpenProjectViewCommand = new RelayCommand<ProjectBrowserView>(o =>
            {
                CurrentView = OpenProjectVM;
            });
        }
    }
}
