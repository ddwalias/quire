#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad._IWorksheetEvents_SinkHelper, interop.mathcad")]
[TypeLibType(TypeLibTypeFlags.FHidden)]
[ClassInterface(ClassInterfaceType.None)]
public sealed class _IWorksheetEvents_SinkHelper : _IWorksheetEvents
{
	public _IWorksheetEvents_ChangedEventHandler m_ChangedDelegate;

	public _IWorksheetEvents_RecalculatedEventHandler m_RecalculatedDelegate;

	public int m_dwCookie;

	public void Changed()
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_ChangedDelegate != null)
		{
			m_ChangedDelegate();
		}
	}

	public void Recalculated()
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_RecalculatedDelegate != null)
		{
			m_RecalculatedDelegate();
		}
	}

	internal _IWorksheetEvents_SinkHelper()
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		m_dwCookie = 0;
		m_ChangedDelegate = null;
		m_RecalculatedDelegate = null;
	}
}
