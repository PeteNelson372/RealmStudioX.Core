using RealmStudioShapeRenderingLib;

namespace RealmStudioX.Core
{
    public class Cmd_DeleteMapFromProject : IUndoableCommand
    {
        private readonly ProjectManager _projectManager;
        private readonly RealmStudioProject _project;
        private readonly MapProjectEntry _entry;

        private int _originalIndex;

        private bool disposedValue;

        public Cmd_DeleteMapFromProject(ProjectManager projectManager, RealmStudioProject project,  MapProjectEntry entry)
        {
            _projectManager = projectManager;
            _project = project;
            _entry = entry;
        }

        public void Execute()
        {
            _originalIndex = _project.Maps.IndexOf(_entry);

            _project.Maps.Remove(_entry);

            _projectManager.NotifyProjectChanged();
        }

        public void Undo()
        {
            _project.Maps.Insert(_originalIndex, _entry);

            _projectManager.NotifyProjectChanged();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Cmd_DeleteMapFromProject()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
