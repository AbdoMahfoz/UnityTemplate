using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public string[] Events;
    [HideInInspector] public Dictionary<string, object> Subscribers;

    public void ListenToEvent<T>(string SubscriberName, string Event, Action<T> Callback)
    {
        if (!Events.Contains(Event))
            throw new ArgumentException(
                $"{SubscriberName} attempted to subscribe to event {Event}, which is not listed in the event list");
        if (Subscribers.ContainsKey(Event))
        {
            try
            {
                ((List<Action<T>>) Subscribers[Event]).Add(Callback);
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException(
                    $"Incompatible event callbacks for event {Event};\n" +
                    "Conflict between already registered " +
                    $"{Subscribers[Event].GetType().GetGenericArguments()[0].Name} and {typeof(Action<T>).Name} " +
                    $"while {SubscriberName} was registering a listener");
            }
        }
        else
        {
            Subscribers.Add(Event, new List<Action<T>> {Callback});
        }
    }

    public void FireEvent<T>(string SourceName, string Event, T arg)
    {
        if (!Events.Contains(Event))
            throw new ArgumentException(
                $"{SourceName} attempted to fire event {Event}, which is not listed in the event list");
        Debug.Log($"{SourceName} fired {Event}");
        List<Action<T>> callbacks;
        try
        {
            callbacks = (List<Action<T>>) Subscribers[Event];
        }
        catch (InvalidCastException)
        {
            throw new InvalidOperationException(
                $"Incompatible event callbacks for event {Event};\n" +
                "Conflict between already registered " +
                $"{Subscribers[Event].GetType().GetGenericArguments()[0].Name} and {typeof(Action<T>).Name} " +
                $"while {SourceName} was igniting this event");
        }
        catch (KeyNotFoundException)
        {
            return;
        }

        foreach (var callback in callbacks)
        {
            callback(arg);
        }
    }
}