using Sandbox;
using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace SideScroller.Hud
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public class SideScrollerHudEntity : HudEntity<RootPanel>
	{
		public SideScrollerHudEntity()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "UI/SideScrollerHud.html" );
			}
		}
	}
}
