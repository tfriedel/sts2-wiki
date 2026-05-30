using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NCustomRunModifiersList.cs")]
public class NCustomRunModifiersList : Control
{
	[Signal]
	public delegate void ModifiersChangedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Initialize = StringName.op_Implicit("Initialize");

		public static readonly StringName UntickMutuallyExclusiveModifiersForTickbox = StringName.op_Implicit("UntickMutuallyExclusiveModifiersForTickbox");

		public static readonly StringName AfterModifiersChanged = StringName.op_Implicit("AfterModifiersChanged");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _container = StringName.op_Implicit("_container");

		public static readonly StringName _mode = StringName.op_Implicit("_mode");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName ModifiersChanged = StringName.op_Implicit("ModifiersChanged");
	}

	[CompilerGenerated]
	private sealed class _003CGetAllModifiers_003Ed__7 : IEnumerable<ModifierModel>, IEnumerable, IEnumerator<ModifierModel>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private ModifierModel _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private IEnumerator<ModifierModel> _003C_003E7__wrap1;

		private CharacterCards _003CcanonicalCharacterCardsModifier_003E5__3;

		private IEnumerator<CharacterModel> _003C_003E7__wrap3;

		ModifierModel IEnumerator<ModifierModel>.Current
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
		public _003CGetAllModifiers_003Ed__7(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if ((uint)(num - -4) <= 1u || (uint)(num - 1) <= 1u)
			{
				try
				{
					if (num == -4 || num == 1)
					{
						try
						{
						}
						finally
						{
							_003C_003Em__Finally2();
						}
					}
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
			_003C_003E7__wrap1 = null;
			_003CcanonicalCharacterCardsModifier_003E5__3 = null;
			_003C_003E7__wrap3 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E7__wrap1 = ModelDb.GoodModifiers.Concat(ModelDb.BadModifiers).GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_0110;
				case 1:
					_003C_003E1__state = -4;
					goto IL_00ce;
				case 2:
					{
						_003C_003E1__state = -3;
						goto IL_0109;
					}
					IL_0110:
					if (_003C_003E7__wrap1.MoveNext())
					{
						ModifierModel current = _003C_003E7__wrap1.Current;
						_003CcanonicalCharacterCardsModifier_003E5__3 = current as CharacterCards;
						if (_003CcanonicalCharacterCardsModifier_003E5__3 != null)
						{
							_003C_003E7__wrap3 = ModelDb.AllCharacters.GetEnumerator();
							_003C_003E1__state = -4;
							goto IL_00ce;
						}
						_003C_003E2__current = current.ToMutable();
						_003C_003E1__state = 2;
						return true;
					}
					_003C_003Em__Finally1();
					_003C_003E7__wrap1 = null;
					return false;
					IL_00ce:
					if (_003C_003E7__wrap3.MoveNext())
					{
						CharacterModel current2 = _003C_003E7__wrap3.Current;
						CharacterCards characterCards = (CharacterCards)_003CcanonicalCharacterCardsModifier_003E5__3.ToMutable();
						characterCards.CharacterModel = current2.Id;
						_003C_003E2__current = characterCards;
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003Em__Finally2();
					_003C_003E7__wrap3 = null;
					goto IL_0109;
					IL_0109:
					_003CcanonicalCharacterCardsModifier_003E5__3 = null;
					goto IL_0110;
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
			if (_003C_003E7__wrap1 != null)
			{
				_003C_003E7__wrap1.Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -3;
			if (_003C_003E7__wrap3 != null)
			{
				_003C_003E7__wrap3.Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<ModifierModel> IEnumerable<ModifierModel>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CGetAllModifiers_003Ed__7(0);
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<ModifierModel>)this).GetEnumerator();
		}
	}

	private readonly List<NRunModifierTickbox> _modifierTickboxes = new List<NRunModifierTickbox>();

	private Control _container;

	private MultiplayerUiMode _mode;

	private ModifiersChangedEventHandler backing_ModifiersChanged;

	public Control? DefaultFocusedControl => (Control?)(object)_modifierTickboxes.FirstOrDefault();

	public event ModifiersChangedEventHandler ModifiersChanged
	{
		add
		{
			backing_ModifiersChanged = (ModifiersChangedEventHandler)Delegate.Combine(backing_ModifiersChanged, value);
		}
		remove
		{
			backing_ModifiersChanged = (ModifiersChangedEventHandler)Delegate.Remove(backing_ModifiersChanged, value);
		}
	}

	public override void _Ready()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		_container = ((Node)this).GetNode<Control>(NodePath.op_Implicit("ScrollContainer/Mask/Content"));
		foreach (Node child in ((Node)_container).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		foreach (ModifierModel allModifier in GetAllModifiers())
		{
			NRunModifierTickbox nRunModifierTickbox = NRunModifierTickbox.Create(allModifier);
			((Node)(object)_container).AddChildSafely((Node?)(object)nRunModifierTickbox);
			_modifierTickboxes.Add(nRunModifierTickbox);
			((GodotObject)nRunModifierTickbox).Connect(NTickbox.SignalName.Toggled, Callable.From<NRunModifierTickbox>((Action<NRunModifierTickbox>)AfterModifiersChanged), 0u);
		}
	}

	public void Initialize(MultiplayerUiMode mode)
	{
		_mode = mode;
		if ((uint)(mode - 3) > 1u)
		{
			return;
		}
		foreach (NRunModifierTickbox modifierTickbox in _modifierTickboxes)
		{
			modifierTickbox.Disable();
		}
	}

	public void SyncModifierList(IReadOnlyList<ModifierModel> modifiers)
	{
		MultiplayerUiMode mode = _mode;
		if ((uint)(mode - 1) <= 1u)
		{
			throw new InvalidOperationException("This should only be called in client or load mode!");
		}
		foreach (NRunModifierTickbox tickbox in _modifierTickboxes)
		{
			tickbox.IsTicked = modifiers.FirstOrDefault((ModifierModel m) => m.IsEquivalent(tickbox.Modifier)) != null;
		}
	}

	[IteratorStateMachine(typeof(_003CGetAllModifiers_003Ed__7))]
	private IEnumerable<ModifierModel> GetAllModifiers()
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CGetAllModifiers_003Ed__7(-2);
	}

	private void UntickMutuallyExclusiveModifiersForTickbox(NRunModifierTickbox tickbox)
	{
		NRunModifierTickbox tickbox2 = tickbox;
		if (!tickbox2.IsTicked)
		{
			return;
		}
		IReadOnlySet<ModifierModel> readOnlySet = ModelDb.MutuallyExclusiveModifiers.FirstOrDefault((IReadOnlySet<ModifierModel> s) => s.Any((ModifierModel m) => m.GetType() == tickbox2.Modifier.GetType()));
		if (readOnlySet == null)
		{
			return;
		}
		foreach (NRunModifierTickbox otherTickbox in _modifierTickboxes)
		{
			if (!(otherTickbox.Modifier.GetType() == tickbox2.Modifier.GetType()) && readOnlySet.Any((ModifierModel m) => m.GetType() == otherTickbox.Modifier.GetType()))
			{
				otherTickbox.IsTicked = false;
			}
		}
	}

	private void AfterModifiersChanged(NRunModifierTickbox tickbox)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		UntickMutuallyExclusiveModifiersForTickbox(tickbox);
		((GodotObject)this).EmitSignal(SignalName.ModifiersChanged, Array.Empty<Variant>());
	}

	public List<ModifierModel> GetModifiersTickedOn()
	{
		return (from t in _modifierTickboxes
			where t.IsTicked
			select t.Modifier).ToList();
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
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("mode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UntickMutuallyExclusiveModifiersForTickbox, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterModifiersChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Initialize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Initialize(VariantUtils.ConvertTo<MultiplayerUiMode>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UntickMutuallyExclusiveModifiersForTickbox && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UntickMutuallyExclusiveModifiersForTickbox(VariantUtils.ConvertTo<NRunModifierTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterModifiersChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AfterModifiersChanged(VariantUtils.ConvertTo<NRunModifierTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.Initialize)
		{
			return true;
		}
		if ((ref method) == MethodName.UntickMutuallyExclusiveModifiersForTickbox)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterModifiersChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._container)
		{
			_container = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mode)
		{
			_mode = VariantUtils.ConvertTo<MultiplayerUiMode>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			value = VariantUtils.CreateFrom<Control>(ref _container);
			return true;
		}
		if ((ref name) == PropertyName._mode)
		{
			value = VariantUtils.CreateFrom<MultiplayerUiMode>(ref _mode);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._container, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._mode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._container, Variant.From<Control>(ref _container));
		info.AddProperty(PropertyName._mode, Variant.From<MultiplayerUiMode>(ref _mode));
		info.AddSignalEventDelegate(SignalName.ModifiersChanged, (Delegate)backing_ModifiersChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._container, ref val))
		{
			_container = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._mode, ref val2))
		{
			_mode = ((Variant)(ref val2)).As<MultiplayerUiMode>();
		}
		ModifiersChangedEventHandler modifiersChangedEventHandler = default(ModifiersChangedEventHandler);
		if (info.TryGetSignalEventDelegate<ModifiersChangedEventHandler>(SignalName.ModifiersChanged, ref modifiersChangedEventHandler))
		{
			backing_ModifiersChanged = modifiersChangedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.ModifiersChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalModifiersChanged()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.ModifiersChanged, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.ModifiersChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_ModifiersChanged?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.ModifiersChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
