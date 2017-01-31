// This is a C# Event Handler (notification center) for Unity. It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other.

using System;
using System.Collections.Generic;

/**
 * A handler for events that have one parameter of type T.
 */
static public class EventHandler
{
	private static Dictionary<EEventID, Delegate> eventTable = new Dictionary<EEventID, Delegate>();

    static public int Count()
    {
        return eventTable.Count;
    }
	
	static public void AddListener(EEventID eventType, Callback handler)
	{
		// Obtain a lock on the event table to keep this thread-safe.
		lock (eventTable)
		{
			// Create an entry for this event type if it doesn't already exist.
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			// Add the handler to the event.
			eventTable[eventType] = (Callback)eventTable[eventType] + handler;
		}
	}
	
	static public void RemoveListener(EEventID eventType, Callback handler)
	{
		// Obtain a lock on the event table to keep this thread-safe.
		lock (eventTable)
		{
			// Only take action if this event type exists.
			if (eventTable.ContainsKey(eventType))
			{
				// Remove the event handler from this event.
				eventTable[eventType] = (Callback)eventTable[eventType] - handler;
				
				// If there's nothing left then remove the event type from the event table.
				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}
	
	static public void TriggerEvent(EEventID eventType, System.Object arg)
	{
		Delegate d;
		// Invoke the delegate only if the event type is in the dictionary.
		if (eventTable.TryGetValue(eventType, out d))
		{
			// Take a local copy to prevent a race condition if another thread
			// were to unsubscribe from this event.
			Callback callback = (Callback)d;
			
			// Invoke the delegate if it's not null.
			if (callback != null)
			{
				callback(arg);
			}
		}
	}

	static public void CleanUpTable()
	{
        List<EEventID> eventsToRemove = new List<EEventID>();
        foreach(var sp in eventTable)
        {
            if((sp.Key != EEventID.EVENT_LOAD_LEVEL))
            {
                eventsToRemove.Add(sp.Key);
            }
        }

        //UnityEngine.Debug.LogError("EVENTS BEFORE : "+eventTable.Count+" TO REMOVE : "+eventsToRemove.Count);
        foreach (var eventToRemove in eventsToRemove)
            eventTable.Remove(eventToRemove);
        //UnityEngine.Debug.LogError("EVENTS AFTER : " + eventTable.Count + " TO REMOVE : " + eventsToRemove.Count);
        GameManager.b_takeInput = true;
        //eventTable.Clear();
    }

}

//static public class EventHandler<T>
//{
//	private static Dictionary<EEventID, Delegate> eventTable = new Dictionary<EEventID, Delegate>();
//	
//	static public void AddListener(EEventID eventType, Callback<T> handler)
//	{
//		// Obtain a lock on the event table to keep this thread-safe.
//		lock (eventTable)
//		{
//			// Create an entry for this event type if it doesn't already exist.
//			if (!eventTable.ContainsKey(eventType))
//			{
//				eventTable.Add(eventType, null);
//			}
//			// Add the handler to the event.
//			eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
//		}
//	}
//	
//	static public void RemoveListener(EEventID eventType, Callback<T> handler)
//	{
//		// Obtain a lock on the event table to keep this thread-safe.
//		lock (eventTable)
//		{
//			// Only take action if this event type exists.
//			if (eventTable.ContainsKey(eventType))
//			{
//				// Remove the event handler from this event.
//				eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;
//				
//				// If there's nothing left then remove the event type from the event table.
//				if (eventTable[eventType] == null)
//				{
//					eventTable.Remove(eventType);
//				}
//			}
//		}
//	}
//	
//	static public void TriggerEvent(EEventID eventType, T arg1)
//	{
//		Delegate d;
//		// Invoke the delegate only if the event type is in the dictionary.
//		if (eventTable.TryGetValue(eventType, out d))
//		{
//			// Take a local copy to prevent a race condition if another thread
//			// were to unsubscribe from this event.
//			Callback<T> callback = (Callback<T>)d;
//			
//			// Invoke the delegate if it's not null.
//			if (callback != null)
//			{
//				callback(arg1);
//			}
//		}
//	}
//}
