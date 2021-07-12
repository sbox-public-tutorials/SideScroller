using Sandbox;
using System;

namespace SideScroller.Camera
{
	//Uses code from kurozael#1337/Conna https://discord.com/channels/833983068468936704/833983449857654795/837660827670675507
	public class SideScrollerCamera : Sandbox.Camera
	{
		[ConVar.Replicated( "sidescroller_debug" )]
		public static bool Debug { get; set; } = false;

		public float Zoom = 250f;
		public float ZoomSensitivity = 10f;
		public const float MinZoom = 200f;
		public const float MaxZoom = 400f;

		public override void Activated()
		{
			//TODO: Probably not hardcode this?
			FieldOfView = 90;

			base.Activated();
		}

		public override void Update()
		{
			if ( Local.Pawn is not AnimEntity PlayerPawn )
				return;

			//Mouse handling
			Vector3 mouseOffset = Screen.GetDirection( Mouse.Position ).WithY( 0.0f );
			Vector3 mouseToLocal = (mouseOffset * 100.0f);

			Vector3 mouseToWorld = PlayerPawn.Position + mouseToLocal;

			if ( Debug )
			{
				DebugOverlay.Line( PlayerPawn.Position, mouseToWorld, Color.Red, 2f, false ); //Server
			}

			Rotation LookRot = Rotation.LookAt( (mouseToWorld).WithY( 0 ).Normal ); ;
			//PlayerPawn.Rotation = LookRot;
			////PlayerPawn.EyeRot = PlayerPawn.Rotation;

			if ( PlayerPawn.LifeState == LifeState.Alive )
			{
				using ( Prediction.Off() )
				{
					GiveWantedRotation( mouseToWorld );
				}
			}

			// align with the player, but offset us -3000 on the y axis (or whatever you want)
			Pos = PlayerPawn.EyePos + new Vector3( 0f, Zoom, 32f );

			var targetDirection = (PlayerPawn.EyePos - Pos).Normal;

			var lookAngles = new Angles(
				MathF.Asin( targetDirection.z ).RadianToDegree() * -1.0f,
				MathF.Atan2( targetDirection.y, targetDirection.x ).RadianToDegree(),
				0.0f
			);

			// look towards the player
			Rot = Rotation.From( lookAngles );
			// Setup Zoom
			Zoom -= Input.MouseWheel * ZoomSensitivity;

			if ( Debug )
			{
				DebugOverlay.ScreenText( $"Zoom: {Zoom}" );
			}

			if ( Zoom < MinZoom )
			{
				Zoom = MinZoom;
			}

			if ( Zoom > MaxZoom )
			{
				Zoom = MaxZoom;
			}

			Viewer = null;
		}

		//From xrayhunter
		[ServerCmd]
		public static void GiveWantedRotation( Vector3 lookAtPos )
		{
			Entity pawn = ConsoleSystem.Caller.Pawn;
			pawn.Rotation = Rotation.LookAt( (lookAtPos - pawn.Position).WithZ( 0 ).Normal );
			pawn.EyeRot = pawn.Rotation;
			//Log.Info( $"Gonna set rotation {pawn} {lookAtPos}" );
			// DebugOverlay.Line(pawn.Position, lookAtPos, Color.Red, 1.0f, false);
		}
	}
}
