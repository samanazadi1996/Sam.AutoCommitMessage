using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace AutoCommitMessage.EventHandlers;

public class FileChangeEventHandle : IVsRunningDocTableEvents, IVsFileChangeEvents
{
    private readonly IVsRunningDocumentTable _rdt = (IVsRunningDocumentTable)Package.GetGlobalService(typeof(SVsRunningDocumentTable));
    private readonly IVsFileChangeEx _fileChangeService = (IVsFileChangeEx)Package.GetGlobalService(typeof(SVsFileChangeEx));
    private uint _rdtCookie;
    private uint _vsFileChangeCookie;

    public event Action OnFileChanged;

    public void StartWatching(string folderPath)
    {
        SubscribeToRunningDocumentTable();
        SubscribeToFileChanges(folderPath);
    }

    public void StopWatching()
    {
        UnsubscribeFromRunningDocumentTable();
        UnsubscribeFromFileChanges();
    }

    private void SubscribeToRunningDocumentTable()
    {
        if (_rdt != null)
        {
            _rdt.AdviseRunningDocTableEvents(this, out _rdtCookie);
        }
    }

    private void UnsubscribeFromRunningDocumentTable()
    {
        if (_rdtCookie != 0 && _rdt != null)
        {
            _rdt.UnadviseRunningDocTableEvents(_rdtCookie);
            _rdtCookie = 0;
        }
    }

    private void SubscribeToFileChanges(string folderPath)
    {
        _fileChangeService?.AdviseDirChange(folderPath, 1, this, out _vsFileChangeCookie);
    }

    private void UnsubscribeFromFileChanges()
    {
        if (_vsFileChangeCookie != 0 && _fileChangeService != null)
        {
            _fileChangeService.UnadviseDirChange(_vsFileChangeCookie);
            _vsFileChangeCookie = 0;
        }
    }

    // IVsRunningDocTableEvents implementation

    public int OnAfterSave(uint docCookie)
    {
        OnFileChanged?.Invoke(); // Trigger event when file is saved
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