#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSObjectExec, interop.vsinterfaceslib")]
[ComImport]
[Guid("7BF377F5-0A1A-11E0-BC04-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVSObjectExec
{
	void GetNumPorts(ref uint pInputs, ref uint pOutputs);

	void StateChange(VSAPPSTATE VSAPPSTATE);

	void StepEnabled(ref int pbEnabled);

	void Step(ref int lpKey);

	void SetInput(uint nPort, [MarshalAs(UnmanagedType.Interface)] IVSQueueInput lpVSQInput);

	void SetOutput(uint nPort, [MarshalAs(UnmanagedType.Interface)] IVSQueueOutput lpVSQOutput);
}
