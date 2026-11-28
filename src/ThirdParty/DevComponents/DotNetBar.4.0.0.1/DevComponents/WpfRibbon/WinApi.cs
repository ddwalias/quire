#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

internal partial class WinApi
{
    [MirrorVerified("Release IL")]
    public static int LOWORD(int n) => (short)(n & 65535);

    [MirrorVerified("Release IL")]
    public static int HIWORD(int n) => (short)(n >> 16 & 65535);

    [MirrorVerified("Release IL")]
    public static int LOWORD(nint n) => LOWORD((int)(long)n);

    [MirrorVerified("Release IL")]
    public static int HIWORD(nint n) => HIWORD((int)(long)n);

    [MirrorVerified("Release IL")]
    public WinApi() { }

    partial struct RECT
    {
        [MirrorVerified("Release IL")]
        public int Left;

        [MirrorVerified("Release IL")]
        public int Top;

        [MirrorVerified("Release IL")]
        public int Right;

        [MirrorVerified("Release IL")]
        public int Bottom;

        [MirrorVerified("Release IL")]
        public RECT(int left_, int top_, int right_, int bottom_)
        {
            Left = left_;
            Top = top_;
            Right = right_;
            Bottom = bottom_;
        }

        [MirrorVerified("Release IL")]
        public Rect ToRectangle() => new Rect(Left, Top, Right - Left, Bottom - Top);

        [MirrorVerified("Release IL")]
        public override int GetHashCode() => Left ^ (Top << 13 | Top >> 19) ^ (Width << 26 | Width >> 6) ^ (Height << 7 | Height >> 25);

        [MirrorVerified("Release IL")]
        public static implicit operator Rect(RECT rect) => new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

        [MirrorVerified("Release IL")]
        public int Height => Bottom - Top;

        [MirrorVerified("Release IL")]
        public int Width => Right - Left;

        [MirrorVerified("Release IL")]
        public Size Size => new Size(Width, Height);

        [MirrorVerified("Release IL")]
        public Point Location => new Point(Left, Top);
    }

    partial struct POINT
    {
        [MirrorVerified("Release IL")]
        public int x;

        [MirrorVerified("Release IL")]
        public int y;

        [MirrorVerified("Release IL")]
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
