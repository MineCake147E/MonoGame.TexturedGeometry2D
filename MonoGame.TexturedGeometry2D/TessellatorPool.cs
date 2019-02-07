using LibTessDotNet;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MonoGame.TexturedGeometry2D
{
	/// <summary>
	/// The Tessellator Pool
	/// </summary>
	public sealed class TessellatorPool
	{
		/// <summary>
		/// The available tessellators
		/// </summary>
		private readonly ConcurrentQueue<Tess> AvailableTessellators = new ConcurrentQueue<Tess>();

		private int TessellatorCount = 0;

		/// <summary>
		/// Gets the tessellator available.
		/// </summary>
		/// <returns></returns>
		public Tess GetTessellatorAvailable()
		{
			if (!AvailableTessellators.TryDequeue(out var tess))
			{
				tess = new Tess
				{
					NoEmptyPolygons = true
				};
				if (Interlocked.Increment(ref TessellatorCount) > Environment.ProcessorCount)
				{
					Console.WriteLine($"WARNING: Greater number of Tessellators than Processors' count has been Created! Number has reached {TessellatorCount}!");
				}
			}
			return tess;
		}

		/// <summary>
		/// Returns the tessellator.
		/// </summary>
		/// <param name="tessellator">The tessellator.</param>
		public void ReturnTessellator(Tess tessellator)
		{
			AvailableTessellators.Enqueue(tessellator);
		}
	}
}