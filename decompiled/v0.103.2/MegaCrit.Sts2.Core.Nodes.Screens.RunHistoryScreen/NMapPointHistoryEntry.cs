using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NMapPointHistoryEntry.cs")]
public class NMapPointHistoryEntry : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName Highlight = StringName.op_Implicit("Highlight");

		public static readonly StringName Unhighlight = StringName.op_Implicit("Unhighlight");

		public static readonly StringName GetSfxVolume = StringName.op_Implicit("GetSfxVolume");

		public static readonly StringName HurryUp = StringName.op_Implicit("HurryUp");

		public static readonly StringName SetupForAnimation = StringName.op_Implicit("SetupForAnimation");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName FloorNum = StringName.op_Implicit("FloorNum");

		public static readonly StringName _baseScale = StringName.op_Implicit("_baseScale");

		public static readonly StringName _texture = StringName.op_Implicit("_texture");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _questIcon = StringName.op_Implicit("_questIcon");

		public static readonly StringName _animateInTween = StringName.op_Implicit("_animateInTween");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _baseAngle = StringName.op_Implicit("_baseAngle");

		public static readonly StringName _hurryUp = StringName.op_Implicit("_hurryUp");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	[CompilerGenerated]
	private sealed class _003CGetAssetPaths_003Ed__4 : IEnumerable<string>, IEnumerable, IEnumerator<string>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private string _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private RoomType[] _003C_003E7__wrap1;

		private int _003C_003E7__wrap2;

		private RoomType _003CroomType_003E5__4;

		private IEnumerator<EncounterModel> _003C_003E7__wrap4;

		private EncounterModel _003Cencounter_003E5__6;

		private IEnumerator<AncientEventModel> _003C_003E7__wrap6;

		private AncientEventModel _003Cancient_003E5__8;

		string IEnumerator<string>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CGetAssetPaths_003Ed__4(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			switch (_003C_003E1__state)
			{
			case -3:
			case 6:
			case 7:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
				break;
			case -4:
			case 8:
			case 9:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally2();
				}
				break;
			}
			_003C_003E7__wrap1 = null;
			_003C_003E7__wrap4 = null;
			_003Cencounter_003E5__6 = null;
			_003C_003E7__wrap6 = null;
			_003Cancient_003E5__8 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				RoomType[] array2;
				string roomIconOutlinePath;
				string roomIconOutlinePath2;
				string roomIconOutlinePath3;
				string roomIconOutlinePath4;
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E2__current = ScenePath;
					_003C_003E1__state = 1;
					return true;
				case 1:
				{
					_003C_003E1__state = -1;
					RoomType[] array = new RoomType[6]
					{
						RoomType.Monster,
						RoomType.Elite,
						RoomType.Event,
						RoomType.Shop,
						RoomType.Treasure,
						RoomType.RestSite
					};
					_003C_003E7__wrap1 = array;
					_003C_003E7__wrap2 = 0;
					goto IL_0109;
				}
				case 2:
					_003C_003E1__state = -1;
					goto IL_00cb;
				case 3:
					_003C_003E1__state = -1;
					goto IL_00fb;
				case 4:
					_003C_003E1__state = -1;
					goto IL_018b;
				case 5:
					_003C_003E1__state = -1;
					goto IL_01bb;
				case 6:
					_003C_003E1__state = -3;
					goto IL_026b;
				case 7:
					_003C_003E1__state = -3;
					goto IL_02a1;
				case 8:
					_003C_003E1__state = -4;
					goto IL_0323;
				case 9:
					{
						_003C_003E1__state = -4;
						goto IL_0357;
					}
					IL_0109:
					if (_003C_003E7__wrap2 < _003C_003E7__wrap1.Length)
					{
						_003CroomType_003E5__4 = _003C_003E7__wrap1[_003C_003E7__wrap2];
						string roomIconPath = ImageHelper.GetRoomIconPath(MapPointType.Monster, _003CroomType_003E5__4, null);
						if (roomIconPath != null)
						{
							_003C_003E2__current = roomIconPath;
							_003C_003E1__state = 2;
							return true;
						}
						goto IL_00cb;
					}
					_003C_003E7__wrap1 = null;
					array2 = new RoomType[4]
					{
						RoomType.Monster,
						RoomType.Elite,
						RoomType.Shop,
						RoomType.Treasure
					};
					_003C_003E7__wrap1 = array2;
					_003C_003E7__wrap2 = 0;
					goto IL_01c9;
					IL_0357:
					_003Cancient_003E5__8 = null;
					goto IL_035e;
					IL_01c9:
					if (_003C_003E7__wrap2 < _003C_003E7__wrap1.Length)
					{
						_003CroomType_003E5__4 = _003C_003E7__wrap1[_003C_003E7__wrap2];
						string roomIconPath2 = ImageHelper.GetRoomIconPath(MapPointType.Unknown, _003CroomType_003E5__4, null);
						if (roomIconPath2 != null)
						{
							_003C_003E2__current = roomIconPath2;
							_003C_003E1__state = 4;
							return true;
						}
						goto IL_018b;
					}
					_003C_003E7__wrap1 = null;
					_003C_003E7__wrap4 = ModelDb.AllEncounters.Where((EncounterModel e) => e.RoomType == RoomType.Boss).GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_02a8;
					IL_026b:
					roomIconOutlinePath = ImageHelper.GetRoomIconOutlinePath(MapPointType.Boss, RoomType.Boss, _003Cencounter_003E5__6.Id);
					if (roomIconOutlinePath != null)
					{
						_003C_003E2__current = roomIconOutlinePath;
						_003C_003E1__state = 7;
						return true;
					}
					goto IL_02a1;
					IL_00fb:
					_003C_003E7__wrap2++;
					goto IL_0109;
					IL_02a1:
					_003Cencounter_003E5__6 = null;
					goto IL_02a8;
					IL_02a8:
					if (_003C_003E7__wrap4.MoveNext())
					{
						_003Cencounter_003E5__6 = _003C_003E7__wrap4.Current;
						string roomIconPath3 = ImageHelper.GetRoomIconPath(MapPointType.Boss, RoomType.Boss, _003Cencounter_003E5__6.Id);
						if (roomIconPath3 != null)
						{
							_003C_003E2__current = roomIconPath3;
							_003C_003E1__state = 6;
							return true;
						}
						goto IL_026b;
					}
					_003C_003Em__Finally1();
					_003C_003E7__wrap4 = null;
					_003C_003E7__wrap6 = ModelDb.AllAncients.GetEnumerator();
					_003C_003E1__state = -4;
					goto IL_035e;
					IL_018b:
					roomIconOutlinePath2 = ImageHelper.GetRoomIconOutlinePath(MapPointType.Unknown, _003CroomType_003E5__4, null);
					if (roomIconOutlinePath2 != null)
					{
						_003C_003E2__current = roomIconOutlinePath2;
						_003C_003E1__state = 5;
						return true;
					}
					goto IL_01bb;
					IL_035e:
					if (_003C_003E7__wrap6.MoveNext())
					{
						_003Cancient_003E5__8 = _003C_003E7__wrap6.Current;
						string roomIconPath4 = ImageHelper.GetRoomIconPath(MapPointType.Ancient, RoomType.Event, _003Cancient_003E5__8.Id);
						if (roomIconPath4 != null)
						{
							_003C_003E2__current = roomIconPath4;
							_003C_003E1__state = 8;
							return true;
						}
						goto IL_0323;
					}
					_003C_003Em__Finally2();
					_003C_003E7__wrap6 = null;
					return false;
					IL_01bb:
					_003C_003E7__wrap2++;
					goto IL_01c9;
					IL_00cb:
					roomIconOutlinePath3 = ImageHelper.GetRoomIconOutlinePath(MapPointType.Monster, _003CroomType_003E5__4, null);
					if (roomIconOutlinePath3 != null)
					{
						_003C_003E2__current = roomIconOutlinePath3;
						_003C_003E1__state = 3;
						return true;
					}
					goto IL_00fb;
					IL_0323:
					roomIconOutlinePath4 = ImageHelper.GetRoomIconOutlinePath(MapPointType.Ancient, RoomType.Event, _003Cancient_003E5__8.Id);
					if (roomIconOutlinePath4 != null)
					{
						_003C_003E2__current = roomIconOutlinePath4;
						_003C_003E1__state = 9;
						return true;
					}
					goto IL_0357;
				}
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap4 != null)
			{
				_003C_003E7__wrap4.Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap6 != null)
			{
				_003C_003E7__wrap6.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CGetAssetPaths_003Ed__4(0);
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<string>)this).GetEnumerator();
		}
	}

	private Vector2 _baseScale = Vector2.One * 0.7f;

	private RunHistory _runHistory;

	private MapPointHistoryEntry _entry;

	private RunHistoryPlayer? _player;

	private TextureRect _texture;

	private TextureRect _outline;

	private TextureRect _questIcon;

	private Tween? _animateInTween;

	private Tween? _hoverTween;

	private float _baseAngle;

	private bool _hurryUp;

	private static string ScenePath => SceneHelper.GetScenePath("screens/run_history_screen/map_point_history_entry");

	public static IEnumerable<string> AssetPaths => GetAssetPaths();

	public int FloorNum { get; private set; }

	[IteratorStateMachine(typeof(_003CGetAssetPaths_003Ed__4))]
	private static IEnumerable<string> GetAssetPaths()
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CGetAssetPaths_003Ed__4(-2);
	}

	public override void _Ready()
	{
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		_baseAngle = Rng.Chaotic.NextGaussianFloat(0f, 1f, 0f, 5f);
		_texture = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_outline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Outline"));
		_questIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%QuestIcon"));
		((Control)_texture).RotationDegrees = (Rng.Chaotic.NextBool() ? _baseAngle : (0f - _baseAngle));
		MapPointType mapPointType = _entry.MapPointType;
		RoomType roomType = _entry.Rooms.First().RoomType;
		MapPointType mapPointType2 = _entry.MapPointType;
		bool flag = (uint)(mapPointType2 - 7) <= 1u;
		string roomIconPath = ImageHelper.GetRoomIconPath(mapPointType, roomType, flag ? _entry.Rooms.First().ModelId : null);
		MapPointType mapPointType3 = _entry.MapPointType;
		RoomType roomType2 = _entry.Rooms.First().RoomType;
		mapPointType2 = _entry.MapPointType;
		flag = (uint)(mapPointType2 - 7) <= 1u;
		string roomIconOutlinePath = ImageHelper.GetRoomIconOutlinePath(mapPointType3, roomType2, flag ? _entry.Rooms.First().ModelId : null);
		if (roomIconPath != null)
		{
			((CanvasItem)_texture).Visible = true;
			_texture.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D(roomIconPath);
		}
		else
		{
			((CanvasItem)_texture).Visible = false;
		}
		if (roomIconOutlinePath != null)
		{
			((CanvasItem)_outline).Visible = true;
			_outline.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D(roomIconOutlinePath);
		}
		else
		{
			((CanvasItem)_outline).Visible = false;
		}
		((CanvasItem)_questIcon).Visible = false;
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = 1f;
		((CanvasItem)this).Modulate = modulate;
		ConnectSignals();
	}

	public static NMapPointHistoryEntry Create(RunHistory history, MapPointHistoryEntry entry, int floorNum)
	{
		NMapPointHistoryEntry nMapPointHistoryEntry = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NMapPointHistoryEntry>((GenEditState)0);
		nMapPointHistoryEntry._runHistory = history;
		nMapPointHistoryEntry._entry = entry;
		nMapPointHistoryEntry.FloorNum = floorNum;
		return nMapPointHistoryEntry;
	}

	public void SetPlayer(RunHistoryPlayer player)
	{
		_player = player;
		((CanvasItem)_questIcon).Visible = _entry.GetEntry(_player.Id).CompletedQuests.Count > 0;
	}

	protected override void OnFocus()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (_player == null)
		{
			throw new InvalidOperationException("Player has not been set!");
		}
		Highlight();
		HoverTipAlignment alignment = HoverTip.GetHoverTipAlignment((Control)(object)this);
		NHoverTipSet tip = NHoverTipSet.CreateAndShowMapPointHistory((Control)(object)this, NMapPointHistoryHoverTip.Create(FloorNum, _player.Id, _entry));
		Callable val = Callable.From((Action)delegate
		{
			tip.SetAlignment((Control)(object)this, alignment);
		});
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		NHoverTipSet nHoverTipSet = tip;
		((Control)nHoverTipSet).GlobalPosition = ((Control)nHoverTipSet).GlobalPosition + Vector2.Down * 96f;
	}

	protected override void OnUnfocus()
	{
		Unhighlight();
		NHoverTipSet.Remove((Control)(object)this);
	}

	public void Highlight()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)_texture, NodePath.op_Implicit("scale"), Variant.op_Implicit(_baseScale * 1.5f), 0.05).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_hoverTween.TweenProperty((GodotObject)(object)_texture, NodePath.op_Implicit("rotation_degrees"), Variant.op_Implicit(0f), 0.05).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_hoverTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.halfTransparentWhite), 0.05).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	public void Unhighlight()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)_texture, NodePath.op_Implicit("scale"), Variant.op_Implicit(_baseScale), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_hoverTween.TweenProperty((GodotObject)(object)_texture, NodePath.op_Implicit("rotation_degrees"), Variant.op_Implicit(_baseAngle), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_hoverTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.quarterTransparentBlack), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	public async Task AnimateIn(int index)
	{
		((CanvasItem)this).Visible = true;
		((Control)this).Scale = Vector2.One * 0.01f;
		Tween? animateInTween = _animateInTween;
		if (animateInTween != null)
		{
			animateInTween.Kill();
		}
		_animateInTween = ((Node)this).CreateTween().SetParallel(true);
		_animateInTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), _hurryUp ? 0.05 : 0.4).SetEase((EaseType)1).SetTrans((TransitionType)11);
		_animateInTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), _hurryUp ? 0.05 : 0.1);
		TaskHelper.RunSafely(DoAnimateInEffects());
		if (_hurryUp)
		{
			await Cmd.Wait(0.05f);
			return;
		}
		float num = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.4f : 0.5f);
		float num2 = 0.2f;
		int num3 = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 5 : 9);
		float num4 = (float)index / (float)(index + num3);
		float seconds = Mathf.Lerp(num, num2, num4);
		await Cmd.Wait(seconds);
	}

	private async Task DoAnimateInEffects()
	{
		PlayerMapPointHistoryEntry entry = _entry.GetEntry(_player.Id);
		RoomType roomType = _entry.Rooms.Last().RoomType;
		if (_runHistory.MapPointHistory.Last().Last() == _entry && !_runHistory.Win)
		{
			SfxCmd.Play("event:/sfx/block_break");
			NDebugAudioManager.Instance?.Play("STS_DeathStinger_v4_Short_SFX.mp3", 0.75f * GetSfxVolume());
			return;
		}
		if ((uint)(roomType - 1) <= 2u)
		{
			await DoCombatAnimateInEffects(roomType);
			return;
		}
		switch (roomType)
		{
		case RoomType.Shop:
			SfxCmd.Play("event:/sfx/npcs/merchant/merchant_welcome");
			break;
		case RoomType.Treasure:
			SfxCmd.Play("event:/sfx/ui/gold/gold_2", GetSfxVolume());
			break;
		case RoomType.RestSite:
			if (entry.RestSiteChoices.Contains("SMITH"))
			{
				NGame.Instance.ScreenRumble(ShakeStrength.Medium, ShakeDuration.Short, RumbleStyle.Rumble);
				NDebugAudioManager.Instance?.Play("card_smith.mp3", GetSfxVolume(), PitchVariance.Small);
			}
			else if (entry.RestSiteChoices.Contains("HEAL"))
			{
				NDebugAudioManager.Instance?.Play("SOTE_SFX_SleepBlanket_v1.mp3", GetSfxVolume(), PitchVariance.Medium);
			}
			else if (entry.RestSiteChoices.Contains("DIG"))
			{
				NDebugAudioManager.Instance.Play("sts_sfx_shovel_v1.mp3", GetSfxVolume(), PitchVariance.Small);
			}
			else if (entry.RestSiteChoices.Contains("HATCH"))
			{
				SfxCmd.Play("event:/sfx/byrdpip/byrdpip_attack");
			}
			else if (entry.RestSiteChoices.Contains("LIFT"))
			{
				NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Short);
			}
			else if (!entry.RestSiteChoices.Contains("MEND"))
			{
			}
			break;
		case RoomType.Event:
			SfxCmd.Play("event:/sfx/ui/clicks/ui_hover");
			break;
		}
	}

	private async Task DoCombatAnimateInEffects(RoomType roomType)
	{
		CharacterModel character = SaveUtil.CharacterOrDeprecated(_player.Character);
		ShakeStrength? shakeStrength = null;
		switch (roomType)
		{
		case RoomType.Monster:
			shakeStrength = ShakeStrength.Weak;
			await PlaySfx(GetSmallHitSfx(character));
			break;
		case RoomType.Elite:
			shakeStrength = ShakeStrength.Medium;
			await PlaySfx(GetBigHitSfx(character));
			break;
		case RoomType.Boss:
			shakeStrength = ShakeStrength.Strong;
			await PlaySfx(GetBigHitSfx(character));
			break;
		}
		if (shakeStrength.HasValue)
		{
			NGame.Instance.ScreenRumble(shakeStrength.Value, ShakeDuration.Normal, RumbleStyle.Rumble);
		}
		await Cmd.Wait(0.25f);
		foreach (ModelId monsterId in _entry.Rooms.Last().MonsterIds)
		{
			MonsterModel monsterModel = SaveUtil.MonsterOrDeprecated(monsterId);
			if (monsterModel.HasDeathSfx)
			{
				SfxCmd.Play(monsterModel.DeathSfx);
				await Cmd.Wait(0.25f);
			}
		}
	}

	private List<string> GetSmallHitSfx(CharacterModel character)
	{
		int num;
		Span<string> span;
		int index;
		if (!(character is Defect))
		{
			if (!(character is Ironclad))
			{
				if (!(character is Necrobinder))
				{
					if (!(character is Regent))
					{
						if (character is Silent)
						{
							num = 1;
							List<string> list = new List<string>(num);
							CollectionsMarshal.SetCount(list, num);
							span = CollectionsMarshal.AsSpan(list);
							index = 0;
							span[index] = "slash_attack.mp3";
							return list;
						}
						return new List<string>();
					}
					index = 1;
					List<string> list2 = new List<string>(index);
					CollectionsMarshal.SetCount(list2, index);
					span = CollectionsMarshal.AsSpan(list2);
					num = 0;
					span[num] = "slash_attack.mp3";
					return list2;
				}
				num = 1;
				List<string> list3 = new List<string>(num);
				CollectionsMarshal.SetCount(list3, num);
				span = CollectionsMarshal.AsSpan(list3);
				index = 0;
				span[index] = "slash_attack.mp3";
				return list3;
			}
			index = 1;
			List<string> list4 = new List<string>(index);
			CollectionsMarshal.SetCount(list4, index);
			span = CollectionsMarshal.AsSpan(list4);
			num = 0;
			span[num] = "blunt_attack.mp3";
			return list4;
		}
		num = 1;
		List<string> list5 = new List<string>(num);
		CollectionsMarshal.SetCount(list5, num);
		span = CollectionsMarshal.AsSpan(list5);
		index = 0;
		span[index] = "slash_attack.mp3";
		return list5;
	}

	private List<string> GetBigHitSfx(CharacterModel character)
	{
		int num;
		Span<string> span;
		int num2;
		if (!(character is Defect))
		{
			if (!(character is Ironclad))
			{
				if (!(character is Necrobinder))
				{
					if (!(character is Regent))
					{
						if (character is Silent)
						{
							num = 2;
							List<string> list = new List<string>(num);
							CollectionsMarshal.SetCount(list, num);
							span = CollectionsMarshal.AsSpan(list);
							num2 = 0;
							span[num2] = "dagger_throw.mp3";
							num2++;
							span[num2] = "dagger_throw.mp3";
							return list;
						}
						return new List<string>();
					}
					num2 = 1;
					List<string> list2 = new List<string>(num2);
					CollectionsMarshal.SetCount(list2, num2);
					span = CollectionsMarshal.AsSpan(list2);
					num = 0;
					span[num] = "heavy_attack.mp3";
					return list2;
				}
				num = 1;
				List<string> list3 = new List<string>(num);
				CollectionsMarshal.SetCount(list3, num);
				span = CollectionsMarshal.AsSpan(list3);
				num2 = 0;
				span[num2] = "heavy_attack.mp3";
				return list3;
			}
			num2 = 1;
			List<string> list4 = new List<string>(num2);
			CollectionsMarshal.SetCount(list4, num2);
			span = CollectionsMarshal.AsSpan(list4);
			num = 0;
			span[num] = "heavy_attack.mp3";
			return list4;
		}
		num = 1;
		List<string> list5 = new List<string>(num);
		CollectionsMarshal.SetCount(list5, num);
		span = CollectionsMarshal.AsSpan(list5);
		num2 = 0;
		span[num2] = "lightning_orb_evoke.mp3";
		return list5;
	}

	private async Task PlaySfx(List<string> sfxPaths)
	{
		for (int i = 0; i < sfxPaths.Count; i++)
		{
			string text = sfxPaths[i];
			if (text.StartsWith("event:"))
			{
				SfxCmd.Play(text, GetSfxVolume());
			}
			else
			{
				NDebugAudioManager.Instance?.Play(text, GetSfxVolume(), PitchVariance.Medium);
			}
			if (i < sfxPaths.Count - 1)
			{
				await Cmd.Wait(0.1f);
			}
		}
	}

	private float GetSfxVolume()
	{
		if (!_hurryUp)
		{
			return 1f;
		}
		return 0.5f;
	}

	public void HurryUp()
	{
		_hurryUp = true;
	}

	public void SetupForAnimation()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Visible = false;
		((CanvasItem)this).Modulate = StsColors.transparentBlack;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Highlight, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Unhighlight, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetSfxVolume, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HurryUp, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetupForAnimation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Highlight && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Highlight();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Unhighlight && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Unhighlight();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetSfxVolume && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float sfxVolume = GetSfxVolume();
			ret = VariantUtils.CreateFrom<float>(ref sfxVolume);
			return true;
		}
		if ((ref method) == MethodName.HurryUp && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HurryUp();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetupForAnimation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetupForAnimation();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.Highlight)
		{
			return true;
		}
		if ((ref method) == MethodName.Unhighlight)
		{
			return true;
		}
		if ((ref method) == MethodName.GetSfxVolume)
		{
			return true;
		}
		if ((ref method) == MethodName.HurryUp)
		{
			return true;
		}
		if ((ref method) == MethodName.SetupForAnimation)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.FloorNum)
		{
			FloorNum = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseScale)
		{
			_baseScale = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._texture)
		{
			_texture = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._questIcon)
		{
			_questIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animateInTween)
		{
			_animateInTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseAngle)
		{
			_baseAngle = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hurryUp)
		{
			_hurryUp = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.FloorNum)
		{
			int floorNum = FloorNum;
			value = VariantUtils.CreateFrom<int>(ref floorNum);
			return true;
		}
		if ((ref name) == PropertyName._baseScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _baseScale);
			return true;
		}
		if ((ref name) == PropertyName._texture)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _texture);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._questIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _questIcon);
			return true;
		}
		if ((ref name) == PropertyName._animateInTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animateInTween);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._baseAngle)
		{
			value = VariantUtils.CreateFrom<float>(ref _baseAngle);
			return true;
		}
		if ((ref name) == PropertyName._hurryUp)
		{
			value = VariantUtils.CreateFrom<bool>(ref _hurryUp);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName._baseScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.FloorNum, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._texture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._questIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animateInTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._baseAngle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._hurryUp, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName floorNum = PropertyName.FloorNum;
		int floorNum2 = FloorNum;
		info.AddProperty(floorNum, Variant.From<int>(ref floorNum2));
		info.AddProperty(PropertyName._baseScale, Variant.From<Vector2>(ref _baseScale));
		info.AddProperty(PropertyName._texture, Variant.From<TextureRect>(ref _texture));
		info.AddProperty(PropertyName._outline, Variant.From<TextureRect>(ref _outline));
		info.AddProperty(PropertyName._questIcon, Variant.From<TextureRect>(ref _questIcon));
		info.AddProperty(PropertyName._animateInTween, Variant.From<Tween>(ref _animateInTween));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._baseAngle, Variant.From<float>(ref _baseAngle));
		info.AddProperty(PropertyName._hurryUp, Variant.From<bool>(ref _hurryUp));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.FloorNum, ref val))
		{
			FloorNum = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseScale, ref val2))
		{
			_baseScale = ((Variant)(ref val2)).As<Vector2>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._texture, ref val3))
		{
			_texture = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val4))
		{
			_outline = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._questIcon, ref val5))
		{
			_questIcon = ((Variant)(ref val5)).As<TextureRect>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._animateInTween, ref val6))
		{
			_animateInTween = ((Variant)(ref val6)).As<Tween>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val7))
		{
			_hoverTween = ((Variant)(ref val7)).As<Tween>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseAngle, ref val8))
		{
			_baseAngle = ((Variant)(ref val8)).As<float>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._hurryUp, ref val9))
		{
			_hurryUp = ((Variant)(ref val9)).As<bool>();
		}
	}
}
