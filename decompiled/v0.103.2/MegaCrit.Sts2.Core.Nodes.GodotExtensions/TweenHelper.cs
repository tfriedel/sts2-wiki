using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

public static class TweenHelper
{
	public static void FastForwardToCompletion(this Tween t)
	{
		t.CustomStep(999999999.0);
	}

	public static Task AwaitFinished(this Tween tween, Node owner)
	{
		Tween tween2 = tween;
		Node owner2 = owner;
		if (!tween2.IsValid() || !tween2.IsRunning())
		{
			return Task.CompletedTask;
		}
		TaskCompletionSource tcs = new TaskCompletionSource();
		bool resolved = false;
		tween2.Finished += OnFinished;
		owner2.TreeExiting += OnExiting;
		return tcs.Task;
		void OnExiting()
		{
			if (!resolved)
			{
				resolved = true;
				tween2.Finished -= OnFinished;
				tcs.TrySetCanceled();
			}
		}
		void OnFinished()
		{
			if (!resolved)
			{
				resolved = true;
				tween2.Finished -= OnFinished;
				if (GodotObject.IsInstanceValid((GodotObject)(object)owner2))
				{
					owner2.TreeExiting -= OnExiting;
				}
				tcs.TrySetResult();
			}
		}
	}

	public static Task AwaitFinished(this Tween tween, CancellationToken ct)
	{
		Tween tween2 = tween;
		TaskCompletionSource tcs = new TaskCompletionSource();
		int unsubscribed = 0;
		CancellationTokenRegistration ctr = default(CancellationTokenRegistration);
		tween2.Finished += OnFinished;
		if (ct.CanBeCanceled)
		{
			ctr = ct.Register(delegate
			{
				if (Interlocked.Exchange(ref unsubscribed, 1) == 0)
				{
					tween2.Finished -= OnFinished;
				}
				tcs.TrySetCanceled(ct);
			});
		}
		return tcs.Task;
		void OnFinished()
		{
			if (Interlocked.Exchange(ref unsubscribed, 1) == 0)
			{
				tween2.Finished -= OnFinished;
			}
			ctr.Dispose();
			tcs.TrySetResult();
		}
	}
}
