using Haiku.App;
using Haiku.Interface;
using Haiku.Interface.LayoutBuilder;
using static Haiku.App.Symbols;
using static Haiku.Interface.Symbols;

namespace HaikuApp;

public class MainWindow : BWindow
{

	private BMenuBar? fMenuBar;
	private Menu? fMenuBuilder;

	private BStringView? fStringView;
	private BButton? fButton;
	private BScrollView? fOutlineScroll;
	private BOutlineListView? fOutline;
	private List<BStringItem>? fStringItems;

	private BCardLayout? fCardLayout;
	private Group? fMainGroupBuilder;

	private Dictionary<uint, BMessage>? fMessages = new();

	private const uint kMsgButtonClick = 1;
	private const uint kMsgShowCard1 = 2;
	private const uint kMsgShowCard2 = 3;

	public MainWindow()
		: base(new BRect(100, 100, 500, 500), "Main Window", WindowType.TitledWindow, B_ASYNCHRONOUS_CONTROLS | B_QUIT_ON_WINDOW_CLOSE)
	{
		// MoveTo(100, 100);
		// ResizeTo(400, 400);

		// messages
		fMessages[kMsgShowCard1] = new BMessage(kMsgShowCard1);
		fMessages[kMsgShowCard2] = new BMessage(kMsgShowCard2);
		fMessages[kMsgButtonClick] = new BMessage(kMsgButtonClick);

		// Menu
		fMenuBar = new BMenuBar("menu");
		fMenuBuilder = new Menu(fMenuBar);
		fMenuBuilder
			.AddMenu("Menu1")
				.AddItem("Item1", fMessages[kMsgShowCard1])
				.AddItem("Item2", fMessages[kMsgShowCard2])
				.End()
			.AddMenu("Menu2")
				.AddItem("Item3", new BMessage())
				.AddItem("Item4", new BMessage())
				.End()
			.End();


		fOutline = new BOutlineListView("listView");
		fStringItems = new() {
			new("item1", 0, true),
			new("item2", 1, true),
			new("item3", 1, true),
			new("item4", 0, true),
			new("item5", 0, true),
			new("item6", 1, true),
			new("item7", 2, true)
		};
		fStringItems.ForEach((item) => fOutline.AddItem(item));
		// fOutline.SetExplicitMinSize(new BSize(50, B_SIZE_UNSET));
		fOutline.SetExplicitMaxSize(new BSize(B_SIZE_UNLIMITED, B_SIZE_UNSET));
		fOutlineScroll = new BScrollView("Outline", fOutline, B_WILL_DRAW,
			false, true, BorderStyle.NoBorder);

		fStringView = new BStringView("stringView", "BStringView");
		fStringView.SetExplicitMaxSize(new BSize(B_SIZE_UNLIMITED, B_SIZE_UNSET));
		fStringView.SetExplicitAlignment(new BAlignment(Alignment.AlignHorizontalCenter,
			VerticalAlignment.AlignVerticalUnset));
		fButton = new BButton("BButton", fMessages[kMsgButtonClick]);
		fButton.SetExplicitMaxSize(new BSize(B_SIZE_UNLIMITED, B_SIZE_UNSET));

		fMainGroupBuilder = new Group(this, Orientation.Vertical, B_USE_SMALL_SPACING);
		var a = fMainGroupBuilder
			.Add(fMenuBar)
			.AddSplit(Orientation.Horizontal)
				.AddGroup(Orientation.Vertical)
					.Add(fOutlineScroll)
					.End()
				.AddGroup(Orientation.Vertical)
					.AddCards()
						.GetLayout(out fCardLayout)
						.AddGroup(Orientation.Vertical)
							.Add(fStringView)
							.End()
						.AddGroup(Orientation.Vertical)
							.Add(fButton)
							.End()
						.SetVisibleItem(0)
						.End()
					.AddGlue()
					.End()
				.End()
			.SetInsets(B_USE_SMALL_INSETS)
			.End();

	}

	public override void MessageReceived(BMessage message)
	{
		switch (message.What) {
			case kMsgButtonClick:
				fStringView?.SetText("Button pressed!");
				break;
			case kMsgShowCard1:
				fCardLayout?.SetVisibleItem(0);
				break;
			case kMsgShowCard2:
				fCardLayout?.SetVisibleItem(1);
				break;
			default:
				base.MessageReceived(message);
				break;
		}
	}

	public override bool QuitRequested()
	{
		be_app.PostMessage(B_QUIT_REQUESTED);
		return true;
	}

}
