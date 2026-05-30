using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.TreasureRelicPicking;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;

[ScriptPath("res://src/Core/Nodes/Screens/TreasureRoomRelic/NTreasureRoomRelicCollection.cs")]
public class NTreasureRoomRelicCollection : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName InitializeRelics = StringName.op_Implicit("InitializeRelics");

		public static readonly StringName SpawnEmptyChestVfx = StringName.op_Implicit("SpawnEmptyChestVfx");

		public static readonly StringName SetSelectionEnabled = StringName.op_Implicit("SetSelectionEnabled");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName AnimOut = StringName.op_Implicit("AnimOut");

		public static readonly StringName PickRelic = StringName.op_Implicit("PickRelic");

		public static readonly StringName RefreshVotes = StringName.op_Implicit("RefreshVotes");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName SingleplayerRelicHolder = StringName.op_Implicit("SingleplayerRelicHolder");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _fightBackstop = StringName.op_Implicit("_fightBackstop");

		public static readonly StringName _fightLabel = StringName.op_Implicit("_fightLabel");

		public static readonly StringName _emptyVfxTween = StringName.op_Implicit("_emptyVfxTween");

		public static readonly StringName _hands = StringName.op_Implicit("_hands");

		public static readonly StringName _openedTicks = StringName.op_Implicit("_openedTicks");

		public static readonly StringName _isEmptyChest = StringName.op_Implicit("_isEmptyChest");
	}

	public class SignalName : SignalName
	{
	}

	private const ulong _noSelectionTimeMsec = 200uL;

	private Control _fightBackstop;

	private MegaLabel _fightLabel;

	private Tween? _emptyVfxTween;

	private NHandImageCollection _hands;

	private readonly List<NTreasureRoomRelicHolder> _multiplayerHolders = new List<NTreasureRoomRelicHolder>();

	private List<NTreasureRoomRelicHolder> _holdersInUse = new List<NTreasureRoomRelicHolder>();

	private readonly TaskCompletionSource _relicPickingBeganTaskCompletionSource = new TaskCompletionSource();

	private readonly TaskCompletionSource _relicPickingCompleteTaskCompletionSource = new TaskCompletionSource();

	private ulong _openedTicks;

	private IRunState _runState;

	private bool _isEmptyChest;

	public NTreasureRoomRelicHolder SingleplayerRelicHolder { get; private set; }

	public Control? DefaultFocusedControl
	{
		get
		{
			if (_holdersInUse.Count <= 0)
			{
				return null;
			}
			return (Control?)(object)_holdersInUse[_runState.GetPlayerSlotIndex(LocalContext.GetMe(_runState.Players))];
		}
	}

	public override void _Ready()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		_fightBackstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%FightBackstop"));
		_hands = ((Node)this).GetNode<NHandImageCollection>(NodePath.op_Implicit("%HandsContainer"));
		Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Container"));
		SingleplayerRelicHolder = ((Node)node).GetNode<NTreasureRoomRelicHolder>(NodePath.op_Implicit("%SingleplayerRelicHolder"));
		foreach (NTreasureRoomRelicHolder item in ((IEnumerable)((Node)node).GetChildren(false)).OfType<NTreasureRoomRelicHolder>())
		{
			if (item != SingleplayerRelicHolder)
			{
				_multiplayerHolders.Add(item);
			}
		}
		Control fightBackstop = _fightBackstop;
		Color modulate = ((CanvasItem)_fightBackstop).Modulate;
		modulate.A = 0f;
		((CanvasItem)fightBackstop).Modulate = modulate;
		((CanvasItem)_fightBackstop).Visible = false;
		_fightLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%FightLabel"));
		((Label)_fightLabel).Text = new LocString("gameplay_ui", "TREASURE_FIGHT_TEXT").GetFormattedText();
		RunManager.Instance.TreasureRoomRelicSynchronizer.VotesChanged += RefreshVotes;
		RunManager.Instance.TreasureRoomRelicSynchronizer.RelicsAwarded += OnRelicsAwarded;
	}

	public override void _ExitTree()
	{
		RunManager.Instance.TreasureRoomRelicSynchronizer.VotesChanged -= RefreshVotes;
		RunManager.Instance.TreasureRoomRelicSynchronizer.RelicsAwarded -= OnRelicsAwarded;
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
		_hands.Initialize(runState);
	}

	public void InitializeRelics()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<RelicModel> currentRelics = RunManager.Instance.TreasureRoomRelicSynchronizer.CurrentRelics;
		if (currentRelics == null || currentRelics.Count == 0)
		{
			_isEmptyChest = true;
			((CanvasItem)SingleplayerRelicHolder).Visible = false;
			foreach (NTreasureRoomRelicHolder multiplayerHolder in _multiplayerHolders)
			{
				((CanvasItem)multiplayerHolder).Visible = false;
			}
			SpawnEmptyChestVfx();
			return;
		}
		if (currentRelics.Count == 1)
		{
			SingleplayerRelicHolder.Initialize(currentRelics[0], _runState);
			((CanvasItem)SingleplayerRelicHolder).Visible = true;
			SingleplayerRelicHolder.Index = 0;
			((GodotObject)SingleplayerRelicHolder).Connect(NClickableControl.SignalName.Released, Callable.From<NTreasureRoomRelicHolder>((Action<NTreasureRoomRelicHolder>)delegate
			{
				PickRelic(SingleplayerRelicHolder);
			}), 0u);
			int num = 1;
			List<NTreasureRoomRelicHolder> list = new List<NTreasureRoomRelicHolder>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<NTreasureRoomRelicHolder> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = SingleplayerRelicHolder;
			_holdersInUse = list;
			{
				foreach (NTreasureRoomRelicHolder multiplayerHolder2 in _multiplayerHolders)
				{
					((CanvasItem)multiplayerHolder2).Visible = false;
				}
				return;
			}
		}
		((CanvasItem)SingleplayerRelicHolder).Visible = false;
		for (int i = 0; i < _multiplayerHolders.Count; i++)
		{
			NTreasureRoomRelicHolder holder = _multiplayerHolders[i];
			if (i < currentRelics.Count)
			{
				((CanvasItem)holder).Visible = true;
				holder.Relic.Model = currentRelics[i];
				holder.Initialize(currentRelics[i], _runState);
			}
			else
			{
				((CanvasItem)holder).Visible = false;
			}
			holder.Index = i;
			((GodotObject)holder).Connect(NClickableControl.SignalName.Released, Callable.From<NTreasureRoomRelicHolder>((Action<NTreasureRoomRelicHolder>)delegate
			{
				PickRelic(holder);
			}), 0u);
			_holdersInUse.Add(holder);
			holder.VoteContainer.RefreshPlayerVotes();
		}
		for (int j = 0; j < _holdersInUse.Count; j++)
		{
			((Control)_holdersInUse[j]).SetFocusMode((FocusModeEnum)2);
			((Control)_holdersInUse[j]).FocusNeighborTop = ((Node)_holdersInUse[j]).GetPath();
			((Control)_holdersInUse[j]).FocusNeighborBottom = ((Node)_holdersInUse[j]).GetPath();
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = _holdersInUse[j];
			NodePath path;
			if (j <= 0)
			{
				List<NTreasureRoomRelicHolder> holdersInUse = _holdersInUse;
				path = ((Node)holdersInUse[holdersInUse.Count - 1]).GetPath();
			}
			else
			{
				path = ((Node)_holdersInUse[j - 1]).GetPath();
			}
			((Control)nTreasureRoomRelicHolder).FocusNeighborLeft = path;
			((Control)_holdersInUse[j]).FocusNeighborRight = ((j < _holdersInUse.Count - 1) ? ((Node)_holdersInUse[j + 1]).GetPath() : ((Node)_holdersInUse[0]).GetPath());
		}
		if (currentRelics.Count == 2)
		{
			((Control)_multiplayerHolders[1]).Position = ((Control)_multiplayerHolders[3]).Position;
		}
	}

	private void SpawnEmptyChestVfx()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		MegaLabel node = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%EmptyLabel"));
		((Label)node).Text = new LocString("gameplay_ui", "TREASURE_EMPTY").GetFormattedText();
		((CanvasItem)node).Visible = true;
		_emptyVfxTween = ((Node)this).CreateTween().SetParallel(true);
		_emptyVfxTween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 0.25);
		_emptyVfxTween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)node).Position.Y), 1.0).From(Variant.op_Implicit(((Control)node).Position.Y + 64f)).SetEase((EaseType)1)
			.SetTrans((TransitionType)11);
		_emptyVfxTween.Chain().TweenInterval(1.0);
		_emptyVfxTween.Chain();
		_emptyVfxTween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(0f), 1.0);
		GpuParticles2D node2 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("%SmokePuffVfx"));
		node2.Emitting = true;
	}

	public void SetSelectionEnabled(bool isEnabled)
	{
		if (isEnabled)
		{
			SingleplayerRelicHolder.Enable();
			{
				foreach (NTreasureRoomRelicHolder multiplayerHolder in _multiplayerHolders)
				{
					multiplayerHolder.Enable();
				}
				return;
			}
		}
		SingleplayerRelicHolder.Disable();
		foreach (NTreasureRoomRelicHolder multiplayerHolder2 in _multiplayerHolders)
		{
			multiplayerHolder2.Disable();
		}
	}

	public Task RelicPickingBegan()
	{
		return _relicPickingBeganTaskCompletionSource.Task;
	}

	public Task RelicPickingFinished()
	{
		return _relicPickingCompleteTaskCompletionSource.Task;
	}

	public void AnimIn(Node chestVisual)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Visible = true;
		((CanvasItem)this).Modulate = Colors.Transparent;
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.4);
		val.TweenProperty((GodotObject)(object)chestVisual, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.halfTransparentWhite), 0.4);
		if (_isEmptyChest)
		{
			LocalContext.GetMe(_runState)?.Relics.OfType<SilverCrucible>().FirstOrDefault()?.Flash();
			val.TweenCallback(Callable.From((Action)delegate
			{
				RunManager.Instance.TreasureRoomRelicSynchronizer.CompleteWithNoRelics();
			})).SetDelay(1.0);
			return;
		}
		foreach (NTreasureRoomRelicHolder holder in _holdersInUse)
		{
			((Control)holder).MouseFilter = (MouseFilterEnum)2;
			float num = ((_holdersInUse.Count == 1) ? 150f : 50f);
			float num2 = 0.2f + 0.2f * Rng.Chaotic.NextFloat();
			((CanvasItem)holder).Modulate = Colors.Black;
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = holder;
			Vector2 position = ((Control)holder).Position;
			position.Y = ((Control)holder).Position.Y + num;
			((Control)nTreasureRoomRelicHolder).Position = position;
			Tween val2 = ((Node)this).CreateTween().SetParallel(true);
			val2.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.2).SetDelay((double)num2);
			val2.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)holder).Position.Y - num), 0.6).SetDelay((double)num2).SetEase((EaseType)1)
				.SetTrans((TransitionType)10);
			val2.TweenCallback(Callable.From<MouseFilterEnum>((Func<MouseFilterEnum>)delegate
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				NTreasureRoomRelicHolder nTreasureRoomRelicHolder2 = holder;
				long num3 = 0L;
				MouseFilterEnum result = (MouseFilterEnum)num3;
				((Control)nTreasureRoomRelicHolder2).MouseFilter = (MouseFilterEnum)num3;
				return result;
			})).SetDelay((double)num2 + 0.6);
		}
		NRun.Instance.ScreenStateTracker.SetIsInSharedRelicPickingScreen(isInSharedRelicPicking: true);
		_hands.AnimateHandsIn();
	}

	public void AnimOut(Node chestVisual)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = Colors.White;
		Tween val = ((Node)this).CreateTween().Parallel();
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.3);
		val.TweenProperty((GodotObject)(object)chestVisual, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.3);
		val.TweenCallback(Callable.From<bool>((Func<bool>)(() => ((CanvasItem)this).Visible = false)));
		NRun.Instance.ScreenStateTracker.SetIsInSharedRelicPickingScreen(isInSharedRelicPicking: false);
	}

	private void PickRelic(NTreasureRoomRelicHolder holder)
	{
		if (Time.GetTicksMsec() - _openedTicks > 200)
		{
			RunManager.Instance.TreasureRoomRelicSynchronizer.PickRelicLocally(holder.Index);
		}
	}

	private void OnRelicsAwarded(List<RelicPickingResult> results)
	{
		TaskHelper.RunSafely(AnimateRelicAwards(results));
	}

	private async Task AnimateRelicAwards(List<RelicPickingResult> results)
	{
		foreach (NTreasureRoomRelicHolder item in _holdersInUse)
		{
			((Control)item).SetFocusMode((FocusModeEnum)0);
		}
		_relicPickingBeganTaskCompletionSource.SetResult();
		foreach (Player player in _runState.Players)
		{
			TreasureRoomRelicSynchronizer.PlayerVote playerVote = RunManager.Instance.TreasureRoomRelicSynchronizer.GetPlayerVote(player);
			if (playerVote.voteReceived && !playerVote.index.HasValue)
			{
				_hands.GetHand(player.NetId)?.SetSkipped();
			}
		}
		_hands.BeforeRelicsAwarded();
		List<Task> tasksToWait = new List<Task>();
		RelicPickingResultType? relicPickingResultType = null;
		results.Sort((RelicPickingResult r1, RelicPickingResult r2) => r1.type.CompareTo(r2.type));
		foreach (RelicPickingResult result2 in results)
		{
			NTreasureRoomRelicHolder holder = _holdersInUse.First((NTreasureRoomRelicHolder h) => h.Relic.Model == result2.relic);
			holder.AnimateAwayVotes();
			if (relicPickingResultType.HasValue && result2.type != relicPickingResultType)
			{
				await Cmd.Wait(0.5f);
			}
			if (result2.type == RelicPickingResultType.FoughtOver)
			{
				((CanvasItem)holder).ZIndex = 1;
				((CanvasItem)_fightBackstop).Visible = true;
				Tween val = ((Node)this).CreateTween();
				val.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("global_position"), Variant.op_Implicit((_fightBackstop.Size - ((Control)holder).Size) * 0.5f), 0.25).SetTrans((TransitionType)10).SetEase((EaseType)0);
				val.TweenProperty((GodotObject)(object)_fightBackstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
				_hands.BeforeFightStarted(result2.fight.playersInvolved);
				await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
				await Cmd.Wait(1f);
				await _hands.DoFight(result2, holder);
				val = ((Node)this).CreateTween();
				val.TweenProperty((GodotObject)(object)_fightBackstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
				await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
				((CanvasItem)_fightBackstop).Visible = false;
				((CanvasItem)holder).ZIndex = 0;
			}
			else if (result2.type != RelicPickingResultType.Skipped)
			{
				NHandImage hand = _hands.GetHand(result2.player.NetId);
				if (hand != null)
				{
					tasksToWait.Add(TaskHelper.RunSafely(hand.GrabRelic(holder)));
					await Cmd.Wait(0.25f);
				}
			}
			relicPickingResultType = result2.type;
		}
		await Task.WhenAll(tasksToWait);
		if (tasksToWait.Count == 0 && _runState.Players.Count > 1)
		{
			await Cmd.Wait(0.7f);
		}
		foreach (RelicPickingResult result in results)
		{
			NTreasureRoomRelicHolder nTreasureRoomRelicHolder = _holdersInUse.First((NTreasureRoomRelicHolder h) => h.Relic.Model == result.relic);
			RelicModel relic = result.relic.ToMutable();
			nTreasureRoomRelicHolder.Disable();
			if (result.type != RelicPickingResultType.Skipped)
			{
				TaskHelper.RunSafely(RelicCmd.Obtain(relic, result.player));
				if (LocalContext.IsMe(result.player))
				{
					NRun.Instance.GlobalUi.RelicInventory.AnimateRelic(relic, ((Control)nTreasureRoomRelicHolder).GlobalPosition, ((Control)nTreasureRoomRelicHolder).Scale);
				}
				if (_runState.Players.Count == 1)
				{
					((CanvasItem)nTreasureRoomRelicHolder).Visible = false;
				}
			}
			foreach (Player player2 in _runState.Players)
			{
				if (player2 != result.player)
				{
					player2.RelicGrabBag.MoveToFallback(result.relic);
				}
			}
		}
		_relicPickingCompleteTaskCompletionSource.SetResult();
	}

	private void RefreshVotes()
	{
		foreach (NTreasureRoomRelicHolder item in _holdersInUse)
		{
			item.VoteContainer.RefreshPlayerVotes();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Expected O, but got Unknown
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeRelics, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SpawnEmptyChestVfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSelectionEnabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isEnabled"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("chestVisual"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("chestVisual"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PickRelic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshVotes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitializeRelics && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeRelics();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SpawnEmptyChestVfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SpawnEmptyChestVfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSelectionEnabled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetSelectionEnabled(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AnimIn(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimOut && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AnimOut(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PickRelic && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PickRelic(VariantUtils.ConvertTo<NTreasureRoomRelicHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshVotes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshVotes();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeRelics)
		{
			return true;
		}
		if ((ref method) == MethodName.SpawnEmptyChestVfx)
		{
			return true;
		}
		if ((ref method) == MethodName.SetSelectionEnabled)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimOut)
		{
			return true;
		}
		if ((ref method) == MethodName.PickRelic)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshVotes)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.SingleplayerRelicHolder)
		{
			SingleplayerRelicHolder = VariantUtils.ConvertTo<NTreasureRoomRelicHolder>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fightBackstop)
		{
			_fightBackstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fightLabel)
		{
			_fightLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._emptyVfxTween)
		{
			_emptyVfxTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hands)
		{
			_hands = VariantUtils.ConvertTo<NHandImageCollection>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._openedTicks)
		{
			_openedTicks = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isEmptyChest)
		{
			_isEmptyChest = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.SingleplayerRelicHolder)
		{
			NTreasureRoomRelicHolder singleplayerRelicHolder = SingleplayerRelicHolder;
			value = VariantUtils.CreateFrom<NTreasureRoomRelicHolder>(ref singleplayerRelicHolder);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._fightBackstop)
		{
			value = VariantUtils.CreateFrom<Control>(ref _fightBackstop);
			return true;
		}
		if ((ref name) == PropertyName._fightLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _fightLabel);
			return true;
		}
		if ((ref name) == PropertyName._emptyVfxTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _emptyVfxTween);
			return true;
		}
		if ((ref name) == PropertyName._hands)
		{
			value = VariantUtils.CreateFrom<NHandImageCollection>(ref _hands);
			return true;
		}
		if ((ref name) == PropertyName._openedTicks)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _openedTicks);
			return true;
		}
		if ((ref name) == PropertyName._isEmptyChest)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isEmptyChest);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._fightBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fightLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._emptyVfxTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hands, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._openedTicks, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isEmptyChest, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.SingleplayerRelicHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName singleplayerRelicHolder = PropertyName.SingleplayerRelicHolder;
		NTreasureRoomRelicHolder singleplayerRelicHolder2 = SingleplayerRelicHolder;
		info.AddProperty(singleplayerRelicHolder, Variant.From<NTreasureRoomRelicHolder>(ref singleplayerRelicHolder2));
		info.AddProperty(PropertyName._fightBackstop, Variant.From<Control>(ref _fightBackstop));
		info.AddProperty(PropertyName._fightLabel, Variant.From<MegaLabel>(ref _fightLabel));
		info.AddProperty(PropertyName._emptyVfxTween, Variant.From<Tween>(ref _emptyVfxTween));
		info.AddProperty(PropertyName._hands, Variant.From<NHandImageCollection>(ref _hands));
		info.AddProperty(PropertyName._openedTicks, Variant.From<ulong>(ref _openedTicks));
		info.AddProperty(PropertyName._isEmptyChest, Variant.From<bool>(ref _isEmptyChest));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.SingleplayerRelicHolder, ref val))
		{
			SingleplayerRelicHolder = ((Variant)(ref val)).As<NTreasureRoomRelicHolder>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._fightBackstop, ref val2))
		{
			_fightBackstop = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._fightLabel, ref val3))
		{
			_fightLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._emptyVfxTween, ref val4))
		{
			_emptyVfxTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hands, ref val5))
		{
			_hands = ((Variant)(ref val5)).As<NHandImageCollection>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._openedTicks, ref val6))
		{
			_openedTicks = ((Variant)(ref val6)).As<ulong>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isEmptyChest, ref val7))
		{
			_isEmptyChest = ((Variant)(ref val7)).As<bool>();
		}
	}
}
