using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Debug;

[ScriptPath("res://src/Core/Nodes/Debug/NDebugInfoLabelManager.cs")]
public class NDebugInfoLabelManager : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName UpdateText = StringName.op_Implicit("UpdateText");

		public static readonly StringName OnModdedWarningHovered = StringName.op_Implicit("OnModdedWarningHovered");

		public static readonly StringName OnModdedWarningUnhovered = StringName.op_Implicit("OnModdedWarningUnhovered");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName isMainMenu = StringName.op_Implicit("isMainMenu");

		public static readonly StringName _releaseInfo = StringName.op_Implicit("_releaseInfo");

		public static readonly StringName _moddedWarning = StringName.op_Implicit("_moddedWarning");

		public static readonly StringName _seed = StringName.op_Implicit("_seed");

		public static readonly StringName _modWarningContainer = StringName.op_Implicit("_modWarningContainer");

		public static readonly StringName _modWarningLabel = StringName.op_Implicit("_modWarningLabel");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	public bool isMainMenu;

	private MegaLabel _releaseInfo;

	private MegaLabel _moddedWarning;

	private MegaLabel? _seed;

	private Control? _modWarningContainer;

	private MegaRichTextLabel? _modWarningLabel;

	public override void _Ready()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		_releaseInfo = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ReleaseInfo"));
		_moddedWarning = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ModdedWarning"));
		_seed = ((Node)this).GetNodeOrNull<MegaLabel>(NodePath.op_Implicit("%DebugSeed"));
		_modWarningContainer = ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("%ModWarningContainer"));
		_modWarningLabel = ((Node)this).GetNodeOrNull<MegaRichTextLabel>(NodePath.op_Implicit("%ModWarningLabel"));
		((GodotObject)_moddedWarning).Connect(SignalName.MouseEntered, Callable.From((Action)OnModdedWarningHovered), 0u);
		((GodotObject)_moddedWarning).Connect(SignalName.MouseExited, Callable.From((Action)OnModdedWarningUnhovered), 0u);
		Control? modWarningContainer = _modWarningContainer;
		if (modWarningContainer != null)
		{
			((CanvasItem)modWarningContainer).SetVisible(false);
		}
		UpdateText(null);
		if (ReleaseInfoManager.Instance.ReleaseInfo == null)
		{
			TaskHelper.RunSafely(SetCommitIdInEditor());
		}
	}

	private async Task SetCommitIdInEditor()
	{
		if (GitHelper.ShortCommitIdTask != null)
		{
			UpdateText(await GitHelper.ShortCommitIdTask);
		}
	}

	private void UpdateText(string? commitId)
	{
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		ReleaseInfo releaseInfo = ReleaseInfoManager.Instance.ReleaseInfo;
		string text = releaseInfo?.Date.ToString("yyyy.MM.dd") ?? "???";
		string text2 = releaseInfo?.Version ?? commitId ?? "NONE";
		if (isMainMenu)
		{
			((Label)_releaseInfo).Text = text2 + "\n" + text;
		}
		else
		{
			((Label)_releaseInfo).Text = $"[{text2}] ({text})";
		}
		((CanvasItem)_moddedWarning).Visible = ModManager.IsRunningModded();
		if (!((CanvasItem)_moddedWarning).Visible)
		{
			return;
		}
		bool flag = ModManager.Mods.Any(delegate(Mod m)
		{
			if (m.state != ModLoadState.Failed)
			{
				List<LocString>? errors = m.errors;
				if (errors == null)
				{
					return false;
				}
				return errors.Count > 0;
			}
			return true;
		});
		if (isMainMenu)
		{
			LocString locString = new LocString("main_menu_ui", "MODDED_WARNING");
			locString.Add("count", ModManager.GetLoadedMods().Count());
			locString.Add("hasError", flag);
			_moddedWarning.SetTextAutoSize(locString.GetFormattedText());
			LocString[] array = ModManager.Mods.SelectMany((Mod m) => m.errors ?? new List<LocString>()).ToArray();
			if (array.Length != 0)
			{
				_modWarningLabel.Text = string.Join("\n", array.Select((LocString s) => s.GetFormattedText()));
			}
			else
			{
				LocString locString2 = new LocString("main_menu_ui", "MOD_ERROR.NONE");
				locString2.Add("mods", string.Join(", ", ModManager.GetLoadedMods().Select(delegate(Mod m)
				{
					object obj = m.manifest?.name;
					if (obj == null)
					{
						ModManifest? manifest = m.manifest;
						if (manifest == null)
						{
							return (string)null;
						}
						obj = manifest.id;
					}
					return (string)obj;
				})));
				_modWarningLabel.Text = locString2.GetFormattedText();
			}
		}
		else
		{
			_moddedWarning.SetTextAutoSize($"MODDED ({ModManager.GetLoadedMods().Count()})");
		}
		if (flag)
		{
			((CanvasItem)_moddedWarning).Modulate = StsColors.redGlow;
		}
	}

	private void OnModdedWarningHovered()
	{
		if (_modWarningContainer != null)
		{
			((CanvasItem)_modWarningContainer).Visible = true;
		}
	}

	private void OnModdedWarningUnhovered()
	{
		if (_modWarningContainer != null)
		{
			((CanvasItem)_modWarningContainer).Visible = false;
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.hideVersionInfo, false))
		{
			((CanvasItem)_releaseInfo).Visible = !((CanvasItem)_releaseInfo).Visible;
			((CanvasItem)_moddedWarning).Visible = ModManager.IsRunningModded() && !((CanvasItem)_moddedWarning).Visible;
			MegaLabel? seed = _seed;
			if (seed != null)
			{
				((CanvasItem)seed).SetVisible(!((CanvasItem)_seed).Visible);
			}
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(((CanvasItem)_releaseInfo).Visible ? "Show Version Info" : "Hide Version Info"));
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("commitId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnModdedWarningHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnModdedWarningUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateText(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnModdedWarningHovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnModdedWarningHovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnModdedWarningUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnModdedWarningUnhovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateText)
		{
			return true;
		}
		if ((ref method) == MethodName.OnModdedWarningHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnModdedWarningUnhovered)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.isMainMenu)
		{
			isMainMenu = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._releaseInfo)
		{
			_releaseInfo = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._moddedWarning)
		{
			_moddedWarning = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._seed)
		{
			_seed = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modWarningContainer)
		{
			_modWarningContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modWarningLabel)
		{
			_modWarningLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
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
		if ((ref name) == PropertyName.isMainMenu)
		{
			value = VariantUtils.CreateFrom<bool>(ref isMainMenu);
			return true;
		}
		if ((ref name) == PropertyName._releaseInfo)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _releaseInfo);
			return true;
		}
		if ((ref name) == PropertyName._moddedWarning)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _moddedWarning);
			return true;
		}
		if ((ref name) == PropertyName._seed)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _seed);
			return true;
		}
		if ((ref name) == PropertyName._modWarningContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _modWarningContainer);
			return true;
		}
		if ((ref name) == PropertyName._modWarningLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _modWarningLabel);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.isMainMenu, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._releaseInfo, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._moddedWarning, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._seed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modWarningContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modWarningLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName.isMainMenu, Variant.From<bool>(ref isMainMenu));
		info.AddProperty(PropertyName._releaseInfo, Variant.From<MegaLabel>(ref _releaseInfo));
		info.AddProperty(PropertyName._moddedWarning, Variant.From<MegaLabel>(ref _moddedWarning));
		info.AddProperty(PropertyName._seed, Variant.From<MegaLabel>(ref _seed));
		info.AddProperty(PropertyName._modWarningContainer, Variant.From<Control>(ref _modWarningContainer));
		info.AddProperty(PropertyName._modWarningLabel, Variant.From<MegaRichTextLabel>(ref _modWarningLabel));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.isMainMenu, ref val))
		{
			isMainMenu = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._releaseInfo, ref val2))
		{
			_releaseInfo = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._moddedWarning, ref val3))
		{
			_moddedWarning = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._seed, ref val4))
		{
			_seed = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._modWarningContainer, ref val5))
		{
			_modWarningContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._modWarningLabel, ref val6))
		{
			_modWarningLabel = ((Variant)(ref val6)).As<MegaRichTextLabel>();
		}
	}
}
