using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NTestSubjectVfx.cs")]
public class NTestSubjectVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName PlayAnim1 = StringName.op_Implicit("PlayAnim1");

		public static readonly StringName SquirtNeck = StringName.op_Implicit("SquirtNeck");

		public static readonly StringName StartDizzies = StringName.op_Implicit("StartDizzies");

		public static readonly StringName EndDizzies = StringName.op_Implicit("EndDizzies");

		public static readonly StringName StartEmbers = StringName.op_Implicit("StartEmbers");

		public static readonly StringName StartFlames = StringName.op_Implicit("StartFlames");

		public static readonly StringName EndFlames = StringName.op_Implicit("EndFlames");

		public static readonly StringName StartBurnVfx = StringName.op_Implicit("StartBurnVfx");

		public static readonly StringName EndBurnVfx = StringName.op_Implicit("EndBurnVfx");

		public static readonly StringName StartCeilingSparks = StringName.op_Implicit("StartCeilingSparks");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _neckParticles = StringName.op_Implicit("_neckParticles");

		public static readonly StringName _dizzyParticles = StringName.op_Implicit("_dizzyParticles");

		public static readonly StringName _emberParticles = StringName.op_Implicit("_emberParticles");

		public static readonly StringName _flameParticles = StringName.op_Implicit("_flameParticles");

		public static readonly StringName _burnParticles = StringName.op_Implicit("_burnParticles");

		public static readonly StringName _targetedBurnParticle = StringName.op_Implicit("_targetedBurnParticle");

		public static readonly StringName _burnParticleFountain = StringName.op_Implicit("_burnParticleFountain");

		public static readonly StringName _ceilingParticles = StringName.op_Implicit("_ceilingParticles");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");

		public static readonly StringName _keyDown = StringName.op_Implicit("_keyDown");

		public static readonly StringName _doingThing = StringName.op_Implicit("_doingThing");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _neckParticles;

	private GpuParticles2D _dizzyParticles;

	private GpuParticles2D _emberParticles;

	private GpuParticles2D _flameParticles;

	private GpuParticles2D _burnParticles;

	private GpuParticles2D _targetedBurnParticle;

	private GpuParticles2D _burnParticleFountain;

	private GpuParticles2D _ceilingParticles;

	private Node2D _parent;

	private MegaSprite _animController;

	private MegaSprite _frontBurnVfxController;

	private MegaSprite _backBurnVfxController;

	private bool _keyDown;

	private bool _doingThing;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_frontBurnVfxController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode(NodePath.op_Implicit("../FrontBurnVfxSlot/FrontBurnVfx"))));
		_backBurnVfxController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode(NodePath.op_Implicit("../BackBurnVfxSlot/BackBurnVfx"))));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_neckParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("NeckParticlesSlot/NeckParticles"));
		_dizzyParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("NeckParticlesSlot/DizzyPaticles"));
		_emberParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("../../EmberParticles"));
		_flameParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("../../FlameParticles"));
		_burnParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("../../BurnParticles"));
		_targetedBurnParticle = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("../../TargetedBurnParticle"));
		_burnParticleFountain = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("../../BurnParticleFountain"));
		_ceilingParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("../../CeilingSparks"));
		_neckParticles.OneShot = true;
		_neckParticles.Emitting = false;
		_dizzyParticles.Emitting = false;
		_emberParticles.OneShot = true;
		_emberParticles.Emitting = false;
		_flameParticles.Emitting = false;
		_burnParticles.Emitting = false;
		_targetedBurnParticle.Emitting = false;
		_burnParticleFountain.Emitting = false;
		_ceilingParticles.OneShot = true;
		_ceilingParticles.Emitting = false;
		_animController.GetAnimationState().SetAnimation("idle_loop3");
		_frontBurnVfxController.GetAnimationState().SetAnimation("empty");
		_backBurnVfxController.GetAnimationState().SetAnimation("empty");
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string eventName = new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName();
		if (eventName == null)
		{
			return;
		}
		switch (eventName.Length)
		{
		case 12:
			switch (eventName[6])
			{
			case 'x':
				if (eventName == "neck_explode")
				{
					SquirtNeck();
				}
				break;
			case 'e':
				if (eventName == "start_embers")
				{
					StartEmbers();
				}
				break;
			case 'f':
				if (eventName == "start_flames")
				{
					StartFlames();
				}
				break;
			case 'r':
				if (eventName == "end_burn_vfx")
				{
					EndBurnVfx();
				}
				break;
			}
			break;
		case 13:
			if (eventName == "start_dizzies")
			{
				StartDizzies();
			}
			break;
		case 11:
			if (eventName == "end_dizzies")
			{
				EndDizzies();
			}
			break;
		case 10:
			if (eventName == "end_flames")
			{
				EndFlames();
			}
			break;
		case 14:
			if (eventName == "start_burn_vfx")
			{
				StartBurnVfx();
			}
			break;
		case 20:
			if (eventName == "start_ceiling_sparks")
			{
				StartCeilingSparks();
			}
			break;
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
			break;
		}
	}

	private void PlayAnim1()
	{
		_animController.GetAnimationState().SetAnimation("die3", loop: false);
		_animController.GetAnimationState().AddAnimation("idle_loop3");
	}

	private void SquirtNeck()
	{
		_neckParticles.Restart();
	}

	private void StartDizzies()
	{
		if (!_dizzyParticles.Emitting)
		{
			_dizzyParticles.Emitting = true;
		}
	}

	private void EndDizzies()
	{
		_dizzyParticles.Emitting = false;
	}

	private void StartEmbers()
	{
		_emberParticles.Restart();
	}

	private void StartFlames()
	{
		_flameParticles.Emitting = true;
	}

	private void EndFlames()
	{
		_flameParticles.Emitting = false;
	}

	private void StartBurnVfx()
	{
		_frontBurnVfxController.GetAnimationState().SetAnimation("burn", loop: false);
		_backBurnVfxController.GetAnimationState().SetAnimation("burn", loop: false);
		_burnParticles.Restart();
		_targetedBurnParticle.Emitting = true;
		_burnParticleFountain.Restart();
	}

	private void EndBurnVfx()
	{
		_burnParticles.Emitting = false;
		_targetedBurnParticle.Emitting = false;
		_burnParticleFountain.Emitting = false;
	}

	private void StartCeilingSparks()
	{
		_ceilingParticles.Restart();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(12);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayAnim1, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SquirtNeck, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartDizzies, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndDizzies, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartEmbers, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartFlames, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndFlames, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartBurnVfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndBurnVfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartCeilingSparks, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			OnAnimationEvent(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayAnim1 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayAnim1();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SquirtNeck && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SquirtNeck();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartDizzies && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartDizzies();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndDizzies && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndDizzies();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartEmbers && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartEmbers();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartFlames && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartFlames();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndFlames && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndFlames();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartBurnVfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartBurnVfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndBurnVfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndBurnVfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartCeilingSparks && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartCeilingSparks();
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
		if ((ref method) == MethodName.OnAnimationEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayAnim1)
		{
			return true;
		}
		if ((ref method) == MethodName.SquirtNeck)
		{
			return true;
		}
		if ((ref method) == MethodName.StartDizzies)
		{
			return true;
		}
		if ((ref method) == MethodName.EndDizzies)
		{
			return true;
		}
		if ((ref method) == MethodName.StartEmbers)
		{
			return true;
		}
		if ((ref method) == MethodName.StartFlames)
		{
			return true;
		}
		if ((ref method) == MethodName.EndFlames)
		{
			return true;
		}
		if ((ref method) == MethodName.StartBurnVfx)
		{
			return true;
		}
		if ((ref method) == MethodName.EndBurnVfx)
		{
			return true;
		}
		if ((ref method) == MethodName.StartCeilingSparks)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._neckParticles)
		{
			_neckParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dizzyParticles)
		{
			_dizzyParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._emberParticles)
		{
			_emberParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flameParticles)
		{
			_flameParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._burnParticles)
		{
			_burnParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetedBurnParticle)
		{
			_targetedBurnParticle = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._burnParticleFountain)
		{
			_burnParticleFountain = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ceilingParticles)
		{
			_ceilingParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._keyDown)
		{
			_keyDown = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._doingThing)
		{
			_doingThing = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._neckParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _neckParticles);
			return true;
		}
		if ((ref name) == PropertyName._dizzyParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _dizzyParticles);
			return true;
		}
		if ((ref name) == PropertyName._emberParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _emberParticles);
			return true;
		}
		if ((ref name) == PropertyName._flameParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _flameParticles);
			return true;
		}
		if ((ref name) == PropertyName._burnParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _burnParticles);
			return true;
		}
		if ((ref name) == PropertyName._targetedBurnParticle)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _targetedBurnParticle);
			return true;
		}
		if ((ref name) == PropertyName._burnParticleFountain)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _burnParticleFountain);
			return true;
		}
		if ((ref name) == PropertyName._ceilingParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _ceilingParticles);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _parent);
			return true;
		}
		if ((ref name) == PropertyName._keyDown)
		{
			value = VariantUtils.CreateFrom<bool>(ref _keyDown);
			return true;
		}
		if ((ref name) == PropertyName._doingThing)
		{
			value = VariantUtils.CreateFrom<bool>(ref _doingThing);
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
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._neckParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dizzyParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._emberParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flameParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._burnParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._targetedBurnParticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._burnParticleFountain, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ceilingParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._keyDown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._doingThing, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._neckParticles, Variant.From<GpuParticles2D>(ref _neckParticles));
		info.AddProperty(PropertyName._dizzyParticles, Variant.From<GpuParticles2D>(ref _dizzyParticles));
		info.AddProperty(PropertyName._emberParticles, Variant.From<GpuParticles2D>(ref _emberParticles));
		info.AddProperty(PropertyName._flameParticles, Variant.From<GpuParticles2D>(ref _flameParticles));
		info.AddProperty(PropertyName._burnParticles, Variant.From<GpuParticles2D>(ref _burnParticles));
		info.AddProperty(PropertyName._targetedBurnParticle, Variant.From<GpuParticles2D>(ref _targetedBurnParticle));
		info.AddProperty(PropertyName._burnParticleFountain, Variant.From<GpuParticles2D>(ref _burnParticleFountain));
		info.AddProperty(PropertyName._ceilingParticles, Variant.From<GpuParticles2D>(ref _ceilingParticles));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
		info.AddProperty(PropertyName._keyDown, Variant.From<bool>(ref _keyDown));
		info.AddProperty(PropertyName._doingThing, Variant.From<bool>(ref _doingThing));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._neckParticles, ref val))
		{
			_neckParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._dizzyParticles, ref val2))
		{
			_dizzyParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._emberParticles, ref val3))
		{
			_emberParticles = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._flameParticles, ref val4))
		{
			_flameParticles = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._burnParticles, ref val5))
		{
			_burnParticles = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetedBurnParticle, ref val6))
		{
			_targetedBurnParticle = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._burnParticleFountain, ref val7))
		{
			_burnParticleFountain = ((Variant)(ref val7)).As<GpuParticles2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._ceilingParticles, ref val8))
		{
			_ceilingParticles = ((Variant)(ref val8)).As<GpuParticles2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val9))
		{
			_parent = ((Variant)(ref val9)).As<Node2D>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._keyDown, ref val10))
		{
			_keyDown = ((Variant)(ref val10)).As<bool>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._doingThing, ref val11))
		{
			_doingThing = ((Variant)(ref val11)).As<bool>();
		}
	}
}
