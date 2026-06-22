using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public sealed class ProjectManager
    {
        public RealmStudioProject? CurrentProject { get; private set; }

        public event EventHandler? ProjectChanged;

        public CommandManager ProjectCommands { get; private set; } = new CommandManager();

        public void OpenProject(RealmStudioProject project)
        {
            CurrentProject = project;
            NotifyProjectChanged();
        }

        public void CloseProject()
        {
            CurrentProject = null;
            NotifyProjectChanged();
        }

        public void NotifyProjectChanged()
        {
            ProjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
