/*
 * Copyright 2024, Nexus6 <nexus6.haiku@icloud.com>
 * All rights reserved. Distributed under the terms of the MIT license.
 */

using Haiku.App;
using Haiku.Interface;
using Haiku.Support;
using static Haiku.App.Symbols;
using static Haiku.Interface.Symbols;
using System.Reflection;

namespace Haiku.Interface.LayoutBuilder
{

	public interface IBuilder<TParentBuilder>
	{
		public TParentBuilder? Parent { get; set; }

		public TParentBuilder? End();
	}

	public class BaseBuilder<TParentBuilder>: IBuilder<TParentBuilder>
	{
		public TParentBuilder? Parent { get; set; }

		private List<Tuple<Type, object>> Children { get; } = new();

		public void AddChild<TChild>(TChild child)
		{
			Children.Add(new Tuple<Type, object>(typeof(TChild), child));
		}

		public TParentBuilder? End()
		{
			return Parent;
		}

	}

	public class Group: Group<Group>
	{
		public Group(Orientation orientation = Orientation.Horizontal,
			float spacing = B_USE_DEFAULT_SPACING)
			: base(orientation, spacing)
		{}

		public Group(BWindow window, Orientation orientation = Orientation.Horizontal,
			float spacing = B_USE_DEFAULT_SPACING)
			: base(window, orientation, spacing)
		{}

		public Group(BView view, Orientation orientation = B_HORIZONTAL,
			float spacing = B_USE_DEFAULT_SPACING)
			: base(view, orientation, spacing)
		{}

		public Group(BGroupLayout layout)
			: base(layout)
		{}

		public Group(BGroupView view)
			: base(view)
		{}
	}

	public class Group<TParentBuilder>: BaseBuilder<TParentBuilder>
	{
		public BGroupLayout? Layout	{ get; set; }

		public BView? View { get => Layout?.View(); }

		public Group(Orientation orientation = Orientation.Horizontal,
			float spacing = B_USE_DEFAULT_SPACING)
		{
			Layout = new BGroupLayout(orientation, spacing);
		}

		public Group(BWindow window, Orientation orientation = Orientation.Horizontal,
			float spacing = B_USE_DEFAULT_SPACING)
		{
			Layout = new BGroupLayout(orientation, spacing);
			window.SetLayout(Layout);
			View?.AdoptSystemColors();
		}

		public Group(BView view, Orientation orientation = B_HORIZONTAL,
			float spacing = B_USE_DEFAULT_SPACING)
		{
			Layout = new BGroupLayout(orientation, spacing);
			if (view.HasDefaultColors())
				view.AdoptSystemColors();
			view.SetLayout(Layout);
		}

		public Group(BGroupLayout layout)
		{
			Layout = layout;
		}

		public Group(BGroupView view)
		{
			Layout = view.GroupLayout();
		}

		~Group()
		{
			Console.WriteLine("Group<> finalized, View = {0}", View?.Name());
		}

		public Group<TParentBuilder> GetLayout(out BGroupLayout? _layout)
		{
			_layout = Layout;
			return this;
		}

		public Group<TParentBuilder> GetView(out BView? _view)
		{
			_view = View;
			return this;
		}

		public Group<TParentBuilder> Add(BView view)
		{
			Layout?.AddView(view);
			return this;
		}

		public Group<TParentBuilder> Add(BView view, float weight)
		{
			Layout?.AddView(view, weight);
			return this;
		}

		public Group<TParentBuilder> Add(BLayoutItem item)
		{
			Layout?.AddItem(item);
			return this;
		}

		public Group<TParentBuilder> Add(BLayoutItem item, float weight)
		{
			Layout?.AddItem(item, weight);
			return this;
		}

		public Group<Group<TParentBuilder>> AddGroup(Orientation orientation,
			float spacing = B_USE_DEFAULT_SPACING, float weight = 1.0f)
		{
			Group<Group<TParentBuilder>> builder = new(orientation, spacing);
			AddChild<Group<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, weight);
			return builder;
		}

		public Group<Group<TParentBuilder>> AddGroup(BGroupView groupView, float weight = 1.0f)
		{
			Group<Group<TParentBuilder>> builder = new(groupView);
			AddChild<Group<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, weight);
			return builder;
		}

		public Group<Group<TParentBuilder>> AddGroup(BGroupLayout groupLayout, float weight = 1.0f)
		{
			Group<Group<TParentBuilder>> builder = new(groupLayout);
			AddChild<Group<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, weight);
			return builder;
		}

		public Grid<Group<TParentBuilder>> AddGrid(float horizontal = B_USE_DEFAULT_SPACING,
			float vertical = B_USE_DEFAULT_SPACING, float weight = 1.0f)
		{
			Grid<Group<TParentBuilder>> builder = new(horizontal, vertical);
			AddChild<Grid<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, weight);
			return builder;
		}

		public Grid<Group<TParentBuilder>> AddGrid(BGridLayout gridLayout, float weight = 1.0f)
		{
			Grid<Group<TParentBuilder>> builder = new(gridLayout);
			AddChild<Grid<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, weight);
			return builder;
		}

		public Grid<Group<TParentBuilder>> AddGrid(BGridView gridView, float weight = 1.0f)
		{
			Grid<Group<TParentBuilder>> builder = new(gridView);
			AddChild<Grid<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, weight);
			return builder;
		}

		public Split<Group<TParentBuilder>> AddSplit(Orientation orientation,
			float spacing = B_USE_DEFAULT_SPACING, float weight = 1.0f)
		{
			Split<Group<TParentBuilder>> builder = new(orientation, spacing);
			AddChild<Split<Group<TParentBuilder>>>(builder);
			builder.Parent = this;

			// This is how LayoutBuilder.h standard header in Haiku adds a BSplitView in C++
			// but apparently using AddView with a BSplitView does not work in dotnet or causes a crash
			//so we add it to the layout's view directly
			// Layout?.AddView(builder.View, weight);
			View?.AddChild(builder.View);
			if (Layout != null)
			{
				int index = Layout.IndexOfView(builder.View);
				Layout?.SetItemWeight(index, weight);
			}
			return builder;
		}

		public Split<Group<TParentBuilder>> AddSplit(BSplitView splitView, float weight = 1.0f)
		{
			Split<Group<TParentBuilder>> builder = new(splitView);
			AddChild<Split<Group<TParentBuilder>>>(builder);
			builder.Parent = this;

			// This is how LayoutBuilder.h standard header in Haiku adds a BSplitView in C++
			// but apparently using AddView with a BSplitView does not work in dotnet or causes a crash
			//so we add it to the layout's view directly
			// Layout?.AddView(builder.View, weight);
			Layout?.View().AddChild(builder.View);
			if (Layout != null)
			{
				int index = Layout.IndexOfView(builder.View);
				Layout?.SetItemWeight(index, weight);
			}
			return builder;
		}


		public Cards<Group<TParentBuilder>> AddCards(float weight = 1.0f)
		{
			Cards<Group<TParentBuilder>> builder = new();
			AddChild<Cards<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View, weight);
			return builder;
		}

		public Cards<Group<TParentBuilder>> AddCards(BCardLayout cardLayout, float weight = 1.0f)
		{
			Cards<Group<TParentBuilder>> builder = new(cardLayout);
			AddChild<Cards<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View, weight);
			return builder;
		}

		public Cards<Group<TParentBuilder>> AddCards(BCardView cardView, float weight = 1.0f)
		{
			Cards<Group<TParentBuilder>> builder = new(cardView);
			AddChild<Cards<Group<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View, weight);
			return builder;
		}

		public Group<TParentBuilder> AddGlue(float weight = 1.0f)
		{
			Layout?.AddItem(BSpaceLayoutItem.CreateGlue(), weight);
			return this;
		}

		public Group<TParentBuilder> AddStrut(float size)
		{
			if (Layout?.Orientation() == B_HORIZONTAL)
				Layout?.AddItem(BSpaceLayoutItem.CreateHorizontalStrut(size));
			else
				Layout?.AddItem(BSpaceLayoutItem.CreateVerticalStrut(size));

			return this;
		}

		public Group<TParentBuilder> SetInsets(float left, float top, float right, float bottom)
		{
			Layout?.SetInsets(left, top, right, bottom);
			return this;
		}

		public Group<TParentBuilder> SetInsets(float horizontal, float vertical)
		{
			Layout?.SetInsets(horizontal, vertical);
			return this;
		}

		public Group<TParentBuilder> SetInsets(float insets)
		{
			Layout?.SetInsets(insets);
			return this;
		}

		public Group<TParentBuilder> SetExplicitMinSize(BSize size)
		{
			Layout?.SetExplicitMinSize(size);
			return this;
		}

		public Group<TParentBuilder> SetExplicitMaxSize(BSize size)
		{
			Layout?.SetExplicitMaxSize(size);
			return this;
		}

		public Group<TParentBuilder> SetExplicitPreferredSize(BSize size)
		{
			Layout?.SetExplicitPreferredSize(size);
			return this;
		}

		public Group<TParentBuilder> SetExplicitAlignment(BAlignment alignment)
		{
			Layout?.SetExplicitAlignment(alignment);
			return this;
		}

	}

	public class Grid: Grid<Grid>
	{
		public Grid(float horizontalSpacing = B_USE_DEFAULT_SPACING,
			float verticalSpacing= B_USE_DEFAULT_SPACING)
			: base(horizontalSpacing, verticalSpacing)
		{}

		public Grid(BWindow window,
			float horizontalSpacing = B_USE_DEFAULT_SPACING,
			float verticalSpacing = B_USE_DEFAULT_SPACING)
			: base(window, horizontalSpacing, verticalSpacing)
		{}

		public Grid(BView view,
			float horizontalSpacing = B_USE_DEFAULT_SPACING,
			float verticalSpacing = B_USE_DEFAULT_SPACING)
			: base(view, horizontalSpacing, verticalSpacing)
		{}

		public Grid(BGridLayout layout)
			: base(layout)
		{}

		public Grid(BGridView view)
			: base(view)
		{}
	}

	public class Grid<TParentBuilder>: BaseBuilder<TParentBuilder>
	{
		public BGridLayout? Layout { get; set; } = null;

		public BView? View { get => Layout?.View(); }

		public Grid(float horizontalSpacing = B_USE_DEFAULT_SPACING,
			float verticalSpacing= B_USE_DEFAULT_SPACING)
		{
			Layout = new BGridView(horizontalSpacing, verticalSpacing).GridLayout();
		}

		public Grid(BWindow window,
			float horizontalSpacing = B_USE_DEFAULT_SPACING,
			float verticalSpacing = B_USE_DEFAULT_SPACING)
		{
			Layout = new BGridLayout(horizontalSpacing, verticalSpacing);
			window.SetLayout(Layout);
			View?.AdoptSystemColors();
		}

		public Grid(BView view,
			float horizontalSpacing = B_USE_DEFAULT_SPACING,
			float verticalSpacing = B_USE_DEFAULT_SPACING)
		{
			Layout = new BGridLayout(horizontalSpacing, verticalSpacing);
			if (view.HasDefaultColors())
				view.AdoptSystemColors();
			view.SetLayout(Layout);
		}

		public Grid(BGridLayout layout)
		{
			Layout = layout;
		}

		public Grid(BGridView view)
		{
			Layout = view.GridLayout();
		}

		~Grid()
		{
			Console.WriteLine("Grid<> finalized.");
		}

		public Grid<TParentBuilder> GetLayout(out BGridLayout? _layout)
		{
			_layout = Layout;
			return this;
		}

		public Grid<TParentBuilder> GetView(out BView? _view)
		{
			_view = View;
			return this;
		}

		public Grid<TParentBuilder> Add(BView view,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Layout?.AddView(view, column, row, columnCount, rowCount);
			return this;
		}

		public Grid<TParentBuilder> Add(BLayoutItem item,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Layout?.AddItem(item, column, row, columnCount, rowCount);
			return this;
		}

		public Grid<TParentBuilder> AddMenuField(BMenuField menuField,
			int column, int row,
			Alignment labelAlignment = Alignment.AlignHorizontalUnset,
			int labelColumnCount = 1, int fieldColumnCount = 1, int rowCount = 1)
		{
			BLayoutItem item = menuField.CreateLabelLayoutItem();
			item.SetExplicitAlignment(new BAlignment(labelAlignment, VerticalAlignment.AlignVerticalUnset));
			Layout?.AddItem(item, column, row, labelColumnCount, rowCount);
			Layout?.AddItem(menuField.CreateMenuBarLayoutItem(),
				column + labelColumnCount, row, fieldColumnCount, rowCount);
			return this;
		}

		public Grid<TParentBuilder> AddTextControl(BTextControl textControl,
			int column, int row,
			Alignment labelAlignment = Alignment.AlignHorizontalUnset,
			int labelColumnCount = 1, int textColumnCount = 1, int rowCount = 1)
		{
			BLayoutItem item = textControl.CreateLabelLayoutItem();
			item.SetExplicitAlignment(new BAlignment(labelAlignment, VerticalAlignment.AlignVerticalUnset));
			Layout?.AddItem(item, column, row, labelColumnCount, rowCount);
			Layout?.AddItem(textControl.CreateTextViewLayoutItem(),
				column + labelColumnCount, row, textColumnCount, rowCount);
			return this;
		}

		public Group<Grid<TParentBuilder>> AddGroup(Orientation orientation,
			float spacing,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			BGroupLayout groupLayout = new BGroupLayout(orientation, spacing);
			Group<Grid<TParentBuilder>> builder = new(groupLayout);
			AddChild<Group<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, column, row, columnCount, rowCount);
			return builder;
		}

		public Group<Grid<TParentBuilder>> AddGroup(BGroupView groupView,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Group<Grid<TParentBuilder>> builder = new(groupView);
			AddChild<Group<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, column, row, columnCount, rowCount);
			return builder;
		}

		public Group<Grid<TParentBuilder>> AddGroup(BGroupLayout groupLayout,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Group<Grid<TParentBuilder>> builder = new(groupLayout);
			AddChild<Group<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, column, row, columnCount, rowCount);
			return builder;
		}

		public Grid<Grid<TParentBuilder>> AddGrid(float horizontalSpacing, float verticalSpacing,
			int column,	int row,
			int columnCount = 1, int rowCount = 1)
		{
			BGridLayout gridLayout = new BGridLayout(horizontalSpacing, verticalSpacing);
			Grid<Grid<TParentBuilder>> builder = new(gridLayout);
			AddChild<Grid<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, column, row, columnCount, rowCount);
			return builder;
		}

		public Grid<Grid<TParentBuilder>> AddGrid(BGridLayout gridLayout,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Grid<Grid<TParentBuilder>> builder = new(gridLayout);
			AddChild<Grid<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, column, row, columnCount, rowCount);
			return builder;
		}

		public Grid<Grid<TParentBuilder>> AddGrid(BGridView gridView,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Grid<Grid<TParentBuilder>> builder = new(gridView);
			AddChild<Grid<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout, column, row, columnCount, rowCount);
			return builder;
		}

		public Split<Grid<TParentBuilder>> AddSplit(Orientation orientation,
			float spacing,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Split<Grid<TParentBuilder>> builder = new(orientation, spacing);
			AddChild<Split<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.View().AddChild(builder.View);
			Layout?.AddView(builder.View, column, row, columnCount, rowCount);
			return builder;
		}

		public Split<Grid<TParentBuilder>> AddSplit(BSplitView splitView,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Split<Grid<TParentBuilder>> builder = new(splitView);
			AddChild<Split<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View, column, row, columnCount, rowCount);
			return builder;
		}


		public Cards<Grid<TParentBuilder>> AddCards(int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Cards<Grid<TParentBuilder>> builder = new();
			AddChild<Cards<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View, column, row, columnCount, rowCount);
			return builder;
		}

		public Cards<Grid<TParentBuilder>> AddCards(BCardLayout cardLayout,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Cards<Grid<TParentBuilder>> builder = new(cardLayout);
			AddChild<Cards<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View, column, row, columnCount, rowCount);
			return builder;
		}

		public Cards<Grid<TParentBuilder>> AddCards(BCardView cardView,
			int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Cards<Grid<TParentBuilder>> builder = new(cardView);
			AddChild<Cards<Grid<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View, column, row, columnCount, rowCount);
			return builder;
		}

		public Grid<TParentBuilder> AddGlue(int column, int row,
			int columnCount = 1, int rowCount = 1)
		{
			Layout?.AddItem(BSpaceLayoutItem.CreateGlue(), column, row, columnCount, rowCount);
			return this;
		}

		Grid<TParentBuilder> SetHorizontalSpacing(float spacing)
		{
			Layout?.SetHorizontalSpacing(spacing);
			return this;
		}

		Grid<TParentBuilder> SetVerticalSpacing(float spacing)
		{
			Layout?.SetVerticalSpacing(spacing);
			return this;
		}

		Grid<TParentBuilder> SetSpacing(float horizontal, float vertical)
		{
			Layout?.SetSpacing(horizontal, vertical);
			return this;
		}

		Grid<TParentBuilder> SetColumnWeight(int column, float weight)
		{
			Layout?.SetColumnWeight(column, weight);
			return this;
		}

		Grid<TParentBuilder> SetRowWeight(int row, float weight)
		{
			Layout?.SetRowWeight(row, weight);
			return this;
		}

		public Grid<TParentBuilder> SetInsets(float left, float top, float right, float bottom)
		{
			Layout?.SetInsets(left, top, right, bottom);
			return this;
		}

		public Grid<TParentBuilder> SetInsets(float horizontal, float vertical)
		{
			Layout?.SetInsets(horizontal, vertical);
			return this;
		}

		public Grid<TParentBuilder> SetInsets(float insets)
		{
			Layout?.SetInsets(insets);
			return this;
		}

		public Grid<TParentBuilder> SetExplicitMinSize(BSize size)
		{
			Layout?.SetExplicitMinSize(size);
			return this;
		}

		public Grid<TParentBuilder> SetExplicitMaxSize(BSize size)
		{
			Layout?.SetExplicitMaxSize(size);
			return this;
		}

		public Grid<TParentBuilder> SetExplicitPreferredSize(BSize size)
		{
			Layout?.SetExplicitPreferredSize(size);
			return this;
		}

		public Grid<TParentBuilder> SetExplicitAlignment(BAlignment alignment)
		{
			Layout?.SetExplicitAlignment(alignment);
			return this;
		}

	}

	public class Split: Split<Split>
	{
		public Split(Orientation orientation = Orientation.Horizontal,
			float spacing = B_USE_DEFAULT_SPACING)
			: base(orientation, spacing)
		{}

		public Split(BSplitView view)
			: base(view)
		{}
	}

	public class Split<TParentBuilder>: BaseBuilder<TParentBuilder>
	{
		public BSplitView View { get; }

		public Split(Orientation orientation = Orientation.Horizontal,
			float spacing = B_USE_DEFAULT_SPACING)
		{
			View = new BSplitView(orientation, spacing);
		}

		public Split(BSplitView view)
		{
			View = view;
		}

		~Split()
		{
			Console.WriteLine("Split<> finalized, View = {0}", View?.Name());
		}

		public Split<TParentBuilder> GetView(out BView _view)
		{
			_view = View;
			return this;
		}

		public Split<TParentBuilder> GetSplitView(out BSplitView _view)
		{
			_view = View;
			return this;
		}

		public Split<TParentBuilder> Add(BView view)
		{
			View.AddChild(view);
			return this;
		}

		public Split<TParentBuilder> Add(BView view, float weight)
		{
			View.AddChild(view, weight);
			return this;
		}

		public Split<TParentBuilder> Add(BLayoutItem item)
		{
			View.AddChild(item);
			return this;
		}

		public Split<TParentBuilder> Add(BLayoutItem item, float weight)
		{
			View.AddChild(item, weight);
			return this;
		}

		public Group<Split<TParentBuilder>> AddGroup(Orientation orientation,
			float spacing = B_USE_DEFAULT_SPACING, float weight = 1.0f)
		{
			Group<Split<TParentBuilder>> builder = new(orientation, spacing);
			AddChild<Group<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.Layout, weight);
			return builder;
		}

		public Group<Split<TParentBuilder>> AddGroup(BGroupView groupView, float weight = 1.0f)
		{
			Group<Split<TParentBuilder>> builder = new(groupView);
			AddChild<Group<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.Layout, weight);
			return builder;
		}

		public Group<Split<TParentBuilder>> AddGroup(BGroupLayout groupLayout, float weight = 1.0f)
		{
			Group<Split<TParentBuilder>> builder = new(groupLayout);
			AddChild<Group<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.Layout, weight);
			return builder;
		}

		public Grid<Split<TParentBuilder>> AddGrid(float horizontal = B_USE_DEFAULT_SPACING,
			float vertical = B_USE_DEFAULT_SPACING, float weight = 1.0f)
		{
			Grid<Split<TParentBuilder>> builder = new(horizontal, vertical);
			AddChild<Grid<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.Layout, weight);
			return builder;
		}

		public Grid<Split<TParentBuilder>> AddGrid(BGridLayout gridLayout, float weight = 1.0f)
		{
			Grid<Split<TParentBuilder>> builder = new(gridLayout);
			AddChild<Grid<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.Layout, weight);
			return builder;
		}

		public Grid<Split<TParentBuilder>> AddGrid(BGridView gridView, float weight = 1.0f)
		{
			Grid<Split<TParentBuilder>> builder = new(gridView);
			AddChild<Grid<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.Layout, weight);
			return builder;
		}

		public Split<Split<TParentBuilder>> AddSplit(Orientation orientation,
			float spacing = B_USE_DEFAULT_SPACING, float weight = 1.0f)
		{
			Split<Split<TParentBuilder>> builder = new(orientation, spacing);
			AddChild<Split<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.View, weight);
			return builder;
		}

		public Split<Split<TParentBuilder>> AddSplit(BSplitView splitView, float weight = 1.0f)
		{
			Split<Split<TParentBuilder>> builder = new(splitView);
			AddChild<Split<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.View, weight);
			return builder;
		}

		public Cards<Split<TParentBuilder>> AddCards(float weight = 1.0f)
		{
			Cards<Split<TParentBuilder>> builder = new();
			AddChild<Cards<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.View, weight);
			return builder;
		}

		public Cards<Split<TParentBuilder>> AddCards(BCardLayout cardLayout, float weight = 1.0f)
		{
			Cards<Split<TParentBuilder>> builder = new(cardLayout);
			AddChild<Cards<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.View, weight);
			return builder;
		}

		public Cards<Split<TParentBuilder>> AddCards(BCardView cardView, float weight = 1.0f)
		{
			Cards<Split<TParentBuilder>> builder = new(cardView);
			AddChild<Cards<Split<TParentBuilder>>>(builder);
			builder.Parent = this;
			View.AddChild(builder.View, weight);
			return builder;
		}

		public Split<TParentBuilder> SetCollapsible(bool collapsible)
		{
			View.SetCollapsible(collapsible);
			return this;
		}

		public Split<TParentBuilder> SetCollapsible(int index, bool collapsible)
		{
			View.SetCollapsible(index, collapsible);
			return this;
		}

		public Split<TParentBuilder> SetCollapsible(int first, int last, bool collapsible)
		{
			View.SetCollapsible(first, last, collapsible);
			return this;
		}

		public Split<TParentBuilder> SetInsets(float left, float top, float right, float bottom)
		{
			View.SetInsets(left, top, right, bottom);
			return this;
		}

		public Split<TParentBuilder> SetInsets(float horizontal, float vertical)
		{
			View.SetInsets(horizontal, vertical);
			return this;
		}

		public Split<TParentBuilder> SetInsets(float insets)
		{
			View.SetInsets(insets);
			return this;
		}

		public Split<TParentBuilder> SetExplicitMinSize(BSize size)
		{
			View.SetExplicitMinSize(size);
			return this;
		}

		public Split<TParentBuilder> SetExplicitMaxSize(BSize size)
		{
			View.SetExplicitMaxSize(size);
			return this;
		}

		public Split<TParentBuilder> SetExplicitPreferredSize(BSize size)
		{
			View.SetExplicitPreferredSize(size);
			return this;
		}

		public Split<TParentBuilder> SetExplicitAlignment(BAlignment alignment)
		{
			View.SetExplicitAlignment(alignment);
			return this;
		}

	}


	public class Cards: Cards<Cards>
	{
		public Cards()
			: base()
		{}

		public Cards(BWindow window)
			: base(window)
		{}

		public Cards(BView view)
			: base(view)
		{}

		public Cards(BCardLayout layout)
			: base(layout)
		{}

		public Cards(BCardView _view)
			: base(_view)
		{}
	}

	public class Cards<TParentBuilder>: BaseBuilder<TParentBuilder>
	{
		public BCardLayout? Layout { get; set; }

		public BView? View { get => Layout?.View(); }

		private BCardView? fCardView;

		public Cards()
		{
			fCardView = new();
			Layout = fCardView.CardLayout();
		}

		public Cards(BWindow window)
		{
			Layout = new BCardLayout();
			window.SetLayout(Layout);
			View?.SetViewColor(Utils.ui_color(ColorWhich.PanelBackgroundColor));
		}

		public Cards(BView view)
		{
			Layout = new BCardLayout();
			view.SetLayout(Layout);
			view.SetViewColor(Utils.ui_color(ColorWhich.PanelBackgroundColor));
			view.AdoptSystemColors();
		}

		public Cards(BCardLayout layout)
		{
			Layout = layout;
		}

		public Cards(BCardView view)
		{
			Layout = view.CardLayout();
		}

		~Cards()
		{
			Console.WriteLine("Cards<> finalized, View = {0}", View?.Name());
		}

		public Cards<TParentBuilder> GetLayout(out BCardLayout? layout)
		{
			layout = Layout;
			return this;
		}

		public Cards<TParentBuilder> GetView(out BView? _view)
		{
			_view = View;
			return this;
		}

		public Cards<TParentBuilder> Add(BView view)
		{
			Layout?.AddView(view);
			return this;
		}

		public Cards<TParentBuilder> Add(BLayoutItem item)
		{
			Layout?.AddItem(item);
			return this;
		}

		public Group<Cards<TParentBuilder>> AddGroup(Orientation orientation,
			float spacing = B_USE_DEFAULT_SPACING)
		{
			Group<Cards<TParentBuilder>> builder = new(orientation, spacing);
			AddChild<Group<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout);
			return builder;
		}

		public Group<Cards<TParentBuilder>> AddGroup(BGroupView groupView)
		{
			Group<Cards<TParentBuilder>> builder = new(groupView);
			AddChild<Group<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout);
			return builder;
		}

		public Group<Cards<TParentBuilder>> AddGroup(BGroupLayout groupLayout)
		{
			Group<Cards<TParentBuilder>> builder = new(groupLayout);
			AddChild<Group<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout);
			return builder;
		}

		public Grid<Cards<TParentBuilder>> AddGrid(float horizontal = B_USE_DEFAULT_SPACING,
			float vertical = B_USE_DEFAULT_SPACING)
		{
			Grid<Cards<TParentBuilder>> builder = new(horizontal, vertical);
			AddChild<Grid<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout);
			return builder;
		}

		public Grid<Cards<TParentBuilder>> AddGrid(BGridLayout gridLayout)
		{
			Grid<Cards<TParentBuilder>> builder = new(gridLayout);
			AddChild<Grid<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout);
			return builder;
		}

		public Grid<Cards<TParentBuilder>> AddGrid(BGridView gridView)
		{
			Grid<Cards<TParentBuilder>> builder = new(gridView);
			AddChild<Grid<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddItem(builder.Layout);
			return builder;
		}

		public Split<Cards<TParentBuilder>> AddSplit(Orientation orientation,
			float spacing = B_USE_DEFAULT_SPACING)
		{
			Split<Cards<TParentBuilder>> builder = new(orientation, spacing);
			AddChild<Split<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;

			// This is how LayoutBuilder.h standard header in Haiku adds a BSplitView in C++
			// but apparently using AddView with a BSplitView does not work in dotnet or causes a crash
			//so we add it to the layout's view directly
			// Layout?.AddView(builder.View);
			View?.AddChild(builder.View);
			return builder;
		}

		public Split<Cards<TParentBuilder>> AddSplit(BSplitView splitView)
		{
			Split<Cards<TParentBuilder>> builder = new(splitView);
			AddChild<Split<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;

			// This is how LayoutBuilder.h standard header in Haiku adds a BSplitView in C++
			// but apparently using AddView with a BSplitView does not work in dotnet or causes a crash
			//so we add it to the layout's view directly
			// Layout?.AddView(builder.View);
			Layout?.View().AddChild(builder.View);
			return builder;
		}


		public Cards<Cards<TParentBuilder>> AddCards()
		{
			Cards<Cards<TParentBuilder>> builder = new();
			AddChild<Cards<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View);
			return builder;
		}

		public Cards<Cards<TParentBuilder>> AddCards(BCardLayout cardLayout)
		{
			Cards<Cards<TParentBuilder>> builder = new(cardLayout);
			AddChild<Cards<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View);
			return builder;
		}

		public Cards<Cards<TParentBuilder>> AddCards(BCardView cardView)
		{
			Cards<Cards<TParentBuilder>> builder = new(cardView);
			AddChild<Cards<Cards<TParentBuilder>>>(builder);
			builder.Parent = this;
			Layout?.AddView(builder.View);
			return builder;
		}

		public Cards<TParentBuilder> SetExplicitMinSize(BSize size)
		{
			Layout?.SetExplicitMinSize(size);
			return this;
		}

		public Cards<TParentBuilder> SetExplicitMaxSize(BSize size)
		{
			Layout?.SetExplicitMaxSize(size);
			return this;
		}

		public Cards<TParentBuilder> SetExplicitPreferredSize(BSize size)
		{
			Layout?.SetExplicitPreferredSize(size);
			return this;
		}

		public Cards<TParentBuilder> SetExplicitAlignment(BAlignment alignment)
		{
			Layout?.SetExplicitAlignment(alignment);
			return this;
		}

		public Cards<TParentBuilder> SetVisibleItem(int index)
		{
			Layout?.SetVisibleItem(index);
			return this;
		}
	}

	public class Menu: Menu<Menu>
	{
		public Menu(BMenu menu)
			: base(menu)
		{
		}
	}

	public class Menu<TParentBuilder>: BaseBuilder<TParentBuilder>
	{

		private BMenu fMenu;
		private List<object> fItems = new();

		public Menu(BMenu menu)
		{
			fMenu = menu;
		}

		public Menu<TParentBuilder> GetMenu(out BMenu menu)
		{
			menu = fMenu;
			return this;
		}

		public MenuItem<TParentBuilder> AddItem(BMenuItem item)
		{
			fItems.Add(item);
			fMenu.AddItem(item);
			MenuItem<TParentBuilder> builder = new(Parent, fMenu, item);
			AddChild<MenuItem<TParentBuilder>>(builder);
			return builder;
		}

		public MenuItem<TParentBuilder> AddItem(BMenu menu)
		{
			fItems.Add(menu);
			if (!fMenu.AddItem(menu))
				throw new ArgumentException();


			MenuItem<TParentBuilder> builder = new(Parent, fMenu, fMenu.ItemAt(fMenu.CountItems() - 1));
			AddChild<MenuItem<TParentBuilder>>(builder);
			return builder;
		}

		public MenuItem<TParentBuilder> AddItem(string label, BMessage message,
			char shortcut = '\0', uint modifiers = 0)
		{
			BMenuItem? item = new(label, message, shortcut, modifiers);
			fItems.Add(item);
			if (!fMenu.AddItem(item))
			{
				item = null;
			}

			MenuItem<TParentBuilder> builder = new(Parent, fMenu, item);
			AddChild<MenuItem<TParentBuilder>>(builder);
			return builder;
		}

		public MenuItem<TParentBuilder> AddItem(string label, uint messageWhat,
			char shortcut = '\0', uint modifiers = 0)
		{
			BMessage message = new BMessage(messageWhat);
			BMenuItem item;
			try
			{
				item = new BMenuItem(label, message, shortcut, modifiers);
			}
			catch (Exception)
			{
				throw;
			}

			if (!fMenu.AddItem(item))
				item = null;

			MenuItem<TParentBuilder> builder = new(Parent, fMenu, item);
			AddChild<MenuItem<TParentBuilder>>(builder);
			return builder;
		}


		public Menu<Menu<TParentBuilder>> AddMenu(BMenu menu)
		{
			if (!fMenu.AddItem(menu))
				throw new ArgumentException();

			Menu<Menu<TParentBuilder>> builder = new(menu);
			AddChild<Menu<Menu<TParentBuilder>>>(builder);
			builder.Parent = this;
			return builder;
		}

		public Menu<Menu<TParentBuilder>> AddMenu(string title,
			MenuLayout layout = MenuLayout.ItemsInColumn)
		{
			BMenu menu = new BMenu(title, layout);
			if (!fMenu.AddItem(menu))
				throw new ArgumentException();
			fItems.Add(menu);

			Menu<Menu<TParentBuilder>> builder = new(menu);
			AddChild<Menu<Menu<TParentBuilder>>>(builder);
			builder.Parent = this;
			return builder;
		}

		public Menu<TParentBuilder> AddSeparator()
		{
			fMenu.AddSeparatorItem();
			return this;
		}
	}

	public class MenuItem: MenuItem<MenuItem>
	{
		public MenuItem(BMenu menu, BMenuItem item)
			: base(default(MenuItem), menu, item)
		{}
	}

	public class MenuItem<TParentBuilder>: Menu<TParentBuilder>
	{
		private BMenuItem fMenuItem;

		public MenuItem(TParentBuilder? parentBuilder, BMenu menu, BMenuItem item)
			: base(menu)
		{
			fMenuItem = item;
			Parent = parentBuilder;
		}

		public MenuItem<TParentBuilder> GetItem(BMenuItem item)
		{
			item = fMenuItem;
			return this;
		}

		public MenuItem<TParentBuilder> SetEnabled(bool enabled)
		{
			fMenuItem.SetEnabled(enabled);
			return this;
		}
	}

}