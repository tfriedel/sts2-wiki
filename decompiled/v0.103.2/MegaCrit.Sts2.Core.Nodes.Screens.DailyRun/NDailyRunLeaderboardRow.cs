using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Leaderboard;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

[ScriptPath("res://src/Core/Nodes/Screens/DailyRun/NDailyRunLeaderboardRow.cs")]
public class NDailyRunLeaderboardRow : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName FormatHoursAndMinutes = StringName.op_Implicit("FormatHoursAndMinutes");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _rank = StringName.op_Implicit("_rank");

		public static readonly StringName _name = StringName.op_Implicit("_name");

		public static readonly StringName _floor = StringName.op_Implicit("_floor");

		public static readonly StringName _badges = StringName.op_Implicit("_badges");

		public static readonly StringName _time = StringName.op_Implicit("_time");

		public static readonly StringName _isYou = StringName.op_Implicit("_isYou");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/daily_run/daily_run_leaderboard_row");

	private MegaLabel _rank;

	private MegaLabel _name;

	private MegaLabel _floor;

	private MegaLabel _badges;

	private MegaLabel _time;

	private LeaderboardEntry _entry;

	private bool _isYou;

	public static NDailyRunLeaderboardRow? Create(LeaderboardEntry entry, bool isYou)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NDailyRunLeaderboardRow nDailyRunLeaderboardRow = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NDailyRunLeaderboardRow>((GenEditState)0);
		nDailyRunLeaderboardRow._entry = entry;
		nDailyRunLeaderboardRow._isYou = isYou;
		return nDailyRunLeaderboardRow;
	}

	public override void _Ready()
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		_rank = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Rank"));
		_floor = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Floor"));
		_name = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Name"));
		_badges = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Badges"));
		_time = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Time"));
		IEnumerable<string> values = _entry.userIds.Select((ulong id) => PlatformUtil.GetPlayerName(LeaderboardManager.CurrentPlatform, id));
		DecodedDailyScore decodedDailyScore = ScoreUtility.DecodeDailyScore(_entry.score);
		if (!decodedDailyScore.isValid)
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		_rank.SetTextAutoSize($"{_entry.rank + 1}");
		_name.SetTextAutoSize(string.Join(",", values));
		if (_isYou)
		{
			((CanvasItem)_name).Modulate = StsColors.blue;
		}
		_floor.SetTextAutoSize($"{decodedDailyScore.floors}");
		if (decodedDailyScore.victory == 2)
		{
			((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Tick"))).Visible = true;
		}
		_badges.SetTextAutoSize($"{decodedDailyScore.badges}");
		_time.SetTextAutoSize(FormatHoursAndMinutes(decodedDailyScore.runTime));
	}

	private static string FormatHoursAndMinutes(int value)
	{
		if (value >= 9999)
		{
			return "--:--";
		}
		int value2 = value / 60;
		int value3 = value % 60;
		return $"{value2}:{value3:D2}";
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FormatHoursAndMinutes, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FormatHoursAndMinutes && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = FormatHoursAndMinutes(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.FormatHoursAndMinutes && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = FormatHoursAndMinutes(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.FormatHoursAndMinutes)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._rank)
		{
			_rank = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._name)
		{
			_name = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._floor)
		{
			_floor = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._badges)
		{
			_badges = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._time)
		{
			_time = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isYou)
		{
			_isYou = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._rank)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _rank);
			return true;
		}
		if ((ref name) == PropertyName._name)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _name);
			return true;
		}
		if ((ref name) == PropertyName._floor)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _floor);
			return true;
		}
		if ((ref name) == PropertyName._badges)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _badges);
			return true;
		}
		if ((ref name) == PropertyName._time)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _time);
			return true;
		}
		if ((ref name) == PropertyName._isYou)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isYou);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._rank, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._name, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._floor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._badges, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._time, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isYou, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._rank, Variant.From<MegaLabel>(ref _rank));
		info.AddProperty(PropertyName._name, Variant.From<MegaLabel>(ref _name));
		info.AddProperty(PropertyName._floor, Variant.From<MegaLabel>(ref _floor));
		info.AddProperty(PropertyName._badges, Variant.From<MegaLabel>(ref _badges));
		info.AddProperty(PropertyName._time, Variant.From<MegaLabel>(ref _time));
		info.AddProperty(PropertyName._isYou, Variant.From<bool>(ref _isYou));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._rank, ref val))
		{
			_rank = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._name, ref val2))
		{
			_name = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._floor, ref val3))
		{
			_floor = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._badges, ref val4))
		{
			_badges = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._time, ref val5))
		{
			_time = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isYou, ref val6))
		{
			_isYou = ((Variant)(ref val6)).As<bool>();
		}
	}
}
