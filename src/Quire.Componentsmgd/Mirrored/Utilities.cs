#define TRACE
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Quire.Mcd;

namespace Quire.Components;

internal static partial class Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    internal partial class CALLBACK_INFO
    {
        public int hwnd;
        public int handle;

        public CALLBACK_INFO()
        {
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal partial class CALLBACK_MSG
    {
        public int msg;
        public int handle;

        public CALLBACK_MSG()
        {
        }
    }

    internal static object Unwrap(int comp)
    {
        return Marshal.GetObjectForIUnknown(new IntPtr(comp));
    }

    internal static void Release(object comp)
    {
        Marshal.ReleaseComObject(comp);
    }

    internal static void ReleaseAll(object comp)
    {
        while (Marshal.ReleaseComObject(comp) > 0)
        {
        }
    }

    internal static void SetProperty(object comp, string name, object val)
    {
        System.Type type = comp.GetType();
        try
        {
            type.InvokeMember(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty, null, comp, new object[1] { val });
        }
        catch
        {
        }
    }

    internal static object GetProperty(object comp, string name)
    {
        System.Type type = comp.GetType();
        try
        {
            return type.InvokeMember(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, comp, null);
        }
        catch
        {
            return null;
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern nint SendMessage(int hWnd, int Msg, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] object[] wParam, [MarshalAs(UnmanagedType.LPStruct)] CALLBACK_MSG lParam);

    [DllImport("ole32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern string ProgIDFromCLSID([In] ref Guid clsid);

    internal static Units GetUnits(object value, bool simplify)
    {
        if (!(value is DimVal dimVal) || Dim.Id_b(dimVal.m_dim))
        {
            return null;
        }
        Trace.Assert(dimVal.IsUniformDimOf(dimVal.m_dim), "dv.IsUniformDimOf(dv.m_dim)");
        Quire.Mcd.Type.Product product = dimVal.m_dim.Product;
        if (!simplify)
        {
            return RuntimeOptions.the_unit_system.Normalize(product);
        }
        return RuntimeOptions.the_unit_system.Simplify(product);
    }

    internal static object ScaleToSI(object value, bool simplify)
    {
        Units units = GetUnits(value, simplify);
        if (units != null)
        {
            return Mcd.Utilities.Multiply(value, units.Magnitude);
        }
        return value;
    }

    internal static object ScaleFromSI(object value, bool simplify)
    {
        Units units = GetUnits(value, simplify);
        if (units != null)
        {
            return Mcd.Utilities.Divide(value, units.Magnitude);
        }
        return value;
    }
}
