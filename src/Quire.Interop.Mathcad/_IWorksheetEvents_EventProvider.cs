#nullable disable
#pragma warning disable CA1416, CS0465
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad._IWorksheetEvents_EventProvider, interop.mathcad")]
internal sealed class _IWorksheetEvents_EventProvider : _IWorksheetEvents_Event, IDisposable
{
	private WeakReference m_wkConnectionPointContainer;

	private ArrayList m_aEventSinkHelpers;

	private IConnectionPoint m_ConnectionPoint;

	private void Init()
	{
		IConnectionPoint ppCP = null;
		Guid riid = new Guid(new byte[16]
		{
			194, 9, 221, 123, 26, 10, 224, 17, 131, 221,
			17, 248, 226, 168, 195, 108
		});
		((IConnectionPointContainer)m_wkConnectionPointContainer.Target).FindConnectionPoint(ref riid, out ppCP);
		m_ConnectionPoint = ppCP;
		m_aEventSinkHelpers = new ArrayList();
	}

	public event _IWorksheetEvents_ChangedEventHandler Changed
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
			_IWorksheetEvents_SinkHelper iWorksheetEvents_SinkHelper = new _IWorksheetEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iWorksheetEvents_SinkHelper, out pdwCookie);
			iWorksheetEvents_SinkHelper.m_dwCookie = pdwCookie;
			iWorksheetEvents_SinkHelper.m_ChangedDelegate = value;
			m_aEventSinkHelpers.Add(iWorksheetEvents_SinkHelper);
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
				_IWorksheetEvents_SinkHelper iWorksheetEvents_SinkHelper = (_IWorksheetEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iWorksheetEvents_SinkHelper.m_ChangedDelegate != null && ((iWorksheetEvents_SinkHelper.m_ChangedDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iWorksheetEvents_SinkHelper.m_dwCookie);
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

	public event _IWorksheetEvents_RecalculatedEventHandler Recalculated
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
			_IWorksheetEvents_SinkHelper iWorksheetEvents_SinkHelper = new _IWorksheetEvents_SinkHelper();
			int pdwCookie = 0;
			m_ConnectionPoint.Advise(iWorksheetEvents_SinkHelper, out pdwCookie);
			iWorksheetEvents_SinkHelper.m_dwCookie = pdwCookie;
			iWorksheetEvents_SinkHelper.m_RecalculatedDelegate = value;
			m_aEventSinkHelpers.Add(iWorksheetEvents_SinkHelper);
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
				_IWorksheetEvents_SinkHelper iWorksheetEvents_SinkHelper = (_IWorksheetEvents_SinkHelper)m_aEventSinkHelpers[num];
				if (iWorksheetEvents_SinkHelper.m_RecalculatedDelegate != null && ((iWorksheetEvents_SinkHelper.m_RecalculatedDelegate.Equals(value) ? 1u : 0u) & 0xFFu) != 0)
				{
					m_aEventSinkHelpers.RemoveAt(num);
					m_ConnectionPoint.Unadvise(iWorksheetEvents_SinkHelper.m_dwCookie);
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

	public _IWorksheetEvents_EventProvider(object P_0)
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
					_IWorksheetEvents_SinkHelper iWorksheetEvents_SinkHelper = (_IWorksheetEvents_SinkHelper)m_aEventSinkHelpers[num];
					m_ConnectionPoint.Unadvise(iWorksheetEvents_SinkHelper.m_dwCookie);
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
