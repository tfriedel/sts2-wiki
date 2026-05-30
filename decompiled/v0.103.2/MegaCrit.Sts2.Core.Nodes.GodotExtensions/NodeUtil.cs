using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

public static class NodeUtil
{
	[CompilerGenerated]
	private sealed class _003CGetChildrenRecursive_003Ed__6<T> : IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable where T : notnull
	{
		private int _003C_003E1__state;

		private T _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Node node;

		public Node _003C_003E3__node;

		private IEnumerator<Node> _003C_003E7__wrap1;

		private Node _003Cchild_003E5__3;

		private IEnumerator<T> _003C_003E7__wrap3;

		T IEnumerator<T>.Current
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
		public _003CGetChildrenRecursive_003Ed__6(int _003C_003E1__state)
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
			_003Cchild_003E5__3 = null;
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
					_003C_003E7__wrap1 = node.GetChildren(false).GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_00fe;
				case 1:
					_003C_003E1__state = -4;
					goto IL_00a5;
				case 2:
					{
						_003C_003E1__state = -3;
						goto IL_00f7;
					}
					IL_00fe:
					if (_003C_003E7__wrap1.MoveNext())
					{
						_003Cchild_003E5__3 = _003C_003E7__wrap1.Current;
						_003C_003E7__wrap3 = _003Cchild_003E5__3.GetChildrenRecursive<T>().GetEnumerator();
						_003C_003E1__state = -4;
						goto IL_00a5;
					}
					_003C_003Em__Finally1();
					_003C_003E7__wrap1 = null;
					return false;
					IL_00f7:
					_003Cchild_003E5__3 = null;
					goto IL_00fe;
					IL_00a5:
					if (_003C_003E7__wrap3.MoveNext())
					{
						T current = _003C_003E7__wrap3.Current;
						_003C_003E2__current = current;
						_003C_003E1__state = 1;
						return true;
					}
					_003C_003Em__Finally2();
					_003C_003E7__wrap3 = null;
					if (_003Cchild_003E5__3 is T)
					{
						Node obj = _003Cchild_003E5__3;
						T val = (T)(object)((obj is T) ? obj : null);
						_003C_003E2__current = val;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_00f7;
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
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			_003CGetChildrenRecursive_003Ed__6<T> _003CGetChildrenRecursive_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CGetChildrenRecursive_003Ed__ = this;
			}
			else
			{
				_003CGetChildrenRecursive_003Ed__ = new _003CGetChildrenRecursive_003Ed__6<T>(0);
			}
			_003CGetChildrenRecursive_003Ed__.node = _003C_003E3__node;
			return _003CGetChildrenRecursive_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}
	}

	public static async Task<float> AwaitProcessFrame(this Node node, CancellationToken ct = default(CancellationToken))
	{
		ct.ThrowIfCancellationRequested();
		SceneTree tree = node.GetTree();
		if (tree == null)
		{
			throw new TaskCanceledException();
		}
		await ((GodotObject)node).ToSignal((GodotObject)(object)tree, SignalName.ProcessFrame);
		ct.ThrowIfCancellationRequested();
		if (!node.IsValid() || !node.IsInsideTree())
		{
			throw new TaskCanceledException();
		}
		return (float)node.GetProcessDeltaTime();
	}

	public static bool IsDescendant(Node parent, Node candidate)
	{
		for (Node parent2 = candidate.GetParent(); parent2 != null; parent2 = parent2.GetParent())
		{
			if (parent2 == parent)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsValid(this Node? node)
	{
		if (node != null && GodotObject.IsInstanceValid((GodotObject)(object)node))
		{
			return !((GodotObject)node).IsQueuedForDeletion();
		}
		return false;
	}

	public static void TryGrabFocus(this Control control)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (NControllerManager.Instance.IsUsingController)
		{
			if (((CanvasItem)control).IsVisibleInTree())
			{
				control.GrabFocus();
				return;
			}
			Callable val = Callable.From((Action)control.GrabFocus);
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
	}

	public static T? GetAncestorOfType<T>(this Node node)
	{
		for (Node parent = node.GetParent(); parent != null; parent = parent.GetParent())
		{
			if (parent is T)
			{
				return (T)(object)((parent is T) ? parent : null);
			}
		}
		return default(T);
	}

	public static Task AwaitSignal(this GodotObject source, StringName signal, Node owner)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		GodotObject source2 = source;
		StringName signal2 = signal;
		Node owner2 = owner;
		if (!GodotObject.IsInstanceValid(source2))
		{
			return Task.CompletedTask;
		}
		TaskCompletionSource tcs = new TaskCompletionSource();
		bool resolved = false;
		Callable callable = default(Callable);
		callable = Callable.From((Action)OnSignal);
		source2.Connect(signal2, callable, 0u);
		owner2.TreeExiting += OnExiting;
		return tcs.Task;
		void OnExiting()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (!resolved)
			{
				resolved = true;
				if (GodotObject.IsInstanceValid(source2))
				{
					source2.Disconnect(signal2, callable);
				}
				tcs.TrySetCanceled();
			}
		}
		void OnSignal()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (!resolved)
			{
				resolved = true;
				if (GodotObject.IsInstanceValid(source2))
				{
					source2.Disconnect(signal2, callable);
				}
				if (GodotObject.IsInstanceValid((GodotObject)(object)owner2))
				{
					owner2.TreeExiting -= OnExiting;
				}
				tcs.TrySetResult();
			}
		}
	}

	[IteratorStateMachine(typeof(_003CGetChildrenRecursive_003Ed__6<>))]
	public static IEnumerable<T> GetChildrenRecursive<T>(this Node node)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CGetChildrenRecursive_003Ed__6<T>(-2)
		{
			_003C_003E3__node = node
		};
	}
}
