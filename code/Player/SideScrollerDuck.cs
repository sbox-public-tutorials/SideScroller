using Sandbox;
using Input = Sandbox.Input;

namespace SideScroller.Player
{
	[Library]
	public class SideScrollerDuck : NetworkComponent
	{
		public BasePlayerController Controller;

		public bool IsActive; // replicate

		public SideScrollerDuck( BasePlayerController controller )
		{
			Controller = controller;
		}

		public virtual void PreTick()
		{
			bool wants = Input.Down( InputButton.Duck ) || Input.Down( InputButton.Back );

			if ( wants != IsActive )
			{
				if ( wants ) TryDuck();
				else TryUnDuck();
			}

			if ( IsActive )
			{
				Controller.SetTag( "ducked" );
				Controller.EyePosLocal *= 0.5f;
			}
		}

		protected virtual void TryDuck()
		{
			IsActive = true;
		}

		protected virtual void TryUnDuck()
		{
			var pm = Controller.TraceBBox( Controller.Position, Controller.Position, originalMins, originalMaxs );
			if ( pm.StartedSolid ) return;

			IsActive = false;
		}

		// Uck, saving off the bbox kind of sucks
		// and we should probably be changing the bbox size in PreTick
		private Vector3 originalMins;

		private Vector3 originalMaxs;

		public virtual void UpdateBBox( ref Vector3 mins, ref Vector3 maxs )
		{
			originalMins = mins;
			originalMaxs = maxs;

			if ( IsActive )
				maxs = maxs.WithZ( 36 );
		}

		//
		// Coudl we do this in a generic callback too?
		//
		public virtual float GetWishSpeed()
		{
			if ( !IsActive ) return -1;
			return 64.0f;
		}
	}
}
