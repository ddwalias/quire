#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[ClassInterface(ClassInterfaceType.None)]
[global::Quire.Mirror.MirrorOf("Mathcad._IApplicationEvents_SinkHelper, interop.mathcad")]
[TypeLibType(TypeLibTypeFlags.FHidden)]
public sealed class _IApplicationEvents_SinkHelper : _IApplicationEvents
{
	public _IApplicationEvents_QuitEventHandler m_QuitDelegate;

	public _IApplicationEvents_WorksheetClosingEventHandler m_WorksheetClosingDelegate;

	public _IApplicationEvents_WorksheetOpenedEventHandler m_WorksheetOpenedDelegate;

	public _IApplicationEvents_WindowActivatedEventHandler m_WindowActivatedDelegate;

	public _IApplicationEvents_WindowDeactivatedEventHandler m_WindowDeactivatedDelegate;

	public _IApplicationEvents_WorksheetSavingEventHandler m_WorksheetSavingDelegate;

	public _IApplicationEvents_WorksheetSavedEventHandler m_WorksheetSavedDelegate;

	public _IApplicationEvents_WorksheetClosedEventHandler m_WorksheetClosedDelegate;

	public _IApplicationEvents_IdleEventHandler m_IdleDelegate;

	public int m_dwCookie;

	public void Quit()
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_QuitDelegate != null)
		{
			m_QuitDelegate();
		}
	}

	public void WorksheetClosing(Worksheet P_0)
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_WorksheetClosingDelegate != null)
		{
			m_WorksheetClosingDelegate(P_0);
		}
	}

	public void WorksheetOpened(Worksheet P_0)
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_WorksheetOpenedDelegate != null)
		{
			m_WorksheetOpenedDelegate(P_0);
		}
	}

	public void WindowActivated(Window P_0)
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_WindowActivatedDelegate != null)
		{
			m_WindowActivatedDelegate(P_0);
		}
	}

	public void WindowDeactivated(Window P_0)
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_WindowDeactivatedDelegate != null)
		{
			m_WindowDeactivatedDelegate(P_0);
		}
	}

	public void WorksheetSaving(Worksheet P_0)
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_WorksheetSavingDelegate != null)
		{
			m_WorksheetSavingDelegate(P_0);
		}
	}

	public void WorksheetSaved(Worksheet P_0)
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_WorksheetSavedDelegate != null)
		{
			m_WorksheetSavedDelegate(P_0);
		}
	}

	public void WorksheetClosed(Worksheet P_0)
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_WorksheetClosedDelegate != null)
		{
			m_WorksheetClosedDelegate(P_0);
		}
	}

	public void Idle()
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		if (m_IdleDelegate != null)
		{
			m_IdleDelegate();
		}
	}

	internal _IApplicationEvents_SinkHelper()
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		m_dwCookie = 0;
		m_QuitDelegate = null;
		m_WorksheetClosingDelegate = null;
		m_WorksheetOpenedDelegate = null;
		m_WindowActivatedDelegate = null;
		m_WindowDeactivatedDelegate = null;
		m_WorksheetSavingDelegate = null;
		m_WorksheetSavedDelegate = null;
		m_WorksheetClosedDelegate = null;
		m_IdleDelegate = null;
	}
}
