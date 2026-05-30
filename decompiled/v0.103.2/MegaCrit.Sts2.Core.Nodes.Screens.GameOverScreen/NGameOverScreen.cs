using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;

[ScriptPath("res://src/Core/Nodes/Screens/GameOverScreen/NGameOverScreen.cs")]
public class NGameOverScreen : NClickableControl, IOverlayScreen, IScreenContext
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName DiscoveredAnyEpochs = StringName.op_Implicit("DiscoveredAnyEpochs");

		public static readonly StringName InitializeBannerAndQuote = StringName.op_Implicit("InitializeBannerAndQuote");

		public static readonly StringName OpenSummaryScreen = StringName.op_Implicit("OpenSummaryScreen");

		public static readonly StringName AddScoreLine = StringName.op_Implicit("AddScoreLine");

		public static readonly StringName PlayUnlockSfx = StringName.op_Implicit("PlayUnlockSfx");

		public static readonly StringName TweenScore = StringName.op_Implicit("TweenScore");

		public static readonly StringName GetScoreThreshold = StringName.op_Implicit("GetScoreThreshold");

		public static readonly StringName ShowLeaderboard = StringName.op_Implicit("ShowLeaderboard");

		public static readonly StringName HideSummary = StringName.op_Implicit("HideSummary");

		public static readonly StringName OpenRunHistoryScreen = StringName.op_Implicit("OpenRunHistoryScreen");

		public static readonly StringName OnMainMenuButtonPressed = StringName.op_Implicit("OnMainMenuButtonPressed");

		public static readonly StringName OpenTimeline = StringName.op_Implicit("OpenTimeline");

		public static readonly StringName ReturnToMainMenu = StringName.op_Implicit("ReturnToMainMenu");

		public static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

		public static readonly StringName MoveCreaturesToDifferentLayerAndDisableUi = StringName.op_Implicit("MoveCreaturesToDifferentLayerAndDisableUi");

		public static readonly StringName UpdateBackstopMaterial = StringName.op_Implicit("UpdateBackstopMaterial");

		public static readonly StringName AfterOverlayClosed = StringName.op_Implicit("AfterOverlayClosed");

		public static readonly StringName AfterOverlayShown = StringName.op_Implicit("AfterOverlayShown");

		public static readonly StringName AfterOverlayHidden = StringName.op_Implicit("AfterOverlayHidden");

		public static readonly StringName GetAscensionMulti = StringName.op_Implicit("GetAscensionMulti");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _continueButton = StringName.op_Implicit("_continueButton");

		public static readonly StringName _viewRunButton = StringName.op_Implicit("_viewRunButton");

		public static readonly StringName _mainMenuButton = StringName.op_Implicit("_mainMenuButton");

		public static readonly StringName _leaderboardButton = StringName.op_Implicit("_leaderboardButton");

		public static readonly StringName _badgeContainer = StringName.op_Implicit("_badgeContainer");

		public static readonly StringName _scoreLineContainer = StringName.op_Implicit("_scoreLineContainer");

		public static readonly StringName _scoreBar = StringName.op_Implicit("_scoreBar");

		public static readonly StringName _scoreFg = StringName.op_Implicit("_scoreFg");

		public static readonly StringName _scoreProgress = StringName.op_Implicit("_scoreProgress");

		public static readonly StringName _unlocksRemaining = StringName.op_Implicit("_unlocksRemaining");

		public static readonly StringName _score = StringName.op_Implicit("_score");

		public static readonly StringName _scoreThreshold = StringName.op_Implicit("_scoreThreshold");

		public static readonly StringName _scoreUnlockedEpochId = StringName.op_Implicit("_scoreUnlockedEpochId");

		public static readonly StringName _leaderboard = StringName.op_Implicit("_leaderboard");

		public static readonly StringName _creatureContainer = StringName.op_Implicit("_creatureContainer");

		public static readonly StringName _summaryContainer = StringName.op_Implicit("_summaryContainer");

		public static readonly StringName _fullBlackBackstop = StringName.op_Implicit("_fullBlackBackstop");

		public static readonly StringName _summaryBackstop = StringName.op_Implicit("_summaryBackstop");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _deathQuote = StringName.op_Implicit("_deathQuote");

		public static readonly StringName _victoryDamageLabel = StringName.op_Implicit("_victoryDamageLabel");

		public static readonly StringName _uiNode = StringName.op_Implicit("_uiNode");

		public static readonly StringName _screenshakeContainer = StringName.op_Implicit("_screenshakeContainer");

		public static readonly StringName _discoveryLabel = StringName.op_Implicit("_discoveryLabel");

		public static readonly StringName _encounterQuote = StringName.op_Implicit("_encounterQuote");

		public static readonly StringName _isAnimatingSummary = StringName.op_Implicit("_isAnimatingSummary");

		public static readonly StringName _backstopMaterial = StringName.op_Implicit("_backstopMaterial");

		public static readonly StringName _quoteTween = StringName.op_Implicit("_quoteTween");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private static readonly StringName _threshold = new StringName("threshold");

	private RunState _runState;

	private SerializableRun _serializableRun;

	private RunHistory _history;

	private Player _localPlayer;

	private NGameOverContinueButton _continueButton;

	private NViewRunButton _viewRunButton;

	private NReturnToMainMenuButton _mainMenuButton;

	private NGameOverContinueButton _leaderboardButton;

	private Control _badgeContainer;

	private GridContainer _scoreLineContainer;

	private readonly List<NScoreLine> _scoreLines = new List<NScoreLine>();

	private Control _scoreBar;

	private Control _scoreFg;

	private MegaLabel _scoreProgress;

	private MegaLabel _unlocksRemaining;

	private int _score;

	private int _scoreThreshold;

	private string? _scoreUnlockedEpochId;

	private NDailyRunLeaderboard _leaderboard;

	private Control _creatureContainer;

	private NRunSummary _summaryContainer;

	private ColorRect _fullBlackBackstop;

	private ColorRect _summaryBackstop;

	private ColorRect _backstop;

	private NCommonBanner _banner;

	private MegaRichTextLabel _deathQuote;

	private MegaRichTextLabel _victoryDamageLabel;

	private Control _uiNode;

	private Control _screenshakeContainer;

	private MegaLabel _discoveryLabel;

	private string _encounterQuote;

	private bool _isAnimatingSummary;

	private ShaderMaterial _backstopMaterial;

	private Tween? _quoteTween;

	private static string ScenePath => SceneHelper.GetScenePath("screens/game_over_screen");

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public NetScreenType ScreenType => NetScreenType.GameOver;

	public bool UseSharedBackstop => false;

	public Control DefaultFocusedControl => (Control)(object)this;

	public override void _Ready()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		bool win = _runState.CurrentRoom?.IsVictoryRoom ?? false;
		_history = RunManager.Instance.History ?? new RunHistory
		{
			Win = win
		};
		_score = ScoreUtility.CalculateScore(_serializableRun, _history.Win);
		_uiNode = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Ui"));
		_continueButton = ((Node)this).GetNode<NGameOverContinueButton>(NodePath.op_Implicit("%ContinueButton"));
		((GodotObject)_continueButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenSummaryScreen), 0u);
		_continueButton.Disable();
		_viewRunButton = ((Node)this).GetNode<NViewRunButton>(NodePath.op_Implicit("%ViewRunButton"));
		((GodotObject)_viewRunButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenRunHistoryScreen), 0u);
		_mainMenuButton = ((Node)this).GetNode<NReturnToMainMenuButton>(NodePath.op_Implicit("%MainMenuButton"));
		((GodotObject)_mainMenuButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnMainMenuButtonPressed), 0u);
		_scoreLineContainer = ((Node)this).GetNode<GridContainer>(NodePath.op_Implicit("%ScoreLineContainer"));
		_badgeContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BadgeContainer"));
		_scoreBar = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ScoreBar"));
		_scoreFg = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ScoreFg"));
		_scoreProgress = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ScoreProgress"));
		_unlocksRemaining = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%UnlocksRemaining"));
		_screenshakeContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ScreenshakeContainer"));
		_leaderboardButton = ((Node)this).GetNode<NGameOverContinueButton>(NodePath.op_Implicit("%LeaderboardButton"));
		((GodotObject)_leaderboardButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ShowLeaderboard), 0u);
		_creatureContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CreatureContainer"));
		_summaryContainer = ((Node)this).GetNode<NRunSummary>(NodePath.op_Implicit("%RunSummaryContainer"));
		_backstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("%Backstop"));
		_fullBlackBackstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("%FullBlackBackstop"));
		_backstopMaterial = (ShaderMaterial)((CanvasItem)_backstop).Material;
		_summaryBackstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("%SummaryBackstop"));
		_leaderboard = ((Node)this).GetNode<NDailyRunLeaderboard>(NodePath.op_Implicit("%DailyRunLeaderboard"));
		_banner = ((Node)this).GetNode<NCommonBanner>(NodePath.op_Implicit("%Banner"));
		_victoryDamageLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%VictoryDamageLabel"));
		_discoveryLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%DiscoveryLabel"));
		_discoveryLabel.SetTextAutoSize(new LocString("game_over_screen", "DISCOVERY_HEADER").GetFormattedText());
		_deathQuote = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%DeathQuoteLabel"));
		InitializeBannerAndQuote();
		ActiveScreenContext.Instance.Update();
		_leaderboardButton.Disable();
		_viewRunButton.Disable();
		_mainMenuButton.Disable();
		((CanvasItem)_leaderboard).Visible = false;
	}

	private bool DiscoveredAnyEpochs()
	{
		return _localPlayer.DiscoveredEpochs.Count > 0;
	}

	private void InitializeBannerAndQuote()
	{
		ModelId id = _localPlayer.Character.Id;
		if (_history.Win)
		{
			_banner.label.SetTextAutoSize(new LocString("game_over_screen", "BANNER.falseWin").GetRawText());
			_deathQuote.Text = string.Empty;
			long personalArchitectDamage = StatsManager.GetPersonalArchitectDamage();
			long? globalArchitectDamage = StatsManager.GetGlobalArchitectDamage();
			StringBuilder stringBuilder = new StringBuilder();
			LocString locString;
			if (globalArchitectDamage.HasValue)
			{
				locString = new LocString("game_over_screen", "VICTORY_DAMAGE");
				locString.Add("TotalDamage", globalArchitectDamage.Value);
			}
			else
			{
				locString = new LocString("game_over_screen", "VICTORY_DAMAGE_LOCAL");
			}
			locString.Add("PlayerDamage", _score);
			locString.Add("PersonalDamage", personalArchitectDamage);
			stringBuilder.Append(locString.GetFormattedText());
			int ascensionLevel = _runState.AscensionLevel;
			if (ascensionLevel < 10 && ascensionLevel > 0 && _runState.AscensionLevel >= _localPlayer.MaxAscensionWhenRunStarted && _runState.GameMode == GameMode.Standard)
			{
				stringBuilder.Append("\n\n");
				LocString locString2 = new LocString("game_over_screen", "VICTORY_UNLOCKED_ASCENSION");
				locString2.Add("AscensionLevel", _runState.AscensionLevel + 1);
				stringBuilder.Append(locString2.GetFormattedText());
			}
			_victoryDamageLabel.Text = stringBuilder.ToString();
		}
		else
		{
			LocTable table = LocManager.Instance.GetTable("game_over_screen");
			IReadOnlyList<LocString> locStringsWithPrefix = table.GetLocStringsWithPrefix("BANNER.lose");
			_banner.label.SetTextAutoSize(Rng.Chaotic.NextItem(locStringsWithPrefix).GetRawText());
			IReadOnlyList<LocString> locStringsWithPrefix2 = table.GetLocStringsWithPrefix("QUOTES");
			_deathQuote.Text = Rng.Chaotic.NextItem(locStringsWithPrefix2).GetFormattedText();
		}
		_encounterQuote = NRunHistory.GetDeathQuote(_history, id, NRunHistory.GetGameOverType(_history));
	}

	private async Task AnimateInQuote()
	{
		if (((CanvasItem)_deathQuote).Modulate.A != 0f)
		{
			Tween? quoteTween = _quoteTween;
			if (quoteTween != null)
			{
				quoteTween.Kill();
			}
			_quoteTween = ((Node)this).CreateTween();
			_quoteTween.TweenProperty((GodotObject)(object)_deathQuote, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
			await ((GodotObject)this).ToSignal((GodotObject)(object)_quoteTween, SignalName.Finished);
			if (!((Node?)(object)this).IsValid())
			{
				return;
			}
			_deathQuote.Text = _encounterQuote;
			_quoteTween.Kill();
			await Cmd.Wait(1f);
		}
		if (((Node?)(object)this).IsValid())
		{
			Tween? quoteTween2 = _quoteTween;
			if (quoteTween2 != null)
			{
				quoteTween2.Kill();
			}
			_quoteTween = ((Node)this).CreateTween().SetParallel(true);
			if (_history.Win)
			{
				_quoteTween.TweenProperty((GodotObject)(object)_victoryDamageLabel, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 2.0).SetEase((EaseType)1).SetTrans((TransitionType)1);
				_quoteTween.TweenProperty((GodotObject)(object)_victoryDamageLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 2.0);
				await ((GodotObject)this).ToSignal((GodotObject)(object)_quoteTween, SignalName.Finished);
			}
			else
			{
				_quoteTween.TweenProperty((GodotObject)(object)_deathQuote, NodePath.op_Implicit("position:y"), Variant.op_Implicit(156f), 2.0).SetEase((EaseType)1).SetTrans((TransitionType)5)
					.From(Variant.op_Implicit(90f));
				_quoteTween.TweenProperty((GodotObject)(object)_deathQuote, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.5);
			}
		}
	}

	public static NGameOverScreen? Create(RunState runState, SerializableRun serializableRun)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NGameOverScreen nGameOverScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NGameOverScreen>((GenEditState)0);
		nGameOverScreen._runState = runState;
		nGameOverScreen._serializableRun = serializableRun;
		nGameOverScreen._localPlayer = LocalContext.GetMe(runState);
		return nGameOverScreen;
	}

	private void OpenSummaryScreen(NButton _)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		_isAnimatingSummary = true;
		_continueButton.Disable();
		((CanvasItem)_victoryDamageLabel).Visible = false;
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_summaryBackstop, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5);
		TaskHelper.RunSafely(AnimateInQuote());
		TaskHelper.RunSafely(AnimateRunSummary());
	}

	private async Task AnimateRunSummary()
	{
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_banner, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)_banner).Position.Y - 32f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		((CanvasItem)_summaryContainer).Visible = true;
		await AnimateScoreLines();
		await AnimateBadges();
		await AnimateScoreBar();
		await AnimateDiscoveries();
		if (_history.GameMode == GameMode.Daily)
		{
			_leaderboard.Initialize(RunManager.Instance.DailyTime.Value, _runState.Players.Select((Player p) => p.NetId), allowPagination: false);
			((CanvasItem)_leaderboardButton).Visible = true;
			_leaderboardButton.Enable();
			return;
		}
		if (DiscoveredAnyEpochs())
		{
			_mainMenuButton.SetLabelForUnlock();
		}
		((CanvasItem)_mainMenuButton).Visible = true;
		_mainMenuButton.Enable();
	}

	private async Task AnimateScoreLines()
	{
		_scoreLines.Clear();
		AddScoreLine("SCORE_LINE.floorsClimbed", "FloorCount", _runState.TotalFloor, $"+{ScoreUtility.GetScoreForFloor(_serializableRun.MapPointHistory)}", "res://images/ui/game_over_screen/score_floor.png");
		int amount = _serializableRun.MapPointHistory.SelectMany((List<MapPointHistoryEntry> actEntries) => actEntries).Sum((MapPointHistoryEntry e) => e.GetEntry(_localPlayer.NetId).GoldGained);
		AddScoreLine("SCORE_LINE.goldGained", "GoldAmount", amount, $"+{ScoreUtility.GetScoreForGoldGained(_serializableRun.MapPointHistory, _serializableRun.Players.Count)}", "res://images/ui/game_over_screen/score_gold.png");
		int elitesKilledCount = ScoreUtility.GetElitesKilledCount(_serializableRun.MapPointHistory);
		if (elitesKilledCount > 0)
		{
			AddScoreLine("SCORE_LINE.elitesKilled", "EliteCount", elitesKilledCount, $"+{ScoreUtility.GetScoreForElitesKilled(elitesKilledCount)}", "res://images/ui/game_over_screen/score_elite.png");
		}
		int bossesSlainCount = ScoreUtility.GetBossesSlainCount(_serializableRun.MapPointHistory, _history.Win);
		if (bossesSlainCount > 0)
		{
			AddScoreLine("SCORE_LINE.bossesSlain", "BossCount", bossesSlainCount, $"+{ScoreUtility.GetScoreForBossesSlain(bossesSlainCount)}", "res://images/ui/game_over_screen/score_boss.png");
		}
		int ascension = _history.Ascension;
		if (ascension > 0)
		{
			AddScoreLine("SCORE_LINE.ascension", "AscensionLevel", ascension, "x" + GetAscensionMulti(ascension), "res://images/ui/game_over_screen/score_ascension.png");
		}
		foreach (NScoreLine scoreLine in _scoreLines)
		{
			await scoreLine.AnimateIn();
		}
		await Cmd.Wait(0.5f);
	}

	private async Task AnimateBadges()
	{
		List<Badge> badges = ScoreUtility.GetBadges(_serializableRun, _localPlayer.NetId, _history.Win);
		foreach (Badge item in badges)
		{
			((Node)(object)_badgeContainer).AddChildSafely((Node?)(object)NBadge.Create(item));
		}
		if (!_serializableRun.GameMode.AreAchievementsAndEpochsLocked())
		{
			SaveBadgesToProgress(badges);
		}
		await Cmd.Wait(0.25f);
		foreach (NBadge item2 in ((IEnumerable)((Node)_badgeContainer).GetChildren(false)).OfType<NBadge>())
		{
			await item2.AnimateIn();
		}
		await Cmd.Wait(0.5f);
	}

	private void SaveBadgesToProgress(List<Badge> badgesToSave)
	{
		CharacterStats characterStats = SaveManager.Instance.Progress.CharacterStats[_localPlayer.Character.Id];
		foreach (Badge badge in badgesToSave)
		{
			BadgeStats badgeStats = characterStats.Badges.FirstOrDefault((BadgeStats b) => b.Id.Equals(badge.Id) && b.Rarity == badge.Rarity);
			if (badgeStats == null)
			{
				BadgeStats item = new BadgeStats
				{
					Id = badge.Id,
					Count = 1,
					Rarity = badge.Rarity
				};
				characterStats.Badges.Add(item);
				Log.Info("You got a new badge: " + badge.Id);
			}
			else
			{
				badgeStats.Count++;
				Log.Info($"You got badge: {badge.Id} again. Now you have {badgeStats.Count}!");
			}
		}
	}

	private void AddScoreLine(string locEntryKey, string? locAmountKey = null, int amount = 0, string scoreLabel = "ERROR", string? iconPath = null)
	{
		LocString locString = new LocString("game_over_screen", locEntryKey);
		if (locAmountKey != null)
		{
			locString.Add(locAmountKey, amount);
		}
		Texture2D icon = ((iconPath == null) ? null : PreloadManager.Cache.GetTexture2D(iconPath));
		NScoreLine nScoreLine = NScoreLine.Create(locString.GetFormattedText(), scoreLabel, icon);
		((Node)(object)_scoreLineContainer).AddChildSafely((Node?)(object)nScoreLine);
		_scoreLines.Add(nScoreLine);
	}

	private async Task AnimateScoreBar()
	{
		int unlocksRemaining = SaveManager.Instance.GetUnlocksRemaining();
		LocString locString = new LocString("game_over_screen", "SCORE.unlocksRemaining");
		locString.Add("UnlockCount", unlocksRemaining);
		_unlocksRemaining.SetTextAutoSize(locString.GetFormattedText());
		if (unlocksRemaining > 0)
		{
			int currentScore3 = SaveManager.Instance.GetCurrentScore();
			_scoreThreshold = GetScoreThreshold(unlocksRemaining);
			_scoreProgress.SetTextAutoSize($"[{currentScore3}/{_scoreThreshold}]");
			_scoreFg.Scale = new Vector2((float)currentScore3 / (float)_scoreThreshold, 1f);
			Tween scoreTween5 = ((Node)this).CreateTween();
			scoreTween5.TweenProperty((GodotObject)(object)_scoreBar, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.3);
			await ((GodotObject)scoreTween5).ToSignal((GodotObject)(object)scoreTween5, SignalName.Finished);
			if (currentScore3 + _score >= _scoreThreshold)
			{
				Log.Info("New Unlock, yay!");
				MegaLabel node = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%UnlockText"));
				_scoreUnlockedEpochId = SaveManager.Instance.IncrementUnlock();
				currentScore3 -= _scoreThreshold;
				int newThreshold = GetScoreThreshold(unlocksRemaining - 1);
				string locEntryKey = ((newThreshold == 0) ? "SCORE.unlockedAllMessage" : "SCORE.unlockedEpochMessage");
				node.SetTextAutoSize(new LocString("game_over_screen", locEntryKey).GetFormattedText());
				scoreTween5 = ((Node)this).CreateTween().SetParallel(true);
				scoreTween5.TweenInterval(1.0);
				scoreTween5.Chain();
				scoreTween5.TweenMethod(Callable.From<int>((Action<int>)TweenScore), Variant.op_Implicit(currentScore3 + _scoreThreshold), Variant.op_Implicit(_scoreThreshold), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
				scoreTween5.TweenProperty((GodotObject)(object)_scoreFg, NodePath.op_Implicit("scale:x"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
				scoreTween5.Chain();
				scoreTween5.TweenCallback(Callable.From((Action)PlayUnlockSfx));
				scoreTween5.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
				scoreTween5.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-60f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)11);
				await ((GodotObject)scoreTween5).ToSignal((GodotObject)(object)scoreTween5, SignalName.Finished);
				if (_scoreUnlockedEpochId != null && !SaveManager.Instance.IsEpochRevealed(_scoreUnlockedEpochId))
				{
					EpochModel epochModel = EpochModel.Get(_scoreUnlockedEpochId);
					SaveManager.Instance.ObtainEpoch(_scoreUnlockedEpochId);
					((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NGainEpochVfx.Create(epochModel));
					_localPlayer.DiscoveredEpochs.Add(epochModel.Id);
					LocalContext.GetMe(_serializableRun).DiscoveredEpochs.Add(epochModel.Id);
				}
				LocString locString2 = new LocString("game_over_screen", "SCORE.unlocksRemaining");
				locString2.Add("UnlockCount", unlocksRemaining - 1);
				_unlocksRemaining.SetTextAutoSize(locString2.GetFormattedText());
				_scoreThreshold = newThreshold;
				currentScore3 += _score;
				if (newThreshold == 0 || currentScore3 == 0)
				{
					Log.Info("Player has gotten all unlocks or they've overflowed exactly 0");
					SaveManager.Instance.Progress.CurrentScore = 0;
				}
				else if (currentScore3 >= newThreshold)
				{
					Log.Info("Score is too awesome. Disallow double unlock.");
					scoreTween5.Kill();
					scoreTween5 = ((Node)this).CreateTween().SetParallel(true);
					scoreTween5.TweenInterval(0.5);
					scoreTween5.Chain();
					scoreTween5.TweenMethod(Callable.From<int>((Action<int>)TweenScore), Variant.op_Implicit(0), Variant.op_Implicit(newThreshold * 99 / 100), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
					scoreTween5.TweenProperty((GodotObject)(object)_scoreFg, NodePath.op_Implicit("scale:x"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
						.From(Variant.op_Implicit(0f));
					await ((GodotObject)scoreTween5).ToSignal((GodotObject)(object)scoreTween5, SignalName.Finished);
					SaveManager.Instance.Progress.CurrentScore = newThreshold - 1;
				}
				else
				{
					Log.Info("Animate overflow score.");
					scoreTween5.Kill();
					scoreTween5 = ((Node)this).CreateTween().SetParallel(true);
					scoreTween5.Chain();
					scoreTween5.TweenInterval(0.5);
					scoreTween5.Chain();
					scoreTween5.TweenMethod(Callable.From<int>((Action<int>)TweenScore), Variant.op_Implicit(0), Variant.op_Implicit(currentScore3), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
					scoreTween5.TweenProperty((GodotObject)(object)_scoreFg, NodePath.op_Implicit("scale:x"), Variant.op_Implicit((float)currentScore3 / (float)newThreshold), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
						.From(Variant.op_Implicit(0f));
					await ((GodotObject)scoreTween5).ToSignal((GodotObject)(object)scoreTween5, SignalName.Finished);
					SaveManager.Instance.Progress.CurrentScore = currentScore3;
				}
			}
			else
			{
				Log.Info("Not enough score to level up");
				scoreTween5 = ((Node)this).CreateTween().SetParallel(true);
				scoreTween5.TweenInterval(0.5);
				scoreTween5.TweenMethod(Callable.From<int>((Action<int>)TweenScore), Variant.op_Implicit(currentScore3), Variant.op_Implicit(currentScore3 + _score), 1.0);
				scoreTween5.TweenProperty((GodotObject)(object)_scoreFg, NodePath.op_Implicit("scale:x"), Variant.op_Implicit((float)(currentScore3 + _score) / (float)_scoreThreshold), 1.0);
				SaveManager.Instance.Progress.CurrentScore += _score;
			}
			SaveManager.Instance.SaveProgressFile();
		}
		else
		{
			Log.Info("This player has all unlocks. No action");
		}
	}

	private void PlayUnlockSfx()
	{
		Log.Info("TODO: Play the ding unlock sfx here pls");
	}

	private void TweenScore(int value)
	{
		_scoreProgress.SetTextAutoSize($"[{value}/{_scoreThreshold}]");
	}

	private int GetScoreThreshold(int unlocksRemaining)
	{
		return (18 - unlocksRemaining) switch
		{
			0 => 200, 
			1 => 500, 
			2 => 750, 
			3 => 1000, 
			4 => 1250, 
			5 => 1500, 
			6 => 1600, 
			7 => 1700, 
			8 => 1800, 
			9 => 1900, 
			10 => 2000, 
			11 => 2100, 
			12 => 2200, 
			13 => 2300, 
			14 => 2400, 
			15 => 2500, 
			16 => 2500, 
			17 => 2500, 
			_ => 0, 
		};
	}

	private void ShowLeaderboard(NButton _)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		_banner.ChangeText(new LocString("main_menu_ui", "DAILY_RUN_MENU.LEADERBOARDS.title").GetRawText());
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		NDailyRunLeaderboard leaderboard = _leaderboard;
		Color modulate = ((CanvasItem)_leaderboard).Modulate;
		modulate.A = 0f;
		((CanvasItem)leaderboard).Modulate = modulate;
		val.TweenProperty((GodotObject)(object)_leaderboard, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
		val.TweenProperty((GodotObject)(object)_summaryContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		val.TweenProperty((GodotObject)(object)_deathQuote, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		val.Chain().TweenCallback(Callable.From((Action)HideSummary));
		((CanvasItem)_leaderboard).Visible = true;
		_leaderboardButton.Disable();
		if (DiscoveredAnyEpochs())
		{
			_mainMenuButton.SetLabelForUnlock();
		}
		((CanvasItem)_mainMenuButton).Visible = true;
		_mainMenuButton.Enable();
	}

	private void HideSummary()
	{
		((CanvasItem)_summaryContainer).Visible = false;
		((CanvasItem)_deathQuote).Visible = false;
	}

	private async Task AnimateDiscoveries()
	{
		await _summaryContainer.AnimateInDiscoveries(_runState);
		_isAnimatingSummary = false;
	}

	private void OpenRunHistoryScreen(NButton _)
	{
		Control child = ResourceLoader.Load<PackedScene>("res://scenes/screens/run_history_screen/run_history_screen_via_game_over_screen.tscn", (string)null, (CacheMode)1).Instantiate<Control>((GenEditState)0);
		((Node)(object)this).AddChildSafely((Node?)(object)child);
	}

	private void OnMainMenuButtonPressed(NButton _)
	{
		if (RunManager.Instance.NetService.Type == NetGameType.Host)
		{
			RunManager.Instance.NetService.Disconnect(NetError.QuitGameOver);
		}
		_mainMenuButton.Disable();
		if (DiscoveredAnyEpochs())
		{
			OpenTimeline();
		}
		else
		{
			ReturnToMainMenu();
		}
	}

	private void OpenTimeline()
	{
		TaskHelper.RunSafely(TransitionOutToTimeline());
	}

	private void ReturnToMainMenu()
	{
		TaskHelper.RunSafely(TransitionOutToMainMenu());
	}

	private async Task TransitionOutToTimeline()
	{
		await NGame.Instance.GoToTimelineAfterRun();
	}

	private async Task TransitionOutToMainMenu()
	{
		await NGame.Instance.ReturnToMainMenuAfterRun();
	}

	public void AfterOverlayOpened()
	{
		MoveCreaturesToDifferentLayerAndDisableUi();
		TaskHelper.RunSafely(AnimateIn());
	}

	private void MoveCreaturesToDifferentLayerAndDisableUi()
	{
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		List<NCreatureVisuals> list = new List<NCreatureVisuals>();
		List<NCreature> list2;
		if (NCombatRoom.Instance != null)
		{
			if (NCombatRoom.Instance.Mode == CombatRoomMode.ActiveCombat)
			{
				NCombatRoom.Instance.Ui.AnimOut();
			}
			list2 = NCombatRoom.Instance.CreatureNodes.ToList();
			list = list2.Select((NCreature c) => c.Visuals).ToList();
		}
		else if (NMerchantRoom.Instance != null)
		{
			list2 = new List<NCreature>();
			foreach (NMerchantCharacter playerVisual in NMerchantRoom.Instance.PlayerVisuals)
			{
				playerVisual.PlayAnimation("die");
				((Node)playerVisual).Reparent((Node)(object)_creatureContainer, true);
			}
		}
		else if (NRestSiteRoom.Instance != null)
		{
			list2 = new List<NCreature>();
			list = new List<NCreatureVisuals>();
			Vector2 val = default(Vector2);
			foreach (Player player in _runState.Players)
			{
				NCreatureVisuals nCreatureVisuals = player.Creature.CreateVisuals();
				list.Add(nCreatureVisuals);
				((Node)(object)_creatureContainer).AddChildSafely((Node?)(object)nCreatureVisuals);
				nCreatureVisuals.SpineAnimation.SetAnimation("die", loop: false);
				NRestSiteCharacter characterForPlayer = NRestSiteRoom.Instance.GetCharacterForPlayer(player);
				((Node2D)nCreatureVisuals).GlobalPosition = ((Node2D)characterForPlayer).GlobalPosition;
				((Node2D)nCreatureVisuals).Scale = ((Node2D)characterForPlayer).Scale;
				((CanvasItem)characterForPlayer).Visible = false;
				((Vector2)(ref val))._002Ector(100f, 100f);
				((Node2D)nCreatureVisuals).Position = ((Node2D)nCreatureVisuals).Position + val * new Vector2((float)Math.Sign(((Node2D)nCreatureVisuals).Scale.X), (float)Math.Sign(((Node2D)nCreatureVisuals).Scale.Y));
			}
		}
		else
		{
			list2 = new List<NCreature>();
			list = new List<NCreatureVisuals>();
			foreach (Player player2 in _runState.Players)
			{
				NCreatureVisuals nCreatureVisuals2 = player2.Creature.CreateVisuals();
				list.Add(nCreatureVisuals2);
				((Node)(object)_creatureContainer).AddChildSafely((Node?)(object)nCreatureVisuals2);
				nCreatureVisuals2.SpineAnimation.SetAnimation("die", loop: false);
			}
			float num = Math.Min(250f, (((Control)this).Size.X - 200f) / (float)(list.Count - 1));
			float num2 = (float)(list.Count - 1) * (0f - num) * 0.5f;
			foreach (NCreatureVisuals item in list)
			{
				((Node2D)item).Position = _creatureContainer.Size * 0.5f + new Vector2(num2, 200f);
				num2 += num;
			}
		}
		list2.Sort((NCreature c1, NCreature c2) => ((Node)c1).GetIndex(false).CompareTo(((Node)c2).GetIndex(false)));
		foreach (NCreature item2 in list2)
		{
			item2.AnimHideIntent();
			item2.AnimDisableUi();
		}
		foreach (NCreatureVisuals item3 in list)
		{
			((Node)item3).Reparent((Node)(object)_creatureContainer, true);
		}
	}

	private async Task AnimateIn()
	{
		Tween backstopTween = ((Node)this).CreateTween();
		((CanvasItem)_uiNode).Modulate = StsColors.transparentWhite;
		if (NEventRoom.Instance != null)
		{
			ColorRect fullBlackBackstop = _fullBlackBackstop;
			Color modulate = ((CanvasItem)_fullBlackBackstop).Modulate;
			modulate.A = 0f;
			((CanvasItem)fullBlackBackstop).Modulate = modulate;
			((CanvasItem)_fullBlackBackstop).Visible = true;
			backstopTween.TweenProperty((GodotObject)(object)_fullBlackBackstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2);
			foreach (NCreatureVisuals item in ((IEnumerable)((Node)_creatureContainer).GetChildren(false)).OfType<NCreatureVisuals>())
			{
				modulate = ((CanvasItem)item).Modulate;
				modulate.A = 0f;
				((CanvasItem)item).Modulate = modulate;
				backstopTween.Parallel().TweenProperty((GodotObject)(object)item, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2);
			}
		}
		Variant shaderParameter = _backstopMaterial.GetShaderParameter(_threshold);
		backstopTween.TweenMethod(Callable.From<float>((Action<float>)UpdateBackstopMaterial), shaderParameter, Variant.op_Implicit(1f), 1.5).SetEase((EaseType)2).SetTrans((TransitionType)1);
		await ((GodotObject)this).ToSignal((GodotObject)(object)backstopTween, SignalName.Finished);
		_banner.AnimateIn();
		backstopTween.Kill();
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_uiNode, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
		TaskHelper.RunSafely(AnimateInQuote());
		_continueButton.Enable();
	}

	private void UpdateBackstopMaterial(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_backstopMaterial.SetShaderParameter(_threshold, Variant.op_Implicit(value));
	}

	public void AfterOverlayClosed()
	{
		((Node)(object)this).QueueFreeSafely();
	}

	public void AfterOverlayShown()
	{
		NGame.Instance.SetScreenShakeTarget(_screenshakeContainer);
		((CanvasItem)this).Visible = true;
	}

	public void AfterOverlayHidden()
	{
		((CanvasItem)this).Visible = false;
	}

	private string GetAscensionMulti(int ascension)
	{
		int value = ascension / 10 + 1;
		int num = ascension % 10;
		if (num != 0)
		{
			return $"{value}.{num}";
		}
		return value.ToString();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Expected O, but got Unknown
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Expected O, but got Unknown
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Expected O, but got Unknown
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(21);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DiscoveredAnyEpochs, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeBannerAndQuote, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenSummaryScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddScoreLine, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("locEntryKey"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("locAmountKey"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("amount"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("scoreLabel"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("iconPath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayUnlockSfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TweenScore, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetScoreThreshold, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("unlocksRemaining"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowLeaderboard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideSummary, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenRunHistoryScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMainMenuButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenTimeline, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReturnToMainMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MoveCreaturesToDifferentLayerAndDisableUi, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateBackstopMaterial, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetAscensionMulti, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("ascension"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DiscoveredAnyEpochs && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = DiscoveredAnyEpochs();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.InitializeBannerAndQuote && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeBannerAndQuote();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenSummaryScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenSummaryScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddScoreLine && ((NativeVariantPtrArgs)(ref args)).Count == 5)
		{
			AddScoreLine(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[3]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[4]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayUnlockSfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayUnlockSfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TweenScore && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TweenScore(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetScoreThreshold && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			int scoreThreshold = GetScoreThreshold(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<int>(ref scoreThreshold);
			return true;
		}
		if ((ref method) == MethodName.ShowLeaderboard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowLeaderboard(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideSummary && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideSummary();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenRunHistoryScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenRunHistoryScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMainMenuButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMainMenuButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenTimeline && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenTimeline();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReturnToMainMenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReturnToMainMenu();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MoveCreaturesToDifferentLayerAndDisableUi && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MoveCreaturesToDifferentLayerAndDisableUi();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateBackstopMaterial && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateBackstopMaterial(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayShown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayHidden();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetAscensionMulti && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string ascensionMulti = GetAscensionMulti(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref ascensionMulti);
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
		if ((ref method) == MethodName.DiscoveredAnyEpochs)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeBannerAndQuote)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenSummaryScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.AddScoreLine)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayUnlockSfx)
		{
			return true;
		}
		if ((ref method) == MethodName.TweenScore)
		{
			return true;
		}
		if ((ref method) == MethodName.GetScoreThreshold)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowLeaderboard)
		{
			return true;
		}
		if ((ref method) == MethodName.HideSummary)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenRunHistoryScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMainMenuButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenTimeline)
		{
			return true;
		}
		if ((ref method) == MethodName.ReturnToMainMenu)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.MoveCreaturesToDifferentLayerAndDisableUi)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateBackstopMaterial)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayShown)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden)
		{
			return true;
		}
		if ((ref method) == MethodName.GetAscensionMulti)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._continueButton)
		{
			_continueButton = VariantUtils.ConvertTo<NGameOverContinueButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewRunButton)
		{
			_viewRunButton = VariantUtils.ConvertTo<NViewRunButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mainMenuButton)
		{
			_mainMenuButton = VariantUtils.ConvertTo<NReturnToMainMenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leaderboardButton)
		{
			_leaderboardButton = VariantUtils.ConvertTo<NGameOverContinueButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._badgeContainer)
		{
			_badgeContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreLineContainer)
		{
			_scoreLineContainer = VariantUtils.ConvertTo<GridContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreBar)
		{
			_scoreBar = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreFg)
		{
			_scoreFg = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreProgress)
		{
			_scoreProgress = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._unlocksRemaining)
		{
			_unlocksRemaining = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._score)
		{
			_score = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreThreshold)
		{
			_scoreThreshold = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreUnlockedEpochId)
		{
			_scoreUnlockedEpochId = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leaderboard)
		{
			_leaderboard = VariantUtils.ConvertTo<NDailyRunLeaderboard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureContainer)
		{
			_creatureContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._summaryContainer)
		{
			_summaryContainer = VariantUtils.ConvertTo<NRunSummary>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fullBlackBackstop)
		{
			_fullBlackBackstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._summaryBackstop)
		{
			_summaryBackstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<NCommonBanner>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathQuote)
		{
			_deathQuote = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._victoryDamageLabel)
		{
			_victoryDamageLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._uiNode)
		{
			_uiNode = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenshakeContainer)
		{
			_screenshakeContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveryLabel)
		{
			_discoveryLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._encounterQuote)
		{
			_encounterQuote = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isAnimatingSummary)
		{
			_isAnimatingSummary = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstopMaterial)
		{
			_backstopMaterial = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._quoteTween)
		{
			_quoteTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ScreenType)
		{
			NetScreenType screenType = ScreenType;
			value = VariantUtils.CreateFrom<NetScreenType>(ref screenType);
			return true;
		}
		if ((ref name) == PropertyName.UseSharedBackstop)
		{
			bool useSharedBackstop = UseSharedBackstop;
			value = VariantUtils.CreateFrom<bool>(ref useSharedBackstop);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._continueButton)
		{
			value = VariantUtils.CreateFrom<NGameOverContinueButton>(ref _continueButton);
			return true;
		}
		if ((ref name) == PropertyName._viewRunButton)
		{
			value = VariantUtils.CreateFrom<NViewRunButton>(ref _viewRunButton);
			return true;
		}
		if ((ref name) == PropertyName._mainMenuButton)
		{
			value = VariantUtils.CreateFrom<NReturnToMainMenuButton>(ref _mainMenuButton);
			return true;
		}
		if ((ref name) == PropertyName._leaderboardButton)
		{
			value = VariantUtils.CreateFrom<NGameOverContinueButton>(ref _leaderboardButton);
			return true;
		}
		if ((ref name) == PropertyName._badgeContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _badgeContainer);
			return true;
		}
		if ((ref name) == PropertyName._scoreLineContainer)
		{
			value = VariantUtils.CreateFrom<GridContainer>(ref _scoreLineContainer);
			return true;
		}
		if ((ref name) == PropertyName._scoreBar)
		{
			value = VariantUtils.CreateFrom<Control>(ref _scoreBar);
			return true;
		}
		if ((ref name) == PropertyName._scoreFg)
		{
			value = VariantUtils.CreateFrom<Control>(ref _scoreFg);
			return true;
		}
		if ((ref name) == PropertyName._scoreProgress)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _scoreProgress);
			return true;
		}
		if ((ref name) == PropertyName._unlocksRemaining)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _unlocksRemaining);
			return true;
		}
		if ((ref name) == PropertyName._score)
		{
			value = VariantUtils.CreateFrom<int>(ref _score);
			return true;
		}
		if ((ref name) == PropertyName._scoreThreshold)
		{
			value = VariantUtils.CreateFrom<int>(ref _scoreThreshold);
			return true;
		}
		if ((ref name) == PropertyName._scoreUnlockedEpochId)
		{
			value = VariantUtils.CreateFrom<string>(ref _scoreUnlockedEpochId);
			return true;
		}
		if ((ref name) == PropertyName._leaderboard)
		{
			value = VariantUtils.CreateFrom<NDailyRunLeaderboard>(ref _leaderboard);
			return true;
		}
		if ((ref name) == PropertyName._creatureContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _creatureContainer);
			return true;
		}
		if ((ref name) == PropertyName._summaryContainer)
		{
			value = VariantUtils.CreateFrom<NRunSummary>(ref _summaryContainer);
			return true;
		}
		if ((ref name) == PropertyName._fullBlackBackstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _fullBlackBackstop);
			return true;
		}
		if ((ref name) == PropertyName._summaryBackstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _summaryBackstop);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<NCommonBanner>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._deathQuote)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _deathQuote);
			return true;
		}
		if ((ref name) == PropertyName._victoryDamageLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _victoryDamageLabel);
			return true;
		}
		if ((ref name) == PropertyName._uiNode)
		{
			value = VariantUtils.CreateFrom<Control>(ref _uiNode);
			return true;
		}
		if ((ref name) == PropertyName._screenshakeContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _screenshakeContainer);
			return true;
		}
		if ((ref name) == PropertyName._discoveryLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _discoveryLabel);
			return true;
		}
		if ((ref name) == PropertyName._encounterQuote)
		{
			value = VariantUtils.CreateFrom<string>(ref _encounterQuote);
			return true;
		}
		if ((ref name) == PropertyName._isAnimatingSummary)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isAnimatingSummary);
			return true;
		}
		if ((ref name) == PropertyName._backstopMaterial)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _backstopMaterial);
			return true;
		}
		if ((ref name) == PropertyName._quoteTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _quoteTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._continueButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewRunButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mainMenuButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leaderboardButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._badgeContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scoreLineContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scoreBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scoreFg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scoreProgress, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unlocksRemaining, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._score, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._scoreThreshold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._scoreUnlockedEpochId, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leaderboard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._creatureContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._summaryContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fullBlackBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._summaryBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathQuote, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._victoryDamageLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._uiNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenshakeContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveryLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._encounterQuote, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isAnimatingSummary, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstopMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._quoteTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSharedBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._continueButton, Variant.From<NGameOverContinueButton>(ref _continueButton));
		info.AddProperty(PropertyName._viewRunButton, Variant.From<NViewRunButton>(ref _viewRunButton));
		info.AddProperty(PropertyName._mainMenuButton, Variant.From<NReturnToMainMenuButton>(ref _mainMenuButton));
		info.AddProperty(PropertyName._leaderboardButton, Variant.From<NGameOverContinueButton>(ref _leaderboardButton));
		info.AddProperty(PropertyName._badgeContainer, Variant.From<Control>(ref _badgeContainer));
		info.AddProperty(PropertyName._scoreLineContainer, Variant.From<GridContainer>(ref _scoreLineContainer));
		info.AddProperty(PropertyName._scoreBar, Variant.From<Control>(ref _scoreBar));
		info.AddProperty(PropertyName._scoreFg, Variant.From<Control>(ref _scoreFg));
		info.AddProperty(PropertyName._scoreProgress, Variant.From<MegaLabel>(ref _scoreProgress));
		info.AddProperty(PropertyName._unlocksRemaining, Variant.From<MegaLabel>(ref _unlocksRemaining));
		info.AddProperty(PropertyName._score, Variant.From<int>(ref _score));
		info.AddProperty(PropertyName._scoreThreshold, Variant.From<int>(ref _scoreThreshold));
		info.AddProperty(PropertyName._scoreUnlockedEpochId, Variant.From<string>(ref _scoreUnlockedEpochId));
		info.AddProperty(PropertyName._leaderboard, Variant.From<NDailyRunLeaderboard>(ref _leaderboard));
		info.AddProperty(PropertyName._creatureContainer, Variant.From<Control>(ref _creatureContainer));
		info.AddProperty(PropertyName._summaryContainer, Variant.From<NRunSummary>(ref _summaryContainer));
		info.AddProperty(PropertyName._fullBlackBackstop, Variant.From<ColorRect>(ref _fullBlackBackstop));
		info.AddProperty(PropertyName._summaryBackstop, Variant.From<ColorRect>(ref _summaryBackstop));
		info.AddProperty(PropertyName._backstop, Variant.From<ColorRect>(ref _backstop));
		info.AddProperty(PropertyName._banner, Variant.From<NCommonBanner>(ref _banner));
		info.AddProperty(PropertyName._deathQuote, Variant.From<MegaRichTextLabel>(ref _deathQuote));
		info.AddProperty(PropertyName._victoryDamageLabel, Variant.From<MegaRichTextLabel>(ref _victoryDamageLabel));
		info.AddProperty(PropertyName._uiNode, Variant.From<Control>(ref _uiNode));
		info.AddProperty(PropertyName._screenshakeContainer, Variant.From<Control>(ref _screenshakeContainer));
		info.AddProperty(PropertyName._discoveryLabel, Variant.From<MegaLabel>(ref _discoveryLabel));
		info.AddProperty(PropertyName._encounterQuote, Variant.From<string>(ref _encounterQuote));
		info.AddProperty(PropertyName._isAnimatingSummary, Variant.From<bool>(ref _isAnimatingSummary));
		info.AddProperty(PropertyName._backstopMaterial, Variant.From<ShaderMaterial>(ref _backstopMaterial));
		info.AddProperty(PropertyName._quoteTween, Variant.From<Tween>(ref _quoteTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._continueButton, ref val))
		{
			_continueButton = ((Variant)(ref val)).As<NGameOverContinueButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewRunButton, ref val2))
		{
			_viewRunButton = ((Variant)(ref val2)).As<NViewRunButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._mainMenuButton, ref val3))
		{
			_mainMenuButton = ((Variant)(ref val3)).As<NReturnToMainMenuButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._leaderboardButton, ref val4))
		{
			_leaderboardButton = ((Variant)(ref val4)).As<NGameOverContinueButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._badgeContainer, ref val5))
		{
			_badgeContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreLineContainer, ref val6))
		{
			_scoreLineContainer = ((Variant)(ref val6)).As<GridContainer>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreBar, ref val7))
		{
			_scoreBar = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreFg, ref val8))
		{
			_scoreFg = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreProgress, ref val9))
		{
			_scoreProgress = ((Variant)(ref val9)).As<MegaLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._unlocksRemaining, ref val10))
		{
			_unlocksRemaining = ((Variant)(ref val10)).As<MegaLabel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._score, ref val11))
		{
			_score = ((Variant)(ref val11)).As<int>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreThreshold, ref val12))
		{
			_scoreThreshold = ((Variant)(ref val12)).As<int>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreUnlockedEpochId, ref val13))
		{
			_scoreUnlockedEpochId = ((Variant)(ref val13)).As<string>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._leaderboard, ref val14))
		{
			_leaderboard = ((Variant)(ref val14)).As<NDailyRunLeaderboard>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureContainer, ref val15))
		{
			_creatureContainer = ((Variant)(ref val15)).As<Control>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._summaryContainer, ref val16))
		{
			_summaryContainer = ((Variant)(ref val16)).As<NRunSummary>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._fullBlackBackstop, ref val17))
		{
			_fullBlackBackstop = ((Variant)(ref val17)).As<ColorRect>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._summaryBackstop, ref val18))
		{
			_summaryBackstop = ((Variant)(ref val18)).As<ColorRect>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val19))
		{
			_backstop = ((Variant)(ref val19)).As<ColorRect>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val20))
		{
			_banner = ((Variant)(ref val20)).As<NCommonBanner>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathQuote, ref val21))
		{
			_deathQuote = ((Variant)(ref val21)).As<MegaRichTextLabel>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._victoryDamageLabel, ref val22))
		{
			_victoryDamageLabel = ((Variant)(ref val22)).As<MegaRichTextLabel>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._uiNode, ref val23))
		{
			_uiNode = ((Variant)(ref val23)).As<Control>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenshakeContainer, ref val24))
		{
			_screenshakeContainer = ((Variant)(ref val24)).As<Control>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveryLabel, ref val25))
		{
			_discoveryLabel = ((Variant)(ref val25)).As<MegaLabel>();
		}
		Variant val26 = default(Variant);
		if (info.TryGetProperty(PropertyName._encounterQuote, ref val26))
		{
			_encounterQuote = ((Variant)(ref val26)).As<string>();
		}
		Variant val27 = default(Variant);
		if (info.TryGetProperty(PropertyName._isAnimatingSummary, ref val27))
		{
			_isAnimatingSummary = ((Variant)(ref val27)).As<bool>();
		}
		Variant val28 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstopMaterial, ref val28))
		{
			_backstopMaterial = ((Variant)(ref val28)).As<ShaderMaterial>();
		}
		Variant val29 = default(Variant);
		if (info.TryGetProperty(PropertyName._quoteTween, ref val29))
		{
			_quoteTween = ((Variant)(ref val29)).As<Tween>();
		}
	}
}
