using System;
using Microsoft.Win32;
using Quire.VSINTERFACESLib;

namespace Quire.Components;

public partial class McxComponent
{
    private static readonly string[] PROGIDS = new string[18]
    {
        "Mathcad.Matlab.Component", "Mathcad.ScriptedObject.Component", "Mathcad.Buddy.Component", "Mathcad.User.Component", "Mathcad.Picture.Component", "Mathcad.SPlus.Component", "Mathcad.SPlusGraph.Component", "Mathcad.Axum2D.Component", "Mathcad.Axum3D.Component", "Mathcad.InputTable.Component",
        "Mathcad.OutputTable.Component", "Mathcad.FileRead.Component", "Mathcad.FileWrite.Component", "Mathcad.DataImport.Component", "Mathcad.Excel.Component", "Mathcad.ODBCRead.Component", "Mathcad.Web.Control", "Mathcad.3DPlot.Component"
    };

    private static readonly bool MultiThreaded = ThreadsPerDocument > 0;

    private IVSObjectExec2 executive;
    private Exception lastError;

    private static int ThreadsPerDocument
    {
        get
        {
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Mathsoft\\Mathcad 15\\Default\\AppSettings");
                if (registryKey == null)
                {
                    registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Mathsoft\\Mathcad 15\\AppSettings");
                }
                object value = registryKey.GetValue("ThreadsPerDoc");
                if (value == null)
                {
                    return 1;
                }
                return (int)value;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }

    public object Origin
    {
        get => Utilities.GetProperty(executive, "Origin");
        set => Utilities.SetProperty(executive, "Origin", value);
    }

    public object CWD
    {
        get => Utilities.GetProperty(executive, "CWD");
        set => Utilities.SetProperty(executive, "CWD", value);
    }

    public IVSObjectExec2 Executive => executive;

    public Exception LastError => lastError;

    private bool ShouldScale()
    {
        if (executive is not IOleObject oleObject)
        {
            return false;
        }
        oleObject.GetUserClassID(out Guid clsid);
        string progid = Utilities.ProgIDFromCLSID(ref clsid);
        if (Array.Find(PROGIDS, (string value) => progid.StartsWith(value)) != null)
        {
            return true;
        }
        return false;
    }

    private bool FixInputs(ref object[] inputs, ref uint nInputs, ref uint nOutputs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CreateInputQueue(uint nInputs, ref McxInput[] minputs, object[] inputs, global::Quire.Automation.UnitAction action) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CreateOutputQueue(uint nOutputs, ref McxOutput[] moutputs, global::Quire.Automation.UnitAction action) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Initialize(object any) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void McError(bool bSet) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public object[] Eval(object[] inputs, object reserved) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Stepped(global::Quire.VSINTERFACESLib.IVSObject lpVSObject, ref int lpKey) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void GetAppState(global::Quire.VSINTERFACESLib.IVSObject lpVSObject, ref global::Quire.VSINTERFACESLib.VSAPPSTATE pState) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Error(global::Quire.VSINTERFACESLib.IVSObject lpVSObject, string pcszError) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SetAppState(global::Quire.VSINTERFACESLib.IVSObject lpVSObject, global::Quire.VSINTERFACESLib.VSAPPSTATE vsState) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
