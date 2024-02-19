/*
 * Copyright 2024, Nexus6 <nexus6.haiku@icloud.com>
 * All rights reserved. Distributed under the terms of the MIT license.
 */

 using System.Runtime.InteropServices;

namespace Haiku.Interface;

public static class Utils
{
	[DllImport("libbe.so", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern Haiku.Interface.RgbColor ui_color(Haiku.Interface.ColorWhich which);
}
