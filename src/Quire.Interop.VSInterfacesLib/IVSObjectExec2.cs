#nullable disable
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quire.VSINTERFACESLib;

[global::Quire.Mirror.MirrorOf("VSINTERFACESLib.IVSObjectExec2, interop.vsinterfaceslib")]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BF39F02-0A1A-11E0-B4D5-11F8E2A8C36C")]
[TypeLibType(TypeLibTypeFlags.FOleAutomation)]
public interface IVSObjectExec2 : IVSObjectExec
{
	new void GetNumPorts(ref uint pInputs, ref uint pOutputs);

	new void StateChange(VSAPPSTATE VSAPPSTATE);

	new void StepEnabled(ref int pbEnabled);

	new void Step(ref int lpKey);

	new void SetInput(uint nPort, [MarshalAs(UnmanagedType.Interface)] IVSQueueInput lpVSQInput);

	new void SetOutput(uint nPort, [MarshalAs(UnmanagedType.Interface)] IVSQueueOutput lpVSQOutput);

	void Error(int flag);

	void SetNumPorts(uint nInputs, uint nOutputs, bool bNotify);
}
