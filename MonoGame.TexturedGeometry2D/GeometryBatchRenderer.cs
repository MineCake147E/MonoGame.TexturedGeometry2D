using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace MonoGame.TexturedGeometry2D
{
	/// <summary>
	/// SpriteBatch expanded.
	/// Support for asynchronous division of polygon.
	/// It is necessary to add items to the outside of Draw thread.
	/// </ summary>
	public sealed partial class GeometryBatchRenderer : IDisposable
	{
		/// <summary>
		/// The sort mode
		/// </summary>
		private SpriteSortMode _sortMode;

		/// <summary>
		/// The blend state
		/// </summary>
		private BlendState _blendState;

		/// <summary>
		/// The sampler state
		/// </summary>
		private SamplerState _samplerState;

		/// <summary>
		/// The depth stencil state
		/// </summary>
		private DepthStencilState _depthStencilState;

		/// <summary>
		/// The rasterizer state
		/// </summary>
		private RasterizerState _rasterizerState;

		/// <summary>
		/// effect
		/// </summary>
		private Effect _effect;

		/// <summary>
		/// whether Begin() has been called
		/// </summary>
		private bool _beginCalled;

		/// <summary>
		/// device
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; set; }

		/// <summary>
		/// default effect
		/// </summary>
		private SpriteEffect _spriteEffect;

		/// <summary>
		/// default effect pass
		/// </summary>
		private readonly EffectPass _spritePass;

		/// <summary>
		/// The matrix transform
		/// </summary>
		private readonly EffectParameter _matrixTransform;

		/// <summary>
		/// The tessellator pool
		/// </summary>
		private TessellatorPool tessellators = new TessellatorPool();

		private Matrix? _matrix;
		private Viewport _lastViewport;
		private Matrix _projection;

		//Rectangle _tempRect = new Rectangle(0, 0, 0, 0);
		private Vector2 _texCoordTL = new Vector2(0, 0);

		private Vector2 _texCoordBR = new Vector2(0, 0);

		internal static bool NeedsHalfPixelOffset = false;

		internal int primitivesCount = 0;
		internal object primBLockObj = new object();
		internal TexturedPolygon[] primitivesBuffer = new TexturedPolygon[256];
		private readonly ActionBlock<int> PolygonCompiler;

		/// <summary>
		/// Initializes a new instance of the <see cref="GeometryBatchRenderer"/> class.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device.</param>
		/// <exception cref="ArgumentNullException">graphicsDevice - GraphicsDevice must not be null.</exception>
		public GeometryBatchRenderer(GraphicsDevice graphicsDevice) : base()
		{
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice), "GraphicsDevice must not be null.");

			_spriteEffect = new SpriteEffect(graphicsDevice);
			_spritePass = _spriteEffect.CurrentTechnique.Passes[0];
			_matrixTransform = _spriteEffect.Parameters["MatrixTransform"];
			//_batcher = new SpriteBatcher(graphicsDevice);

			_beginCalled = false;

			_index = new short[65536];
			_vertexArray = new VertexPositionColorTexture[32768];

			PolygonCompiler = new ActionBlock<int>(a => CompilePolygon(a),
				new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount });
		}

		/// <summary>
		/// Begins a new sprite and text batch with the specified render state.
		/// </summary>
		/// <param name="sortMode">The drawing order for sprite and text drawing. <see cref="SpriteSortMode.Deferred"/> by default.</param>
		/// <param name="blendState">State of the blending. Uses <see cref="BlendState.AlphaBlend"/> if null.</param>
		/// <param name="samplerState">State of the sampler. Uses <see cref="SamplerState.LinearClamp"/> if null.</param>
		/// <param name="depthStencilState">State of the depth-stencil buffer. Uses <see cref="DepthStencilState.None"/> if null.</param>
		/// <param name="rasterizerState">State of the rasterization. Uses <see cref="RasterizerState.CullCounterClockwise"/> if null.</param>
		/// <param name="effect">A custom <see cref="Effect"/> to override the default sprite effect. Uses default sprite effect if null.</param>
		/// <param name="transformMatrix">An optional matrix used to transform the sprite geometry. Uses <see cref="Matrix.Identity"/> if null.</param>
		/// <exception cref="InvalidOperationException">Thrown if <see cref="Begin"/> is called next time without previous <see cref="End"/>.</exception>
		/// <remarks>This method uses optional parameters.</remarks>
		/// <remarks>The <see cref="Begin"/> Begin should be called before drawing commands, and you cannot call it again before subsequent <see cref="End"/>.</remarks>
		public void Begin
		(
			 SpriteSortMode sortMode = SpriteSortMode.Deferred,
			 BlendState blendState = null,
			 SamplerState samplerState = null,
			 DepthStencilState depthStencilState = null,
			 RasterizerState rasterizerState = null,
			 Effect effect = null,
			 Matrix? transformMatrix = null
		)
		{
			if (_beginCalled)
				throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");

			// defaults
			_sortMode = sortMode;
			_blendState = blendState ?? BlendState.AlphaBlend;
			_samplerState = samplerState ?? SamplerState.LinearClamp;
			_depthStencilState = depthStencilState ?? DepthStencilState.None;
			_rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
			_effect = effect;
			_matrix = transformMatrix;

			// Setup things now so a user can change them.
			if (sortMode == SpriteSortMode.Immediate)
			{
				throw new NotSupportedException("Immediate drawing mode is not supported.");
			}
			else if (sortMode == SpriteSortMode.Texture)
			{
				throw new NotSupportedException("Texture sort mode is not supported.");
			}

			_beginCalled = true;
		}

		/// <summary>
		/// Flushes all batched text and sprites to the screen.
		/// </summary>
		/// <remarks>This command should be called after <see cref="Begin"/> and drawing commands.</remarks>
		public void End()
		{
			if (!_beginCalled)
				throw new InvalidOperationException("Begin must be called before calling End.");
			_beginCalled = false;
		}

		private void Setup()
		{
			_blendState = _blendState ?? BlendState.AlphaBlend;
			_samplerState = _samplerState ?? SamplerState.LinearClamp;
			_depthStencilState = _depthStencilState ?? DepthStencilState.None;
			_rasterizerState = _rasterizerState ?? RasterizerState.CullCounterClockwise;
			var gd = GraphicsDevice;
			gd.BlendState = _blendState;
			gd.DepthStencilState = _depthStencilState;
			gd.RasterizerState = _rasterizerState;
			gd.SamplerStates[0] = _samplerState;

			var vp = gd.Viewport;
			if ((vp.Width != _lastViewport.Width) || (vp.Height != _lastViewport.Height))
			{
				// Normal 3D cameras look into the -z direction (z = 1 is in front of z = 0). The
				// sprite batch layer depth is the opposite (z = 0 is in front of z = 1).
				// --> We get the correct matrix with near plane 0 and far plane -1.
				Matrix.CreateOrthographicOffCenter(0, vp.Width, vp.Height, 0, 0, -1, out _projection);

				// Some platforms require a half pixel offset to match DX.
				if (NeedsHalfPixelOffset)
				{
					_projection.M41 += -0.5f * _projection.M11;
					_projection.M42 += -0.5f * _projection.M22;
				}

				_lastViewport = vp;
			}

			if (_matrix.HasValue)
				_matrixTransform.SetValue(_matrix.GetValueOrDefault() * _projection);
			else
				_matrixTransform.SetValue(_projection);

			_spritePass.Apply();
		}

		private void AddPolygon(in TexturedPolygon polygon)
		{
			if (!_beginCalled) throw new InvalidOperationException("Attempted to post new polygons to dataflow before calling Begin!");
			primitivesBuffer[primitivesCount] = polygon;
			if (primitivesCount >= primitivesBuffer.Length - 4)
			{
				var oldlen = primitivesBuffer.Length;
				Array.Resize(ref primitivesBuffer, oldlen + 256);
			}
			if (polygon.Indices == null) PolygonCompiler.SendAsync(primitivesCount);
			primitivesCount++;
		}

		/// <summary>
		/// Compiles the polygon.
		/// </summary>
		/// <param name="index">The index.</param>
		public void CompilePolygon(int index)
		{
			primitivesBuffer[index].Tessellate(tessellators);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="GeometryBatchRenderer"/> is rendered.
		/// </summary>
		/// <value>
		///   <c>true</c> if rendered; otherwise, <c>false</c>.
		/// </value>
		public bool Rendered { get => primitivesCount == 0; }

		#region Validation

		private void CheckValid(Texture2D texture)
		{
			if (texture == null)
				throw new ArgumentNullException(nameof(texture));
			if (!_beginCalled)
				throw new InvalidOperationException("Draw was called, but Begin has not yet been called. Begin must be called successfully before you can call Draw.");
		}

		private void CheckValid(SpriteFont spriteFont, string text)
		{
			if (spriteFont == null)
				throw new ArgumentNullException(nameof(spriteFont));
			if (text == null)
				throw new ArgumentNullException(nameof(text));
			if (!_beginCalled)
				throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
		}

		private void CheckValid(SpriteFont spriteFont, StringBuilder text)
		{
			if (spriteFont == null)
				throw new ArgumentNullException(nameof(spriteFont));
			if (text == null)
				throw new ArgumentNullException(nameof(text));
			if (!_beginCalled)
				throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
		}

		#endregion Validation

		#region IDisposable Support

		private bool disposedValue = false; // To detect duplicate calls

		private void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// Discard the managed state (managed object).
					_spriteEffect?.Dispose();
					_spriteEffect = null;
				}

				// Release the unmanaged resource (unmanaged object) and override the finalizer below.
				// Set the large field to null.
				GraphicsDevice = null;
				disposedValue = true;
			}
		}

		// Override the finalizer only if Dispose (bool disposing) above contains code to release unmanaged resources.
		// ~ GeometryBatch () {
		// Please do not change this code.Write the cleanup code in Dispose(bool disposing) above.
		// Dispose (false);
		//}

		// This code has been added to correctly implement discardable patterns.
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// Please do not change this code. Write the cleanup code in Dispose (bool disposing) above.
			Dispose(true);
			// If the finalizer above is overridden, uncomment the following line.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}