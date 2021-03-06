using System;
using Bunit.Extensions;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bunit
{
	/// <summary>
	/// A test context is a factory that makes it possible to create components under tests.
	/// </summary>
	public class TestContext : TestContextBase
	{
		/// <summary>
		/// Gets bUnits JSInterop, that allows setting up handlers for <see cref="IJSRuntime.InvokeAsync{TValue}(string, object[])"/> invocations
		/// that components under tests will issue during testing. It also makes it possible to verify that the invocations has happened as expected.
		/// </summary>
		public BunitJSInterop JSInterop { get; } = new BunitJSInterop();

		/// <summary>
		/// Initializes a new instance of the <see cref="TestContext"/> class.
		/// </summary>
		public TestContext()
		{
			Services.AddDefaultTestContextServices(this, JSInterop);
		}

		/// <summary>
		/// Instantiates and performs a first render of a component of type <typeparamref name="TComponent"/>.
		/// </summary>
		/// <typeparam name="TComponent">Type of the component to render.</typeparam>
		/// <param name="parameters">Parameters to pass to the component when it is rendered.</param>
		/// <returns>The rendered <typeparamref name="TComponent"/>.</returns>
		public virtual IRenderedComponent<TComponent> RenderComponent<TComponent>(params ComponentParameter[] parameters)
			where TComponent : IComponent
		{
			var renderFragment = new ComponentParameterCollection { parameters }
				.ToRenderFragment<TComponent>();

			return Render<TComponent>(renderFragment);
		}

		/// <summary>
		/// Instantiates and performs a first render of a component of type <typeparamref name="TComponent"/>.
		/// </summary>
		/// <typeparam name="TComponent">Type of the component to render.</typeparam>
		/// <param name="parameterBuilder">The ComponentParameterBuilder action to add type safe parameters to pass to the component when it is rendered.</param>
		/// <returns>The rendered <typeparamref name="TComponent"/>.</returns>
		public virtual IRenderedComponent<TComponent> RenderComponent<TComponent>(Action<ComponentParameterCollectionBuilder<TComponent>> parameterBuilder)
			where TComponent : IComponent
		{
			var renderFragment = new ComponentParameterCollectionBuilder<TComponent>(parameterBuilder)
				.Build()
				.ToRenderFragment<TComponent>();

			return Render<TComponent>(renderFragment);
		}

		/// <summary>
		/// Renders the <paramref name="renderFragment"/> and returns the first <typeparamref name="TComponent"/> in the resulting render tree.
		/// </summary>
		/// <remarks>
		/// Calling this method is equivalent to calling <c>Render(renderFragment).FindComponent&lt;TComponent&gt;()</c>.
		/// </remarks>
		/// <typeparam name="TComponent">The type of component to find in the render tree.</typeparam>
		/// <param name="renderFragment">The render fragment to render.</param>
		/// <returns>The <see cref="IRenderedComponent{TComponent}"/>.</returns>
		public virtual IRenderedComponent<TComponent> Render<TComponent>(RenderFragment renderFragment)
			where TComponent : IComponent
			=> (IRenderedComponent<TComponent>)this.RenderInsideRenderTree<TComponent>(renderFragment);

		/// <summary>
		/// Renders the <paramref name="renderFragment"/> and returns it as a <see cref="IRenderedFragment"/>.
		/// </summary>
		/// <param name="renderFragment">The render fragment to render.</param>
		/// <returns>The <see cref="IRenderedFragment"/>.</returns>
		public virtual IRenderedFragment Render(RenderFragment renderFragment)
			=> (IRenderedFragment)this.RenderInsideRenderTree(renderFragment);

#if NET5_0_OR_GREATER
		/// <summary>
		/// Instantiates and performs a first render of a component of type <typeparamref name="TComponent"/>,
		/// and stubs out all child components.
		/// </summary>
		/// <remarks>
		/// If you pass child components to <typeparamref name="TComponent"/> through parameters,
		/// these will be stubbed out as well.
		/// </remarks>
		/// <typeparam name="TComponent">Type of the component to render.</typeparam>
		/// <param name="parameters">Parameters to pass to the component when it is rendered.</param>
		/// <returns>The rendered <typeparamref name="TComponent"/>.</returns>
		public virtual IRenderedComponent<TComponent> ShallowRenderComponent<TComponent>(params ComponentParameter[] parameters)
			where TComponent : IComponent
		{
			var renderFragment = new ComponentParameterCollection { parameters }
				.ToRenderFragment<TComponent>();

			return ShallowRender<TComponent>(renderFragment);
		}

		/// <summary>
		/// Instantiates and performs a first render of a component of type <typeparamref name="TComponent"/>,
		/// and stubs out all child components.
		/// </summary>
		/// <remarks>
		/// If you pass child components to <typeparamref name="TComponent"/> through parameters,
		/// these will be stubbed out as well.
		/// </remarks>
		/// <typeparam name="TComponent">Type of the component to render.</typeparam>
		/// <param name="parameterBuilder">The ComponentParameterBuilder action to add type safe parameters
		/// to pass to the component when it is rendered.</param>
		/// <param name="options">Render options for the <see cref="Stub{TComponent}"/> components.</param>
		/// <returns>The rendered <typeparamref name="TComponent"/>.</returns>
		public virtual IRenderedComponent<TComponent> ShallowRenderComponent<TComponent>(Action<ComponentParameterCollectionBuilder<TComponent>> parameterBuilder, StubOptions? options = null)
			where TComponent : IComponent
		{
			var renderFragment = new ComponentParameterCollectionBuilder<TComponent>(parameterBuilder)
				.Build()
				.ToRenderFragment<TComponent>();

			return ShallowRender<TComponent>(renderFragment, options);
		}

		/// <summary>
		/// Renders the first component (root component) in the <paramref name="renderFragment"/>,
		/// and stubs out all other components, both child components as well as sibling components.
		/// </summary>
		/// <remarks>
		/// The first component in the <paramref name="renderFragment"/> must be of type <typeparamref name="TComponent"/>
		/// to use this method, since it is equivalent to calling <c>ShallowRender(renderFragment).FindComponent&lt;TComponent&gt;()</c>.
		/// </remarks>
		/// <typeparam name="TComponent">The type of component to find in the render tree.</typeparam>
		/// <param name="renderFragment">The render fragment to render.</param>
		/// <param name="options">Render options for the <see cref="Stub{TComponent}"/> components.</param>
		/// <returns>The <see cref="IRenderedComponent{TComponent}"/>.</returns>
		public virtual IRenderedComponent<TComponent> ShallowRender<TComponent>(RenderFragment renderFragment, StubOptions? options = null)
			where TComponent : IComponent
			=> (IRenderedComponent<TComponent>)this.ShallowRenderInsideRenderTree<TComponent>(renderFragment, options);

		/// <summary>
		/// Renders the first component (root component) in the <paramref name="renderFragment"/>,
		/// and stubs out all other components, both child components as well as sibling components.
		/// </summary>
		/// <param name="renderFragment">The render fragment to render.</param>
		/// <param name="options">Render options for the <see cref="Stub{TComponent}"/> components.</param>
		/// <returns>The <see cref="IRenderedFragment"/>.</returns>
		public virtual IRenderedFragment ShallowRender(RenderFragment renderFragment, StubOptions? options = null)
			=> (IRenderedFragment)this.ShallowRenderInsideRenderTree(renderFragment, options);
#endif

		/// <summary>
		/// Dummy method required to allow Blazor's compiler to generate
		/// C# from .razor files.
		/// </summary>
		protected virtual void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder) { }
	}
}
