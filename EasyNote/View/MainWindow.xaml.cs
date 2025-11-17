using System;
﻿using System.Runtime.InteropServices;
﻿using System.Windows;
﻿using System.Windows.Input;
﻿using System.Windows.Interop;
﻿using System.Windows.Media;
﻿
﻿namespace EasyNote
﻿{
﻿    public partial class MainWindow : Window
﻿    {
﻿        public MainWindow()
﻿        {
﻿            InitializeComponent();
﻿            SourceInitialized += OnSourceInitialized;
﻿        }
﻿
﻿        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
﻿        {
﻿            if (e.ChangedButton == MouseButton.Left)
﻿                DragMove();
﻿        }
﻿
﻿        private void Minimize_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            WindowState = WindowState.Minimized;
﻿        }
﻿
﻿        private void Maximize_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
﻿        }
﻿
﻿        private void Close_Click(object sender, RoutedEventArgs e)
﻿        {
﻿            Close();
﻿        }
﻿
﻿        private void Close_MouseEnter(object sender, MouseEventArgs e)
﻿        {
﻿            ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
﻿        }
﻿
﻿        private void Close_MouseLeave(object sender, MouseEventArgs e)
﻿        {
﻿            ((System.Windows.Controls.Button)sender).Background = Brushes.Transparent;
﻿        }
﻿        
﻿        private void OnSourceInitialized(object sender, EventArgs e)
﻿        {
﻿            var helper = new WindowInteropHelper(this);
﻿            HwndSource.FromHwnd(helper.Handle)?.AddHook(WndProc);
﻿        }
﻿
﻿        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
﻿        {
﻿            switch (msg)
﻿            {
﻿                case 0x0024: // WM_GETMINMAXINFO
﻿                    WmGetMinMaxInfo(hwnd, lParam);
﻿                    handled = true;
﻿                    break;
﻿            }
﻿            return IntPtr.Zero;
﻿        }
﻿
﻿        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
﻿        {
﻿            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
﻿            var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
﻿
﻿            if (monitor != IntPtr.Zero)
﻿            {
﻿                var monitorInfo = new MONITORINFO();
﻿                monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
﻿                GetMonitorInfo(monitor, monitorInfo);
﻿                RECT rcWorkArea = monitorInfo.rcWork;
﻿                RECT rcMonitorArea = monitorInfo.rcMonitor;
﻿                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
﻿                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
﻿                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
﻿                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
﻿            }
﻿
﻿            Marshal.StructureToPtr(mmi, lParam, true);
﻿        }
﻿
﻿        [StructLayout(LayoutKind.Sequential)]
﻿        public struct POINT
﻿        {
﻿            public int x;
﻿            public int y;
﻿        }
﻿
﻿        [StructLayout(LayoutKind.Sequential)]
﻿        public struct MINMAXINFO
﻿        {
﻿            public POINT ptReserved;
﻿            public POINT ptMaxSize;
﻿            public POINT ptMaxPosition;
﻿            public POINT ptMinTrackSize;
﻿            public POINT ptMaxTrackSize;
﻿        }
﻿
﻿        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
﻿        public class MONITORINFO
﻿        {
﻿            public int cbSize;
﻿            public RECT rcMonitor;
﻿            public RECT rcWork;
﻿            public uint dwFlags;
﻿        }
﻿
﻿        [StructLayout(LayoutKind.Sequential, Pack = 0)]
﻿        public struct RECT
﻿        {
﻿            public int left;
﻿            public int top;
﻿            public int right;
﻿            public int bottom;
﻿        }
﻿
﻿        [DllImport("user32.dll")]
﻿        [return: MarshalAs(UnmanagedType.Bool)]
﻿        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
﻿
﻿        [DllImport("user32.dll")]
﻿        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
﻿
﻿        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;
﻿    }
﻿}
﻿