using System;

namespace CMP.Scripts.EventSystem
{
	public static class EventBus<T> where T : struct
	{
		private static Action<T> _callback;

		public static void Subscribe(Action<T> callback)
		{
			_callback += callback;
		}
		
		public static void Unsubscribe(Action<T> callback)
		{
			_callback -= callback;
		}
		
		public static void Invoke(T t)
		{
			_callback?.Invoke(t);
		} 
	}
}