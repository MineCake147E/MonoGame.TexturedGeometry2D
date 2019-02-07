using LibTessDotNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.TexturedGeometry2D
{
	/// <summary>
	/// Definition of textured polygon
	/// </summary>
	public struct TexturedPolygon : IEquatable<TexturedPolygon>, IComparable<TexturedPolygon>
	{
		/// <summary>
		/// Gets the sprite indices.
		/// </summary>
		/// <value>
		/// The sprite indices.
		/// </value>
		internal static short[] SpriteIndices { get; } = new short[] { 0, 1, 2, 1, 3, 2 };

		/// <summary>
		/// Gets the tirangle indices.
		/// </summary>
		/// <value>
		/// The tirangle indices.
		/// </value>
		internal static short[] TirangleIndices { get; } = new short[] { 0, 1, 2 };

		/// <summary>
		/// Gets or sets the sort key.
		/// </summary>
		/// <value>
		/// The sort key.
		/// </value>
		public float SortKey { get; set; }

		/// <summary>
		/// Gets the texture.
		/// </summary>
		/// <value>
		/// The texture.
		/// </value>
		public Texture2D Texture { get; }

		/// <summary>
		/// Gets the vertices.
		/// </summary>
		/// <value>
		/// The vertices.
		/// </value>
		public IReadOnlyList<VertexPositionColorTexture> Vertices { get; }

		/// <summary>
		/// The actual positions
		/// </summary>
		internal readonly VertexPositionColorTexture[] actualPositions;

		/// <summary>
		/// The indices
		/// </summary>
		internal short[] Indices;

		/// <summary>
		/// Initializes a new instance of the <see cref="TexturedPolygon"/> struct.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="vertices">The vertices.</param>
		public TexturedPolygon(Texture2D texture, params VertexPositionColorTexture[] vertices)
		{
			Texture = texture;
			actualPositions = vertices;
			Vertices = actualPositions;
			Indices = null;
			SortKey = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TexturedPolygon"/> struct.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="vertices">The vertices.</param>
		/// <param name="indices">The indices.</param>
		public TexturedPolygon(Texture2D texture, VertexPositionColorTexture[] vertices, short[] indices)
		{
			Texture = texture;
			actualPositions = vertices;
			Vertices = actualPositions;
			Indices = indices;
			SortKey = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TexturedPolygon"/> struct.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="sortKey">The sort key.</param>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="dx">The dx.</param>
		/// <param name="dy">The dy.</param>
		/// <param name="w">The w.</param>
		/// <param name="h">The h.</param>
		/// <param name="sin">The sin.</param>
		/// <param name="cos">The cos.</param>
		/// <param name="color">The color.</param>
		/// <param name="texCoordTL">The tex coord tl.</param>
		/// <param name="texCoordBR">The tex coord br.</param>
		/// <param name="depth">The depth.</param>
		internal TexturedPolygon(Texture2D texture, float sortKey, float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depth)
		{
			Indices = SpriteIndices;
			Texture = texture;
			SortKey = sortKey;
			var vertexTL = new VertexPositionColorTexture();
			vertexTL.Position.X = x + dx * cos - dy * sin;
			vertexTL.Position.Y = y + dx * sin + dy * cos;
			vertexTL.Position.Z = depth;
			vertexTL.Color = color;
			vertexTL.TextureCoordinate.X = texCoordTL.X;
			vertexTL.TextureCoordinate.Y = texCoordTL.Y;
			var vertexTR = new VertexPositionColorTexture();
			vertexTR.Position.X = x + (dx + w) * cos - dy * sin;
			vertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
			vertexTR.Position.Z = depth;
			vertexTR.Color = color;
			vertexTR.TextureCoordinate.X = texCoordBR.X;
			vertexTR.TextureCoordinate.Y = texCoordTL.Y;
			var vertexBL = new VertexPositionColorTexture();
			vertexBL.Position.X = x + dx * cos - (dy + h) * sin;
			vertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
			vertexBL.Position.Z = depth;
			vertexBL.Color = color;
			vertexBL.TextureCoordinate.X = texCoordTL.X;
			vertexBL.TextureCoordinate.Y = texCoordBR.Y;
			var vertexBR = new VertexPositionColorTexture();
			vertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
			vertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
			vertexBR.Position.Z = depth;
			vertexBR.Color = color;
			vertexBR.TextureCoordinate.X = texCoordBR.X;
			vertexBR.TextureCoordinate.Y = texCoordBR.Y;
			actualPositions = new VertexPositionColorTexture[] { vertexTL, vertexTR, vertexBL, vertexBR };
			Vertices = actualPositions;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TexturedPolygon"/> struct.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="sortKey">The sort key.</param>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="w">The w.</param>
		/// <param name="h">The h.</param>
		/// <param name="color">The color.</param>
		/// <param name="texCoordTL">The tex coord tl.</param>
		/// <param name="texCoordBR">The tex coord br.</param>
		/// <param name="depth">The depth.</param>
		internal TexturedPolygon(Texture2D texture, float sortKey, float x, float y, float w, float h, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depth)
		{
			Indices = SpriteIndices;
			Texture = texture;
			SortKey = sortKey;
			var vertexTL = new VertexPositionColorTexture();
			vertexTL.Position.X = x;
			vertexTL.Position.Y = y;
			vertexTL.Position.Z = depth;
			vertexTL.Color = color;
			vertexTL.TextureCoordinate.X = texCoordTL.X;
			vertexTL.TextureCoordinate.Y = texCoordTL.Y;
			var vertexTR = new VertexPositionColorTexture();
			vertexTR.Position.X = x + w;
			vertexTR.Position.Y = y;
			vertexTR.Position.Z = depth;
			vertexTR.Color = color;
			vertexTR.TextureCoordinate.X = texCoordBR.X;
			vertexTR.TextureCoordinate.Y = texCoordTL.Y;
			var vertexBL = new VertexPositionColorTexture();
			vertexBL.Position.X = x;
			vertexBL.Position.Y = y + h;
			vertexBL.Position.Z = depth;
			vertexBL.Color = color;
			vertexBL.TextureCoordinate.X = texCoordTL.X;
			vertexBL.TextureCoordinate.Y = texCoordBR.Y;
			var vertexBR = new VertexPositionColorTexture();
			vertexBR.Position.X = x + w;
			vertexBR.Position.Y = y + h;
			vertexBR.Position.Z = depth;
			vertexBR.Color = color;
			vertexBR.TextureCoordinate.X = texCoordBR.X;
			vertexBR.TextureCoordinate.Y = texCoordBR.Y;
			actualPositions = new VertexPositionColorTexture[] { vertexTL, vertexTR, vertexBL, vertexBR };
			Vertices = actualPositions;
		}

		internal void Tessellate(TessellatorPool tessellators)
		{
			if (Indices == null)
			{
				if (actualPositions.Length == 4)
				{
					Indices = SpriteIndices;
					return;
				}
				var pos = new ContourVertex[actualPositions.Length];
				var tessellator = tessellators.GetTessellatorAvailable();
				try
				{
					for (int i = 0; i < actualPositions.Length; i++)
					{
						pos[i] = actualPositions[i].GetContourVertex();
					}

					tessellator.AddContour(pos, ContourOrientation.Clockwise);
					tessellator.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);
					Indices = new short[tessellator.Elements.Length];
					Span<short> indexConversionTable = stackalloc short[actualPositions.Length];
					indexConversionTable.Fill(-1);
					if (tessellator.VertexCount != actualPositions.Length)  //Detecting addition or deletion of points
					{
						throw new ArithmeticException("The shapes may be intersecting.");
					}
					else
					{
						for (var i = 0; i < tessellator.Elements.Length; i++)
						{
							int index = tessellator.Elements[i];
							short conv = indexConversionTable[index];
							if (conv == -1)
							{
								var position = tessellator.Vertices[index].Position.GetVector();
								short q = (short)Array.FindIndex(actualPositions, a => a.Position == position);
								if (q == -1) throw new KeyNotFoundException("No matching value was found.");
								indexConversionTable[index] = Indices[i] = q;
							}
							else
							{
								Indices[i] = conv;
							}
						}
					}
				}
				finally
				{
					tessellators.ReturnTessellator(tessellator);
				}
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			return obj is TexturedPolygon && Equals((TexturedPolygon)obj);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
		/// </returns>
		public bool Equals(TexturedPolygon other)
		{
			return SortKey == other.SortKey &&
				   EqualityComparer<Texture2D>.Default.Equals(Texture, other.Texture) &&
				   EqualityComparer<VertexPositionColorTexture[]>.Default.Equals(actualPositions, other.actualPositions) &&
				   EqualityComparer<short[]>.Default.Equals(Indices, other.Indices);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			var hashCode = -329965925;
			hashCode = hashCode * -1521134295 + SortKey.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(Texture);
			hashCode = hashCode * -1521134295 + EqualityComparer<VertexPositionColorTexture[]>.Default.GetHashCode(actualPositions);
			return hashCode;
		}

		/// <summary>
		/// Compares the value of this instance to a specified <see cref="TexturedPolygon"/> value and returns an integer that indicates whether this instance is less than, equal to, or greater than the specified <see cref="TexturedPolygon"/> value.
		/// </summary>
		/// <param name="other">The <see cref="TexturedPolygon"/> to compare to the current instance.</param>
		/// <returns>
		/// A signed number indicating the relative values of this instance and the other parameter.
		/// </returns>
		public int CompareTo(TexturedPolygon other) => SortKey.CompareTo(other.SortKey);

		/// <summary>
		/// Indicates whether the values of two specified <see cref="TexturedPolygon"/> objects are equal.
		/// </summary>
		/// <param name="polygon1">The first <see cref="TexturedPolygon"/> to compare.</param>
		/// <param name="polygon2">The second <see cref="TexturedPolygon"/> to compare.</param>
		/// <returns>
		///   <c>true</c> if the value of polygon1 is the same as the value of polygon2; otherwise, <c>false</c>.
		/// </returns>
		public static bool operator ==(TexturedPolygon polygon1, TexturedPolygon polygon2)
		{
			return polygon1.Equals(polygon2);
		}

		/// <summary>
		/// Indicates whether the values of two specified <see cref="TexturedPolygon"/> objects are not equal.
		/// </summary>
		/// <param name="polygon1">The first <see cref="TexturedPolygon"/> to compare.</param>
		/// <param name="polygon2">The second <see cref="TexturedPolygon"/> to compare.</param>
		/// <returns>
		///   <c>true</c> if polygon1 and polygon2 are not equal; otherwise, <c>false</c>.
		/// </returns>
		public static bool operator !=(TexturedPolygon polygon1, TexturedPolygon polygon2)
		{
			return !(polygon1 == polygon2);
		}

		/// <summary>
		/// Determines whether one specified <see cref="TexturedPolygon"/> is less than another specified <see cref="TexturedPolygon"/>.
		/// </summary>
		/// <param name="left">The first <see cref="TexturedPolygon"/> to compare.</param>
		/// <param name="right">The second <see cref="TexturedPolygon"/> to compare.</param>
		/// <returns>
		///   <c>true</c> if left is less than right; otherwise, <c>false</c>.
		/// </returns>
		public static bool operator <(TexturedPolygon left, TexturedPolygon right)
		{
			return left.CompareTo(right) < 0;
		}

		/// <summary>
		/// Returns a value that indicates whether a specified <see cref="TexturedPolygon"/> is less than or equal to another specified <see cref="TexturedPolygon"/>.
		/// </summary>
		/// <param name="left">The first <see cref="TexturedPolygon"/> to compare.</param>
		/// <param name="right">The second <see cref="TexturedPolygon"/> to compare.</param>
		/// <returns>
		///   <c>true</c> if left is less than or equal to right; otherwise, <c>false</c>.
		/// </returns>
		public static bool operator <=(TexturedPolygon left, TexturedPolygon right)
		{
			return left.CompareTo(right) <= 0;
		}

		/// <summary>
		/// Determines whether one specified <see cref="TexturedPolygon"/> is greater than another specified <see cref="TexturedPolygon"/> value.
		/// </summary>
		/// <param name="left">The first <see cref="TexturedPolygon"/> to compare.</param>
		/// <param name="right">The second <see cref="TexturedPolygon"/> to compare.</param>
		/// <returns>
		///   <c>true</c> if left is greater than right; otherwise, <c>false</c>.
		/// </returns>
		public static bool operator >(TexturedPolygon left, TexturedPolygon right)
		{
			return left.CompareTo(right) > 0;
		}

		/// <summary>
		/// Determines whether one specified <see cref="TexturedPolygon"/> is greater than or equal to another specified <see cref="TexturedPolygon"/>.
		/// </summary>
		/// <param name="left">The first <see cref="TexturedPolygon"/> to compare.</param>
		/// <param name="right">The second  <see cref="TexturedPolygon"/> to compare.</param>
		/// <returns>
		///   <c>true</c> if <see cref="TexturedPolygon"/> is greater than or equal to <see cref="TexturedPolygon"/>; otherwise, <c>false</c>.
		/// </returns>
		public static bool operator >=(TexturedPolygon left, TexturedPolygon right)
		{
			return left.CompareTo(right) >= 0;
		}
	}
}