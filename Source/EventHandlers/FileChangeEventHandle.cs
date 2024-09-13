using AutoCommitMessage.Helper;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace AutoCommitMessage.EventHandlers;

public class FileChangeEventHandle : IVsRunningDocTableEvents, IVsFileChangeEvents
{
    private readonly IVsRunningDocumentTable _rdt = (IVsRunningDocumentTable)Package.GetGlobalService(typeof(SVsRunningDocumentTable));
    private readonly IVsFileChangeEx _fileChangeService = (IVsFileChangeEx)Package.GetGlobalService(typeof(SVsFileChangeEx));
    public string FolderPath;

    public event Action OnFileChanged;

    public void StartWatching()
    {
        var newFolderPath = ApplicationContext.GetOpenedFolder();

        if (FolderPath == newFolderPath) return;

        _rdt?.AdviseRunningDocTableEvents(this, out var _rdtCookie);
        _fileChangeService?.AdviseDirChange(newFolderPath, 1, this, out var _vsFileChangeCookie);

        FolderPath = newFolderPath;
    }


    public int OnAfterSave(uint docCookie)
    {
        OnFileChanged?.Invoke();
        return VSConstants.S_OK;
    }

    public int OnAfterAttributeChange(uint docCookie, uint grfAttribs) { return VSConstants.S_OK; }
    public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame) { return VSConstants.S_OK; }
    public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) { return VSConstants.S_OK; }
    public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame) { return VSConstants.S_OK; }
    public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) => VSConstants.S_OK;


    // IVsFileChangeEvents implementation
    public int DirectoryChanged(string pszDirectory)
    {
        OnFileChanged?.Invoke(); // Trigger event when directory changes
        return VSConstants.S_OK;
    }

    public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
    {
        OnFileChanged?.Invoke(); // Trigger event when files change
        return VSConstants.S_OK;
    }
}