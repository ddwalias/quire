using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Quire.Automation;
using Quire.Mpl.Run;
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

    [global::Quire.Mirror.MirrorStub]
    private bool FixInputs(ref object[] inputs, ref uint nInputs, ref uint nOutputs) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void CreateInputQueue(uint nInputs, ref McxInput[] minputs, object[] inputs, UnitAction action)
    {
        for (uint i = 0; i < nInputs; i++)
        {
            minputs[i] = new McxInput(action);
            minputs[i].Data = VSValueFactory.Instance.AutomationToVSData(inputs[i]) as VSDataValue;
            executive.SetInput(i, minputs[i]);
        }
    }

    private void CreateOutputQueue(uint nOutputs, ref McxOutput[] moutputs, UnitAction action)
    {
        for (uint i = 0; i < nOutputs; i++)
        {
            moutputs[i] = new McxOutput(action);
            executive.SetOutput(i, moutputs[i]);
        }
    }

    public void Initialize(object any)
    {
        if (any == null && executive != null)
        {
            Utilities.ReleaseAll(executive);
        }
        executive = (IVSObjectExec2)any;
    }

    public void McError(bool bSet)
    {
        if (executive != null)
        {
            executive.Error(bSet ? 1 : 0);
        }
    }

    public object[] Eval(object[] inputs, object reserved)
    {
        lock (this)
        {
            if (running)
            {
                return null;
            }
            running = true;
        }
        lastError = null;
        try
        {
            try
            {
                UnitAction action = ShouldScale() ? (reserved == null ? UnitAction.Normalize : UnitAction.Simplify) : UnitAction.None;
                uint nInputs = 0;
                uint nOutputs = 0;
                executive.GetNumPorts(ref nInputs, ref nOutputs);
                uint originalInputs = nInputs;
                bool fixedInputs = FixInputs(ref inputs, ref nInputs, ref nOutputs);
                try
                {
                    McxInput[] minputs = new McxInput[nInputs];
                    GC.KeepAlive(minputs);
                    CreateInputQueue(nInputs, ref minputs, inputs, action);
                    McxOutput[] moutputs = new McxOutput[nOutputs];
                    GC.KeepAlive(moutputs);
                    CreateOutputQueue(nOutputs, ref moutputs, action);
                    key = 1;
                    executive.StateChange(VSAPPSTATE.VSAPPSTATE_RUNNING);
                    try
                    {
                        executive.Step(ref key);
                        while (key > 0)
                        {
                            if (MultiThreaded)
                            {
                                Thread.Sleep(0);
                            }
                            else
                            {
                                Application.DoEvents();
                            }
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref key);
                        executive.StateChange(VSAPPSTATE.VSAPPSTATE_STOPPED);
                    }
                    if (nOutputs == 0)
                    {
                        return null;
                    }
                    object[] outputs = new object[nOutputs];
                    for (uint i = 0; i < nOutputs; i++)
                    {
                        outputs[i] = VSValueFactory.Instance.VSDataToAutomation(moutputs[i].Data);
                        if (outputs[i] == null)
                        {
                            throw new Exception("null pointer");
                        }
                    }
                    return outputs;
                }
                finally
                {
                    if (fixedInputs)
                    {
                        executive.SetNumPorts(originalInputs, nOutputs, false);
                    }
                }
            }
            catch (Exception ex)
            {
                if (lastError == null)
                {
                    lastError = ex;
                }
                throw lastError;
            }
        }
        finally
        {
            running = false;
        }
    }

    public void Stepped(IVSObject lpVSObject, ref int lpKey)
    {
        key = lpKey = 0;
    }

    public void GetAppState(IVSObject lpVSObject, ref VSAPPSTATE pState)
    {
        pState = VSAPPSTATE.VSAPPSTATE_RUNNING;
    }

    public void Error(IVSObject lpVSObject, string pcszError)
    {
        McError(true);
        lastError = new AppException(pcszError);
    }

    public void SetAppState(IVSObject lpVSObject, VSAPPSTATE vsState)
    {
    }
}
