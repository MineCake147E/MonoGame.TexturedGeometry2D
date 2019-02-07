using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.TexturedGeometry2D.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoGame.TexturedGeometry2D
{
	//SpriteBatch Compatibility function group
	public partial class GeometryBatchRenderer
	{
		private readonly TextureTexelCache texelCache = new TextureTexelCache();

		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this sprite.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this sprite.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this sprite.</param>
		public void DrawSprite(Texture2D texture,
				Vector2 position,
				Rectangle? sourceRectangle,
				Color color,
				float rotation,
				Vector2 origin,
				Vector2 scale,
				SpriteEffects effects,
				float layerDepth)
		{
			CheckValid(texture);
			float SortKey = float.NaN;
			switch (_sortMode)
			{
				// Comparison of Depth
				case SpriteSortMode.FrontToBack:
					SortKey = layerDepth;
					break;
				// Comparison of Depth in reverse
				case SpriteSortMode.BackToFront:
					SortKey = -layerDepth;
					break;
			}
			origin = origin * scale;
			//UV Setting
			float w, h;
			if (sourceRectangle.HasValue)
			{
				var srcRect = sourceRectangle.GetValueOrDefault();
				w = srcRect.Width * scale.X;
				h = srcRect.Height * scale.Y;
				var texelScale = texelCache.GetTexelScale(texture);
				_texCoordTL.X = srcRect.X * texelScale.X;
				_texCoordTL.Y = srcRect.Y * texelScale.Y;
				_texCoordBR.X = (srcRect.X + srcRect.Width) * texelScale.X;
				_texCoordBR.Y = (srcRect.Y + srcRect.Height) * texelScale.Y;
			}
			else
			{
				w = texture.Width * scale.X;
				h = texture.Height * scale.Y;
				_texCoordTL = Vector2.Zero;
				_texCoordBR = Vector2.One;
			}
			//Flip
			if ((effects & SpriteEffects.FlipVertically) != 0)
			{
				var temp = _texCoordBR.Y;
				_texCoordBR.Y = _texCoordTL.Y;
				_texCoordTL.Y = temp;
			}
			if ((effects & SpriteEffects.FlipHorizontally) != 0)
			{
				var temp = _texCoordBR.X;
				_texCoordBR.X = _texCoordTL.X;
				_texCoordTL.X = temp;
			}
			if (rotation == 0)
			{
				AddPolygon(new TexturedPolygon(texture, SortKey,
						position.X - origin.X,
						position.Y - origin.Y,
						w,
						h,
						color,
						_texCoordTL,
						_texCoordBR,
						layerDepth));
			}
			else
			{
				AddPolygon(new TexturedPolygon(texture, SortKey,
					position.X,
						position.Y,
						-origin.X,
						-origin.Y,
						w,
						h,
						(float)Math.Sin(rotation),
						(float)Math.Cos(rotation),
						color,
						_texCoordTL,
						_texCoordBR,
						layerDepth));
			}
		}

		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this sprite.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this sprite.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this sprite.</param>
		public void DrawSprite(Texture2D texture,
				Vector2 position,
				Rectangle? sourceRectangle,
				Color color,
				float rotation,
				Vector2 origin,
				float scale,
				SpriteEffects effects,
				float layerDepth)
		{
			var scaleVec = new Vector2(scale, scale);
			DrawSprite(texture, position, sourceRectangle, color, rotation, origin, scaleVec, effects, layerDepth);
		}

		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="destinationRectangle">The drawing bounds on screen.</param>
		/// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this sprite.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this sprite.</param>
		public void DrawSprite(Texture2D texture,
			Rectangle destinationRectangle,
			Rectangle? sourceRectangle,
			Color color,
			float rotation,
			Vector2 origin,
			SpriteEffects effects,
			float layerDepth)
		{
			CheckValid(texture);
			float SortKey = float.NaN;
			switch (_sortMode)
			{
				// Comparison of Depth
				case SpriteSortMode.FrontToBack:
					SortKey = layerDepth;
					break;
				// Comparison of Depth in reverse
				case SpriteSortMode.BackToFront:
					SortKey = -layerDepth;
					break;
			}
			float w, h;
			var texelScale = texelCache.GetTexelScale(texture);
			if (sourceRectangle.HasValue)
			{
				var srcRect = sourceRectangle.GetValueOrDefault();
				w = srcRect.Width;
				h = srcRect.Height;

				_texCoordTL.X = srcRect.X * texelScale.X;
				_texCoordTL.Y = srcRect.Y * texelScale.Y;
				_texCoordBR.X = (srcRect.X + srcRect.Width) * texelScale.X;
				_texCoordBR.Y = (srcRect.Y + srcRect.Height) * texelScale.Y;
				if (srcRect.Width != 0)
					origin.X = origin.X * destinationRectangle.Width / srcRect.Width;
				else
					origin.X = origin.X * destinationRectangle.Width * texelScale.X;
				if (srcRect.Height != 0)
					origin.Y = origin.Y * destinationRectangle.Height / srcRect.Height;
				else
					origin.Y = origin.Y * destinationRectangle.Height * texelScale.Y;
			}
			else
			{
				origin.X = origin.X * destinationRectangle.Width * texelScale.X;
				origin.Y = origin.Y * destinationRectangle.Height * texelScale.Y;
				_texCoordTL = Vector2.Zero;
				_texCoordBR = Vector2.One;
			}
			if ((effects & SpriteEffects.FlipVertically) != 0)
			{
				var temp = _texCoordBR.Y;
				_texCoordBR.Y = _texCoordTL.Y;
				_texCoordTL.Y = temp;
			}
			if ((effects & SpriteEffects.FlipHorizontally) != 0)
			{
				var temp = _texCoordBR.X;
				_texCoordBR.X = _texCoordTL.X;
				_texCoordTL.X = temp;
			}
			if (rotation == 0)
			{
				AddPolygon(new TexturedPolygon(texture, SortKey,
						destinationRectangle.X - origin.X,
						destinationRectangle.Y - origin.Y,
						destinationRectangle.Width,
						destinationRectangle.Height,
						color,
						_texCoordTL,
						_texCoordBR,
						layerDepth));
			}
			else
			{
				AddPolygon(new TexturedPolygon(texture, SortKey,
					destinationRectangle.X,
					destinationRectangle.Y,
					-origin.X,
					-origin.Y,
					destinationRectangle.Width,
					destinationRectangle.Height,
					(float)Math.Sin(rotation),
					(float)Math.Cos(rotation),
					color,
					_texCoordTL,
					_texCoordBR,
					layerDepth));
			}
		}

		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		/// <param name="color">A color mask.</param>
		public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
			CheckValid(texture);
			var SortKey = 0;

			Vector2 size;

			if (sourceRectangle.HasValue)
			{
				var srcRect = sourceRectangle.GetValueOrDefault();
				size = new Vector2(srcRect.Width, srcRect.Height);
				var texelScale = texelCache.GetTexelScale(texture);
				_texCoordTL.X = srcRect.X * texelScale.X;
				_texCoordTL.Y = srcRect.Y * texelScale.Y;
				_texCoordBR.X = (srcRect.X + srcRect.Width) * texelScale.X;
				_texCoordBR.Y = (srcRect.Y + srcRect.Height) * texelScale.Y;
			}
			else
			{
				size = new Vector2(texture.Width, texture.Height);
				_texCoordTL = Vector2.Zero;
				_texCoordBR = Vector2.One;
			}
			AddPolygon(new TexturedPolygon(texture, SortKey,
				position.X,
				position.Y,
				size.X,
				size.Y,
				color,
				_texCoordTL,
				_texCoordBR,
				0));
		}

		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="destinationRectangle">The drawing bounds on screen.</param>
		/// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		/// <param name="color">A color mask.</param>
		public void DrawSprite(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
			CheckValid(texture);
			Vector2 size;
			if (sourceRectangle.HasValue)
			{
				var srcRect = sourceRectangle.GetValueOrDefault();
				size = new Vector2(srcRect.Width, srcRect.Height);
				var texelScale = texelCache.GetTexelScale(texture);
				_texCoordTL.X = srcRect.X * texelScale.X;
				_texCoordTL.Y = srcRect.Y * texelScale.Y;
				_texCoordBR.X = (srcRect.X + srcRect.Width) * texelScale.X;
				_texCoordBR.Y = (srcRect.Y + srcRect.Height) * texelScale.Y;
			}
			else

			{
				size = new Vector2(texture.Width, texture.Height);
				_texCoordTL = Vector2.Zero;
				_texCoordBR = Vector2.One;
			}
			AddPolygon(new TexturedPolygon(texture, 0,
				destinationRectangle.X,
				destinationRectangle.Y,
				destinationRectangle.Width,
				destinationRectangle.Height,
				color,
				_texCoordTL,
				_texCoordBR,
				0));
		}

		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		public void DrawSprite(Texture2D texture, Vector2 position, Color color)
		{
			CheckValid(texture);
			AddPolygon(new TexturedPolygon(texture, 0,
				position.X,
				position.Y,
				texture.Width,
				texture.Height,
				color,
				Vector2.Zero,
				Vector2.One,
				0));
		}

		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="destinationRectangle">The drawing bounds on screen.</param>
		/// <param name="color">A color mask.</param>
		public void DrawSprite(Texture2D texture, Rectangle destinationRectangle, Color color)
		{
			CheckValid(texture);
			AddPolygon(new TexturedPolygon(texture, 0,
				destinationRectangle.X,
				destinationRectangle.Y,
				destinationRectangle.Width,
				destinationRectangle.Height,
				color,
				Vector2.Zero,
				Vector2.One,
				0));
		}

		private MethodInfo SpriteFont_GetGlyphIndexOrDefault = typeof(SpriteFont).GetMethod("GetGlyphIndexOrDefault", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		public unsafe void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
		{
			CheckValid(spriteFont, text);
			float sortKey = 0;
			var offset = Vector2.Zero;
			var firstGlyphOfLine = true;

			fixed (SpriteFont.Glyph* pGlyphs = spriteFont.Glyphs)
				for (var i = 0; i < text.Length; ++i)
				{
					var c = text[i];

					if (c == '\r')
						continue;

					if (c == '\n')
					{
						offset.X = 0;
						offset.Y += spriteFont.LineSpacing;
						firstGlyphOfLine = true;
						continue;
					}
					var currentGlyphIndex = (int)SpriteFont_GetGlyphIndexOrDefault.Invoke(spriteFont, new object[] { c });
					var pCurrentGlyph = pGlyphs + currentGlyphIndex;

					// The first character on a line might have a negative left side bearing.
					// In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
					//  so that text does not hang off the left side of its rectangle.
					if (firstGlyphOfLine)
					{
						offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
						firstGlyphOfLine = false;
					}
					else
					{
						offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
					}

					var p = offset;
					p.X += pCurrentGlyph->Cropping.X;
					p.Y += pCurrentGlyph->Cropping.Y;
					p += position;

					var texelScale = texelCache.GetTexelScale(spriteFont.Texture);
					_texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * texelScale.X;
					_texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * texelScale.Y;
					_texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * texelScale.X;
					_texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * texelScale.Y;
					AddPolygon(new TexturedPolygon(spriteFont.Texture, sortKey,
							p.X,
							p.Y,
							pCurrentGlyph->BoundsInTexture.Width,
							pCurrentGlyph->BoundsInTexture.Height,
							color,
							_texCoordTL,
							_texCoordBR,
							0));

					offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
				}
		}

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawString(
			SpriteFont spriteFont, string text, Vector2 position, Color color,
			float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			var scaleVec = new Vector2(scale, scale);
			DrawString(spriteFont, text, position, color, rotation, origin, scaleVec, effects, layerDepth);
		}

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public unsafe void DrawString(
			SpriteFont spriteFont, string text, Vector2 position, Color color,
			float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			CheckValid(spriteFont, text);

			float sortKey = 0;
			// set SortKey based on SpriteSortMode.
			switch (_sortMode)
			{
				// Comparison of Depth
				case SpriteSortMode.FrontToBack:
					sortKey = layerDepth;
					break;
				// Comparison of Depth in reverse
				case SpriteSortMode.BackToFront:
					sortKey = -layerDepth;
					break;
			}

			var flipAdjustment = Vector2.Zero;

			var flippedVert = (effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
			var flippedHorz = (effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;

			if (flippedVert || flippedHorz)
			{
				Vector2 size;

				size = spriteFont.MeasureString(text);

				if (flippedHorz)
				{
					origin.X *= -1;
					flipAdjustment.X = -size.X;
				}

				if (flippedVert)
				{
					origin.Y *= -1;
					flipAdjustment.Y = spriteFont.LineSpacing - size.Y;
				}
			}

			Matrix transformation = Matrix.Identity;
			float cos = 0, sin = 0;
			if (rotation == 0)
			{
				transformation.M11 = flippedHorz ? -scale.X : scale.X;
				transformation.M22 = flippedVert ? -scale.Y : scale.Y;
				transformation.M41 = ((flipAdjustment.X - origin.X) * transformation.M11) + position.X;
				transformation.M42 = ((flipAdjustment.Y - origin.Y) * transformation.M22) + position.Y;
			}
			else
			{
				cos = (float)Math.Cos(rotation);
				sin = (float)Math.Sin(rotation);
				transformation.M11 = (flippedHorz ? -scale.X : scale.X) * cos;
				transformation.M12 = (flippedHorz ? -scale.X : scale.X) * sin;
				transformation.M21 = (flippedVert ? -scale.Y : scale.Y) * (-sin);
				transformation.M22 = (flippedVert ? -scale.Y : scale.Y) * cos;
				transformation.M41 = ((flipAdjustment.X - origin.X) * transformation.M11) + (flipAdjustment.Y - origin.Y) * transformation.M21 + position.X;
				transformation.M42 = ((flipAdjustment.X - origin.X) * transformation.M12) + (flipAdjustment.Y - origin.Y) * transformation.M22 + position.Y;
			}

			var offset = Vector2.Zero;
			var firstGlyphOfLine = true;

			fixed (SpriteFont.Glyph* pGlyphs = spriteFont.Glyphs)
				for (var i = 0; i < text.Length; ++i)
				{
					var c = text[i];

					if (c == '\r')
						continue;

					if (c == '\n')
					{
						offset.X = 0;
						offset.Y += spriteFont.LineSpacing;
						firstGlyphOfLine = true;
						continue;
					}
					var currentGlyphIndex = (int)SpriteFont_GetGlyphIndexOrDefault.Invoke(spriteFont, new object[] { c });
					var pCurrentGlyph = pGlyphs + currentGlyphIndex;

					// The first character on a line might have a negative left side bearing.
					// In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
					//  so that text does not hang off the left side of its rectangle.
					if (firstGlyphOfLine)
					{
						offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
						firstGlyphOfLine = false;
					}
					else
					{
						offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
					}

					var p = offset;

					if (flippedHorz)
						p.X += pCurrentGlyph->BoundsInTexture.Width;
					p.X += pCurrentGlyph->Cropping.X;

					if (flippedVert)
						p.Y += pCurrentGlyph->BoundsInTexture.Height - spriteFont.LineSpacing;
					p.Y += pCurrentGlyph->Cropping.Y;

					Vector2.Transform(ref p, ref transformation, out p);

					var texelScale = texelCache.GetTexelScale(spriteFont.Texture);
					_texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * texelScale.X;
					_texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * texelScale.Y;
					_texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * texelScale.X;
					_texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * texelScale.Y;

					if ((effects & SpriteEffects.FlipVertically) != 0)
					{
						var temp = _texCoordBR.Y;
						_texCoordBR.Y = _texCoordTL.Y;
						_texCoordTL.Y = temp;
					}
					if ((effects & SpriteEffects.FlipHorizontally) != 0)
					{
						var temp = _texCoordBR.X;
						_texCoordBR.X = _texCoordTL.X;
						_texCoordTL.X = temp;
					}

					if (rotation == 0f)
					{
						AddPolygon(new TexturedPolygon(spriteFont.Texture, sortKey,
								p.X,
								p.Y,
								pCurrentGlyph->BoundsInTexture.Width,
								pCurrentGlyph->BoundsInTexture.Height,
								color,
								_texCoordTL,
								_texCoordBR,
								layerDepth));
					}
					else
					{
						AddPolygon(new TexturedPolygon(spriteFont.Texture, sortKey,
								p.X,
								p.Y,
								0,
								0,
								pCurrentGlyph->BoundsInTexture.Width * scale.X,
								pCurrentGlyph->BoundsInTexture.Height * scale.Y,
								sin,
								cos,
								color,
								_texCoordTL,
								_texCoordBR,
								layerDepth));
					}

					offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
				}
		}

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		public unsafe void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
		{
			CheckValid(spriteFont, text);

			float sortKey = 0;

			var offset = Vector2.Zero;
			var firstGlyphOfLine = true;

			fixed (SpriteFont.Glyph* pGlyphs = spriteFont.Glyphs)
				for (var i = 0; i < text.Length; ++i)
				{
					var c = text[i];

					if (c == '\r')
						continue;

					if (c == '\n')
					{
						offset.X = 0;
						offset.Y += spriteFont.LineSpacing;
						firstGlyphOfLine = true;
						continue;
					}

					var currentGlyphIndex = (int)SpriteFont_GetGlyphIndexOrDefault.Invoke(spriteFont, new object[] { c });
					var pCurrentGlyph = pGlyphs + currentGlyphIndex;

					// The first character on a line might have a negative left side bearing.
					// In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
					//  so that text does not hang off the left side of its rectangle.
					if (firstGlyphOfLine)
					{
						offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
						firstGlyphOfLine = false;
					}
					else
					{
						offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
					}

					var p = offset;
					p.X += pCurrentGlyph->Cropping.X;
					p.Y += pCurrentGlyph->Cropping.Y;
					p += position;

					var texelScale = texelCache.GetTexelScale(spriteFont.Texture);
					_texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * texelScale.X;
					_texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * texelScale.Y;
					_texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * texelScale.X;
					_texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * texelScale.Y;

					AddPolygon(new TexturedPolygon(spriteFont.Texture, sortKey,
							p.X,
							p.Y,
							pCurrentGlyph->BoundsInTexture.Width,
							pCurrentGlyph->BoundsInTexture.Height,
							color,
							_texCoordTL,
							_texCoordBR,
							0));

					offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
				}
		}

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawString(
			SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color,
			float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			var scaleVec = new Vector2(scale, scale);
			DrawString(spriteFont, text, position, color, rotation, origin, scaleVec, effects, layerDepth);
		}

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public unsafe void DrawString(
			SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color,
			float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			CheckValid(spriteFont, text);

			float sortKey = 0;
			// set SortKey based on SpriteSortMode.
			switch (_sortMode)
			{
				// Comparison of Depth
				case SpriteSortMode.FrontToBack:
					sortKey = layerDepth;
					break;
				// Comparison of Depth in reverse
				case SpriteSortMode.BackToFront:
					sortKey = -layerDepth;
					break;
			}

			var flipAdjustment = Vector2.Zero;

			var flippedVert = (effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
			var flippedHorz = (effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;

			if (flippedVert || flippedHorz)
			{
				Vector2 size = spriteFont.MeasureString(text);

				if (flippedHorz)
				{
					origin.X *= -1;
					flipAdjustment.X = -size.X;
				}

				if (flippedVert)
				{
					origin.Y *= -1;
					flipAdjustment.Y = spriteFont.LineSpacing - size.Y;
				}
			}

			Matrix transformation = Matrix.Identity;
			float cos = 0, sin = 0;
			if (rotation == 0)
			{
				transformation.M11 = (flippedHorz ? -scale.X : scale.X);
				transformation.M22 = (flippedVert ? -scale.Y : scale.Y);
				transformation.M41 = ((flipAdjustment.X - origin.X) * transformation.M11) + position.X;
				transformation.M42 = ((flipAdjustment.Y - origin.Y) * transformation.M22) + position.Y;
			}
			else
			{
				cos = (float)Math.Cos(rotation);
				sin = (float)Math.Sin(rotation);
				transformation.M11 = (flippedHorz ? -scale.X : scale.X) * cos;
				transformation.M12 = (flippedHorz ? -scale.X : scale.X) * sin;
				transformation.M21 = (flippedVert ? -scale.Y : scale.Y) * (-sin);
				transformation.M22 = (flippedVert ? -scale.Y : scale.Y) * cos;
				transformation.M41 = (((flipAdjustment.X - origin.X) * transformation.M11) + (flipAdjustment.Y - origin.Y) * transformation.M21) + position.X;
				transformation.M42 = (((flipAdjustment.X - origin.X) * transformation.M12) + (flipAdjustment.Y - origin.Y) * transformation.M22) + position.Y;
			}

			var offset = Vector2.Zero;
			var firstGlyphOfLine = true;

			fixed (SpriteFont.Glyph* pGlyphs = spriteFont.Glyphs)
				for (var i = 0; i < text.Length; ++i)
				{
					var c = text[i];

					if (c == '\r')
						continue;

					if (c == '\n')
					{
						offset.X = 0;
						offset.Y += spriteFont.LineSpacing;
						firstGlyphOfLine = true;
						continue;
					}

					var currentGlyphIndex = (int)SpriteFont_GetGlyphIndexOrDefault.Invoke(spriteFont, new object[] { c });
					var pCurrentGlyph = pGlyphs + currentGlyphIndex;

					// The first character on a line might have a negative left side bearing.
					// In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
					//  so that text does not hang off the left side of its rectangle.
					if (firstGlyphOfLine)
					{
						offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
						firstGlyphOfLine = false;
					}
					else
					{
						offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
					}

					var p = offset;

					if (flippedHorz)
						p.X += pCurrentGlyph->BoundsInTexture.Width;
					p.X += pCurrentGlyph->Cropping.X;

					if (flippedVert)
						p.Y += pCurrentGlyph->BoundsInTexture.Height - spriteFont.LineSpacing;
					p.Y += pCurrentGlyph->Cropping.Y;

					Vector2.Transform(ref p, ref transformation, out p);

					var texelScale = texelCache.GetTexelScale(spriteFont.Texture);
					_texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * texelScale.X;
					_texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * texelScale.Y;
					_texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * texelScale.X;
					_texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * texelScale.Y;

					if ((effects & SpriteEffects.FlipVertically) != 0)
					{
						var temp = _texCoordBR.Y;
						_texCoordBR.Y = _texCoordTL.Y;
						_texCoordTL.Y = temp;
					}
					if ((effects & SpriteEffects.FlipHorizontally) != 0)
					{
						var temp = _texCoordBR.X;
						_texCoordBR.X = _texCoordTL.X;
						_texCoordTL.X = temp;
					}

					if (rotation == 0f)
					{
						AddPolygon(new TexturedPolygon(spriteFont.Texture, sortKey,
								p.X,
								p.Y,
								pCurrentGlyph->BoundsInTexture.Width * scale.X,
								pCurrentGlyph->BoundsInTexture.Height * scale.Y,
								color,
								_texCoordTL,
								_texCoordBR,
								layerDepth));
					}
					else
					{
						AddPolygon(new TexturedPolygon(spriteFont.Texture, sortKey,
								p.X,
								p.Y,
								0,
								0,
								pCurrentGlyph->BoundsInTexture.Width * scale.X,
								pCurrentGlyph->BoundsInTexture.Height * scale.Y,
								sin,
								cos,
								color,
								_texCoordTL,
								_texCoordBR,
								layerDepth));
					}

					offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
				}
		}
	}
}