using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.TexturedGeometry2D.Core
{
	/// <summary>
	/// Texel caches of Textures.
	/// </summary>
	public sealed class TextureTexelCache
	{
		private Dictionary<Texture2D, Vector2> TexelCaches = new Dictionary<Texture2D, Vector2>();

		/// <summary>
		/// Gets the texel scale.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <returns></returns>
		public Vector2 GetTexelScale(Texture2D texture)
		{
			if (!TexelCaches.TryGetValue(texture, out var vector))
			{
				TexelCaches[texture] = vector = new Vector2(1.0f / texture.Width, 1.0f / texture.Height);
			}
			return vector;
		}
	}
}