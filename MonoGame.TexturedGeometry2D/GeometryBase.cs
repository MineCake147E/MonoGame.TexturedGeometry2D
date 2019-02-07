using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.TexturedGeometry2D
{
	/// <summary>
	/// Geometry base class.
	/// </summary>
	public abstract class GeometryBase
	{
		/// <summary>
		/// Compiles the specified view port.
		/// </summary>
		/// <param name="viewport">The view port.</param>
		/// <returns></returns>
		public abstract IEnumerable<TexturedPolygon> Compile(Viewport viewport);
	}
}