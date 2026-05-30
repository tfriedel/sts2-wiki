using System;
using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;

public class ScreenStateTracker
{
	private NetScreenType _capstoneScreen;

	private NetScreenType _overlayScreen;

	private bool _mapScreenVisible;

	private bool _isInSharedRelicPicking;

	private readonly Callable _onRewardsScreenCompleted;

	public ScreenStateTracker(NMapScreen mapScreen, NCapstoneContainer capstoneContainer, NOverlayStack overlayStack)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		_onRewardsScreenCompleted = Callable.From((Action)SyncLocalScreen);
		((GodotObject)capstoneContainer).Connect(NCapstoneContainer.SignalName.Changed, Callable.From((Action)OnCapstoneScreenChanged), 0u);
		((GodotObject)overlayStack).Connect(NOverlayStack.SignalName.Changed, Callable.From((Action)OnOverlayStackChanged), 0u);
		((GodotObject)mapScreen).Connect(SignalName.VisibilityChanged, Callable.From((Action)OnMapScreenVisibilityChanged), 0u);
	}

	private void OnCapstoneScreenChanged()
	{
		if (!RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			_capstoneScreen = NCapstoneContainer.Instance.CurrentCapstoneScreen?.ScreenType ?? NetScreenType.None;
			SyncLocalScreen();
		}
	}

	private void OnOverlayStackChanged()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			IOverlayScreen overlayScreen = NOverlayStack.Instance.Peek();
			if (overlayScreen is NRewardsScreen nRewardsScreen && !((GodotObject)nRewardsScreen).IsConnected(NRewardsScreen.SignalName.Completed, _onRewardsScreenCompleted))
			{
				((GodotObject)nRewardsScreen).Connect(NRewardsScreen.SignalName.Completed, _onRewardsScreenCompleted, 0u);
			}
			_overlayScreen = overlayScreen?.ScreenType ?? NetScreenType.None;
			SyncLocalScreen();
		}
	}

	private void SyncLocalScreen()
	{
		RunManager.Instance.InputSynchronizer.SyncLocalScreen(GetCurrentScreen());
	}

	private void OnMapScreenVisibilityChanged()
	{
		_mapScreenVisible = ((CanvasItem)NMapScreen.Instance).Visible;
		RunManager.Instance.InputSynchronizer.SyncLocalScreen(GetCurrentScreen());
	}

	public void SetIsInSharedRelicPickingScreen(bool isInSharedRelicPicking)
	{
		_isInSharedRelicPicking = isInSharedRelicPicking;
		RunManager.Instance.InputSynchronizer.SyncLocalScreen(GetCurrentScreen());
	}

	private NetScreenType GetCurrentScreen()
	{
		if (_capstoneScreen != 0)
		{
			return _capstoneScreen;
		}
		if (_mapScreenVisible)
		{
			return NetScreenType.Map;
		}
		if (_overlayScreen == NetScreenType.Rewards)
		{
			if (NOverlayStack.Instance.Peek() is NRewardsScreen { IsComplete: false })
			{
				return _overlayScreen;
			}
		}
		else if (_overlayScreen != 0)
		{
			return _overlayScreen;
		}
		if (_isInSharedRelicPicking)
		{
			return NetScreenType.SharedRelicPicking;
		}
		return NetScreenType.Room;
	}
}
