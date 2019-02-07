using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.TexturedGeometry2D
{
	//実際の描画を行ふ部分
	public partial class GeometryBatchRenderer
	{
		private short[] _index;
		private VertexPositionColorTexture[] _vertexArray;

		/// <summary>
		/// Renders this instance.
		/// </summary>
		/// <exception cref="ObjectDisposedException">effect</exception>
		public void Render()
		{
			Setup();
			if (_effect != null && _effect.IsDisposed)
				throw new ObjectDisposedException("effect");
			if (primitivesCount == 0) return;
			Array.Sort(primitivesBuffer, 0, primitivesCount);
			unsafe
			{
				fixed (VertexPositionColorTexture* vertexArrayFixedPtr = _vertexArray)
				{
					var vertexArrayPtr = vertexArrayFixedPtr;
					fixed (short* indexArrayFixedPtr = _index)
					{
						var indexArrayPtr = indexArrayFixedPtr;
						int IndexBufferWriteIndex = 0;
						int VertexBufferWriteIndex = 0;
						Texture2D tex = null;
						for (int i = 0; i < primitivesBuffer.Length && i < primitivesCount; i++)
						{
							var item = primitivesBuffer[i];
							if (item == null || item.Indices == null) break;
							var shouldFlush = !ReferenceEquals(item.Texture, tex) ||
								IndexBufferWriteIndex + item.Indices.Length > _index.Length ||
								VertexBufferWriteIndex + item.actualPositions.Length > _vertexArray.Length;
							if (shouldFlush)
							{
								FlushVertexArray(IndexBufferWriteIndex / 3, VertexBufferWriteIndex, _effect, tex);
								tex = item.Texture;
								IndexBufferWriteIndex = VertexBufferWriteIndex = 0;
								vertexArrayPtr = vertexArrayFixedPtr;
								indexArrayPtr = indexArrayFixedPtr;
								GraphicsDevice.Textures[0] = tex;
							}
							unchecked
							{
								for (int j = 0; j < item.Indices.Length; j++)
								{
									*indexArrayPtr++ = item.Indices[j];
								}
							}
							Array.Copy(item.actualPositions, 0, _vertexArray, VertexBufferWriteIndex, item.actualPositions.Length);
							//foreach (var vertex in item.actualPositions)
							//{
							//	*vertexArrayPtr++ = vertex; //たいして変らない
							//}

							IndexBufferWriteIndex += item.Indices.Length;
							VertexBufferWriteIndex += item.actualPositions.Length;
							primitivesBuffer[i] = default;
						}
						FlushVertexArray(IndexBufferWriteIndex / 3, VertexBufferWriteIndex, _effect, tex);
					}
				}
			}
			primitivesCount = 0;
		}

		private void FlushVertexArray(int PrimitiveCount, int VertexCount, Effect effect, Texture texture)
		{
			if (VertexCount == 0)
				return;
			// If the effect is not null, then apply each pass and render the geometry
			if (effect != null)
			{
				var passes = effect.CurrentTechnique.Passes;
				foreach (var pass in passes)
				{
					pass.Apply();

					// Whatever happens in pass.Apply, make sure the texture being drawn
					// ends up in Textures[0].
					GraphicsDevice.Textures[0] = texture;

					GraphicsDevice.DrawUserIndexedPrimitives(
						PrimitiveType.TriangleList,
						_vertexArray,
						0,
						VertexCount,
						_index,
						0,
						PrimitiveCount,
						VertexPositionColorTexture.VertexDeclaration);
				}
			}
			else
			{
				// If no custom effect is defined, then simply render.
				GraphicsDevice.DrawUserIndexedPrimitives(
					PrimitiveType.TriangleList,
					_vertexArray,
					0,
					VertexCount,
					_index,
					0,
					PrimitiveCount,
					VertexPositionColorTexture.VertexDeclaration);
			}
		}
	}
}