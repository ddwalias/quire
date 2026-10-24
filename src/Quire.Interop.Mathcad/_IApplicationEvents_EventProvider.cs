#nullable disable
#pragma warning disable CA1416, CS0465
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad._IApplicationEvents_EventProvider, interop.mathcad")]
internal sealed class _IApplicationEvents_EventProvider : _IApplicationEvents_Event, IDisposable
{
	private WeakReference m_wkConnectionPointContainer;

	private ArrayList m_aEventSinkHelpers;

	private IConnectionPoint m_ConnectionPoint;

	private void Init()
	{
		IConnectionPoint ppCP = null;
		Guid riid = new Guid(new byte[16]
		{
			195, 9, 221, 123, 26, 10, 224, 17, 160, 174,
			17, 248, 226, 168, 195, 108
		});
		((IConnectionPointContainer)m_wkConnectionPointContainer.Target).FindConnectionPoint(ref riid, out ppCP);
		m_ConnectionPoint = ppCP;
		m_aEventSinkHelpers = new ArrayList();
	}

	public void add_Quit(_IApplicationEvents_QuitEventHandler value)
	{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_QuitDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
	}

	public void remove_Quit(_IApplicationEvents_QuitEventHandler value)
	{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_QuitDelegate != null && ((iApplicationEvents_SinkHelper.m_QuitDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
	}

	public event _IApplicationEvents_WorksheetClosingEventHandler WorksheetClosing
	{
		add
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_WorksheetClosingDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
		remove
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_WorksheetClosingDelegate != null && ((iApplicationEvents_SinkHelper.m_WorksheetClosingDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
	}

	public event _IApplicationEvents_WorksheetOpenedEventHandler WorksheetOpened
	{
		add
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_WorksheetOpenedDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
		remove
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_WorksheetOpenedDelegate != null && ((iApplicationEvents_SinkHelper.m_WorksheetOpenedDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
	}

	public event _IApplicationEvents_WindowActivatedEventHandler WindowActivated
	{
		add
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_WindowActivatedDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
		remove
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_WindowActivatedDelegate != null && ((iApplicationEvents_SinkHelper.m_WindowActivatedDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
	}

	public event _IApplicationEvents_WindowDeactivatedEventHandler WindowDeactivated
	{
		add
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_WindowDeactivatedDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
		remove
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_WindowDeactivatedDelegate != null && ((iApplicationEvents_SinkHelper.m_WindowDeactivatedDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
	}

	public event _IApplicationEvents_WorksheetSavingEventHandler WorksheetSaving
	{
		add
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_WorksheetSavingDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
		remove
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_WorksheetSavingDelegate != null && ((iApplicationEvents_SinkHelper.m_WorksheetSavingDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
	}

	public event _IApplicationEvents_WorksheetSavedEventHandler WorksheetSaved
	{
		add
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_WorksheetSavedDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
		remove
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_WorksheetSavedDelegate != null && ((iApplicationEvents_SinkHelper.m_WorksheetSavedDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
	}

	public event _IApplicationEvents_WorksheetClosedEventHandler WorksheetClosed
	{
		add
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_WorksheetClosedDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
		remove
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_WorksheetClosedDelegate != null && ((iApplicationEvents_SinkHelper.m_WorksheetClosedDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
	}

	public event _IApplicationEvents_IdleEventHandler Idle
	{
		add
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				Init();
			}
			_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = new _IApplicationEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iApplicationEvents_SinkHelper, out pdwCookie);
			iApplicationEvents_SinkHelper.m_dwCookie = pdwCookie;
			iApplicationEvents_SinkHelper.m_IdleDelegate = value;
			m_aEventSinkHelpers.Add(iApplicationEvents_SinkHelper);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
		remove
		{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_aEventSinkHelpers == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 >= count)
			{
				return;
			}
			do
			{
				_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iApplicationEvents_SinkHelper.m_IdleDelegate != null && ((iApplicationEvents_SinkHelper.m_IdleDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					if (count <= 1)
					{
						Marshal.ReleaseComObject(m_ConnectionPoint);
						m_ConnectionPoint = null;
						m_aEventSinkHelpers = null;
					}
					break;
				}
				num++;
			}
			while (num < count);
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
		}
	}

	public _IApplicationEvents_EventProvider(object P_0)
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		m_wkConnectionPointContainer = new WeakReference((IConnectionPointContainer)P_0, trackResurrection: false);
	}

	public void Finalize()
	{
		bool lockTaken = default;
		try
		{
			Monitor.Enter(this, ref lockTaken);
			if (m_ConnectionPoint == null)
			{
				return;
			}
			int count = m_aEventSinkHelpers.Count;
			int num = 0;
			if (0 < count)
			{
				do
				{
					_IApplicationEvents_SinkHelper iApplicationEvents_SinkHelper = (_IApplicationEvents_SinkHelper)m_aEventSinkHelpers[num];
					m_ConnectionPoint.Unadvise(iApplicationEvents_SinkHelper.m_dwCookie);
					num++;
				}
				while (num < count);
			}
			Marshal.ReleaseComObject(m_ConnectionPoint);
		}
		catch (Exception)
		{
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(this);
			}
		}
	}

	public void Dispose()
	{
		//Error decoding local variables: Signature type sequence must have at least one element.
		Finalize();
		GC.SuppressFinalize(this);
	}
}
