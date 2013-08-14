using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace FunkyTrinity
{
	 /// <summary>
	 /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
	 /// </summary>
	 public class ScreenCapture
	 {
		  /// <summary>
		  /// Creates an Image object containing a screen shot of the entire desktop
		  /// </summary>
		  /// <returns></returns>
		  public Image CaptureScreen()
		  {
				return CaptureWindow(User32.GetDesktopWindow());
		  }
		  /// <summary>
		  /// Creates an Image object containing a screen shot of a specific window
		  /// </summary>
		  /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
		  /// <returns></returns>
		  public Image CaptureWindow(IntPtr handle)
		  {
				// get te hDC of the target window
				IntPtr hdcSrc=User32.GetWindowDC(handle);
				// get the size
				User32.RECT windowRect=new User32.RECT();
				User32.GetWindowRect(handle, ref windowRect);
				int width=windowRect.right-windowRect.left;
				int height=windowRect.bottom-windowRect.top;
				// create a device context we can copy to
				IntPtr hdcDest=GDI32.CreateCompatibleDC(hdcSrc);
				// create a bitmap we can copy it to,
				// using GetDeviceCaps to get the width/height
				IntPtr hBitmap=GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
				// select the bitmap object
				IntPtr hOld=GDI32.SelectObject(hdcDest, hBitmap);
				// bitblt over
				GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
				// restore selection
				GDI32.SelectObject(hdcDest, hOld);
				// clean up
				GDI32.DeleteDC(hdcDest);
				User32.ReleaseDC(handle, hdcSrc);
				// get a .NET image object for it
				Image img=Image.FromHbitmap(hBitmap);
				// free up the Bitmap object
				GDI32.DeleteObject(hBitmap);
				return img;
		  }
		  /// <summary>
		  /// Captures a screen shot of a specific window, and saves it to a file
		  /// </summary>
		  /// <param name="handle"></param>
		  /// <param name="filename"></param>
		  /// <param name="format"></param>
		  public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
		  {
				Image img=CaptureWindow(handle);
				img.Save(filename, format);
		  }
		  /// <summary>
		  /// Captures a screen shot of the entire desktop, and saves it to a file
		  /// </summary>
		  /// <param name="filename"></param>
		  /// <param name="format"></param>
		  public void CaptureScreenToFile(string filename, ImageFormat format)
		  {
				Image img=CaptureScreen();
				img.Save(filename, format);
		  }

		  [DllImport("user32.dll")]
		  [return: MarshalAs(UnmanagedType.Bool)]
		  public static extern bool SetForegroundWindow(IntPtr hWnd);

		  /// <summary>
		  /// The MoveWindow function changes the position and dimensions of the specified window. For a top-level window, the position and dimensions are relative to the upper-left corner of the screen. For a child window, they are relative to the upper-left corner of the parent window's client area.
		  /// </summary>
		  /// <param name="hWnd">Handle to the window.</param>
		  /// <param name="X">Specifies the new position of the left side of the window.</param>
		  /// <param name="Y">Specifies the new position of the top of the window.</param>
		  /// <param name="nWidth">Specifies the new width of the window.</param>
		  /// <param name="nHeight">Specifies the new height of the window.</param>
		  /// <param name="bRepaint">Specifies whether the window is to be repainted. If this parameter is TRUE, the window receives a message. If the parameter is FALSE, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of moving a child window.</param>
		  /// <returns>If the function succeeds, the return value is nonzero.
		  /// <para>If the function fails, the return value is zero. To get extended error information, call GetLastError.</para></returns>
		  [DllImport("user32.dll", SetLastError=true)]
		  public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		  public struct RECT
		  {
				public int left;
				public int top;
				public int right;
				public int bottom;

				public int Width()
				{
					 return right-left;
				}
				public int Height()
				{
					 return bottom-top;
				}
		  }

		  [DllImport("user32.dll")]
		  public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

		  public const Int32 MOUSEEVENTF_MOVE=0x00000001; //  mouse move
		  public const Int32 MOUSEEVENTF_LEFTDOWN=0x00000002; //  left button down
		  public const Int32 MOUSEEVENTF_LEFTUP=0x00000004; //'  left button up
		  public const Int32 MOUSEEVENTF_RIGHTDOWN=0x00000008; //'  right button down
		  public const Int32 MOUSEEVENTF_RIGHTUP=0x00000010; //'  right button up
		  public const Int32 MOUSEEVENTF_ABSOLUTE=0x00008000;// '  absolute move

		  public static void LeftClick(int x, int y)
		  {
				int cur_x, cur_y, dest_x, dest_y;
				cur_x=System.Windows.Forms.Cursor.Position.X*65535/System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
				cur_y=System.Windows.Forms.Cursor.Position.Y*65535/System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
				Point pt=new Point(x, y);

				dest_x=pt.X*65535/System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
				dest_y=pt.Y*65535/System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

				mouse_event(MOUSEEVENTF_ABSOLUTE+MOUSEEVENTF_MOVE+MOUSEEVENTF_LEFTDOWN+MOUSEEVENTF_LEFTUP, dest_x, dest_y, 0, 0);
		  }

		  [DllImport("user32.dll", CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall, ExactSpelling=true, SetLastError=true)]
		  public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

		  /// <summary>
		  /// Helper class containing Gdi32 API functions
		  /// </summary>
		  private class GDI32
		  {

				public const int SRCCOPY=0x00CC0020; // BitBlt dwRop parameter
				[DllImport("gdi32.dll")]
				public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
					 int nWidth, int nHeight, IntPtr hObjectSource,
					 int nXSrc, int nYSrc, int dwRop);
				[DllImport("gdi32.dll")]
				public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
					 int nHeight);
				[DllImport("gdi32.dll")]
				public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
				[DllImport("gdi32.dll")]
				public static extern bool DeleteDC(IntPtr hDC);
				[DllImport("gdi32.dll")]
				public static extern bool DeleteObject(IntPtr hObject);
				[DllImport("gdi32.dll")]
				public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
		  }

		  /// <summary>
		  /// Helper class containing User32 API functions
		  /// </summary>
		  private class User32
		  {
				[StructLayout(LayoutKind.Sequential)]
				public struct RECT
				{
					 public int left;
					 public int top;
					 public int right;
					 public int bottom;
				}
				[DllImport("user32.dll")]
				public static extern IntPtr GetDesktopWindow();
				[DllImport("user32.dll")]
				public static extern IntPtr GetWindowDC(IntPtr hWnd);
				[DllImport("user32.dll")]
				public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
				[DllImport("user32.dll")]
				public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
		  }
	 }
}