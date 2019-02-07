using LibTessDotNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.TexturedGeometry2D
{
	/// <summary>
	/// LibTessDotNet Extensions.
	/// </summary>
	public static class MGTessExtensions
	{
		/// <summary>
		/// Gets the contour vertex.
		/// </summary>
		/// <param name="vertexPositionColorTexture">The vertex position color texture.</param>
		/// <returns></returns>
		public static ContourVertex GetContourVertex(this VertexPositionColorTexture vertexPositionColorTexture)
		{
			var g = new ContourVertex();
			g.Position = new Vec3() { X = vertexPositionColorTexture.Position.X, Y = vertexPositionColorTexture.Position.Y, Z = vertexPositionColorTexture.Position.Z };
			return g;
		}

		/// <summary>
		/// Gets the vector.
		/// </summary>
		/// <param name="vec">The vec.</param>
		/// <returns></returns>
		public static Vector3 GetVector(this Vec3 vec)
		{
			return new Vector3(vec.X, vec.Y, vec.Z);
		}
	}
}